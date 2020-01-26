using System;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Highlight
{
    /// <summary>
    /// Base class for all highlight context.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class HighlightContext<TVertex, TEdge, TGraph> : IHighlightContext<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        /// <inheritdoc />
        public TGraph Graph { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightContext{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="graph">Target graph.</param>
        public HighlightContext([NotNull] TGraph graph)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
    }
}