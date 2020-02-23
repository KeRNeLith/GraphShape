using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape
{
    /// <summary>
    /// Represents a hierarchical bidirectional graph.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
	public interface IHierarchicalBidirectionalGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, TEdge>
        where TEdge : TypedEdge<TVertex>
    {
        /// <summary>
        /// Gets the set of hierarchical edges.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> HierarchicalEdges { get; }

        /// <summary>
        /// Gets the number of hierarchical edges.
        /// </summary>
        int HierarchicalEdgeCount { get; }

        /// <summary>
        /// Gets the set of hierarchical edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get hierarchical edges.</param>
        /// <returns>Hierarchical edges.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> HierarchicalEdgesFor([NotNull] TVertex vertex);
        
        /// <summary>
        /// Gets the number of hierarchical edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get hierarchical edges.</param>
        /// <returns>Hierarchical edges count.</returns>
        [Pure]
        int HierarchicalEdgeCountFor([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the set of hierarchical in-edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get hierarchical in-edges.</param>
        /// <returns>Hierarchical in-edges.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> InHierarchicalEdges([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the number of hierarchical in-edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get hierarchical in-edges.</param>
        /// <returns>Hierarchical in-edges count.</returns>
        [Pure]
        int InHierarchicalEdgeCount([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the set of hierarchical out-edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get hierarchical out-edges.</param>
        /// <returns>Hierarchical out-edges.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> OutHierarchicalEdges([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the number of hierarchical out-edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get hierarchical out-edges.</param>
        /// <returns>Hierarchical out-edges count.</returns>
        [Pure]
        int OutHierarchicalEdgeCount([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the set of general edges.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> GeneralEdges { get; }

        /// <summary>
        /// Gets the number of general edges.
        /// </summary>
        int GeneralEdgeCount { get; }

        /// <summary>
        /// Gets the set of general edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get general edges.</param>
        /// <returns>General edges.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> GeneralEdgesFor([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the number of general edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get general edges.</param>
        /// <returns>General edges count.</returns>
        [Pure]
        int GeneralEdgeCountFor([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the set of general in-edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get general in-edges.</param>
        /// <returns>General in-edges.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> InGeneralEdges([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the number of general in-edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get general in-edges.</param>
        /// <returns>General in-edges count.</returns>
        [Pure]
        int InGeneralEdgeCount([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the set of general out-edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get general out-edges.</param>
        /// <returns>General out-edges.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> OutGeneralEdges([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the number of general out-edges for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get general out-edges.</param>
        /// <returns>General out-edges count.</returns>
        [Pure]
        int OutGeneralEdgeCount([NotNull] TVertex vertex);
    }
}