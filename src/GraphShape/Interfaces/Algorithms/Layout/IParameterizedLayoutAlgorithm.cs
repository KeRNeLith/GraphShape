using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Represents a parametrized layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface IParameterizedLayoutAlgorithm<TVertex, in TEdge, out TGraph> : ILayoutAlgorithm<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Gets layout parameters.
        /// </summary>
        /// <returns>Layout parameters.</returns>
        [Pure]
        [NotNull]
        ILayoutParameters GetParameters();
    }

    /// <summary>
    /// Represents a parametrized layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    /// <typeparam name="TParameters">Parameters type.</typeparam>
    public interface IParameterizedLayoutAlgorithm<TVertex, in TEdge, out TGraph, out TParameters> : IParameterizedLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
        where TParameters : ILayoutParameters
    {
        /// <summary>
        /// Layout parameters.
        /// </summary>
        [NotNull]
        TParameters Parameters { get; }
    }
}