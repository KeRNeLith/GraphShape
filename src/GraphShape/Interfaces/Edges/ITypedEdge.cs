using QuikGraph;

namespace GraphShape
{
    /// <summary>
    /// Enumeration of possible edge types.
    /// </summary>
    public enum EdgeTypes
    {
        /// <summary>
        /// General edge.
        /// </summary>
        General,

        /// <summary>
        /// Hierarchical edge.
        /// </summary>
        Hierarchical
    }

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