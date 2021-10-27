using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Highlight
{
    /// <summary>
    /// Represents an highlight controller.
    /// Able to set/remove highlight, check highlighting of entity.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface IHighlightController<TVertex, TEdge, out TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Graph concerned by the highlighting.
        /// </summary>
        TGraph Graph { get; }

        /// <summary>
        /// Set of highlighted vertices.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<TVertex> HighlightedVertices { get; }

        /// <summary>
        /// Set of semi-highlighted vertices.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<TVertex> SemiHighlightedVertices { get; }

        /// <summary>
        /// Set of highlighted edges.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> HighlightedEdges { get; }

        /// <summary>
        /// Set of semi-highlighted edges.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> SemiHighlightedEdges { get; }

        /// <summary>
        /// Checks if the given <paramref name="vertex"/> is highlighted.
        /// </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <returns>True if the <paramref name="vertex"/> is highlighted, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        bool IsHighlightedVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Checks if the given <paramref name="vertex"/> is highlighted.
        /// It highlighted, provides additional <paramref name="highlightInfo"/>.
        /// </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <param name="highlightInfo">Additional highlight information.</param>
        /// <returns>True if the <paramref name="vertex"/> is highlighted, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        bool IsHighlightedVertex([NotNull] TVertex vertex, out object highlightInfo);

        /// <summary>
        /// Checks if the given <paramref name="vertex"/> is semi-highlighted.
        /// </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <returns>True if the <paramref name="vertex"/> is semi-highlighted, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        bool IsSemiHighlightedVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Checks if the given <paramref name="vertex"/> is semi-highlighted.
        /// It semi-highlighted, provides additional <paramref name="semiHighlightInfo"/>.
        /// </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <param name="semiHighlightInfo">Additional semi-highlight information.</param>
        /// <returns>True if the <paramref name="vertex"/> is semi-highlighted, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        bool IsSemiHighlightedVertex([NotNull] TVertex vertex, out object semiHighlightInfo);

        /// <summary>
        /// Checks if the given <paramref name="edge"/> is highlighted.
        /// </summary>
        /// <param name="edge">Edge to check.</param>
        /// <returns>True if the <paramref name="edge"/> is highlighted, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        [Pure]
        bool IsHighlightedEdge([NotNull] TEdge edge);

        /// <summary>
        /// Checks if the given <paramref name="edge"/> is highlighted.
        /// It semi-highlighted, provides additional <paramref name="highlightInfo"/>.
        /// </summary>
        /// <param name="edge">Edge to check.</param>
        /// <param name="highlightInfo">Additional highlight information.</param>
        /// <returns>True if the <paramref name="edge"/> is highlighted, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        [Pure]
        bool IsHighlightedEdge([NotNull] TEdge edge, out object highlightInfo);

        /// <summary>
        /// Checks if the given <paramref name="edge"/> is semi-highlighted.
        /// </summary>
        /// <param name="edge">Edge to check.</param>
        /// <returns>True if the <paramref name="edge"/> is semi-highlighted, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        [Pure]
        bool IsSemiHighlightedEdge([NotNull] TEdge edge);

        /// <summary>
        /// Checks if the given <paramref name="edge"/> is semi-highlighted.
        /// It semi-highlighted, provides additional <paramref name="semiHighlightInfo"/>.
        /// </summary>
        /// <param name="edge">Edge to check.</param>
        /// <param name="semiHighlightInfo">Additional semi-highlight information.</param>
        /// <returns>True if the <paramref name="edge"/> is semi-highlighted, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        [Pure]
        bool IsSemiHighlightedEdge([NotNull] TEdge edge, out object semiHighlightInfo);

        /// <summary>
        /// Asks to highlight the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to highlight.</param>
        /// <param name="highlightInfo">Additional highlight information.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        void HighlightVertex([NotNull] TVertex vertex, [CanBeNull] object highlightInfo);

        /// <summary>
        /// Asks to semi-highlight the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to highlight.</param>
        /// <param name="semiHighlightInfo">Additional semi-highlight information.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        void SemiHighlightVertex([NotNull] TVertex vertex, [CanBeNull] object semiHighlightInfo);

        /// <summary>
        /// Asks to highlight the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Edge to highlight.</param>
        /// <param name="highlightInfo">Additional highlight information.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        void HighlightEdge([NotNull] TEdge edge, [CanBeNull] object highlightInfo);

        /// <summary>
        /// Asks to semi-highlight the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Edge to semi-highlight.</param>
        /// <param name="semiHighlightInfo">Additional semi-highlight information.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        void SemiHighlightEdge([NotNull] TEdge edge, [CanBeNull] object semiHighlightInfo);

        /// <summary>
        /// Removes the highlight of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex from which removing highlight.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        void RemoveHighlightFromVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Removes the semi-highlight of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex from which removing semi-highlight.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        void RemoveSemiHighlightFromVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Removes the highlight of the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Edge from which removing highlight.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        void RemoveHighlightFromEdge([NotNull] TEdge edge);

        /// <summary>
        /// Removes the semi-highlight of the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Edge from which removing highlight.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        void RemoveSemiHighlightFromEdge([NotNull] TEdge edge);
    }
}