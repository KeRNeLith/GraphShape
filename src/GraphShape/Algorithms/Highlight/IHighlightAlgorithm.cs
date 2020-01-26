using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Highlight
{
    /// <summary>
    /// Represents a graph highlighting algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IHighlightAlgorithm<in TVertex, in TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Highlight parameters.
        /// </summary>
        IHighlightParameters Parameters { get; }

        /// <summary>
        /// Resets the whole highlight.
        /// </summary>
        void ResetHighlight();

        /// <summary>
        /// Asks to highlight the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to highlight.</param>
        /// <returns>True if the <paramref name="vertex"/> was highlighted, false otherwise.</returns>
        bool OnVertexHighlighting([NotNull] TVertex vertex);

        /// <summary>
        /// Asks to remove the highlighting of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to remove its highlighting.</param>
        /// <returns>True if the <paramref name="vertex"/> highlight was removed, false otherwise.</returns>
        bool OnVertexHighlightRemoving([NotNull] TVertex vertex);

        /// <summary>
        /// Asks to highlight the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Edge to highlight.</param>
        /// <returns>True if the <paramref name="edge"/> was highlighted, false otherwise.</returns>
        bool OnEdgeHighlighting([NotNull] TEdge edge);

        /// <summary>
        /// Asks to remove the highlighting of the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Edge to remove its highlighting.</param>
        /// <returns>True if the <paramref name="edge"/> highlight was removed, false otherwise.</returns>
        bool OnEdgeHighlightRemoving([NotNull] TEdge edge);

        /// <summary>
        /// Checks if given highlight algorithm <paramref name="parameters"/>
        /// can be set to this highlight algorithm.
        /// </summary>
        /// <param name="parameters">Highlight parameters to check.</param>
        /// <returns>True if parameters can be set, false otherwise.</returns>
        bool IsParametersSettable([CanBeNull] IHighlightParameters parameters);

        /// <summary>
        /// Tries to set the given highlight algorithm <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">Highlight parameters to set.</param>
        /// <returns>True if parameters were set, false otherwise.</returns>
        bool TrySetParameters([CanBeNull] IHighlightParameters parameters);
    }
}