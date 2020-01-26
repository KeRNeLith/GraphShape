using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Highlight
{
    /// <summary>
    /// Represents a factory of highlight algorithms.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface IHighlightAlgorithmFactory<TVertex, TEdge, in TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Highlight modes.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<string> HighlightModes { get; }

        /// <summary>
        /// Creates an algorithm corresponding using given <paramref name="highlightMode"/>,
        /// <paramref name="context"/>, <paramref name="controller"/> and <paramref name="parameters"/>.
        /// </summary>
        /// <param name="highlightMode">Highlight mode.</param>
        /// <param name="context">Creation context.</param>
        /// <param name="controller">Highlight controller.</param>
        /// <param name="parameters">Algorithm parameters.</param>
        /// <returns>Created algorithm.</returns>
        [Pure]
        IHighlightAlgorithm<TVertex, TEdge> CreateAlgorithm(
            [NotNull] string highlightMode,
            [NotNull] IHighlightContext<TVertex, TEdge, TGraph> context,
            [NotNull] IHighlightController<TVertex, TEdge, TGraph> controller,
            [CanBeNull] IHighlightParameters parameters);

        /// <summary>
        /// Creates algorithm parameters for an algorithm using given <paramref name="highlightMode"/>
        /// and <paramref name="oldParameters"/>.
        /// </summary>
        /// <param name="highlightMode">Highlight mode.</param>
        /// <param name="oldParameters">Old parameters.</param>
        /// <returns>New algorithm parameters.</returns>
        [Pure]
        IHighlightParameters CreateParameters([NotNull] string highlightMode, [CanBeNull] IHighlightParameters oldParameters);

        /// <summary>
        /// Checks if the given <paramref name="mode"/> is a valid one.
        /// </summary>
        /// <param name="mode">Highlight mode.</param>
        /// <returns>True if the highlight mode is valid, false otherwise.</returns>
        [Pure]
        bool IsValidMode([CanBeNull] string mode);

        /// <summary>
        /// Gets the algorithm highlight mode from a given <paramref name="algorithm"/>.
        /// </summary>
        /// <param name="algorithm">Algorithm from which getting highlight mode.</param>
        /// <returns>Highlight mode.</returns>
        [Pure]
        string GetHighlightMode([NotNull] IHighlightAlgorithm<TVertex, TEdge> algorithm);
    }
}