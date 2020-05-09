using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Random layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class RandomLayoutAlgorithm<TVertex, TEdge, TGraph> : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, RandomLayoutParameters>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        [NotNull]
        private readonly IDictionary<TVertex, Size> _verticesSizes;

        [NotNull]
        private readonly IDictionary<TVertex, RandomVertexType> _verticesTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="verticesTypes">Vertices types.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        public RandomLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] IDictionary<TVertex, RandomVertexType> verticesTypes,
            [CanBeNull] RandomLayoutParameters parameters = null)
            : this(visitedGraph, null, verticesSizes, verticesTypes, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="verticesTypes">Vertices types.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        public RandomLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] IDictionary<TVertex, RandomVertexType> verticesTypes,
            [CanBeNull] RandomLayoutParameters parameters = null)
            : base(visitedGraph, verticesPositions, parameters)
        {
            _verticesSizes = new Dictionary<TVertex, Size>(verticesSizes);
            _verticesTypes = verticesTypes is null
                ? new Dictionary<TVertex, RandomVertexType>(0)
                : new Dictionary<TVertex, RandomVertexType>(verticesTypes);
        }


        #region AlgorithmBase

        private IDictionary<TVertex, Point> _fixedPositions;

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            _fixedPositions = VerticesPositions
                .Where(pair => _verticesTypes.TryGetValue(pair.Key, out RandomVertexType type)
                               && type == RandomVertexType.Fixed)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            VerticesPositions.Clear();

            foreach (KeyValuePair<TVertex, Point> pair in _fixedPositions)
            {
                VerticesPositions.Add(pair);
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            int x = (int)Parameters.XOffset;
            int y = (int)Parameters.YOffset;
            int xBound = (int)Parameters.Width;
            int yBound = (int)Parameters.Height;
            foreach (TVertex vertex in VisitedGraph.Vertices.Except(_fixedPositions.Keys))
            {
                _verticesSizes.TryGetValue(vertex, out Size vertexSize);
                VerticesPositions[vertex] = new Point(
                    Rand.Next(x, x + xBound - (int)vertexSize.Width),
                    Rand.Next(y, y + yBound - (int)vertexSize.Height));
            }
        }

        #endregion
    }
}