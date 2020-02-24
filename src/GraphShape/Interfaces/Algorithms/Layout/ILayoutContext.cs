using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Represents a graph layout context.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface ILayoutContext<TVertex, TEdge, out TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Vertices positions.
        /// </summary>
        [CanBeNull]
        IDictionary<TVertex, Point> Positions { get; }

        /// <summary>
        /// Vertices sizes.
        /// </summary>
        [NotNull]
        IDictionary<TVertex, Size> Sizes { get; }

        /// <summary>
        /// Graph concerned by the layout.
        /// </summary>
        TGraph Graph { get; }

        /// <summary>
        /// Layout mode.
        /// </summary>
        LayoutMode Mode { get; }
    }
}