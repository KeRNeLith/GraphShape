using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;
using QuikGraph.Algorithms;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Represents a layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface ILayoutAlgorithm<TVertex, in TEdge, out TGraph> : IAlgorithm<TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Vertices positions associations.
        /// </summary>
        [NotNull]
        IDictionary<TVertex, Point> VerticesPositions { get; }

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

        /// <summary>
        /// Fired when layout algorithm progress changed.
        /// </summary>
        event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Fired when a layout iteration has been done.
        /// </summary>
        event LayoutIterationEndedEventHandler<TVertex> IterationEnded;
    }

    /// <summary>
    /// Represents a layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    /// <typeparam name="TVertexInfo">Vertex information type.</typeparam>
    /// <typeparam name="TEdgeInfo">Edge information type.</typeparam>
    public interface ILayoutAlgorithm<TVertex, TEdge, out TGraph, TVertexInfo, TEdgeInfo> : ILayoutAlgorithm<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Extra vertices information, calculated by the layout.
        /// </summary>
        [NotNull]
        IDictionary<TVertex, TVertexInfo> VerticesInfos { get; }

        /// <summary>
        /// Extra edges information, calculated by the layout.
        /// </summary>
        [NotNull]
        IDictionary<TEdge, TEdgeInfo> EdgesInfos { get; }

        /// <summary>
        /// Fired when a layout iteration has been done (with more detailed information).
        /// </summary>
        event LayoutIterationEndedEventHandler<TVertex, TEdge, TVertexInfo, TEdgeInfo> InfoIterationEnded;
    }
}