using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Compound graph layout context.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class CompoundLayoutContext<TVertex, TEdge, TGraph>
        : LayoutContext<TVertex, TEdge, TGraph>
        , ICompoundLayoutContext<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundLayoutContext{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="graph">Graph to layout.</param>
        /// <param name="positions">Vertices positions.</param>
        /// <param name="sizes">Vertices sizes.</param>
        /// <param name="mode">Layout mode.</param>
        /// <param name="verticesBorders">Vertices borders.</param>
        /// <param name="layoutTypes">Layout types.</param>
        public CompoundLayoutContext(
            [CanBeNull] TGraph graph,
            [CanBeNull] IDictionary<TVertex, Point> positions,
            [NotNull] IDictionary<TVertex, Size> sizes,
            LayoutMode mode,
            [NotNull] IDictionary<TVertex, Thickness> verticesBorders,
            [NotNull] IDictionary<TVertex, CompoundVertexInnerLayoutType> layoutTypes)
            : base( graph, positions, sizes, mode )
        {
            VerticesBorders = verticesBorders ?? throw new ArgumentNullException(nameof(verticesBorders));
            LayoutTypes = layoutTypes ?? throw new ArgumentNullException(nameof(layoutTypes));
        }

        /// <inheritdoc />
        public IDictionary<TVertex, Thickness> VerticesBorders { get; }

        /// <inheritdoc />
        public IDictionary<TVertex, CompoundVertexInnerLayoutType> LayoutTypes { get; }
    }
}
