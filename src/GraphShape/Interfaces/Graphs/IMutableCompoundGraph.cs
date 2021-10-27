using QuikGraph;

namespace GraphShape
{
    /// <summary>
    /// Represents a mutable graph with parent/children relationships between vertices.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IMutableCompoundGraph<TVertex, TEdge>
        : ICompoundGraph<TVertex, TEdge>
        , IMutableBidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
    }
}