using System.Linq;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Highlight
{
    /// <summary>
    /// Simple highlight algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class SimpleHighlightAlgorithm<TVertex, TEdge, TGraph> : HighlightAlgorithmBase<TVertex, TEdge, TGraph, IHighlightParameters>
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleHighlightAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="controller">Highlight controller.</param>
        /// <param name="parameters">Highlight algorithm parameters.</param>
        public SimpleHighlightAlgorithm(
            [NotNull] IHighlightController<TVertex, TEdge, TGraph> controller,
            [CanBeNull] IHighlightParameters parameters)
            : base(controller, parameters)
        {
        }

        private void ClearSemiHighlights()
        {
            foreach (TVertex vertex in Controller.SemiHighlightedVertices.ToArray())
                Controller.RemoveSemiHighlightFromVertex(vertex);

            foreach (TEdge edge in Controller.SemiHighlightedEdges.ToArray())
                Controller.RemoveSemiHighlightFromEdge(edge);
        }

        private void ClearAllHighlights()
        {
            ClearSemiHighlights();

            foreach (TVertex vertex in Controller.HighlightedVertices.ToArray())
                Controller.RemoveHighlightFromVertex(vertex);

            foreach (TEdge edge in Controller.HighlightedEdges.ToArray())
                Controller.RemoveHighlightFromEdge(edge);
        }

        /// <summary>
        /// Resets the semi-highlights according to the actually
        /// highlighted vertices/edges.
        /// 
        /// This method should be called if the graph changed,
        /// or the highlights should be reset.
        /// </summary>
        public override void ResetHighlight()
        {
            ClearAllHighlights();
        }

        /// <inheritdoc />
        public override bool OnVertexHighlighting(TVertex vertex)
        {
            ClearAllHighlights();

            if (!Controller.Graph.ContainsVertex(vertex))
                return false;

            // Semi-highlight the in-edges, and the neighbors on their other side
            foreach (TEdge edge in Controller.Graph.InEdges(vertex))
            {
                Controller.SemiHighlightEdge(edge, "InEdge");
                if (Equals(edge.Source, vertex) || Controller.IsHighlightedVertex(edge.Source))
                    continue;

                Controller.SemiHighlightVertex(edge.Source, "Source");
            }

            // Semi-highlight the out-edges
            foreach (TEdge edge in Controller.Graph.OutEdges(vertex))
            {
                Controller.SemiHighlightEdge(edge, "OutEdge");
                if (Equals(edge.Target, vertex) || Controller.IsHighlightedVertex(edge.Target))
                    continue;

                Controller.SemiHighlightVertex(edge.Target, "Target");
            }

            Controller.HighlightVertex(vertex, "None");
            return true;
        }

        /// <inheritdoc />
        public override bool OnVertexHighlightRemoving(TVertex vertex)
        {
            if (!Controller.Graph.ContainsVertex(vertex))
                return false;

            ClearAllHighlights();
            return true;
        }

        /// <inheritdoc />
        public override bool OnEdgeHighlighting(TEdge edge)
        {
            ClearAllHighlights();

            // Highlight the source and the target
            if (!Controller.Graph.ContainsEdge(edge))
                return false;

            Controller.HighlightEdge(edge, null);
            Controller.SemiHighlightVertex(edge.Source, "Source");
            Controller.SemiHighlightVertex(edge.Target, "Target");
            return true;
        }

        /// <inheritdoc />
        public override bool OnEdgeHighlightRemoving(TEdge edge)
        {
            if (!Controller.Graph.ContainsEdge(edge))
                return false;

            ClearAllHighlights();
            return true;
        }
    }
}