using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using GraphShape.Algorithms.EdgeRouting;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Sugiyama layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, SugiyamaLayoutParameters>
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
        /// Initializes a new instance of the <see cref="SugiyamaLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        public SugiyamaLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] SugiyamaLayoutParameters parameters = null)
            : this(visitedGraph, null, null, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SugiyamaLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        public SugiyamaLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] SugiyamaLayoutParameters parameters = null)
            : this(visitedGraph, null, verticesSizes, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SugiyamaLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        public SugiyamaLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] SugiyamaLayoutParameters parameters = null)
            : base(visitedGraph, verticesPositions, parameters)
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
            switch (VisitedGraph.VertexCount)
            {
                case 0:
                    return;
                case 1:
                    VerticesPositions[VisitedGraph.Vertices.First()] = default(Point);
                    return;
                default:
                    RunSugiyama();
                    return;
            }

            #region Local function

            void RunSugiyama()
            {
                CopyToWorkingGraph();

                // First step
                PrepareGraph();

                // Graph is only made of isolated vertices
                if (_graph.IsEdgesEmpty)
                {
                    CalculateOnlyIsolatedVerticesPositions();
                }
                else
                {
                    BuildSparseNormalizedGraph();
                    DoCrossingMinimizations();
                    CalculatePositions();
                }
            }

            #endregion
        }

        #endregion

        #region IEdgeRoutingAlgorithm<TVertex,TEdge,TGraph>

        /// <inheritdoc />
        public IDictionary<TEdge, Point[]> EdgeRoutes { get; } = new Dictionary<TEdge, Point[]>();

        #endregion
    }
}