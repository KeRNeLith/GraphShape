using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Represents a factory of layout algorithms.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>
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
        [Pure]
        ILayoutAlgorithm<TVertex, TEdge, TGraph> CreateAlgorithm(
            [NotNull] string algorithmType,
            [NotNull] ILayoutContext<TVertex, TEdge, TGraph> context,
            [CanBeNull] ILayoutParameters parameters);

        /// <summary>
        /// Creates algorithm parameters for an algorithm of given <paramref name="algorithmType"/>
        /// and <paramref name="parameters"/>.
        /// </summary>
        /// <param name="algorithmType">Algorithm type.</param>
        /// <param name="parameters">Algorithm parameters.</param>
        /// <returns>Parameters for the algorithm.</returns>
        [Pure]
        ILayoutParameters CreateParameters([NotNull] string algorithmType, [CanBeNull] ILayoutParameters parameters);

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
        [Pure]
        string GetAlgorithmType([NotNull] ILayoutAlgorithm<TVertex, TEdge, TGraph> algorithm);

        /// <summary>
        /// Indicates if the given <paramref name="algorithmType"/> require edge routing.
        /// </summary>
        /// <param name="algorithmType">Algorithm type.</param>
        /// <returns>True if the algorithm require edge routing, false otherwise.</returns>
        [Pure]
        bool NeedEdgeRouting([NotNull] string algorithmType);

        /// <summary>
        /// Indicates if the given <paramref name="algorithmType"/> require overlap removal.
        /// </summary>
        /// <param name="algorithmType">Algorithm type.</param>
        /// <returns>True if the algorithm require overlap removal, false otherwise.</returns>
        [Pure]
        bool NeedOverlapRemoval([NotNull] string algorithmType);
    }
}