using QuikGraph;
using System.Diagnostics.Contracts;

namespace GraphShape.Algorithms.Layout.Contextual
{
    public interface IContextualLayoutAlgorithmFactory<TVertex, TEdge, TGraph> : ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
    }
}
