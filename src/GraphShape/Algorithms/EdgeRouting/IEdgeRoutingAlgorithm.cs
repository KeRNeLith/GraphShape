using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;
using QuikGraph.Algorithms;

namespace GraphShape.Algorithms.EdgeRouting
{
    /// <summary>
    /// Represents an edge routing algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface IEdgeRoutingAlgorithm<TVertex, TEdge, out TGraph> : IAlgorithm<TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// The routing points of the edges (routes).
        /// </summary>
        [NotNull]
        IDictionary<TEdge, Point[]> EdgeRoutes { get; }
    }
}