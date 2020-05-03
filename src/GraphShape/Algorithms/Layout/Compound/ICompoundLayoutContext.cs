using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Compound
{
    /// <summary>
    /// Represents a compound graph layout context.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface ICompoundLayoutContext<TVertex, TEdge, out TGraph> : ILayoutContext<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Vertices borders.
        /// </summary>
        [NotNull]
        IDictionary<TVertex, Thickness> VerticesBorders { get; }

        /// <summary>
        /// Layout types per vertex.
        /// </summary>
        [NotNull]
        IDictionary<TVertex, CompoundVertexInnerLayoutType> LayoutTypes { get; }
    }
}
