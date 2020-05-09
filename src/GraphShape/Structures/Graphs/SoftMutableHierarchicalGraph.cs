using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape
{
    /// <summary>
    /// Hierarchical graph that implements soft mutability.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [Serializable]
    public class SoftMutableHierarchicalGraph<TVertex, TEdge>
        : HierarchicalGraph<TVertex, TEdge>
        , ISoftMutableGraph<TVertex, TEdge>
        where TEdge : TypedEdge<TVertex>
    {
        [NotNull]
        private readonly GraphHideHelpers<TVertex, TEdge> _hideHelpers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftMutableHierarchicalGraph{TVertex,TEdge}"/> class.
        /// </summary>
        public SoftMutableHierarchicalGraph()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftMutableHierarchicalGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        public SoftMutableHierarchicalGraph(bool allowParallelEdges)
            : this(allowParallelEdges, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftMutableHierarchicalGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <param name="capacity">Vertex capacity.</param>
        public SoftMutableHierarchicalGraph(bool allowParallelEdges, int capacity)
            : base(allowParallelEdges, capacity)
        {
            _hideHelpers = new GraphHideHelpers<TVertex, TEdge>(this);
        }

        #region Events

        /// <summary>
        /// Fired when a vertex has been hidden.
        /// </summary>
        public event VertexAction<TVertex> VertexHidden
        {
            add => _hideHelpers.VertexHidden += value;
            remove => _hideHelpers.VertexHidden -= value;
        }

        /// <summary>
        /// Fired when a vertex has been unhidden.
        /// </summary>
        public event VertexAction<TVertex> VertexUnhidden
        {
            add => _hideHelpers.VertexUnhidden += value;
            remove => _hideHelpers.VertexUnhidden -= value;
        }

        /// <summary>
        /// Fired when an edge has been hidden.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> EdgeHidden
        {
            add => _hideHelpers.EdgeHidden += value;
            remove => _hideHelpers.EdgeHidden -= value;
        }

        /// <summary>
        /// Fired when an edge has been unhidden.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> EdgeUnhidden
        {
            add => _hideHelpers.EdgeUnhidden += value;
            remove => _hideHelpers.EdgeUnhidden -= value;
        }

        #endregion

        // Delegate calls to the GraphHideHelpers helper class

        #region ISoftMutableGraph<TVertex,TEdge>

        /// <inheritdoc />
        public IEnumerable<TVertex> HiddenVertices => _hideHelpers.HiddenVertices;

        /// <inheritdoc />
        public int HiddenVertexCount => _hideHelpers.HiddenVertexCount;

        /// <inheritdoc />
        public bool HideVertex(TVertex vertex)
        {
            return _hideHelpers.HideVertex(vertex);
        }

        /// <inheritdoc />
        public bool HideVertex(TVertex vertex, string tag)
        {
            return _hideHelpers.HideVertex(vertex, tag);
        }

        /// <inheritdoc />
        public void HideVertices(IEnumerable<TVertex> vertices)
        {
            _hideHelpers.HideVertices(vertices);
        }

        /// <inheritdoc />
        public void HideVertices(IEnumerable<TVertex> vertices, string tag)
        {
            _hideHelpers.HideVertices(vertices, tag);
        }

        /// <inheritdoc />
        public void HideVerticesIf(Predicate<TVertex> predicate, string tag)
        {
            _hideHelpers.HideVerticesIf(predicate, tag);
        }

        /// <inheritdoc />
        public bool IsHiddenVertex(TVertex vertex)
        {
            return _hideHelpers.IsHiddenVertex(vertex);
        }

        /// <inheritdoc />
        public bool UnhideVertex(TVertex vertex)
        {
            return _hideHelpers.UnhideVertex(vertex);
        }

        /// <inheritdoc />
        public void UnhideVertexAndEdges(TVertex vertex)
        {
            _hideHelpers.UnhideVertexAndEdges(vertex);
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> HiddenEdges => _hideHelpers.HiddenEdges;

        /// <inheritdoc />
        public int HiddenEdgeCount => _hideHelpers.HiddenEdgeCount;

        /// <inheritdoc />
        public bool HideEdge(TEdge edge)
        {
            return _hideHelpers.HideEdge(edge);
        }

        /// <inheritdoc />
        public bool HideEdge(TEdge edge, string tag)
        {
            return _hideHelpers.HideEdge(edge, tag);
        }

        /// <inheritdoc />
        public void HideEdges(IEnumerable<TEdge> edges)
        {
            _hideHelpers.HideEdges(edges);
        }

        /// <inheritdoc />
        public void HideEdges(IEnumerable<TEdge> edges, string tag)
        {
            _hideHelpers.HideEdges(edges, tag);
        }

        /// <inheritdoc />
        public void HideEdgesIf(Predicate<TEdge> predicate, string tag)
        {
            _hideHelpers.HideEdgesIf(predicate, tag);
        }

        /// <inheritdoc />
        public bool IsHiddenEdge(TEdge edge)
        {
            return _hideHelpers.IsHiddenEdge(edge);
        }

        /// <inheritdoc />
        public bool UnhideEdge(TEdge edge)
        {
            return _hideHelpers.UnhideEdge(edge);
        }

        /// <inheritdoc />
        public void UnhideEdges(IEnumerable<TEdge> edges)
        {
            _hideHelpers.UnhideEdges(edges);
        }

        /// <inheritdoc />
        public void UnhideEdgesIf(Predicate<TEdge> predicate)
        {
            _hideHelpers.UnhideEdgesIf(predicate);
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> HiddenEdgesOf(TVertex vertex)
        {
            return _hideHelpers.HiddenEdgesOf(vertex);
        }

        /// <inheritdoc />
        public int HiddenEdgeCountOf(TVertex vertex)
        {
            return _hideHelpers.HiddenEdgeCountOf(vertex);
        }

        /// <inheritdoc />
        public bool Unhide(string tag)
        {
            return _hideHelpers.Unhide(tag);
        }

        /// <inheritdoc />
        public bool UnhideAll()
        {
            return _hideHelpers.UnhideAll();
        }

        #endregion
    }
}