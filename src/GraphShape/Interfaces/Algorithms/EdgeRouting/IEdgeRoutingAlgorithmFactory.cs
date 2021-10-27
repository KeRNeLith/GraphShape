using System.Collections.Generic;
using GraphShape.Algorithms.Layout;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.EdgeRouting
{
    /// <summary>
    /// Represents a factory of edge routing algorithms.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface IEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// The set of available algorithms.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<string> AlgorithmTypes { get; }

        /// <summary>
        /// Creates an algorithm corresponding to given <paramref name="algorithmType"/>,
        /// <paramref name="context"/> and <paramref name="parameters"/>.
        /// </summary>
        /// <param name="algorithmType">Algorithm type.</param>
        /// <param name="context">Creation context.</param>
        /// <param name="parameters">Algorithm parameters.</param>
        /// <returns>Created algorithm.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="algorithmType"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        [Pure]
        IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> CreateAlgorithm(
            [NotNull] string algorithmType,
            [NotNull] ILayoutContext<TVertex, TEdge, TGraph> context,
            [CanBeNull] IEdgeRoutingParameters parameters);

        /// <summary>
        /// Creates algorithm parameters for an algorithm of given <paramref name="algorithmType"/>
        /// and <paramref name="parameters"/>.
        /// </summary>
        /// <param name="algorithmType">Algorithm type.</param>
        /// <param name="parameters">Algorithm parameters.</param>
        /// <returns>Parameters for the algorithm.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="algorithmType"/> is <see langword="null"/>.</exception>
        [Pure]
        IEdgeRoutingParameters CreateParameters([NotNull] string algorithmType, [CanBeNull] IEdgeRoutingParameters parameters);

        /// <summary>
        /// Checks if the given <paramref name="algorithmType"/> is a valid one.
        /// </summary>
        /// <param name="algorithmType">Algorithm type.</param>
        /// <returns>True if the algorithm type is valid, false otherwise.</returns>
        [Pure]
        bool IsValidAlgorithm([CanBeNull] string algorithmType);

        /// <summary>
        /// Gets the algorithm type from a given <paramref name="algorithm"/>.
        /// </summary>
        /// <param name="algorithm">Algorithm to get its type.</param>
        /// <returns>Algorithm type.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="algorithm"/> is <see langword="null"/>.</exception>
        [Pure]
        string GetAlgorithmType([NotNull] IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> algorithm);
    }
}