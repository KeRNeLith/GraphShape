using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape
{
    /// <summary>
    /// Represents a graph with parent/children relationships between vertices.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface ICompoundGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Gets the set of simple vertices.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<TVertex> SimpleVertices { get; }

        /// <summary>
        /// Gets the set of compound vertices.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<TVertex> CompoundVertices { get; }

        /// <summary>
        /// Adds the <paramref name="child"/> vertex to the graph if not already inside and sets
        /// it as child vertex of <paramref name="parent"/> vertex.
        /// </summary>
        /// <param name="parent">Parent vertex.</param>
        /// <param name="child">Vertex to add as child.</param>
        /// <returns>True if vertex is added as child with success, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="parent"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="child"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="child"/> already has a parent vertex.</exception>
        /// <exception cref="T:QuikGraph.VertexNotFoundException"><paramref name="parent"/> is not part of the graph.</exception>
        bool AddChildVertex([NotNull] TVertex parent, [NotNull] TVertex child);

        /// <summary>
        /// Adds the set of <paramref name="children"/> vertices to the graph if not already inside and sets
        /// them as children vertices of <paramref name="parent"/> vertex.
        /// </summary>
        /// <param name="parent">Parent vertex.</param>
        /// <param name="children">Vertices to add as children.</param>
        /// <returns>The number of vertices added to the graph.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="parent"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="children"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">At least on of <paramref name="children"/> already has a parent vertex.</exception>
        /// <exception cref="T:QuikGraph.VertexNotFoundException"><paramref name="parent"/> is not part of the graph.</exception>
        int AddChildVertexRange([NotNull] TVertex parent, [NotNull, ItemNotNull] IEnumerable<TVertex> children);

        /// <summary>
        /// Gets the parent vertex of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get the parent.</param>
        /// <returns>Parent vertex if there is one, <see langword="null"/> otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:QuikGraph.VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        [CanBeNull]
        TVertex GetParent([NotNull] TVertex vertex);

        /// <summary>
        /// Checks if the given <paramref name="vertex"/> is a child vertex of another one.
        /// </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <returns>True if the vertex is a child one, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:QuikGraph.VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        bool IsChildVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the children vertices of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get its children.</param>
        /// <returns>Children vertices.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:QuikGraph.VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        IEnumerable<TVertex> GetChildrenVertices([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the number of children vertices of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get its children count.</param>
        /// <returns>Number of children vertices.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:QuikGraph.VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        int GetChildrenCount([NotNull] TVertex vertex);

        /// <summary>
        /// Checks if the given <paramref name="vertex"/> is a compound vertex (child vertex).
        /// </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <returns>True if the vertex is a compound one, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:QuikGraph.VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        bool IsCompoundVertex([NotNull] TVertex vertex);
    }
}