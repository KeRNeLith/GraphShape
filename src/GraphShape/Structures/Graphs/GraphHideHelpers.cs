using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape
{
    internal sealed class GraphHideHelpers<TVertex, TEdge> : ISoftMutableGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        #region Helper Types

        private sealed class HiddenCollection
        {
            [NotNull, ItemNotNull]
            public List<TVertex> HiddenVertices { get; } = new List<TVertex>();

            [NotNull, ItemNotNull]
            public List<TEdge> HiddenEdges { get; } = new List<TEdge>();
        }

        #endregion

        #region Properties, fields, events

        [NotNull]
        private readonly IMutableBidirectionalGraph<TVertex, TEdge> _graph;

        [NotNull, ItemNotNull]
        private readonly List<TVertex> _hiddenVertices = new List<TVertex>();

        [NotNull, ItemNotNull]
        private readonly List<TEdge> _hiddenEdges = new List<TEdge>();

        [NotNull]
        private readonly IDictionary<string, HiddenCollection> _hiddenCollections = new Dictionary<string, HiddenCollection>();

        [NotNull]
        private readonly IDictionary<TVertex, List<TEdge>> _hiddenEdgesOf = new Dictionary<TVertex, List<TEdge>>();

        public event VertexAction<TVertex> VertexHidden;
        public event VertexAction<TVertex> VertexUnhidden;

        public event EdgeAction<TVertex, TEdge> EdgeHidden;
        public event EdgeAction<TVertex, TEdge> EdgeUnhidden;

        #endregion

        public GraphHideHelpers([NotNull] IMutableBidirectionalGraph<TVertex, TEdge> managedGraph)
        {
            Debug.Assert(managedGraph != null);

            _graph = managedGraph;
        }

        #region Event handlers, helper methods

        /// <summary>
        /// Returns every edges connected with the <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>Edges, adjacent to the vertex <code>vertex</code>.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        private IEnumerable<TEdge> EdgesFor([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);
            return _graph.InEdges(vertex)
                .Concat(_graph.OutEdges(vertex).Where(e => !e.IsSelfEdge()));
        }

        [Pure]
        [NotNull]
        private HiddenCollection GetHiddenCollection([NotNull] string tag)
        {
            if (!_hiddenCollections.TryGetValue(tag, out HiddenCollection collection))
            {
                collection = new HiddenCollection();
                _hiddenCollections[tag] = collection;
            }

            return collection;
        }

        private void OnVertexHidden([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            VertexHidden?.Invoke(vertex);
        }

        private void OnVertexUnhidden([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            VertexUnhidden?.Invoke(vertex);
        }

        private void OnEdgeHidden([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeHidden?.Invoke(edge);
        }

        private void OnEdgeUnhidden([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeUnhidden?.Invoke(edge);
        }

        #endregion

        #region ISoftMutableGraph<TVertex,TEdge>

        /// <inheritdoc />
        public IEnumerable<TVertex> HiddenVertices => _hiddenVertices;

        /// <inheritdoc />
        public int HiddenVertexCount => _hiddenVertices.Count;

        /// <inheritdoc />
        public bool HideVertex(TVertex vertex)
        {
            if (!_hiddenVertices.Contains(vertex))
            {
                if (_graph.ContainsVertex(vertex))
                {
                    HideEdges(EdgesFor(vertex));

                    // Hide the vertex
                    _graph.RemoveVertex(vertex);
                    _hiddenVertices.Add(vertex);
                    OnVertexHidden(vertex);
                    return true;
                }

                throw new VertexNotFoundException();
            }

            return false;
        }

        /// <inheritdoc />
        public bool HideVertex(TVertex vertex, string tag)
        {
            HiddenCollection collection = GetHiddenCollection(tag);
            var hideEdgeHandler = new EdgeAction<TVertex, TEdge>(e => collection.HiddenEdges.Add(e));
            var hideVertexHandler = new VertexAction<TVertex>(v => collection.HiddenVertices.Add(v));

            EdgeHidden += hideEdgeHandler;
            VertexHidden += hideVertexHandler;

            bool hidden = HideVertex(vertex);

            EdgeHidden -= hideEdgeHandler;
            VertexHidden -= hideVertexHandler;

            return hidden;
        }

        /// <inheritdoc />
        public void HideVertices(IEnumerable<TVertex> vertices)
        {
            foreach (TVertex vertex in vertices.ToArray())
            {
                HideVertex(vertex);
            }
        }

        /// <inheritdoc />
        public void HideVertices(IEnumerable<TVertex> vertices, string tag)
        {
            foreach (TVertex vertex in vertices.ToArray())
            {
                HideVertex(vertex, tag);
            }
        }

        /// <inheritdoc />
        public void HideVerticesIf(Predicate<TVertex> predicate, string tag)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));
            if (tag is null)
                throw new ArgumentNullException(nameof(tag));

            HideVertices(_graph.Vertices.Where(vertex => predicate(vertex)), tag);
        }

        /// <inheritdoc />
        public bool IsHiddenVertex(TVertex vertex)
        {
            if (!_graph.ContainsVertex(vertex))
            {
                if (_hiddenVertices.Contains(vertex))
                    return true;

                throw new VertexNotFoundException();
            }

            return false;
        }

        /// <inheritdoc />
        public bool UnhideVertex(TVertex vertex)
        {
            // If the vertex is not hidden, it's an error
            if (!IsHiddenVertex(vertex))
                return false;

            // Unhide the vertex
            _graph.AddVertex(vertex);
            _hiddenVertices.Remove(vertex);
            OnVertexUnhidden(vertex);

            return true;
        }

        /// <inheritdoc />
        public void UnhideVertexAndEdges(TVertex vertex)
        {
            UnhideVertex(vertex);
            _hiddenEdgesOf.TryGetValue(vertex, out List<TEdge> hiddenEdges);
            if (hiddenEdges != null)
            {
                UnhideEdges(hiddenEdges);
            }
        }

        [CanBeNull, ItemNotNull]
        [ContractAnnotation("createIfNotExists:true => notnull")]
        private List<TEdge> GetHiddenEdgeListOf([NotNull] TVertex vertex, bool createIfNotExists)
        {
            Debug.Assert(vertex != null);

            if (_hiddenEdgesOf.TryGetValue(vertex, out List<TEdge> hiddenEdges) || !createIfNotExists)
                return hiddenEdges;

            hiddenEdges = new List<TEdge>();
            _hiddenEdgesOf[vertex] = hiddenEdges;
            return hiddenEdges;
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> HiddenEdges => _hiddenEdges;

        /// <inheritdoc />
        public int HiddenEdgeCount => _hiddenEdges.Count;

        /// <inheritdoc />
        public bool HideEdge(TEdge edge)
        {
            if (_graph.ContainsEdge(edge) && !_hiddenEdges.Contains(edge))
            {
                _graph.RemoveEdge(edge);
                _hiddenEdges.Add(edge);

                GetHiddenEdgeListOf(edge.Source, true).Add(edge);
                if (!edge.IsSelfEdge())
                {
                    GetHiddenEdgeListOf(edge.Target, true).Add(edge);
                }

                OnEdgeHidden(edge);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool HideEdge(TEdge edge, string tag)
        {
            HiddenCollection collection = GetHiddenCollection(tag);
            var hideEdgeHandler = new EdgeAction<TVertex, TEdge>(e => collection.HiddenEdges.Add(e));
            EdgeHidden += hideEdgeHandler;
            bool hidden = HideEdge(edge);
            EdgeHidden -= hideEdgeHandler;
            return hidden;
        }

        /// <inheritdoc />
        public void HideEdges(IEnumerable<TEdge> edges)
        {
            foreach (TEdge edge in edges.ToArray())
            {
                HideEdge(edge);
            }
        }

        /// <inheritdoc />
        public void HideEdges(IEnumerable<TEdge> edges, string tag)
        {
            foreach (TEdge edge in edges.ToArray())
            {
                HideEdge(edge, tag);
            }
        }

        /// <inheritdoc />
        public void HideEdgesIf(Predicate<TEdge> predicate, string tag)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));
            if (tag is null)
                throw new ArgumentNullException(nameof(tag));

            HideEdges(_graph.Edges.Where(edge => predicate(edge)), tag);
        }

        /// <inheritdoc />
        public bool IsHiddenEdge(TEdge edge)
        {
            return !_graph.ContainsEdge(edge) && _hiddenEdges.Contains(edge);
        }

        /// <inheritdoc />
        public bool UnhideEdge(TEdge edge)
        {
            // If edge is not hidden or has at least one of its vertex hidden => does nothing
            if (!IsHiddenEdge(edge) || IsHiddenVertex(edge.Source) || IsHiddenVertex(edge.Target))
                return false;

            // Unhide the edge
            _graph.AddEdge(edge);
            _hiddenEdges.Remove(edge);

            // ReSharper disable PossibleNullReferenceException
            // Justification: The list must exists (at least empty) if unhiding an edge
            // because it has been created on HideEdge call
            GetHiddenEdgeListOf(edge.Source, false).Remove(edge);
            GetHiddenEdgeListOf(edge.Target, false).Remove(edge);
            // ReSharper restore PossibleNullReferenceException

            OnEdgeUnhidden(edge);
            return true;
        }

        /// <inheritdoc />
        public void UnhideEdges(IEnumerable<TEdge> edges)
        {
            foreach (TEdge edge in edges.ToArray())
            {
                UnhideEdge(edge);
            }
        }

        /// <inheritdoc />
        public void UnhideEdgesIf(Predicate<TEdge> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            UnhideEdges(_hiddenEdges.Where(edge => predicate(edge)));
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> HiddenEdgesOf(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            if (!_graph.ContainsVertex(vertex) && !_hiddenVertices.Contains(vertex))
                throw new VertexNotFoundException();
            return GetHiddenEdgeListOf(vertex, false) ?? Enumerable.Empty<TEdge>();
        }

        /// <inheritdoc />
        public int HiddenEdgeCountOf(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            if (!_graph.ContainsVertex(vertex) && !_hiddenVertices.Contains(vertex))
                throw new VertexNotFoundException();
            return GetHiddenEdgeListOf(vertex, false)?.Count ?? 0;
        }

        /// <inheritdoc />
        public bool Unhide(string tag)
        {
            HiddenCollection collection = GetHiddenCollection(tag);
            foreach (TVertex vertex in collection.HiddenVertices)
            {
                UnhideVertex(vertex);
            }

            foreach (TEdge edge in collection.HiddenEdges)
            {
                UnhideEdge(edge);
            }

            return _hiddenCollections.Remove(tag);
        }

        /// <inheritdoc />
        public bool UnhideAll()
        {
            while (_hiddenVertices.Count > 0)
            {
                UnhideVertex(_hiddenVertices[0]);
            }

            while (_hiddenEdges.Count > 0)
            {
                UnhideEdge(_hiddenEdges[0]);
            }

            return true;
        }

        #endregion

        #region IGraph<TVertex,TEdge>

        /// <summary>
        /// <see cref="TryGetOutEdges"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool IsDirected => throw new NotSupportedException();

        /// <summary>
        /// <see cref="TryGetOutEdges"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool AllowParallelEdges => throw new NotSupportedException();

        #endregion

        #region IVertexSet<TVertex,TEdge>

        /// <summary>
        /// <see cref="ContainsVertex"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool ContainsVertex(TVertex vertex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="IsVerticesEmpty"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool IsVerticesEmpty => throw new NotSupportedException();

        /// <summary>
        /// <see cref="VertexCount"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public int VertexCount => throw new NotSupportedException();

        /// <summary>
        /// <see cref="Vertices"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public IEnumerable<TVertex> Vertices => throw new NotSupportedException();

        #endregion

        #region IEdgeListGraph<TVertex,TEdge>

        /// <summary>
        /// <see cref="IsEdgesEmpty"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool IsEdgesEmpty => throw new NotSupportedException();

        /// <summary>
        /// <see cref="EdgeCount"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public int EdgeCount => throw new NotSupportedException();

        /// <summary>
        /// <see cref="Edges"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public IEnumerable<TEdge> Edges => throw new NotSupportedException();

        /// <summary>
        /// <see cref="ContainsEdge(TEdge)"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool ContainsEdge(TEdge edge)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="ContainsEdge(TVertex,TVertex)"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IImplicitGraph<TVertex,TEdge>

        /// <summary>
        /// <see cref="IsOutEdgesEmpty"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool IsOutEdgesEmpty(TVertex vertex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="OutDegree"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public int OutDegree(TVertex vertex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="OutEdges"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public IEnumerable<TEdge> OutEdges(TVertex vertex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="TryGetOutEdges"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool TryGetOutEdges(TVertex vertex, out IEnumerable<TEdge> edges)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="OutEdge"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public TEdge OutEdge(TVertex vertex, int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IIncidenceGraph<TVertex,TEdge>

        /// <summary>
        /// <see cref="TryGetEdge"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="TryGetEdges"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IBidirectionalGraph<TVertex,TEdge>

        /// <summary>
        /// <see cref="IsInEdgesEmpty"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool IsInEdgesEmpty(TVertex vertex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="InDegree"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public int InDegree(TVertex vertex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="InEdges"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public IEnumerable<TEdge> InEdges(TVertex vertex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="TryGetInEdges"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public bool TryGetInEdges(TVertex vertex, out IEnumerable<TEdge> edges)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="InEdge"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public TEdge InEdge(TVertex vertex, int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="Degree"/> is not implemented for this helper.
        /// </summary>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public int Degree(TVertex vertex)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}