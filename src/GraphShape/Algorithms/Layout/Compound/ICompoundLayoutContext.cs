using System.Collections.Generic;
using QuikGraph;
using System.Windows;

namespace GraphShape.Algorithms.Layout.Compound
{
    public interface ICompoundLayoutContext<TVertex, TEdge, TGraph> : ILayoutContext<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        IDictionary<TVertex, Thickness> VertexBorders { get; }
        IDictionary<TVertex, CompoundVertexInnerLayoutType> LayoutTypes { get; }
    }
}
