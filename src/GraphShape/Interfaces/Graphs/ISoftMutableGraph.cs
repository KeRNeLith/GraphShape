using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape
{
    /// <summary>
    /// Bidirectional graph with soft mutability behavior.
    /// Soft mutation means that we can hide some vertices or edges.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface ISoftMutableGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Gets the set of hidden vertices.
        /// </summary>
        IEnumerable<TVertex> HiddenVertices { get; }

        /// <summary>
        /// Gets the number of hidden vertices.
        /// </summary>
        int HiddenVertexCount { get; }

        /// <summary>
        /// Hides the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to hide.</param>
        /// <returns>True if the vertex is hidden, false otherwise.</returns>
        bool HideVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Hides the given <paramref name="vertex"/>, and gives a <paramref name="tag"/> associated to this hidden vertex.
        /// </summary>
        /// <param name="vertex">Vertex to hide.</param>
        /// <param name="tag">Tag associated to the hidden vertex.</param>
        /// <returns>True if the vertex is hidden, false otherwise.</returns>
        bool HideVertex([NotNull] TVertex vertex, [NotNull] string tag);

        /// <summary>
        /// Hides the given set of <paramref name="vertices"/>.
        /// </summary>
        /// <param name="vertices">Vertices to hide.</param>
        void HideVertices([NotNull, ItemNotNull] IEnumerable<TVertex> vertices);

        /// <summary>
        /// Hides the given set of <paramref name="vertices"/>, and gives a <paramref name="tag"/> associated to these hidden vertices.
        /// </summary>
        /// <param name="vertices">Vertices to hide.</param>
        /// <param name="tag">Tag associated to the hidden vertices.</param>
        void HideVertices([NotNull, ItemNotNull] IEnumerable<TVertex> vertices, [NotNull] string tag);
        
        /// <summary>
        /// Hides vertices matching the given <paramref name="predicate"/>, and gives a <paramref name="tag"/> associated to these hidden vertices.
        /// </summary>
        /// <param name="predicate">Hide predicate.</param>
        /// <param name="tag">Tag associated to the hidden vertices.</param>
        void HideVerticesIf([NotNull, InstantHandle] Predicate<TVertex> predicate, [NotNull] string tag);

        /// <summary>
        /// Checks if the given <paramref name="vertex"/> is hidden.
        /// </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <returns>True if the vertex is hidden, false otherwise.</returns>
        [Pure]
        bool IsHiddenVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Un-hides the given <paramref name="vertex"/> if it is hidden.
        /// </summary>
        /// <param name="vertex">Vertex to un-hide.</param>
        /// <returns>True if the vertex has been un-hidden, false otherwise.</returns>
        bool UnhideVertex([NotNull] TVertex vertex);

        /// <summary>
        /// Un-hides the given <paramref name="vertex"/> and its connected edges if they are hidden.
        /// </summary>
        /// <param name="vertex">Vertex to un-hide.</param>
        void UnhideVertexAndEdges([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the set of hidden edges.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> HiddenEdges { get; }

        /// <summary>
        /// Gets the number of hidden edges.
        /// </summary>
        int HiddenEdgeCount { get; }

        /// <summary>
        /// Hides the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Edge to hide.</param>
        /// <returns>True if the edge is hidden, false otherwise.</returns>
        bool HideEdge([NotNull] TEdge edge);

        /// <summary>
        /// Hides the given <paramref name="edge"/>, and gives a <paramref name="tag"/> associated to this hidden edge.
        /// </summary>
        /// <param name="edge">Edge to hide.</param>
        /// <param name="tag">Tag associated to the hidden edge.</param>
        /// <returns>True if the edge is hidden, false otherwise.</returns>
        bool HideEdge([NotNull] TEdge edge, [NotNull] string tag);

        /// <summary>
        /// Hides the given set of <paramref name="edges"/>.
        /// </summary>
        /// <param name="edges">Edges to hide.</param>
        void HideEdges([NotNull, ItemNotNull] IEnumerable<TEdge> edges);

        /// <summary>
        /// Hides the given set of <paramref name="edges"/>, and gives a <paramref name="tag"/> associated to this hidden edges.
        /// </summary>
        /// <param name="edges">Edges to hide.</param>
        /// <param name="tag">Tag associated to the hidden edges.</param>
        void HideEdges([NotNull, ItemNotNull] IEnumerable<TEdge> edges, [NotNull] string tag);

        /// <summary>
        /// Hides edges matching the given <paramref name="predicate"/>, and gives a <paramref name="tag"/> associated to these hidden edges.
        /// </summary>
        /// <param name="predicate">Hide predicate.</param>
        /// <param name="tag">Tag associated to the hidden edges.</param>
        void HideEdgesIf([NotNull, InstantHandle] Predicate<TEdge> predicate, [NotNull] string tag);

        /// <summary>
        /// Checks if the given <paramref name="edge"/> is hidden.
        /// </summary>
        /// <param name="edge">Edge to check.</param>
        /// <returns>True if the edge is hidden, false otherwise.</returns>
        [Pure]
        bool IsHiddenEdge([NotNull] TEdge edge);

        /// <summary>
        /// Un-hides the given <paramref name="edge"/> if it is hidden.
        /// </summary>
        /// <param name="edge">Edge to un-hide.</param>
        /// <returns>True if the edge has been un-hidden, false otherwise.</returns>
        bool UnhideEdge([NotNull] TEdge edge);

        /// <summary>
        /// Un-hides the given <paramref name="edges"/> if they are hidden.
        /// </summary>
        /// <param name="edges">Edges to un-hide.</param>
        void UnhideEdges([NotNull, ItemNotNull] IEnumerable<TEdge> edges);

        /// <summary>
        /// Un-hides edges matching the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Un-hide predicate.</param>
        void UnhideEdgesIf([NotNull, InstantHandle] Predicate<TEdge> predicate);

        /// <summary>
        /// Gets the hidden edges connected to the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <returns>Hidden edges.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        IEnumerable<TEdge> HiddenEdgesOf([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the number of hidden edges connected to the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to check.</param>
        /// <returns>Number of hidden edges.</returns>
        [Pure]
        int HiddenEdgeCountOf([NotNull] TVertex vertex);

        /// <summary>
        /// Un-hides all vertices and edges that were hidden with the given <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">Tag to un-hide.</param>
        /// <returns>True if the tag has been un-hidden, false otherwise.</returns>
        bool Unhide([NotNull] string tag);

        /// <summary>
        /// Un-hides all hidden vertices and edges.
        /// </summary>
        /// <returns>True if operation succeed, false otherwise.</returns>
        bool UnhideAll();
    }
}