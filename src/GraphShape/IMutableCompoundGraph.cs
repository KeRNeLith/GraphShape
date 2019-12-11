using QuikGraph;

namespace GraphShape
{
    public interface IMutableCompoundGraph<TVertex, TEdge> 
        : ICompoundGraph<TVertex, TEdge>,
          IMutableBidirectionalGraph<TVertex, TEdge> 
        where TEdge : IEdge<TVertex>
    {
    }
}
