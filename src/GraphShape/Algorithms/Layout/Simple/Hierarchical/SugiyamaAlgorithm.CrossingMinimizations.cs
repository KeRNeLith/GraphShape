﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuikGraph;
using GraphShape.Utils;
using JetBrains.Annotations;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout
{
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        private int[] _crossCounts;

        [ItemCanBeNull]
        private IList<Edge<Data>>[] _sparseCompactionByLayerBackup;

        [ItemCanBeNull]
        private AlternatingLayer[] _alternatingLayers;

        /// <summary>
        /// Minimizes the crossings between the layers by sweeping up and down
        /// while there could be something optimized.
        /// </summary>
        private void DoCrossingMinimizations()
        {
            int prevCrossings;
            int crossings = int.MaxValue;
            int phase = 1;

            _crossCounts = new int[_layers.Count];
            _sparseCompactionByLayerBackup = new IList<Edge<Data>>[_layers.Count];
            _alternatingLayers = new AlternatingLayer[_layers.Count];
            for (int i = 0; i < _layers.Count; ++i)
            {
                _crossCounts[i] = int.MaxValue;
            }

            int phase1IterationLeft = 100;
            int phase2IterationLeft = _layers.Count;
            const bool enableSameMeasureOptimization = true;
            bool changed;
            bool wasPhase2;
            do
            {
                ThrowIfCancellationRequested();

                prevCrossings = crossings;
                if (phase == 1)
                {
                    --phase1IterationLeft;
                }
                else if (phase == 2)
                {
                    --phase2IterationLeft;
                }
                wasPhase2 = phase == 2;

                crossings = Sweeping(0, _layers.Count - 1, 1, enableSameMeasureOptimization, out bool c, ref phase);
                changed = c;
                if (crossings == 0)
                    break;

                crossings = Sweeping(_layers.Count - 1, 0, -1, enableSameMeasureOptimization, out c, ref phase);
                changed = changed || c;
                if (phase == 1 && (!changed || crossings >= prevCrossings) && phase2IterationLeft > 0)
                {
                    phase = 2;
                }
                else if (phase == 2)
                {
                    phase = 1;
                }
            } while (crossings > 0
                     && (phase2IterationLeft > 0
                         || wasPhase2
                         || phase1IterationLeft > 0 && crossings < prevCrossings && changed));
        }

        /// <summary>
        /// Sweeps between the <paramref name="startLayerIndex"/> and <paramref name="endLayerIndex"/>
        /// in the way represented by the step.
        /// </summary>
        /// <param name="startLayerIndex">The index of the start layer (where the sweeping starts from).</param>
        /// <param name="endLayerIndex">The index of the last layer (where the sweeping ends).</param>
        /// <param name="step">Increment or decrement of the layer index. (1 or -1)</param>
        /// <param name="enableSameMeasureOptimization">Indicates if the same measure optimization is enabled.</param>
        /// <param name="changed">Indicates if layer has changed or not.</param>
        /// <param name="phase">Algorithm phase.</param>
        /// <returns>The number of the edge crossings.</returns>
        private int Sweeping(
            int startLayerIndex,
            int endLayerIndex,
            int step,
            bool enableSameMeasureOptimization,
            out bool changed,
            ref int phase)
        {
            int crossings = 0;
            changed = false;

            AlternatingLayer alternatingLayer;
            AlternatingLayer layer = _alternatingLayers[startLayerIndex];
            if (layer is null)
            {
                alternatingLayer = new AlternatingLayer();
                alternatingLayer.AddRange(
                    _layers[startLayerIndex]
#if !SUPPORTS_ENUMERABLE_COVARIANT
                        .OfType<IData>()
#endif
                    );
                alternatingLayer.EnsureAlternatingAndPositions();
                AddAlternatingLayerToSparseCompactionGraph(alternatingLayer, startLayerIndex);
                _alternatingLayers[startLayerIndex] = alternatingLayer;
            }
            else
            {
                alternatingLayer = layer;
            }

            for (int i = startLayerIndex; i != endLayerIndex; i += step)
            {
                int ci = Math.Min(i, i + step);
                int prevCrossCount = _crossCounts[ci];

                AlternatingLayer nextAlternatingLayer = _alternatingLayers[i + step];
                if (nextAlternatingLayer != null)
                {
                    alternatingLayer?.SetPositions();
                    nextAlternatingLayer.SetPositions();
                    prevCrossCount = DoCrossCountingAndOptimization(
                        alternatingLayer,
                        nextAlternatingLayer,
                        i < i + step,
                        false,
                        phase == 2,
                        int.MaxValue);
                    _crossCounts[ci] = prevCrossCount;
                }

                int crossCount = CrossingMinimizationBetweenLayers(
                    ref alternatingLayer,
                    i, i + step,
                    enableSameMeasureOptimization,
                    prevCrossCount,
                    phase);

                if (crossCount < prevCrossCount || phase == 2 || changed)
                {
                    // Set the sparse compaction graph
                    AddAlternatingLayerToSparseCompactionGraph(alternatingLayer, i + step);
                    ReplaceLayer(alternatingLayer, i + step);
                    _alternatingLayers[i + step] = alternatingLayer;
                    _crossCounts[i] = crossCount;
                    crossings += crossCount;
                    changed = true;
                }
                else
                {
                    alternatingLayer = nextAlternatingLayer;
                    crossings += prevCrossCount;
                }
            }

            return crossings;
        }

        private void ReplaceLayer([NotNull, ItemNotNull] AlternatingLayer alternatingLayer, int i)
        {
            Debug.Assert(alternatingLayer != null);

            _layers[i].Clear();
            foreach (IData item in alternatingLayer)
            {
                var vertex = item as SugiVertex;
                if (vertex is null)
                    continue;
                _layers[i].Add(vertex);
                vertex.IndexInsideLayer = i;
            }
        }

        [Pure]
        private int CrossingMinimizationBetweenLayers(
            [NotNull, ItemNotNull] ref AlternatingLayer alternatingLayer,
            int actualLayerIndex,
            int nextLayerIndex,
            bool enableSameMeasureOptimization,
            int prevCrossCount,
            int phase)
        {
            // Decide which way we are sweeping (up or down)
            // Straight = down, reverse = up
            bool straightSweep = actualLayerIndex < nextLayerIndex;
            AlternatingLayer nextAlternatingLayer = alternatingLayer.Clone();

            // 1
            AppendSegmentsToAlternatingLayer(nextAlternatingLayer, straightSweep);

            // 2
            ComputeMeasureValues(alternatingLayer, nextLayerIndex, straightSweep);
            nextAlternatingLayer.SetPositions();

            // 3
            nextAlternatingLayer = InitialOrderingOfNextLayer(nextAlternatingLayer, _layers[nextLayerIndex], straightSweep);

            // 4
            PlaceQVertices(nextAlternatingLayer, _layers[nextLayerIndex], straightSweep);
            nextAlternatingLayer.SetPositions();

            // 5
            int crossCount = DoCrossCountingAndOptimization(
                alternatingLayer,
                nextAlternatingLayer,
                straightSweep,
                enableSameMeasureOptimization,
                phase == 2,
                prevCrossCount);

            // 6
            nextAlternatingLayer.EnsureAlternatingAndPositions();

            alternatingLayer = nextAlternatingLayer;
            return crossCount;
        }

        /// <summary>
        /// Replaces the P or Q vertices of the <paramref name="alternatingLayer"/> with their segment on the next layer.
        /// </summary>
        /// <param name="alternatingLayer">The actual alternating layer. It will be modified.</param>
        /// <param name="straightSweep">If true, we are sweeping down else we're sweeping up.</param>
        private static void AppendSegmentsToAlternatingLayer(
            [NotNull, ItemNotNull] AlternatingLayer alternatingLayer,
            bool straightSweep)
        {
            Debug.Assert(alternatingLayer != null);

            VertexTypes type = straightSweep ? VertexTypes.PVertex : VertexTypes.QVertex;
            for (int i = 1; i < alternatingLayer.Count; i += 2)
            {
                var vertex = (SugiVertex)alternatingLayer[i];
                if (vertex.Type == type)
                {
                    var precedingContainer = (SegmentContainer)alternatingLayer[i - 1];
                    var succeedingContainer = (SegmentContainer)alternatingLayer[i + 1];
                    precedingContainer.Append(vertex.Segment);
                    precedingContainer.Join(succeedingContainer);

                    // Remove the vertex and the succeeding container from the alternating layer
                    alternatingLayer.RemoveRange(i, 2);
                    i -= 2;
                }
            }
        }

        private void ComputeMeasureValues(
            [NotNull, ItemNotNull] AlternatingLayer alternatingLayer,
            int nextLayerIndex,
            bool straightSweep)
        {
            Debug.Assert(alternatingLayer != null);

            AssignPositionsOnActualLayer(alternatingLayer);
            AssignMeasuresOnNextLayer(_layers[nextLayerIndex], straightSweep);
        }

        /// <summary>
        /// Assigns the positions of the vertices and segment container on the actual layer.
        /// </summary>
        /// <param name="alternatingLayer">The actual layer (L_i).</param>
        private static void AssignPositionsOnActualLayer([NotNull, ItemNotNull] AlternatingLayer alternatingLayer)
        {
            Debug.Assert(alternatingLayer != null);

            // Assign positions to vertices on the actualLayer (L_i)
            for (int i = 1; i < alternatingLayer.Count; i += 2)
            {
                var precedingContainer = (SegmentContainer)alternatingLayer[i - 1];
                var vertex = (SugiVertex)alternatingLayer[i];
                if (i == 1)
                {
                    vertex.Position = precedingContainer.Count;
                }
                else
                {
                    var previousVertex = (SugiVertex)alternatingLayer[i - 2];
                    vertex.Position = previousVertex.Position + precedingContainer.Count + 1;
                }
            }

            // Assign positions to containers on the actualLayer (L_i+1)
            for (int i = 0; i < alternatingLayer.Count; i += 2)
            {
                var container = (SegmentContainer)alternatingLayer[i];
                if (i == 0)
                {
                    container.Position = 0;
                }
                else
                {
                    var precedingVertex = (SugiVertex)alternatingLayer[i - 1];
                    container.Position = precedingVertex.Position + 1;
                }
            }
        }

        private void AssignMeasuresOnNextLayer([NotNull, ItemNotNull] IEnumerable<SugiVertex> layer, bool straightSweep)
        {
            Debug.Assert(layer != null);

            // Measures of the containers is the same as their positions
            // So we should set the measures only for the vertices
            foreach (SugiVertex vertex in layer)
            {
                if (straightSweep && vertex.Type == VertexTypes.QVertex
                    ||
                    !straightSweep && vertex.Type == VertexTypes.PVertex)
                    continue;

                IEnumerable<SugiEdge> edges = straightSweep ? _graph.InEdges(vertex) : _graph.OutEdges(vertex);
                double oldMeasuredPosition = vertex.MeasuredPosition;
                vertex.MeasuredPosition = 0;
                vertex.DoNotOptimize = false;

                int count = 0;
                foreach (SugiEdge edge in edges)
                {
                    ThrowIfCancellationRequested();

                    SugiVertex otherVertex = edge.GetOtherVertex(vertex);
                    vertex.MeasuredPosition += otherVertex.Position;
                    ++count;
                }

                if (count > 0)
                {
                    vertex.MeasuredPosition /= count;
                }
                else
                {
                    vertex.MeasuredPosition = oldMeasuredPosition;
                    vertex.DoNotOptimize = true;
                }
            }
        }

        [Pure]
        [NotNull, ItemNotNull]
        private static AlternatingLayer InitialOrderingOfNextLayer(
            [NotNull, ItemNotNull] AlternatingLayer alternatingLayer,
            [NotNull, ItemNotNull] IEnumerable<SugiVertex> nextLayer,
            bool straightSweep)
        {
            Debug.Assert(alternatingLayer != null);
            Debug.Assert(nextLayer != null);

            // Get the list of the containers and vertices
            var segmentContainerStack = new Stack<ISegmentContainer>(alternatingLayer.OfType<ISegmentContainer>().Reverse());
            VertexTypes ignorableVertexType = straightSweep ? VertexTypes.QVertex : VertexTypes.PVertex;
            var vertexStack = new Stack<SugiVertex>(
                nextLayer
                    .Where(v => v.Type != ignorableVertexType)
                    .OrderBy(v => v.MeasuredPosition)
                    .Reverse());
            var newAlternatingLayer = new AlternatingLayer();

            while (vertexStack.Count > 0 && segmentContainerStack.Count > 0)
            {
                SugiVertex vertex = vertexStack.Peek();
                ISegmentContainer segmentContainer = segmentContainerStack.Peek();
                if (vertex.MeasuredPosition <= segmentContainer.Position)
                {
                    newAlternatingLayer.Add(vertexStack.Pop());
                }
                else if (vertex.MeasuredPosition >= segmentContainer.Position + segmentContainer.Count - 1)
                {
                    newAlternatingLayer.Add(segmentContainerStack.Pop());
                }
                else
                {
                    vertexStack.Pop();
                    segmentContainerStack.Pop();
                    int k = (int)Math.Ceiling(vertex.MeasuredPosition - segmentContainer.Position);
                    segmentContainer.Split(k, out ISegmentContainer container1, out ISegmentContainer container2);
                    newAlternatingLayer.Add(container1);
                    newAlternatingLayer.Add(vertex);
                    container2.Position = segmentContainer.Position + k;
                    segmentContainerStack.Push(container2);
                }
            }

            if (vertexStack.Count > 0)
            {
                newAlternatingLayer.AddRange(
                    vertexStack
#if !SUPPORTS_ENUMERABLE_COVARIANT
                        .OfType<IData>()
#endif
                    );
            }

            if (segmentContainerStack.Count > 0)
            {
                newAlternatingLayer.AddRange(
                    segmentContainerStack
#if !SUPPORTS_ENUMERABLE_COVARIANT
                        .OfType<IData>()
#endif
                    );
            }

            return newAlternatingLayer;
        }

        private void PlaceQVertices(
            [NotNull, ItemNotNull] AlternatingLayer alternatingLayer,
            [NotNull, ItemNotNull] IEnumerable<SugiVertex> nextLayer,
            bool straightSweep)
        {
            Debug.Assert(alternatingLayer != null);
            Debug.Assert(nextLayer != null);

            VertexTypes type = straightSweep ? VertexTypes.QVertex : VertexTypes.PVertex;
            var qVertices = new HashSet<SugiVertex>();
            foreach (SugiVertex vertex in nextLayer)
            {
                if (vertex.Type != type)
                    continue;

                qVertices.Add(vertex);
            }

            for (int i = 0; i < alternatingLayer.Count; ++i)
            {
                var segmentContainer = alternatingLayer[i] as SegmentContainer;
                if (segmentContainer is null)
                    continue;

                for (int j = 0; j < segmentContainer.Count; ++j)
                {
                    ThrowIfCancellationRequested();

                    Segment segment = segmentContainer[j];
                    SugiVertex vertex = straightSweep ? segment.QVertex : segment.PVertex;
                    if (!qVertices.Contains(vertex))
                        continue;

                    alternatingLayer.RemoveAt(i);
                    segmentContainer.Split(segment, out ISegmentContainer container1, out ISegmentContainer container2);
                    container1.Position = segmentContainer.Position;
                    container2.Position = segmentContainer.Position + container1.Count + 1;
                    alternatingLayer.Insert(i, container1);
                    alternatingLayer.Insert(i + 1, vertex);
                    alternatingLayer.Insert(i + 2, container2);
                    ++i;
                    break;
                }
            }
        }

        [Pure]
        [NotNull, ItemNotNull]
        private List<SugiVertex> FindVerticesWithSameMeasure(
            [NotNull, ItemNotNull] AlternatingLayer nextAlternatingLayer,
            bool straightSweep,
            [NotNull] out IList<int> ranges,
            out int maxRangeLength)
        {
            Debug.Assert(nextAlternatingLayer != null);

            VertexTypes ignorableVertexType = straightSweep ? VertexTypes.QVertex : VertexTypes.PVertex;
            var verticesWithSameMeasure = new List<SugiVertex>();
            SugiVertex[] vertices = nextAlternatingLayer.OfType<SugiVertex>().ToArray();
            int startIndex;
            int endIndex;
            maxRangeLength = 0;
            ranges = new List<int>();
            for (startIndex = 0; startIndex < vertices.Length; startIndex = endIndex + 1)
            {
                ThrowIfCancellationRequested();

                endIndex = FindNearVertexEndIndex(startIndex, vertices);

                if (endIndex > startIndex)
                {
                    int rangeLength = 0;
                    for (int i = startIndex; i <= endIndex; ++i)
                    {
                        ThrowIfCancellationRequested();

                        if (vertices[i].Type == ignorableVertexType || vertices[i].DoNotOptimize)
                            continue;

                        ++rangeLength;
                        verticesWithSameMeasure.Add(vertices[i]);
                    }

                    if (rangeLength > 0)
                    {
                        maxRangeLength = Math.Max(rangeLength, maxRangeLength);
                        ranges.Add(rangeLength);
                    }
                }
            }

            return verticesWithSameMeasure;
        }

        [Pure]
        private static int FindNearVertexEndIndex(int startIndex, [NotNull, ItemNotNull] SugiVertex[] vertices)
        {
            int endIndex;
            for (
                endIndex = startIndex + 1;
                endIndex < vertices.Length
                && NearEqual(vertices[startIndex].MeasuredPosition, vertices[endIndex].MeasuredPosition);
                ++endIndex)
            {
                // Nothing to do...
            }
            return --endIndex;
        }

        private void AddAlternatingLayerToSparseCompactionGraph(
            [NotNull, ItemNotNull] AlternatingLayer nextAlternatingLayer,
            int layerIndex)
        {
            Debug.Assert(nextAlternatingLayer != null);

            var sparseCompactionGraphEdgesOfLayer = _sparseCompactionByLayerBackup[layerIndex];
            if (sparseCompactionGraphEdgesOfLayer != null)
            {
                foreach (Edge<Data> edge in sparseCompactionGraphEdgesOfLayer)
                {
                    _sparseCompactionGraph.RemoveEdge(edge);
                }
            }

            sparseCompactionGraphEdgesOfLayer = new List<Edge<Data>>();
            SugiVertex prevVertex = null;
            for (int i = 1; i < nextAlternatingLayer.Count; i += 2)
            {
                var vertex = (SugiVertex)nextAlternatingLayer[i];
                var prevContainer = nextAlternatingLayer[i - 1] as SegmentContainer;
                var nextContainer = nextAlternatingLayer[i + 1] as SegmentContainer;
                if (prevContainer != null && prevContainer.Count > 0)
                {
                    Segment lastSegment = prevContainer[prevContainer.Count - 1];
                    var edge = new Edge<Data>(lastSegment, vertex);
                    sparseCompactionGraphEdgesOfLayer.Add(edge);
                    _sparseCompactionGraph.AddVerticesAndEdge(edge);
                }
                else if (prevVertex != null)
                {
                    var edge = new Edge<Data>(prevVertex, vertex);
                    sparseCompactionGraphEdgesOfLayer.Add(edge);
                    _sparseCompactionGraph.AddVerticesAndEdge(edge);
                }

                if (nextContainer != null && nextContainer.Count > 0)
                {
                    Segment firstSegment = nextContainer[0];
                    var edge = new Edge<Data>(vertex, firstSegment);
                    sparseCompactionGraphEdgesOfLayer.Add(edge);
                    _sparseCompactionGraph.AddVerticesAndEdge(edge);
                }

                if (!_sparseCompactionGraph.ContainsVertex(vertex))
                    _sparseCompactionGraph.AddVertex(vertex);
                prevVertex = vertex;
            }

            _sparseCompactionByLayerBackup[layerIndex] = sparseCompactionGraphEdgesOfLayer;
        }

        private sealed class VertexGroup
        {
            public int Position { get; }
            public int Size { get; set; }

            public VertexGroup()
            {
            }

            public VertexGroup(int position)
            {
                Position = position;
            }
        }

        private sealed class CrossCounterPair : Pair
        {
            public EdgeTypes Type { get; }

            public SugiEdge NonInnerSegment { get; set; }

            public CrossCounterPair()
                : this(EdgeTypes.InnerSegment)
            {
            }

            public CrossCounterPair(EdgeTypes type)
            {
                Type = type;
            }
        }

        private sealed class CrossCounterTreeNode
        {
            public int Accumulator { get; set; }
            public bool InnerSegmentMarker { get; set; }

            [NotNull, ItemNotNull]
            public readonly Queue<SugiEdge> NonInnerSegmentQueue = new Queue<SugiEdge>();
        }

        private int DoCrossCountingAndOptimization(
            [ItemNotNull] AlternatingLayer alternatingLayer,
            [ItemNotNull] AlternatingLayer nextAlternatingLayer,
            bool straightSweep,
            bool enableSameMeasureOptimization,
            bool reverseVerticesWithSameMeasure,
            int prevCrossCount)
        {
            AlternatingLayer topLayer = straightSweep ? alternatingLayer : nextAlternatingLayer;
            AlternatingLayer bottomLayer = straightSweep ? nextAlternatingLayer : alternatingLayer;

            IData lastOnTopLayer = topLayer[topLayer.Count - 1];
            IData lastOnBottomLayer = bottomLayer[bottomLayer.Count - 1];
            int firstLayerSize = lastOnTopLayer.Position + (lastOnTopLayer is ISegmentContainer topContainer ? topContainer.Count : 1);
            int secondLayerSize = lastOnBottomLayer.Position + (lastOnBottomLayer is ISegmentContainer bottomContainer ? bottomContainer.Count : 1);

            IList<CrossCounterPair> virtualEdgePairs = FindVirtualEdgePairs(topLayer, bottomLayer);
            IList<SugiEdge> realEdges = FindRealEdges(topLayer);

            if (enableSameMeasureOptimization || reverseVerticesWithSameMeasure)
            {
                IList<SugiVertex> verticesWithSameMeasure = FindVerticesWithSameMeasure(
                    nextAlternatingLayer,
                    straightSweep,
                    out IList<int> ranges,
                    out _);
                var verticesWithSameMeasureSet = new HashSet<SugiVertex>(verticesWithSameMeasure);

                // Initialize permutation indices
                for (int i = 0; i < verticesWithSameMeasure.Count; ++i)
                {
                    verticesWithSameMeasure[i].PermutationIndex = i;
                }

                int bestCrossCount = prevCrossCount;
                foreach (SugiEdge realEdge in realEdges)
                {
                    realEdge.SaveMarkedToTemp();
                }

                List<SugiVertex> sortedVertexList = ComputeSortedVertexList(
                    verticesWithSameMeasure,
                    reverseVerticesWithSameMeasure);

                int maxPermutations = SugiyamaLayoutParameters.MaxPermutations;
                do
                {
                    --maxPermutations;
                    if (!reverseVerticesWithSameMeasure)
                    {
                        // Sort by permutation index and measure
                        sortedVertexList.Sort((v1, v2) =>
                        {
                            if (!NearEqual(v1.MeasuredPosition, v2.MeasuredPosition))
                                return Math.Sign(v1.MeasuredPosition - v2.MeasuredPosition);
                            return v1.PermutationIndex - v2.PermutationIndex;
                        });
                    }

                    // Reinsert the vertices into the layer
                    ReinsertVerticesIntoLayer(nextAlternatingLayer, verticesWithSameMeasureSet, sortedVertexList);

                    // Set the positions
                    nextAlternatingLayer.SetPositions();

                    var edgePairs = new List<CrossCounterPair>();
                    edgePairs.AddRange(virtualEdgePairs);
                    edgePairs.AddRange(ConvertRealEdgesToCrossCounterPairs(realEdges, true));

                    int crossCount = BiLayerCrossCount(edgePairs, firstLayerSize, secondLayerSize);

                    if (reverseVerticesWithSameMeasure)
                        return crossCount;

                    // If the cross count is better than the best known
                    // save the actual state
                    if (crossCount < bestCrossCount)
                    {
                        foreach (SugiVertex vertex in verticesWithSameMeasure)
                        {
                            vertex.SavePositionToTemp();
                        }

                        foreach (SugiEdge edge in realEdges)
                        {
                            edge.SaveMarkedToTemp();
                        }

                        bestCrossCount = crossCount;
                    }

                    if (crossCount == 0)
                        break;
                } while (maxPermutations > 0 && Swap(verticesWithSameMeasure, ranges));

                // Reload the best solution
                foreach (SugiVertex vertex in verticesWithSameMeasure)
                {
                    vertex.LoadPositionFromTemp();
                }

                foreach (SugiEdge edge in realEdges)
                {
                    edge.LoadMarkedFromTemp();
                }

                // Sort by permutation index and measure
                sortedVertexList.Sort((v1, v2) => v1.Position - v2.Position);

                // Reinsert the vertices into the layer
                ReinsertVerticesIntoLayer(nextAlternatingLayer, verticesWithSameMeasureSet, sortedVertexList);
                nextAlternatingLayer.SetPositions();

                return bestCrossCount;
            }

            var pairs = new List<CrossCounterPair>();
            pairs.AddRange(virtualEdgePairs);
            pairs.AddRange(ConvertRealEdgesToCrossCounterPairs(realEdges, true));

            return BiLayerCrossCount(pairs, firstLayerSize, secondLayerSize);
        }

        [Pure]
        [NotNull, ItemNotNull]
        private List<SugiVertex> ComputeSortedVertexList(
            [NotNull, ItemNotNull] ICollection<SugiVertex> verticesWithSameMeasure,
            bool reverseVerticesWithSameMeasure)
        {
            List<SugiVertex> sortedVertexList;
            if (!reverseVerticesWithSameMeasure)
            {
                sortedVertexList = new List<SugiVertex>(verticesWithSameMeasure);
            }
            else
            {
                sortedVertexList = new List<SugiVertex>(verticesWithSameMeasure.Count);
                var stack = new Stack<SugiVertex>(verticesWithSameMeasure.Count);
                foreach (SugiVertex vertex in verticesWithSameMeasure)
                {
                    if (stack.Count > 0 && (!NearEqual(stack.Peek().MeasuredPosition, vertex.MeasuredPosition) ||
                                            Rand.NextDouble() > 0.8))
                    {
                        while (stack.Count > 0)
                        {
                            sortedVertexList.Add(stack.Pop());
                        }
                    }

                    stack.Push(vertex);
                }

                while (stack.Count > 0)
                {
                    sortedVertexList.Add(stack.Pop());
                }
            }

            return sortedVertexList;
        }

        private static void ReinsertVerticesIntoLayer(
            [NotNull, ItemNotNull] AlternatingLayer layer,
            [NotNull, ItemNotNull] ICollection<SugiVertex> vertexSet,
            [NotNull, ItemNotNull] IList<SugiVertex> vertexList)
        {
            Debug.Assert(layer != null);
            Debug.Assert(vertexSet != null);
            Debug.Assert(vertexList != null);

            int reinsertIndex = 0;
            for (int i = 0; i < layer.Count; ++i)
            {
                var vertex = layer[i] as SugiVertex;
                if (vertex is null || !vertexSet.Contains(vertex))
                    continue;

                layer.RemoveAt(i);
                layer.Insert(i, vertexList[reinsertIndex]);
                ++reinsertIndex;
            }
        }

        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<CrossCounterPair> ConvertRealEdgesToCrossCounterPairs(
            [NotNull, ItemNotNull] IEnumerable<SugiEdge> edges,
            bool clearMark)
        {
            foreach (SugiEdge edge in edges)
            {
                SugiVertex source = edge.Source;
                SugiVertex target = edge.Target;

                if (clearMark)
                    edge.Marked = false;

                yield return new CrossCounterPair(EdgeTypes.NonInnerSegment)
                {
                    First = source.Position,
                    Second = target.Position,
                    Weight = 1,
                    NonInnerSegment = edge
                };
            }
        }

        [Pure]
        [NotNull, ItemNotNull]
        private IList<SugiEdge> FindRealEdges([NotNull, ItemNotNull] AlternatingLayer topLayer)
        {
            Debug.Assert(topLayer != null);

            var realEdges = new List<SugiEdge>();
            foreach (IData item in topLayer)
            {
                var vertex = item as SugiVertex;
                if (vertex is null || vertex.Type == VertexTypes.PVertex)
                    continue;

                realEdges.AddRange(_graph.OutEdges(vertex));
            }

            return realEdges;
        }

        [Pure]
        [NotNull, ItemNotNull]
        private static IList<CrossCounterPair> FindVirtualEdgePairs(
            [NotNull, ItemNotNull] AlternatingLayer topLayer,
            [NotNull, ItemNotNull] AlternatingLayer bottomLayer)
        {
            var virtualEdgePairs = new List<CrossCounterPair>();
            Queue<VertexGroup> firstLayerQueue = GetContainerLikeItems(topLayer, VertexTypes.PVertex);
            Queue<VertexGroup> secondLayerQueue = GetContainerLikeItems(bottomLayer, VertexTypes.QVertex);
            var group1 = new VertexGroup();
            var group2 = new VertexGroup();
            while (firstLayerQueue.Count > 0 || secondLayerQueue.Count > 0)
            {
                if (group1.Size == 0)
                {
                    group1 = firstLayerQueue.Dequeue();
                }

                if (group2.Size == 0)
                {
                    group2 = secondLayerQueue.Dequeue();
                }

                if (group1.Size <= group2.Size)
                {
                    virtualEdgePairs.Add(
                        new CrossCounterPair
                        {
                            First = group1.Position,
                            Second = group2.Position,
                            Weight = group1.Size
                        });
                    group2.Size -= group1.Size;
                    group1.Size = 0;
                }
                else
                {
                    virtualEdgePairs.Add(
                        new CrossCounterPair
                        {
                            First = group1.Position,
                            Second = group2.Position,
                            Weight = group2.Size
                        });
                    group1.Size -= group2.Size;
                    group2.Size = 0;
                }
            }

            return virtualEdgePairs;
        }

        [Pure]
        private bool Swap([NotNull, ItemNotNull] IList<SugiVertex> vertices, [NotNull] IList<int> ranges)
        {
            bool swapped = false;

            for (int i = 0, startIndex = 0; i < ranges.Count; startIndex += ranges[i], ++i)
            {
                swapped |= SwapSomeHow(vertices, startIndex, ranges[i]);
            }

            return swapped;
        }

        [Pure]
        private bool SwapSomeHow([NotNull, ItemNotNull] IList<SugiVertex> vertices, int startIndex, int count)
        {
            if (count <= 4)
                return Swap(vertices, startIndex, count);
            return RandomSwap(vertices, startIndex, count);
        }

        [Pure]
        private bool RandomSwap([NotNull, ItemNotNull] IList<SugiVertex> vertices, int startIndex, int count)
        {
            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; ++i)
            {
                vertices[i].PermutationIndex = Rand.Next(count);
            }

            return true;
        }

        [Pure]
        private static bool Swap([NotNull, ItemNotNull] IList<SugiVertex> vertices, int startIndex, int count)
        {
            // Initial ordering
            int n = startIndex + count;
            int i;
            int j;

            // Find place to start
            for (i = n - 1;
                i > startIndex && vertices[i - 1].PermutationIndex >= vertices[i].PermutationIndex;
                --i)
            {
                // Nothing to do...
            }

            // All in reverse order
            if (i < startIndex + 1)
                return false; // No more permutation

            // Do next permutation
            for (j = n;
                j > startIndex + 1 && vertices[j - 1].PermutationIndex <= vertices[i - 1].PermutationIndex;
                --j)
            {
                // Nothing to do...
            }

            // Swap values i-1, j-1
            int c = vertices[i - 1].PermutationIndex;
            vertices[i - 1].PermutationIndex = vertices[j - 1].PermutationIndex;
            vertices[j - 1].PermutationIndex = c;

            // Need more swaps
            for (i++, j = n; i < j; ++i, --j)
            {
                c = vertices[i - 1].PermutationIndex;
                vertices[i - 1].PermutationIndex = vertices[j - 1].PermutationIndex;
                vertices[j - 1].PermutationIndex = c;
            }

            return true; // New permutation generated
        }

        [Pure]
        private int BiLayerCrossCount(
            [CanBeNull, ItemNotNull] IEnumerable<CrossCounterPair> pairs,
            int firstLayerVertexCount,
            int secondLayerVertexCount)
        {
            if (pairs is null)
                return 0;

            // Radix sort of the pairs, order by First asc, Second asc

            #region Sort by Second ASC

            var radixBySecond = new List<CrossCounterPair>[secondLayerVertexCount];
            List<CrossCounterPair> r;
            int pairCount = 0;
            foreach (CrossCounterPair pair in pairs)
            {
                // Get the radix where the pair should be inserted
                r = radixBySecond[pair.Second];
                if (r is null)
                {
                    r = new List<CrossCounterPair>();
                    radixBySecond[pair.Second] = r;
                }
                r.Add(pair);
                pairCount = Math.Max(pairCount, pair.Second);
            }

            ++pairCount;

            #endregion

            #region Sort By First ASC

            var radixByFirst = new List<CrossCounterPair>[firstLayerVertexCount];
            foreach (List<CrossCounterPair> list in radixBySecond)
            {
                if (list is null)
                    continue;

                foreach (CrossCounterPair pair in list)
                {
                    ThrowIfCancellationRequested();

                    // Get the radix where the pair should be inserted
                    r = radixByFirst[pair.First];
                    if (r is null)
                    {
                        r = new List<CrossCounterPair>();
                        radixByFirst[pair.First] = r;
                    }
                    r.Add(pair);
                }
            }

            #endregion

            // Build the accumulator tree
            int firstIndex = 1;
            while (firstIndex < pairCount)
            {
                firstIndex *= 2;
            }
            int treeSize = 2 * firstIndex - 1;
            --firstIndex;
            var tree = new CrossCounterTreeNode[treeSize];
            for (int i = 0; i < treeSize; ++i)
            {
                tree[i] = new CrossCounterTreeNode();
            }

            // Count the crossings
            int crossCount = 0;
            foreach (List<CrossCounterPair> list in radixByFirst)
            {
                if (list is null)
                    continue;

                foreach (CrossCounterPair pair in list)
                {
                    int index = pair.Second + firstIndex;
                    tree[index].Accumulator += pair.Weight;
                    switch (pair.Type)
                    {
                        case EdgeTypes.InnerSegment:
                            tree[index].InnerSegmentMarker = true;
                            break;
                        case EdgeTypes.NonInnerSegment:
                            tree[index].NonInnerSegmentQueue.Enqueue(pair.NonInnerSegment);
                            break;
                    }

                    while (index > 0)
                    {
                        ThrowIfCancellationRequested();

                        if (index % 2 > 0)
                        {
                            crossCount += tree[index + 1].Accumulator * pair.Weight;
                            switch (pair.Type)
                            {
                                case EdgeTypes.InnerSegment:
                                    Queue<SugiEdge> queue = tree[index + 1].NonInnerSegmentQueue;
                                    while (queue.Count > 0)
                                    {
                                        queue.Dequeue().Marked = true;
                                    }
                                    break;

                                case EdgeTypes.NonInnerSegment:
                                    if (tree[index + 1].InnerSegmentMarker)
                                    {
                                        pair.NonInnerSegment.Marked = true;
                                    }
                                    break;
                            }
                        }

                        index = (index - 1) / 2;
                        tree[index].Accumulator += pair.Weight;

                        switch (pair.Type)
                        {
                            case EdgeTypes.InnerSegment:
                                tree[index].InnerSegmentMarker = true;
                                break;
                            case EdgeTypes.NonInnerSegment:
                                tree[index].NonInnerSegmentQueue.Enqueue(pair.NonInnerSegment);
                                break;
                        }
                    }
                }
            }

            return crossCount;
        }

        [Pure]
        [NotNull, ItemNotNull]
        private static Queue<VertexGroup> GetContainerLikeItems(
            [NotNull, ItemNotNull] AlternatingLayer alternatingLayer,
            VertexTypes containerLikeVertexType)
        {
            var queue = new Queue<VertexGroup>();
            foreach (IData item in alternatingLayer)
            {
                var vertex = item as SugiVertex;
                if (vertex != null && vertex.Type == containerLikeVertexType)
                {
                    queue.Enqueue(new VertexGroup(vertex.Position) { Size = 1 });
                }
                else if (vertex is null)
                {
                    var container = (ISegmentContainer)item;
                    if (container.Count > 0)
                    {
                        queue.Enqueue(new VertexGroup(container.Position) { Size = container.Count });
                    }
                }
            }

            return queue;
        }
    }
}