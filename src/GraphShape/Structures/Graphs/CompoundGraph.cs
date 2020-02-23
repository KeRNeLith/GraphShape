using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuikGraph;
using System.Linq;
using JetBrains.Annotations;

namespace GraphShape
{
    /// <summary>
    /// Compound graph data structure.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class CompoundGraph<TVertex, TEdge> : BidirectionalGraph<TVertex, TEdge>, IMutableCompoundGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly IDictionary<TVertex, TVertex> _parentRegistry =
            new Dictionary<TVertex, TVertex>();

        [NotNull]
        private readonly IDictionary<TVertex, List<TVertex>> _childrenRegistry =
            new Dictionary<TVertex, List<TVertex>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundGraph{TVertex,TEdge}"/> class.
        /// </summary>
        public CompoundGraph()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        public CompoundGraph(bool allowParallelEdges)
            : base(allowParallelEdges)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <param name="capacity">Vertex capacity.</param>
        public CompoundGraph(bool allowParallelEdges, int capacity)
            : base(allowParallelEdges, capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="graph">Graph from which initializing this graph.</param>
        public CompoundGraph([NotNull] IEdgeListGraph<TVertex, TEdge> graph)
            // ReSharper disable once ConstantConditionalAccessQualifier
            : base(graph?.AllowParallelEdges ?? throw new ArgumentNullException(nameof(graph)), graph.VertexCount)
        {
            // Copy the vertices
            // ReSharper disable once VirtualMemberCallInConstructor
            AddVertexRange(graph.Vertices);

            // Copy the edges
            AddEdgeRange(graph.Edges);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="graph">Graph from which initializing this graph.</param>
        public CompoundGraph([NotNull] ICompoundGraph<TVertex, TEdge> graph)
            // ReSharper disable once ConstantConditionalAccessQualifier
            : base(graph?.AllowParallelEdges ?? throw new ArgumentNullException(nameof(graph)), graph.VertexCount)
        {
            // Copy the vertices
            // ReSharper disable once VirtualMemberCallInConstructor
            AddVertexRange(graph.Vertices);

            // Copy the containment information
            foreach (TVertex vertex in graph.Vertices.Where(graph.IsChildVertex))
            {
                TVertex parent = graph.GetParent(vertex);
                // ReSharper disable once AssignNullToNotNullAttribute, Justification: Is a child vertex so must have a parent
                AddChildVertex(parent, vertex);
            }

            // Copy the edges
            AddEdgeRange(graph.Edges);
        }

        [Pure]
        [CanBeNull, ItemNotNull]
        [ContractAnnotation("createIfNotExists:true => notnull")]
        private IList<TVertex> GetChildrenList([NotNull] TVertex vertex, bool createIfNotExists)
        {
            Debug.Assert(vertex != null);

            if (_childrenRegistry.TryGetValue(vertex, out List<TVertex> childrenList) || !createIfNotExists)
                return childrenList;

            childrenList = new List<TVertex>();
            _childrenRegistry[vertex] = childrenList;
            return childrenList;
        }

        #region ICompoundGraph<TVertex,TEdge>

        /// <inheritdoc />
        public IEnumerable<TVertex> SimpleVertices => Vertices.Where(v => !_childrenRegistry.ContainsKey(v));

        /// <inheritdoc />
        public IEnumerable<TVertex> CompoundVertices => _childrenRegistry.Keys;

        /// <inheritdoc />
        public bool AddChildVertex(TVertex parent, TVertex child)
        {
            if (!ContainsVertex(parent))
                throw new VertexNotFoundException("Parent vertex must already be part of the graph.");
            if (!ContainsVertex(child))
                AddVertex(child);
            _parentRegistry[child] = parent;
            GetChildrenList(parent, true).Add(child);
            return true;
        }

        /// <inheritdoc />
        public int AddChildVertexRange(TVertex parent, IEnumerable<TVertex> children)
        {
            if (!ContainsVertex(parent))
                throw new VertexNotFoundException("Parent vertex must already be part of the graph.");
            TVertex[] childrenArray = children.ToArray();
            int count = AddVertexRange(childrenArray);
            IList<TVertex> childrenList = GetChildrenList(parent, true);
            foreach (TVertex vertex in childrenArray)
            {
                _parentRegistry[vertex] = parent;
                childrenList.Add(vertex);
            }
            return count;
        }

        /// <inheritdoc />
        public TVertex GetParent(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
                throw new VertexNotFoundException();
            if (_parentRegistry.TryGetValue(vertex, out TVertex parent))
                return parent;
            return default(TVertex);
        }

        /// <inheritdoc />
        public bool IsChildVertex(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
                throw new VertexNotFoundException();
            return _parentRegistry.ContainsKey(vertex);
        }

        /// <inheritdoc />
        public IEnumerable<TVertex> GetChildrenVertices(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
                throw new VertexNotFoundException();
            return GetChildrenList(vertex, false) ?? Enumerable.Empty<TVertex>();
        }

        /// <inheritdoc />
        public int GetChildrenCount(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
                throw new VertexNotFoundException();
            return GetChildrenList(vertex, false)?.Count ?? 0;
        }

        /// <inheritdoc />
        public bool IsCompoundVertex(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
                throw new VertexNotFoundException();
            return GetChildrenList(vertex, false) != null;
        }

        #endregion

        /// <inheritdoc />
        public override bool RemoveVertex(TVertex vertex)
        {
            bool removed = base.RemoveVertex(vertex);
            if (removed)
            {
                _parentRegistry.Remove(vertex);
                _childrenRegistry.Remove(vertex);

                List<TVertex> verticesToClean = null;
                foreach (KeyValuePair<TVertex, List<TVertex>> pair in _childrenRegistry)
                {
                    pair.Value.RemoveAll(v => Equals(v, vertex));
                    if (pair.Value.Count == 0)
                    {
                        if (verticesToClean is null)
                            verticesToClean = new List<TVertex>();
                        verticesToClean.Add(pair.Key);
                    }
                }

                verticesToClean?.ForEach(v => _childrenRegistry.Remove(v));
            }
            return removed;
        }
    }
}
