using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Graph layout context.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class LayoutContext<TVertex, TEdge, TGraph> : ILayoutContext<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <inheritdoc />
        public IDictionary<TVertex, Point> Positions { get; }

        /// <inheritdoc />
        public IDictionary<TVertex, Size> Sizes { get; }

        /// <inheritdoc />
        public TGraph Graph { get; }

        /// <inheritdoc />
        public LayoutMode Mode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutContext{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="graph">Graph to layout.</param>
        /// <param name="positions">Vertices positions.</param>
        /// <param name="sizes">Vertices sizes.</param>
        /// <param name="mode">Layout mode.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="sizes"/> is <see langword="null"/>.</exception>
        public LayoutContext(
            [CanBeNull] TGraph graph,
            [CanBeNull] IDictionary<TVertex, Point> positions,
            [NotNull] IDictionary<TVertex, Size> sizes,
            LayoutMode mode)
        {
            Graph = graph;
            Positions = positions;
            Sizes = sizes ?? throw new ArgumentNullException(nameof(sizes));
            Mode = mode;
        }
    }
}