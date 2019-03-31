using QuickGraph;
using System.Diagnostics.Contracts;

namespace GraphSharp.Algorithms.Layout.Contextual
{
    public interface IContextualLayoutAlgorithmFactory<TVertex, TEdge, TGraph> : ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
    }
}
