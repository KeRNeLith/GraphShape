using System;
using System.Collections.Generic;
using System.Linq;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        [NotNull]
        private readonly IMutableBidirectionalGraph<Data, IEdge<Data>> _sparseCompactionGraph
            = new BidirectionalGraph<Data, IEdge<Data>>();

        private double[] _layerSizes;
        private double[] _layerPositions;

        // Vertical layout
        //   >
        // \/
        // Layer1: Slice1 Slice2 ... SliceN
        // Layer2: Slice1 Slice2 ... SliceN
        // ...
        // LayerN: Slice1 Slice2 ... SliceN

        // Horizontal layout
        //   >
        // \/
        // Slice1: Layer1 Layer2 ... LayerN
        // Slice2: Layer1 Layer2 ... LayerN
        // ...
        // SliceN: Layer1 Layer2 ... LayerN

        // Some namings are based on a vertical layout

        private enum LeftRightMode : byte
        {
            Left = 0,
            Right = 1
        }

        private enum UpperLowerEdges : byte
        {
            Upper = 0,
            Lower = 1
        }

        [Pure]
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsVerticalLayout()
        {
            return Parameters.Direction == LayoutDirection.TopToBottom
                   || Parameters.Direction == LayoutDirection.BottomToTop;
        }

        private void CalculatePositions()
        {
            PutBackIsolatedVertices();
            CalculateLayerSizesAndPositions();
            CalculateLayerPositions();

            if (Parameters.PositionMode < 0 || Parameters.PositionMode == 0)
                CalculateSlicePositions(LeftRightMode.Left, UpperLowerEdges.Upper);
            if (Parameters.PositionMode < 0 || Parameters.PositionMode == 1)
                CalculateSlicePositions(LeftRightMode.Right, UpperLowerEdges.Upper);
            if (Parameters.PositionMode < 0 || Parameters.PositionMode == 2)
                CalculateSlicePositions(LeftRightMode.Left, UpperLowerEdges.Lower);
            if (Parameters.PositionMode < 0 || Parameters.PositionMode == 3)
                CalculateSlicePositions(LeftRightMode.Right, UpperLowerEdges.Lower);

            CalculateRealPositions();
            DoEdgeRouting();

            SavePositions();
        }

        private void CalculateOnlyIsolatedVerticesPositions()
        {
            PutBackIsolatedVertices();

            // Make a cubic display of all isolated vertices
            int nbVerticesPerSide = (int) Math.Ceiling(Math.Sqrt(VisitedGraph.VertexCount));

            double maxWidth = _verticesSizes?.Max(v => v.Value.Width) ?? 0;
            double maxHeight = _verticesSizes?.Max(v => v.Value.Height) ?? 0;

            int vertexCounter = 1; 
            double xOffset = 0;
            double yOffset = 0;

            if (IsVerticalLayout())
            {
                foreach (SugiVertex vertex in _isolatedVertices)
                {
                    VerticesPositions[vertex.OriginalVertex] = new Point(xOffset, yOffset);

                    yOffset += Parameters.LayerGap + maxHeight;

                    if (vertexCounter % nbVerticesPerSide == 0)
                    {
                        xOffset += Parameters.SliceGap + maxWidth;
                        yOffset = 0;
                    }

                    ++vertexCounter;
                }
            }
            else
            {
                foreach (SugiVertex vertex in _isolatedVertices)
                {
                    VerticesPositions[vertex.OriginalVertex] = new Point(xOffset, yOffset);

                    xOffset += Parameters.LayerGap + maxWidth;

                    if (vertexCounter % nbVerticesPerSide == 0)
                    {
                        yOffset += Parameters.SliceGap + maxHeight;
                        xOffset = 0;
                    }

                    ++vertexCounter;
                }
            }
        }

        private void PutBackIsolatedVertices()
        {
            _sparseCompactionGraph.AddVertexRange(_isolatedVertices);
            _graph.AddVertexRange(_isolatedVertices);

            // Null if no edge exist (only isolated vertices graph)
            if (_sparseCompactionByLayerBackup != null)
            {
                int layer = 0;
                SugiVertex prevIsolatedVertex = null;
                foreach (SugiVertex isolatedVertex in _isolatedVertices)
                {
                    _layers[layer].Add(isolatedVertex);
                    isolatedVertex.LayerIndex = layer;
                    isolatedVertex.Position = _layers[layer].Count - 1;

                    Edge<Data> lastOnLayer = _sparseCompactionByLayerBackup[layer].LastOrDefault();
                    var edge = lastOnLayer is null
                        ? new Edge<Data>(_layers[layer][0], isolatedVertex)
                        : new Edge<Data>(lastOnLayer.Target, isolatedVertex);

                    _sparseCompactionByLayerBackup[layer].Add(edge);
                    _sparseCompactionGraph.AddEdge(edge);

                    if (layer > 0 && prevIsolatedVertex != null)
                    {
                        _graph.AddEdge(new SugiEdge(default(TEdge), prevIsolatedVertex, isolatedVertex));
                    }

                    layer = (layer + 1) % _layers.Count;
                    prevIsolatedVertex = isolatedVertex;
                }
            }
        }

        private void DoEdgeRouting()
        {
            switch (Parameters.EdgeRouting)
            {
                case SugiyamaEdgeRouting.Traditional:
                    DoTraditionalEdgeRouting();
                    break;
                case SugiyamaEdgeRouting.Orthogonal:
                    DoOrthogonalEdgeRouting();
                    break;
            }
        }

        private void DoTraditionalEdgeRouting()
        {
            foreach (KeyValuePair<TEdge, IList<SugiVertex>> pair in _dummyVerticesOfEdges)
            {
                var routePoints = new Point[pair.Value.Count];
                for (int i = 0; i < pair.Value.Count; ++i)
                {
                    SugiVertex vertex = pair.Value[i];
                    routePoints[i] = IsVerticalLayout()
                        ? new Point(vertex.SlicePosition, vertex.LayerPosition)
                        : new Point(vertex.LayerPosition, vertex.SlicePosition);
                }

                EdgeRoutes[pair.Key] = routePoints;
            }
        }

        private void DoOrthogonalEdgeRouting()
        {
            bool isVerticalLayout = IsVerticalLayout();
            AssignEdgesRoutes(isVerticalLayout);
            AssignDummyVerticesEdgesRoutes(isVerticalLayout);
        }

        private void AssignEdgesRoutes(bool isVerticalLayout)
        {
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                var orthogonalRoutePoints = new Point[2];
                SugiVertex sourceVertex = _verticesMap[edge.Source];
                SugiVertex targetVertex = _verticesMap[edge.Target];
                bool notSwitched = sourceVertex.LayerIndex < targetVertex.LayerIndex;
                int sourceIndex = notSwitched ? 0 : 1;
                int targetIndex = notSwitched ? 1 : 0;

                if (isVerticalLayout)
                {
                    orthogonalRoutePoints[sourceIndex] = new Point
                    {
                        X = sourceVertex.SlicePosition,
                        Y = _layerPositions[sourceVertex.LayerIndex] + _layerSizes[sourceVertex.LayerIndex] + Parameters.LayerGap / 2.0
                    };
                    orthogonalRoutePoints[targetIndex] = new Point
                    {
                        X = targetVertex.SlicePosition,
                        Y = _layerPositions[targetVertex.LayerIndex] - Parameters.LayerGap / 2.0
                    };
                }
                else
                {
                    orthogonalRoutePoints[sourceIndex] = new Point
                    {
                        X = _layerPositions[sourceVertex.LayerIndex] + _layerSizes[sourceVertex.LayerIndex] + Parameters.LayerGap / 2.0,
                        Y = sourceVertex.SlicePosition,
                    };
                    orthogonalRoutePoints[targetIndex] = new Point
                    {
                        X = _layerPositions[targetVertex.LayerIndex] - Parameters.LayerGap / 2.0,
                        Y = targetVertex.SlicePosition,
                    };
                }

                EdgeRoutes[edge] = orthogonalRoutePoints;
            }
        }

        private void AssignDummyVerticesEdgesRoutes(bool isVerticalLayout)
        {
            foreach (KeyValuePair<TEdge, IList<SugiVertex>> pair in _dummyVerticesOfEdges)
            {
                Point[] orthogonalRoutePoints = EdgeRoutes[pair.Key];

                var routePoints = new Point[pair.Value.Count + 4];
                routePoints[0] = orthogonalRoutePoints[0];
                routePoints[pair.Value.Count + 3] = orthogonalRoutePoints[1];
                for (int i = 0; i < pair.Value.Count; ++i)
                {
                    SugiVertex vertex = pair.Value[i];
                    routePoints[i + 2] = isVerticalLayout
                        ? new Point(vertex.SlicePosition, vertex.LayerPosition)
                        : new Point(vertex.LayerPosition, vertex.SlicePosition);
                }

                routePoints[1] = isVerticalLayout
                    ? new Point(routePoints[2].X, routePoints[0].Y)
                    : new Point(routePoints[0].X, routePoints[2].Y);
                routePoints[pair.Value.Count + 2] = new Point(
                    routePoints[pair.Value.Count + 1].X,
                    routePoints[pair.Value.Count + 3].Y);
                EdgeRoutes[pair.Key] = routePoints;
            }
        }

        private void SavePositions()
        {
            foreach (SugiVertex vertex in _graph.Vertices)
            {
                if (vertex.Type == VertexTypes.Original)
                {
                    VerticesPositions[vertex.OriginalVertex] = IsVerticalLayout()
                        ? new Point(vertex.SlicePosition, vertex.LayerPosition)
                        : new Point(vertex.LayerPosition, vertex.SlicePosition);
                }
            }
        }

        private void CalculateLayerSizesAndPositions()
        {
            _layerSizes = new double[_layers.Count];
            for (int i = 0; i < _layers.Count; ++i)
            {
                _layerSizes[i] = _layers[i].Max(v => IsVerticalLayout() ? v.Size.Height : v.Size.Width);
            }

            double layerDistance = Parameters.LayerGap;
            _layerPositions = new double[_layers.Count];
            _layerPositions[0] = 0;
            for (int i = 1; i < _layers.Count; ++i)
            {
                _layerPositions[i] = _layerPositions[i - 1] + _layerSizes[i - 1] + layerDistance;
            }
        }

        private void CalculateLayerPositions()
        {
            foreach (SugiVertex vertex in _graph.Vertices)
            {
                double size = IsVerticalLayout()
                    ? vertex.Size.Height
                    : vertex.Size.Width;
                vertex.LayerPosition = _layerPositions[vertex.LayerIndex] + (size <= 0 ? _layerSizes[vertex.LayerIndex] : size) / 2.0;
            }
        }

        /// <summary>
        /// Calculates the slice positions based on the selected modes.
        /// </summary>
        /// <param name="leftRightMode">Mode of the vertical alignment.</param>
        /// <param name="upperLowerEdges">Alignment based on which edges (upper or lower ones).</param>
        private void CalculateSlicePositions(LeftRightMode leftRightMode, UpperLowerEdges upperLowerEdges)
        {
            int modeIndex = (byte)upperLowerEdges * 2 + (byte)leftRightMode;
            InitializeRootsAndAligns(modeIndex);
            DoAlignment(modeIndex, leftRightMode, upperLowerEdges);
            InitializeSinksAndShifts(modeIndex);
            DoSliceCompaction(modeIndex, leftRightMode, upperLowerEdges);
        }

        private void InitializeRootsAndAligns(int modeIndex)
        {
            foreach (SugiVertex vertex in _graph.Vertices)
            {
                vertex.Roots[modeIndex] = vertex;
                vertex.Aligns[modeIndex] = vertex;
                vertex.BlockWidths[modeIndex] = IsVerticalLayout() ? vertex.Size.Width : vertex.Size.Height;
            }
        }

        private void DoAlignment(int modeIndex, LeftRightMode leftRightMode, UpperLowerEdges upperLowerEdges)
        {
            int layerStart, layerEnd, layerStep;
            if (upperLowerEdges == UpperLowerEdges.Upper)
            {
                layerStart = 0;
                layerEnd = _layers.Count;
                layerStep = 1;
            }
            else
            {
                layerStart = _layers.Count - 1;
                layerEnd = -1;
                layerStep = -1;
            }

            for (int i = layerStart; i != layerEnd; i += layerStep)
            {
                int r = leftRightMode == LeftRightMode.Left ? int.MinValue : int.MaxValue;
                IList<SugiVertex> layer = _layers[i];
                int vertexStart, vertexEnd, vertexStep;

                if (leftRightMode == LeftRightMode.Left)
                {
                    vertexStart = 0;
                    vertexEnd = layer.Count;
                    vertexStep = 1;
                }
                else
                {
                    vertexStart = layer.Count - 1;
                    vertexEnd = -1;
                    vertexStep = -1;
                }

                for (int j = vertexStart; j != vertexEnd; j += vertexStep)
                {
                    SugiVertex vertex = layer[j];
                    switch (vertex.Type)
                    {
                        case VertexTypes.Original:
                        case VertexTypes.RVertex:
                        case VertexTypes.PVertex when upperLowerEdges == UpperLowerEdges.Upper:
                        case VertexTypes.QVertex when upperLowerEdges == UpperLowerEdges.Lower:
                            {
                                SugiEdge[] neighborEdges = upperLowerEdges == UpperLowerEdges.Upper
                                    ? _graph.InEdges(vertex).OrderBy(e => e.Source.Position).ToArray()
                                    : _graph.OutEdges(vertex).OrderBy(e => e.Target.Position).ToArray();
                                if (neighborEdges.Length <= 0)
                                    continue;

                                int c1 = (int)Math.Floor((neighborEdges.Length + 1) / 2.0) - 1;
                                int c2 = (int)Math.Ceiling((neighborEdges.Length + 1) / 2.0) - 1;
                                int[] medians;
                                if (c1 == c2)
                                {
                                    medians = new[] { c1 };
                                }
                                else
                                {
                                    medians = leftRightMode == LeftRightMode.Left
                                        ? new[] { c1, c2 }
                                        : new[] { c2, c1 };
                                }

                                foreach (int median in medians)
                                {
                                    if (vertex.Aligns[modeIndex] != vertex)
                                        continue;

                                    SugiEdge edge = neighborEdges[median];
                                    SugiVertex neighbor = edge.GetOtherVertex(vertex);

                                    if (!edge.Marked &&
                                        (leftRightMode == LeftRightMode.Left && r < neighbor.Position
                                         ||
                                         leftRightMode == LeftRightMode.Right && r > neighbor.Position))
                                    {
                                        neighbor.Aligns[modeIndex] = vertex;
                                        neighbor.BlockWidths[modeIndex] = Math.Max(neighbor.BlockWidths[modeIndex], vertex.Size.Width);
                                        vertex.Roots[modeIndex] = neighbor.Roots[modeIndex];
                                        vertex.Aligns[modeIndex] = vertex.Roots[modeIndex];
                                        r = neighbor.Position;
                                    }
                                }
                                break;
                            }

                        case VertexTypes.PVertex:
                            // Align the segment of the PVertex
                            vertex.Roots[modeIndex] = vertex.Segment.QVertex.Roots[modeIndex];
                            vertex.Aligns[modeIndex] = vertex.Roots[modeIndex];
                            r = vertex.Segment.Position;
                            break;

                        case VertexTypes.QVertex:
                            // Align the segment of the QVertex
                            vertex.Roots[modeIndex] = vertex.Segment.PVertex.Roots[modeIndex];
                            vertex.Aligns[modeIndex] = vertex.Roots[modeIndex];
                            r = vertex.Segment.Position;
                            break;
                    }
                }
            }
        }

        private void InitializeSinksAndShifts(int modeIndex)
        {
            foreach (SugiVertex vertex in _graph.Vertices)
            {
                vertex.Sinks[modeIndex] = vertex;
                vertex.Shifts[modeIndex] = double.PositiveInfinity;
                vertex.SlicePositions[modeIndex] = double.NaN;
            }
        }

        private void DoSliceCompaction(int modeIndex, LeftRightMode leftRightMode, UpperLowerEdges upperLowerEdges)
        {
            foreach (SugiVertex vertex in _graph.Vertices)
            {
                if (vertex.Roots[modeIndex] == vertex)
                    PlaceBlock(modeIndex, leftRightMode, upperLowerEdges, vertex);
            }

            foreach (SugiVertex vertex in _graph.Vertices)
            {
                vertex.SlicePositions[modeIndex] = vertex.Roots[modeIndex].SlicePositions[modeIndex];
                if (vertex.Roots[modeIndex].Sinks[modeIndex].Shifts[modeIndex] < double.PositiveInfinity
                    && vertex.Roots[modeIndex] == vertex)
                {
                    vertex.SlicePositions[modeIndex] += vertex.Roots[modeIndex].Sinks[modeIndex].Shifts[modeIndex];
                }
            }
        }

        private void PlaceBlock(
            int modeIndex,
            LeftRightMode leftRightMode,
            UpperLowerEdges upperLowerEdges,
            [NotNull] SugiVertex v)
        {
            if (!double.IsNaN(v.SlicePositions[modeIndex]))
                return;

            double delta = Parameters.SliceGap;
            v.SlicePositions[modeIndex] = 0;
            Data w = v;
            do
            {
                var wVertex = w as SugiVertex;
                var wSegment = w as Segment;
                if (_sparseCompactionGraph.ContainsVertex(w) &&
                    (leftRightMode == LeftRightMode.Left && _sparseCompactionGraph.InDegree(w) > 0
                     || leftRightMode == LeftRightMode.Right && _sparseCompactionGraph.OutDegree(w) > 0))
                {
                    IEnumerable<IEdge<Data>> edges = leftRightMode == LeftRightMode.Left
                        ? _sparseCompactionGraph.InEdges(w)
                        : _sparseCompactionGraph.OutEdges(w);

                    foreach (IEdge<Data> edge in edges)
                    {
                        SugiVertex u;
                        Data predecessor = leftRightMode == LeftRightMode.Left ? edge.Source : edge.Target;
                        if (predecessor is SugiVertex vertex)
                        {
                            u = vertex.Roots[modeIndex];
                        }
                        else
                        {
                            var segment = (Segment)predecessor;
                            u = upperLowerEdges == UpperLowerEdges.Upper ? segment.PVertex.Roots[modeIndex] : segment.QVertex.Roots[modeIndex];
                        }

                        PlaceBlock(modeIndex, leftRightMode, upperLowerEdges, u);

                        if (v.Sinks[modeIndex] == v)
                            v.Sinks[modeIndex] = u.Sinks[modeIndex];

                        double xDelta = delta
                                        + (
                                            (wVertex?.Size.Width ?? 0.0)
                                            + (predecessor is SugiVertex sugiVertex
                                                ? sugiVertex.Size.Width
                                                : u.BlockWidths[modeIndex])
                                        ) / 2.0;

                        if (v.Sinks[modeIndex] != u.Sinks[modeIndex])
                        {
                            double s = leftRightMode == LeftRightMode.Left
                                ? v.SlicePositions[modeIndex] - u.SlicePositions[modeIndex] - xDelta
                                : u.SlicePositions[modeIndex] - v.SlicePositions[modeIndex] - xDelta;

                            u.Sinks[modeIndex].Shifts[modeIndex] = leftRightMode == LeftRightMode.Left
                                ? Math.Min(u.Sinks[modeIndex].Shifts[modeIndex], s)
                                : Math.Max(u.Sinks[modeIndex].Shifts[modeIndex], s);
                        }
                        else
                        {
                            v.SlicePositions[modeIndex] = leftRightMode == LeftRightMode.Left
                                ? Math.Max(v.SlicePositions[modeIndex], u.SlicePositions[modeIndex] + xDelta)
                                : Math.Min(v.SlicePositions[modeIndex], u.SlicePositions[modeIndex] - xDelta);
                        }
                    }
                }

                if (wSegment != null)
                {
                    w = upperLowerEdges == UpperLowerEdges.Upper ? wSegment.QVertex : wSegment.PVertex;
                }
                // ReSharper disable once PossibleNullReferenceException
                // Justification: If not a segment then it's a vertex
                else if (wVertex.Type == VertexTypes.PVertex && upperLowerEdges == UpperLowerEdges.Upper)
                {
                    w = wVertex.Segment;
                }
                else if (wVertex.Type == VertexTypes.QVertex && upperLowerEdges == UpperLowerEdges.Lower)
                {
                    w = wVertex.Segment;
                }
                else
                {
                    w = wVertex.Aligns[modeIndex];
                }
            } while (w != v);
        }

        private void CalculateRealPositions()
        {
            foreach (SugiVertex vertex in _graph.Vertices)
            {
                if (Parameters.PositionMode < 0)
                {
                    vertex.SlicePosition =
                        (vertex.SlicePositions[0]
                        + vertex.SlicePositions[1]
                        + vertex.SlicePositions[2]
                        + vertex.SlicePositions[3]) / 4.0;
                }
                else
                {
                    vertex.SlicePosition = vertex.SlicePositions[Parameters.PositionMode];
                }
            }
        }
    }
}
