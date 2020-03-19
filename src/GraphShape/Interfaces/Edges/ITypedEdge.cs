using QuikGraph;

namespace GraphShape
{
    /// <summary>
    /// Represents an edge that has a type.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    public interface ITypedEdge<out TVertex> : IEdge<TVertex>
    {
        /// <summary>
        /// Edge type.
        /// </summary>
        EdgeTypes Type { get; }
    }
}