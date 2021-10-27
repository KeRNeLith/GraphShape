using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Contextual
{
    /// <summary>
    /// Contextual graph layout context.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class ContextualLayoutContext<TVertex, TEdge, TGraph> : LayoutContext<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// The selected vertex.
        /// </summary>
        [NotNull]
        public TVertex SelectedVertex { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextualLayoutContext{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="graph">Graph to layout.</param>
        /// <param name="selectedVertex">THe selected vertex.</param>
        /// <param name="positions">Vertices positions.</param>
        /// <param name="sizes">Vertices sizes.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="selectedVertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="sizes"/> is <see langword="null"/>.</exception>
        public ContextualLayoutContext(
            [CanBeNull] TGraph graph,
            [NotNull] TVertex selectedVertex,
            [CanBeNull] IDictionary<TVertex, Point> positions,
            [NotNull] IDictionary<TVertex, Size> sizes)
            : base(graph, positions, sizes, LayoutMode.Simple)
        {
            if (selectedVertex == null)
                throw new ArgumentNullException(nameof(selectedVertex));

            SelectedVertex = selectedVertex;
        }
    }
}