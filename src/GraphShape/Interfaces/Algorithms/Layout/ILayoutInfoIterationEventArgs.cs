using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Represents information on a layout algorithm iteration.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface ILayoutInfoIterationEventArgs<TVertex, in TEdge>
        : ILayoutIterationEventArgs<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Returns the extra layout information of the <paramref name="vertex"/> (or null).
        /// </summary>
        [Pure]
        [CanBeNull]
        object GetVertexInfo([NotNull] TVertex vertex);

        /// <summary>
        /// Returns the extra layout information of the <paramref name="edge"/> (or null).
        /// </summary>
        [Pure]
        [CanBeNull]
        object GetEdgeInfo([NotNull] TEdge edge);
    }

    /// <summary>
    /// Represents information on a layout algorithm iteration.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TVertexInfo">Vertex information type.</typeparam>
    /// <typeparam name="TEdgeInfo">Edge information type.</typeparam>
    public interface ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
        : ILayoutInfoIterationEventArgs<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Extra vertices information.
        /// </summary>
        [CanBeNull]
        IDictionary<TVertex, TVertexInfo> VerticesInfos { get; }

        /// <summary>
        /// Extra edges information.
        /// </summary>
        [CanBeNull]
        IDictionary<TEdge, TEdgeInfo> EdgesInfos { get; }
    }
}
