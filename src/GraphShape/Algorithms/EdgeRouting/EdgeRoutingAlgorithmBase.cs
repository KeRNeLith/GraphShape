using System;
using System.Collections.Generic;
using QuikGraph;
using QuikGraph.Algorithms;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.EdgeRouting
{
    /// <summary>
    /// Base class for all edge routing algorithms.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public abstract class EdgeRoutingAlgorithmBase<TVertex, TEdge, TGraph>
        : AlgorithmBase<TGraph>
        , IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Vertices positions associations.
        /// </summary>
        [NotNull]
        protected IDictionary<TVertex, Point> VerticesPositions;

        /// <inheritdoc />
        public IDictionary<TEdge, Point[]> EdgeRoutes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeRoutingAlgorithmBase{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        protected EdgeRoutingAlgorithmBase(
            [NotNull] TGraph visitedGraph,
            [NotNull] IDictionary<TVertex, Point> verticesPositions)
            : base(visitedGraph)
        {
            EdgeRoutes = new Dictionary<TEdge, Point[]>();
            VerticesPositions = verticesPositions ?? throw new ArgumentNullException(nameof(verticesPositions));
        }
    }
}