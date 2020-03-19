using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.EdgeRouting;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    /// <summary>
    /// Efficient Sugiyama layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public partial class EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, EfficientSugiyamaLayoutParameters>
        , IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// The copy of the <see cref="LayoutAlgorithmBase{TVertex,TEdge,TGraph}.VisitedGraph"/> which should be laid out.
        /// </summary>
        private IMutableBidirectionalGraph<SugiVertex, SugiEdge> _graph;

        [NotNull]
        private readonly IDictionary<TEdge, IList<SugiVertex>> _dummyVerticesOfEdges =
            new Dictionary<TEdge, IList<SugiVertex>>();

        [CanBeNull]
        private readonly IDictionary<TVertex, Size> _verticesSizes;

        [NotNull]
        private readonly IDictionary<TVertex, SugiVertex> _verticesMap =
            new Dictionary<TVertex, SugiVertex>();

        /// <summary>
        /// Isolated vertices in the visited graph, which will be handled only in
        /// the last step of the layout.
        /// </summary>
        [ItemNotNull]
        private SugiVertex[] _isolatedVertices;

        /// <summary>
        /// It stores the vertices or segments which inside the layers.
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly IList<IList<SugiVertex>> _layers = new List<IList<SugiVertex>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EfficientSugiyamaLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public EfficientSugiyamaLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] EfficientSugiyamaLayoutParameters oldParameters)
            : this(visitedGraph, null, verticesSizes, oldParameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfficientSugiyamaLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public EfficientSugiyamaLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] EfficientSugiyamaLayoutParameters oldParameters)
            : base(visitedGraph, verticesPositions, oldParameters)
        {
            _verticesSizes = verticesSizes;
        }

        /// <summary>
        /// Initializes the <see cref="_graph"/> field which stores the graph that we operate on.
        /// </summary>
        private void CopyToWorkingGraph()
        {
            // Make a copy of the original graph
            _graph = new BidirectionalGraph<SugiVertex, SugiEdge>();

            // Copy the vertices
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                var size = default(Size);
                _verticesSizes?.TryGetValue(vertex, out size);

                var vertexWrapper = new SugiVertex(vertex, size);
                _graph.AddVertex(vertexWrapper);
                _verticesMap[vertex] = vertexWrapper;
            }

            // Copy the edges
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                var edgeWrapper = new SugiEdge(edge, _verticesMap[edge.Source], _verticesMap[edge.Target]);
                _graph.AddEdge(edgeWrapper);
            }
        }

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            CopyToWorkingGraph();

            // First step
            PrepareGraph();

            BuildSparseNormalizedGraph();
            DoCrossingMinimizations();
            CalculatePositions();
        }

        #endregion

        #region IEdgeRoutingAlgorithm<TVertex,TEdge,TGraph>

        /// <inheritdoc />
        public IDictionary<TEdge, Point[]> EdgeRoutes { get; } = new Dictionary<TEdge, Point[]>();

        #endregion
    }
}