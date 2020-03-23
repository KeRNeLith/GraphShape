using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using QuikGraph;
using QuikGraph.Algorithms.Search;
using GraphShape.Algorithms.EdgeRouting;
using JetBrains.Annotations;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    /// <summary>
    /// Sugiyama layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph> : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, SugiyamaLayoutParameters>, IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        [NotNull]
        private readonly SoftMutableHierarchicalGraph<SugiVertex, SugiEdge> _graph;

        [NotNull, ItemNotNull]
        private readonly VertexLayerCollection _layers = new VertexLayerCollection();
        private double _statusInPercent;

        private const int PercentOfPreparation = 5;
        private const int PercentOfSugiyama = 60;
        private const int PercentOfIncrementalExtension = 30;

        // Tags
        private const string IsolatedVerticesTag = "IsolatedVertices";
        private const string LoopsTag = "Loops";
        private const string GeneralEdgesTag = "GeneralEdges";
        private const string GeneralEdgesBetweenDifferentLayersTag = "GeneralEdgesBetweenDifferentLayers";
        private const string LongEdgesTag = "LongEdges"; // Long edges will be replaced with dummy vertices

        /// <inheritdoc />
        public IDictionary<TEdge, Point[]> EdgeRoutes { get; } = new Dictionary<TEdge, Point[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EfficientSugiyamaLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        /// <param name="edgeConverter">Edge converter.</param>
        public SugiyamaLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] SugiyamaLayoutParameters oldParameters,
            [NotNull, InstantHandle] Func<TEdge, EdgeTypes> edgeConverter)
            : this(visitedGraph, verticesSizes, null, oldParameters, edgeConverter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfficientSugiyamaLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        /// <param name="edgeConverter">Edge converter.</param>
        public SugiyamaLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] SugiyamaLayoutParameters oldParameters,
            [NotNull, InstantHandle] Func<TEdge, EdgeTypes> edgeConverter)
            : base(visitedGraph, verticesPositions, oldParameters)
        {
            if (verticesSizes is null)
                throw new ArgumentNullException(nameof(verticesSizes));
            if (edgeConverter is null)
                throw new ArgumentNullException(nameof(edgeConverter));

            _graph = ConvertGraph();

            #region Local function

            SoftMutableHierarchicalGraph<SugiVertex, SugiEdge> ConvertGraph()
            {
                // Create the graph with the new type
                var graph = new SoftMutableHierarchicalGraph<SugiVertex, SugiEdge>(true);

                var vertexDict = new Dictionary<TVertex, SugiVertex>();

                // Wrapping the vertices
                foreach (TVertex vertex in VisitedGraph.Vertices)
                {
                    Size size = verticesSizes[vertex];
                    size.Height += Parameters.VerticalGap;
                    size.Width += Parameters.HorizontalGap;
                    var wrapped = new SugiVertex(vertex, size);

                    graph.AddVertex(wrapped);
                    vertexDict[vertex] = wrapped;
                }

                // Create the new edges
                foreach (TEdge edge in VisitedGraph.Edges)
                {
                    var wrapped = new SugiEdge(edge, vertexDict[edge.Source], vertexDict[edge.Target], edgeConverter(edge));
                    graph.AddEdge(wrapped);
                }

                return graph;
            }

            #endregion
        }

        #region Filters - used in the preparation phase

        /// <summary>
        /// Removes the cycles from the given graph.
        /// It reverts some edges, so the cycles disappears.
        /// </summary>
        private void FilterCycles()
        {
            var cycleEdges = new List<SugiEdge>();
            var dfs = new DepthFirstSearchAlgorithm<SugiVertex, SugiEdge>(_graph);
            dfs.BackEdge += cycleEdges.Add;
            // Non-tree edges selected
            dfs.Compute();
            dfs.BackEdge -= cycleEdges.Add;

            // Put back the reverted ones
            foreach (SugiEdge edge in cycleEdges)
            {
                _graph.RemoveEdge(edge);

                var revertEdge = new SugiEdge(edge.Original, edge.Target, edge.Source, edge.Type);
                _graph.AddEdge(revertEdge);
            }
        }

        private static void FilterIsolatedVertices<TVertexType, TEdgeType>(
            [NotNull] ISoftMutableGraph<TVertexType, TEdgeType> graph)
            where TEdgeType : class, IEdge<TVertexType>
        {
            graph.HideVerticesIf(v => graph.Degree(v) == 0, IsolatedVerticesTag);
        }

        private static void FilterLoops<TVertexType, TEdgeType>(
            [NotNull] ISoftMutableGraph<TVertexType, TEdgeType> graph)
            where TEdgeType : class, IEdge<TVertexType>
            where TVertexType : class
        {
            graph.HideEdgesIf(e => e.Source == e.Target, LoopsTag);
        }

        /// <summary>
        /// First step of the algorithm.
        /// Filters the un-appropriate vertices and edges.
        /// </summary>
        protected void FiltersAndRemovals()
        {
            // Hide every edge but hierarchical ones
            _graph.HideEdges(_graph.GeneralEdges, GeneralEdgesTag);

            // Remove the cycles from the graph
            FilterCycles();

            // Remove every isolated vertex
            FilterIsolatedVertices(_graph);

            // Filter loops - edges with source = target
            FilterLoops(_graph);
        }

        #endregion

        /// <summary>
        /// Creates the layering of the graph. (Assigns every vertex to a layer.)
        /// </summary>
        private void AssignLayers()
        {
            var topologicalSort = new LayeredTopologicalSortAlgorithm<SugiVertex, SugiEdge>(_graph);
            topologicalSort.Compute();

            for (int i = 0; i < topologicalSort.LayerCount; ++i)
            {
                var vertexLayer = new VertexLayer(_graph, i, topologicalSort.Layers[i].ToList());
                _layers.Add(vertexLayer);
            }
        }

        #region Preparation for Sugiyama

        /// <summary>
        /// Minimizes the long of the hierarchical edges by putting
        /// down the vertices to the layer above its descendants.
        /// </summary>
        private void MinimizeHierarchicalEdgeLong()
        {
            if (!Parameters.MinimizeHierarchicalEdgeLong)
                return;

            for (int i = _layers.Count - 1; i >= 0; --i)
            {
                VertexLayer layer = _layers[i];
                foreach (SugiVertex vertex in layer.ToArray())
                {
                    if (_graph.OutHierarchicalEdgeCount(vertex) == 0)
                        continue;

                    // Put the vertex above the descendant on the highest layer
                    int newLayerIndex = _graph.OutHierarchicalEdges(vertex).Min(edge => edge.Target.LayerIndex - 1);
                    if (newLayerIndex != vertex.LayerIndex)
                    {
                        // We're changing layer
                        layer.Remove(vertex);
                        _layers[newLayerIndex].Add(vertex);
                    }
                }
            }
        }

        /// <summary>
        /// Long edges ( span(e) > 1 ) will be replaced by 
        /// span(e) edges (1 edge between every 2 neighbor layer)
        /// and span(e)-1 dummy vertices will be added to graph.
        /// </summary>
        private void ReplaceLongEdges()
        {
            // If an edge goes through multiple layers, we split the edge at every layer and insert a dummy node
            // (only for the hierarchical edges)
            foreach (SugiEdge edge in _graph.HierarchicalEdges.ToArray())
            {
                int sourceLayerIndex = edge.Source.LayerIndex;
                int targetLayerIndex = edge.Target.LayerIndex;

                if (Math.Abs(sourceLayerIndex - targetLayerIndex) <= 1)
                    continue; // span(e) <= 1, not long edge

                // The edge goes through multiple layers
                edge.IsLongEdge = true;
                _graph.HideEdge(edge, LongEdgesTag);

                // sourcelayer must be above the targetlayer
                if (targetLayerIndex < sourceLayerIndex)
                {
                    int c = targetLayerIndex;
                    targetLayerIndex = sourceLayerIndex;
                    sourceLayerIndex = c;
                }

                SugiVertex prev = edge.Source;
                for (int i = sourceLayerIndex + 1; i <= targetLayerIndex; ++i)
                {
                    // The last vertex is the Target, the other ones are dummy vertices
                    SugiVertex dummy;
                    if (i == targetLayerIndex)
                    {
                        dummy = edge.Target;
                    }
                    else
                    {
                        dummy = new SugiVertex(null, new Size(0, 0));
                        _graph.AddVertex(dummy);
                        _layers[i].Add(dummy);
                        edge.DummyVertices.Add(dummy);
                    }

                    _graph.AddEdge(new SugiEdge(edge.Original, prev, dummy, EdgeTypes.Hierarchical));
                    prev = dummy;
                }
            }
        }

        private static void ConstrainWidth()
        {
            // TODO
        }

        private void PrepareForSugiyama()
        {
            MinimizeHierarchicalEdgeLong();

            #region 1) Unhide general edges between vertices participating in the hierarchy

            var analyze = new HashSet<SugiVertex>();
            EdgeAction<SugiVertex, SugiEdge> edgeAction =
                edge =>
                {
                    analyze.Add(edge.Source);
                    analyze.Add(edge.Target);
                };
            _graph.EdgeUnhidden += edgeAction;
            _graph.UnhideEdgesIf(e => e.Type == EdgeTypes.General && _graph.ContainsVertex(e.Source) && _graph.ContainsVertex(e.Target));
            _graph.EdgeUnhidden -= edgeAction;

            #endregion

            #region 2) Move the vertices with general edges if possible

            foreach (SugiVertex vertex in analyze)
            {
                // Can be put down only if there is no hierarchical down output on the nearest layer
                if (_graph.OutHierarchicalEdgeCount(vertex) == 0)
                {
                    // General edges should be put near down layers
                    int newLayerIndex = _layers.Count;
                    foreach (SugiEdge edge in _graph.InGeneralEdges(vertex))
                    {
                        // No need to for further, cannot go down more
                        if (newLayerIndex == vertex.LayerIndex)
                            break;
                        if (edge.Source.LayerIndex >= vertex.LayerIndex && edge.Source.LayerIndex < newLayerIndex)
                            newLayerIndex = edge.Source.LayerIndex;
                    }

                    foreach (SugiEdge edge in _graph.OutGeneralEdges(vertex))
                    {
                        // No need to for further, cannot go down more
                        if (newLayerIndex == vertex.LayerIndex)
                            break;
                        if (edge.Target.LayerIndex >= vertex.LayerIndex && edge.Target.LayerIndex < newLayerIndex)
                            newLayerIndex = edge.Target.LayerIndex;
                    }

                    if (newLayerIndex < _layers.Count)
                    {
                        _layers[vertex.LayerIndex].Remove(vertex);
                        _layers[newLayerIndex].Add(vertex);
                    }
                }
            }

            #endregion

            // 3) Hide the general edges between different layers
            _graph.HideEdgesIf(
                e => e.Type == EdgeTypes.General && e.Source.LayerIndex != e.Target.LayerIndex,
                GeneralEdgesBetweenDifferentLayersTag);

            // Replace long edges with more segments and dummy vertices
            ReplaceLongEdges();

            ConstrainWidth();

            CopyPositions();
            OnIterationEnded("Preparation of the positions done.");
        }

        #endregion

        #region Sugiyama Layout

        /// <summary>
        /// Sweeps in one direction in the 1st Phase of the Sugiyama's algorithm.
        /// </summary>
        /// <param name="start">The index of the layer where the sweeping starts.</param>
        /// <param name="end">The index of the layer where the sweeping ends.</param>
        /// <param name="step">Step count.</param>
        /// <param name="barycenters">Kind of the barycentering (Up/Down-barycenters).</param>
        /// <param name="dirty">If this is a dirty sweep.</param>
        /// <param name="byRealPosition"></param>
        private bool SugiyamaPhase1Sweep(int start, int end, int step, Barycenters barycenters, bool dirty, bool byRealPosition)
        {
            bool hasOptimization = false;
            CrossCounts crossCounting = barycenters == Barycenters.Up ? CrossCounts.Up : CrossCounts.Down;
            bool sourceByMeasure = crossCounting == CrossCounts.Down;
            for (int i = start; i != end; i += step)
            {
                VertexLayer layer = _layers[i];
                int modifiedCrossing = 0;
                int originalCrossing = 0;

                if (!dirty)
                {
                    // Get the count of the edge crossings
                    originalCrossing = layer.CalculateCrossCount(crossCounting);
                }

                // Measure the vertices by the given barycenters
                layer.Measure(barycenters, byRealPosition);

                if (!dirty)
                {
                    // Get the modified crossing count
                    modifiedCrossing = layer.CalculateCrossCount(crossCounting, sourceByMeasure, !sourceByMeasure);
                }

                if (modifiedCrossing < originalCrossing || dirty)
                {
                    layer.SortByMeasure();
                    hasOptimization = true;
                }

                if (byRealPosition)
                {
                    HorizontalPositionAssignmentOnLayer(i, barycenters);
                    CopyPositionsSilent(false);
                }
                else
                {
                    CopyPositions();
                }

                OnIterationEnded($" Phase 1 sweepstep finished [{barycenters}-barycentering on layer {i}]");
            }

            return hasOptimization;
        }

        /// <returns>
        /// The index of the layer which is not ordered by <paramref name="barycenters"/> anymore.
        /// If all of the layers ordered, and phase2 sweep done it returns with -1.
        /// </returns>
        private int SugiyamaPhase2Sweep(int start, int end, int step, Barycenters barycenters, bool byRealPosition)
        {
            CrossCounts crossCountsDirection = barycenters == Barycenters.Up ? CrossCounts.Up : CrossCounts.Down;
            for (int i = start; i != end; i += step)
            {
                VertexLayer layer = _layers[i];

                // Switch the vertices with the same barycenters, if and only if there will be less barycenters
                layer.Measure(barycenters, byRealPosition);
                layer.FindBestPermutation(crossCountsDirection);

                if (byRealPosition)
                {
                    HorizontalPositionAssignmentOnLayer(i, barycenters);
                    CopyPositionsSilent(false);
                }
                else
                {
                    CopyPositions();
                }

                OnIterationEnded($" Phase 2 sweepstep finished [{barycenters}-barycentering on layer {i}]");

                if (i + step != end)
                {
                    VertexLayer nextLayer = _layers[i + step];
                    if (!nextLayer.IsOrderedByBarycenters(barycenters, byRealPosition))
                        return i + step;
                }
            }

            return -1;
        }

        private void SugiyamaDirtyPhase(bool byRealPosition)
        {
            if (_layers.Count < 2)
                return;

            const bool dirty = true;
            SugiyamaPhase1Sweep(1, _layers.Count, 1, Barycenters.Up, dirty, byRealPosition);
            SugiyamaPhase1Sweep(_layers.Count - 2, -1, -1, Barycenters.Down, dirty, byRealPosition);
        }

        [Pure]
        private bool SugiyamaPhase1(int startLayerIndex, Barycenters startBaryCentering, bool byRealPosition)
        {
            if (_layers.Count < 2)
                return false;

            const bool dirty = false;
            bool sweepDownOptimized = false;

            if (startBaryCentering == Barycenters.Up)
            {
                sweepDownOptimized = SugiyamaPhase1Sweep(
                    startLayerIndex == -1 ? 1 : startLayerIndex,
                    _layers.Count,
                    1,
                    Barycenters.Up,
                    dirty,
                    byRealPosition);

                startLayerIndex = -1;
            }

            bool sweepUpOptimized = SugiyamaPhase1Sweep(
                startLayerIndex == -1 ? _layers.Count - 2 : startLayerIndex,
                -1,
                -1,
                Barycenters.Down,
                dirty,
                byRealPosition);

            return sweepUpOptimized || sweepDownOptimized;
        }

        private void SugiyamaPhase2(out int unorderedLayerIndex, out Barycenters barycenters, bool byRealPosition)
        {
            // Sweeping up
            unorderedLayerIndex = SugiyamaPhase2Sweep(1, _layers.Count, 1, Barycenters.Up, byRealPosition);
            if (unorderedLayerIndex != -1)
            {
                barycenters = Barycenters.Up;
                return;
            }

            // Sweeping down
            unorderedLayerIndex = SugiyamaPhase2Sweep(_layers.Count - 2, -1, -1, Barycenters.Down, byRealPosition);
            barycenters = Barycenters.Down;
        }

        private void SugiyamaLayout()
        {
            bool barycenteringByRealPositions = Parameters.PositionCalculationMethod == PositionCalculationMethodTypes.PositionBased;
            if (Parameters.DirtyRound)
            {
                // Start with a dirty round (sort by barycenters, even if the number of the crossings will rise)
                SugiyamaDirtyPhase(barycenteringByRealPositions);
            }

            bool changed = true;
            int iteration1Left = Parameters.Phase1IterationCount;
            int iteration2Left = Parameters.Phase2IterationCount;
            double maxIterations = iteration1Left * iteration2Left;

            int startLayerIndex = -1;
            Barycenters startBarycentering = Barycenters.Up;

            while (changed && (iteration1Left > 0 || iteration2Left > 0))
            {
                changed = false;

                // Phase 1 - while there's any optimization
                while (iteration1Left > 0 && SugiyamaPhase1(startLayerIndex, startBarycentering, barycenteringByRealPositions))
                {
                    --iteration1Left;
                    changed = true;
                }

                startLayerIndex = -1;
                startBarycentering = Barycenters.Up;

                // Phase 2
                if (iteration2Left > 0)
                {
                    SugiyamaPhase2(out startLayerIndex, out startBarycentering, barycenteringByRealPositions);
                    --iteration2Left;
                }

                // Phase fallback
                if (startLayerIndex != -1)
                {
                    iteration1Left = Parameters.Phase1IterationCount;
                    changed = true;
                }

                _statusInPercent += PercentOfSugiyama / maxIterations;
            }
        }

        #endregion

        #region Last phase - Horizontal Assignment, edge routing, copying of the positions

        private void AssignPriorities()
        {
            foreach (SugiVertex vertex in _graph.Vertices)
                vertex.Priority = vertex.IsDummyVertex ? int.MaxValue : _graph.HierarchicalEdgeCountFor(vertex);
        }

        [Pure]
        private double CalculateOverlap([NotNull] SugiVertex a, [NotNull] SugiVertex b, double plusGap = 0)
        {
            Debug.Assert(a != null);
            Debug.Assert(b != null);

            return Math.Max(
                0,
                (b.Size.Width + a.Size.Width) * 0.5 + plusGap + Parameters.HorizontalGap - (b.RealPosition.X - a.RealPosition.X));
        }

        private void HorizontalPositionAssignmentOnLayer(int layerIndex, Barycenters barycenters)
        {
            VertexLayer layer = _layers[layerIndex];

            // Compute where the vertices should be placed
            layer.Measure(barycenters, true);
            layer.CalculateSubPriorities();

            // Set the RealPositions to NaN
            foreach (SugiVertex vertex in layer)
                vertex.RealPosition.X = float.NaN;

            // Set the positions in the order of the priorities, start with the lower priorities
            foreach (SugiVertex v in from vertex in layer orderby vertex.Priority, vertex.SubPriority select vertex)
            {
                // First set the new position
                v.RealPosition.X = v.Measure;

                // Check if there's any overlap between the actual vertex and the vertices which position has already been set
                SugiVertex v1 = v;
                SugiVertex[] alreadySetVertices = layer
                    .Where(vertex => !double.IsNaN(vertex.RealPosition.X) && vertex != v1)
                    .ToArray();

                if (alreadySetVertices.Length == 0)
                {
                    // There can't be any overlap
                    continue;
                }

                // Get the index of the 'v' vertex between the vertices which position has already been set
                int indexOfV;
                for (indexOfV = 0;
                    indexOfV < alreadySetVertices.Length && alreadySetVertices[indexOfV].Position < v.Position;
                    ++indexOfV)
                {
                }

                SugiVertex leftNeighbor = null;
                SugiVertex rightNeighbor = null;
                double leftOverlap = 0;
                double rightOverlap = 0;

                // Check the overlap with vertex on the left
                if (indexOfV > 0)
                {
                    leftNeighbor = alreadySetVertices[indexOfV - 1];
                    leftOverlap = CalculateOverlap(leftNeighbor, v);
                }

                if (indexOfV < alreadySetVertices.Length)
                {
                    rightNeighbor = alreadySetVertices[indexOfV];
                    rightOverlap = CalculateOverlap(v, rightNeighbor);
                }

                // ReSharper disable PossibleNullReferenceException
                // Only one neighbor overlaps
                if (leftOverlap > 0 && IsZero(rightOverlap))
                {
                    if (leftNeighbor.Priority == v.Priority)
                    {
                        double leftMove = leftOverlap * 0.5;
                        if (rightNeighbor != null)
                            rightOverlap = CalculateOverlap(v, rightNeighbor, leftMove);
                        leftNeighbor.RealPosition.X -= leftMove;
                        v.RealPosition.X += leftMove;
                        if (rightOverlap > 0)
                        {
                            if (v.Priority == rightNeighbor.Priority)
                            {
                                double rightMove = rightOverlap * 0.5;
                                rightNeighbor.RealPosition.X += rightMove;
                                v.RealPosition.X -= rightMove;
                                leftNeighbor.RealPosition.X -= rightMove;
                            }
                            else
                            {
                                rightNeighbor.RealPosition.X += rightOverlap;
                            }
                        }
                    }
                    else
                    {
                        leftNeighbor.RealPosition.X -= leftOverlap;
                    }
                }
                else if (IsZero(leftOverlap) && rightOverlap > 0)
                {
                    if (v.Priority == rightNeighbor.Priority)
                    {
                        double rightMove = rightOverlap * 0.5;
                        if (leftNeighbor != null)
                            leftOverlap = CalculateOverlap(leftNeighbor, v, rightMove);
                        rightNeighbor.RealPosition.X += rightMove;
                        v.RealPosition.X -= rightMove;
                        if (leftOverlap > 0)
                        {
                            if (leftNeighbor.Priority == v.Priority)
                            {
                                double leftMove = leftOverlap * 0.5;
                                leftNeighbor.RealPosition.X -= leftMove;
                                v.RealPosition.X += leftMove;
                                rightNeighbor.RealPosition.X += leftMove;
                            }
                            else
                            {
                                leftNeighbor.RealPosition.X -= leftOverlap;
                            }
                        }
                    }
                    else
                    {
                        rightNeighbor.RealPosition.X += rightOverlap;
                    }
                }
                else if (leftOverlap > 0 && rightOverlap > 0)
                {
                    // If both neighbor overlapped
                    // priorities equals, 1 priority lower, 2 priority lower
                    if (leftNeighbor.Priority < v.Priority && v.Priority == rightNeighbor.Priority)
                    {
                        double rightMove = rightOverlap * 0.5;
                        rightNeighbor.RealPosition.X += rightMove;
                        v.RealPosition.X -= rightMove;
                        leftNeighbor.RealPosition.X -= (leftOverlap + rightMove);
                    }
                    else if (leftNeighbor.Priority == v.Priority && v.Priority > rightNeighbor.Priority)
                    {
                        double leftMove = leftOverlap * 0.5;
                        leftNeighbor.RealPosition.X -= leftMove;
                        v.RealPosition.X += leftMove;
                        rightNeighbor.RealPosition.X = (rightOverlap + leftMove);
                    }
                    else
                    {
                        // Priorities of the neighbors are lower, or equal
                        leftNeighbor.RealPosition.X -= leftOverlap;
                        rightNeighbor.RealPosition.X += rightOverlap;
                    }
                }
                // ReSharper restore PossibleNullReferenceException

                // The vertices on the left side of the leftNeighbor will be moved, if they overlap
                if (leftOverlap > 0)
                {
                    for (int index = indexOfV - 1;
                        index > 0 && (leftOverlap = CalculateOverlap(alreadySetVertices[index - 1], alreadySetVertices[index])) > 0;
                        --index)
                    {
                        alreadySetVertices[index - 1].RealPosition.X -= leftOverlap;
                    }
                }

                // The vertices on the right side of the rightNeighbor will be moved, if they overlap
                if (rightOverlap > 0)
                {
                    for (int index = indexOfV;
                        index < alreadySetVertices.Length - 1
                        && (rightOverlap = CalculateOverlap(alreadySetVertices[index], alreadySetVertices[index + 1])) > 0;
                        ++index)
                    {
                        alreadySetVertices[index + 1].RealPosition.X += rightOverlap;
                    }
                }
            }
        }

        private void HorizontalPositionAssignmentSweep(int start, int end, int step, Barycenters barycenters)
        {
            for (int i = start; i != end; i += step)
                HorizontalPositionAssignmentOnLayer(i, barycenters);
        }

        private void HorizontalPositionAssignment()
        {
            // Sweeping up & down, assigning the positions for the vertices in the order of the priorities
            // positions computed with the barycenters method, based on the real positions
            AssignPriorities();

            if (_layers.Count > 1)
            {
                HorizontalPositionAssignmentSweep(1, _layers.Count, 1, Barycenters.Up);
                HorizontalPositionAssignmentSweep(_layers.Count - 2, -1, -1, Barycenters.Down);
            }
        }

        private void AssignPositions()
        {
            // Initialize positions
            double verticalPos = 0;
            for (int i = 0; i < _layers.Count; ++i)
            {
                double pos = 0;
                double layerHeight = _layers[i].Height;
                foreach (SugiVertex vertex in _layers[i])
                {
                    vertex.RealPosition.X = pos;
                    vertex.RealPosition.Y = i == 0
                        ? layerHeight - vertex.Size.Height
                        : verticalPos + layerHeight * (float)0.5;

                    pos += vertex.Size.Width + Parameters.HorizontalGap;
                }
                verticalPos += layerHeight + Parameters.VerticalGap;
            }

            // Assign the horizontal positions
            HorizontalPositionAssignment();
        }

        private void CopyPositionsSilent(bool shouldTranslate = true)
        {
            // Calculate the topLeft position
            var translation = new Vector(float.PositiveInfinity, float.PositiveInfinity);
            if (shouldTranslate)
            {
                foreach (SugiVertex vertex in _graph.Vertices)
                {
                    if (double.IsNaN(vertex.RealPosition.X) || double.IsNaN(vertex.RealPosition.Y))
                        continue;

                    translation.X = Math.Min(vertex.RealPosition.X, translation.X);
                    translation.Y = Math.Min(vertex.RealPosition.Y, translation.Y);
                }
                translation *= -1;
                translation.X += Parameters.VerticalGap / 2;
                translation.Y += Parameters.HorizontalGap / 2;

                // Translate with the topLeft position
                foreach (SugiVertex vertex in _graph.Vertices)
                    vertex.RealPosition += translation;
            }
            else
            {
                translation = new Vector(0, 0);
            }

            // Copy the positions of the vertices
            VerticesPositions.Clear();
            foreach (SugiVertex vertex in _graph.Vertices.Where(v => !v.IsDummyVertex))
            {
                Point pos = vertex.RealPosition;
                if (!shouldTranslate)
                {
                    pos.X += vertex.Size.Width * 0.5 + translation.X;
                    pos.Y += vertex.Size.Height * 0.5 + translation.Y;
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                // Justification: Not dummy vertex (only dummy are null)
                VerticesPositions[vertex.Original] = pos;
            }

            // Copy the edge routes
            EdgeRoutes.Clear();
            foreach (SugiEdge edge in _graph.HiddenEdges.Where(e => e.IsLongEdge))
            {
                EdgeRoutes[edge.Original] = edge.IsReverted
                    ? edge.DummyVertices.Reverse().Select(dv => dv.RealPosition).ToArray()
                    : edge.DummyVertices.Select(dv => dv.RealPosition).ToArray();
            }
        }

        /// <summary>
        /// Copies the coordinates of the vertices to the <see cref="LayoutAlgorithmBase{TVertex,TEdge,TGraph}.VerticesPositions"/> dictionary.
        /// </summary>
        protected void CopyPositions()
        {
            AssignPositions();

            CopyPositionsSilent();
        }

        #endregion

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            // Phase 1 - Filters & Removals
            FiltersAndRemovals();
            _statusInPercent = PercentOfPreparation;

            // Phase 2 - Layer assignment
            AssignLayers();

            // Phase 3 - Crossing reduction
            PrepareForSugiyama();
            SugiyamaLayout();
            _statusInPercent = PercentOfPreparation + PercentOfSugiyama;

            // Phase 4 - Horizontal position assignment
            CopyPositions();
            OnIterationEnded("Position adjusting finished");

            // Phase 5 - Incremental extension, add vertices connected with only general edges
            _statusInPercent = PercentOfPreparation + PercentOfSugiyama + PercentOfIncrementalExtension;
            _statusInPercent = 100;
        }

        private void OnIterationEnded([NotNull] string message)
        {
            OnIterationEnded(0, _statusInPercent, message, true);
        }

        #endregion
    }
}