using System.Collections.Generic;
using System.Diagnostics;
using GraphShape.Algorithms.Highlight;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Controls
{
    public partial class GraphLayout<TVertex, TEdge, TGraph> : IHighlightController<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        #region IHighlightController<TVertex,TEdge,TGraph>

        [NotNull]
        private readonly IDictionary<TVertex, object> _highlightedVertices = new Dictionary<TVertex, object>();

        [NotNull]
        private readonly IDictionary<TVertex, object> _semiHighlightedVertices = new Dictionary<TVertex, object>();

        [NotNull]
        private readonly IDictionary<TEdge, object> _highlightedEdges = new Dictionary<TEdge, object>();

        [NotNull]
        private readonly IDictionary<TEdge, object> _semiHighlightedEdges = new Dictionary<TEdge, object>();

        /// <inheritdoc />
        public IEnumerable<TVertex> HighlightedVertices => _highlightedVertices.Keys;

        /// <inheritdoc />
        public IEnumerable<TVertex> SemiHighlightedVertices => _semiHighlightedVertices.Keys;

        /// <inheritdoc />
        public IEnumerable<TEdge> HighlightedEdges => _highlightedEdges.Keys;

        /// <inheritdoc />
        public IEnumerable<TEdge> SemiHighlightedEdges => _semiHighlightedEdges.Keys;

        /// <inheritdoc />
        public bool IsHighlightedVertex(TVertex vertex)
        {
            return _highlightedVertices.ContainsKey(vertex);
        }

        /// <inheritdoc />
        public bool IsHighlightedVertex(TVertex vertex, out object highlightInfo)
        {
            return _highlightedVertices.TryGetValue(vertex, out highlightInfo);
        }

        /// <inheritdoc />
        public bool IsSemiHighlightedVertex(TVertex vertex)
        {
            return _semiHighlightedVertices.ContainsKey(vertex);
        }

        /// <inheritdoc />
        public bool IsSemiHighlightedVertex(TVertex vertex, out object semiHighlightInfo)
        {
            return _semiHighlightedVertices.TryGetValue(vertex, out semiHighlightInfo);
        }

        /// <inheritdoc />
        public bool IsHighlightedEdge(TEdge edge)
        {
            return _highlightedEdges.ContainsKey(edge);
        }

        /// <inheritdoc />
        public bool IsHighlightedEdge(TEdge edge, out object highlightInfo)
        {
            return _highlightedEdges.TryGetValue(edge, out highlightInfo);
        }

        /// <inheritdoc />
        public bool IsSemiHighlightedEdge(TEdge edge)
        {
            return _semiHighlightedEdges.ContainsKey(edge);
        }

        /// <inheritdoc />
        public bool IsSemiHighlightedEdge(TEdge edge, out object semiHighlightInfo)
        {
            return _semiHighlightedEdges.TryGetValue(edge, out semiHighlightInfo);
        }

        /// <inheritdoc />
        public void HighlightVertex(TVertex vertex, object highlightInfo)
        {
            _highlightedVertices[vertex] = highlightInfo;
            if (VerticesControls.TryGetValue(vertex, out VertexControl control))
            {
                GraphElementBehaviour.SetIsHighlighted(control, true);
                GraphElementBehaviour.SetHighlightInfo(control, highlightInfo);
            }
        }

        /// <inheritdoc />
        public void SemiHighlightVertex(TVertex vertex, object semiHighlightInfo)
        {
            _semiHighlightedVertices[vertex] = semiHighlightInfo;
            if (VerticesControls.TryGetValue(vertex, out VertexControl control))
            {
                GraphElementBehaviour.SetIsSemiHighlighted(control, true);
                GraphElementBehaviour.SetSemiHighlightInfo(control, semiHighlightInfo);
            }
        }

        /// <inheritdoc />
        public void HighlightEdge(TEdge edge, object highlightInfo)
        {
            _highlightedEdges[edge] = highlightInfo;
            if (EdgesControls.TryGetValue(edge, out EdgeControl control))
            {
                GraphElementBehaviour.SetIsHighlighted(control, true);
                GraphElementBehaviour.SetHighlightInfo(control, highlightInfo);
            }
        }

        /// <inheritdoc />
        public void SemiHighlightEdge(TEdge edge, object semiHighlightInfo)
        {
            _semiHighlightedEdges[edge] = semiHighlightInfo;
            if (EdgesControls.TryGetValue(edge, out EdgeControl control))
            {
                GraphElementBehaviour.SetIsSemiHighlighted(control, true);
                GraphElementBehaviour.SetSemiHighlightInfo(control, semiHighlightInfo);
            }
        }

        /// <inheritdoc />
        public void RemoveHighlightFromVertex(TVertex vertex)
        {
            _highlightedVertices.Remove(vertex);
            if (VerticesControls.TryGetValue(vertex, out VertexControl control))
            {
                GraphElementBehaviour.SetIsHighlighted(control, false);
                GraphElementBehaviour.SetHighlightInfo(control, null);
            }
        }

        /// <inheritdoc />
        public void RemoveSemiHighlightFromVertex(TVertex vertex)
        {
            _semiHighlightedVertices.Remove(vertex);
            if (VerticesControls.TryGetValue(vertex, out VertexControl control))
            {
                GraphElementBehaviour.SetIsSemiHighlighted(control, false);
                GraphElementBehaviour.SetSemiHighlightInfo(control, null);
            }
        }

        /// <inheritdoc />
        public void RemoveHighlightFromEdge(TEdge edge)
        {
            _highlightedEdges.Remove(edge);
            if (EdgesControls.TryGetValue(edge, out EdgeControl control))
            {
                GraphElementBehaviour.SetIsHighlighted(control, false);
                GraphElementBehaviour.SetHighlightInfo(control, null);
            }
        }

        /// <inheritdoc />
        public void RemoveSemiHighlightFromEdge(TEdge edge)
        {
            _semiHighlightedEdges.Remove(edge);
            if (EdgesControls.TryGetValue(edge, out EdgeControl control))
            {
                GraphElementBehaviour.SetIsSemiHighlighted(control, false);
                GraphElementBehaviour.SetSemiHighlightInfo(control, null);
            }
        }

        #endregion

        private void SetHighlightProperties([NotNull] TVertex vertex, [NotNull] VertexControl vertexControl)
        {
            Debug.Assert(vertex != null);
            Debug.Assert(vertexControl != null);

            if (IsHighlightedVertex(vertex, out object highlightInfo))
            {
                GraphElementBehaviour.SetIsHighlighted(vertexControl, true);
                GraphElementBehaviour.SetHighlightInfo(vertexControl, highlightInfo);
            }

            if (IsSemiHighlightedVertex(vertex, out object semiHighlightInfo))
            {
                GraphElementBehaviour.SetIsSemiHighlighted(vertexControl, true);
                GraphElementBehaviour.SetSemiHighlightInfo(vertexControl, semiHighlightInfo);
            }
        }

        private void SetHighlightProperties(TEdge edge, EdgeControl edgeControl)
        {
            Debug.Assert(edge != null);
            Debug.Assert(edgeControl != null);

            if (IsHighlightedEdge(edge, out object highlightInfo))
            {
                GraphElementBehaviour.SetIsHighlighted(edgeControl, true);
                GraphElementBehaviour.SetHighlightInfo(edgeControl, highlightInfo);
            }

            if (IsSemiHighlightedEdge(edge, out object semiHighlightInfo))
            {
                GraphElementBehaviour.SetIsSemiHighlighted(edgeControl, true);
                GraphElementBehaviour.SetSemiHighlightInfo(edgeControl, semiHighlightInfo);
            }
        }
    }
}