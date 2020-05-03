using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Compound
{
    /// <summary>
    /// Represents a compound layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public interface ICompoundLayoutAlgorithm<TVertex, in TEdge, out TGraph> : ILayoutAlgorithm<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Inner canvas vertices sizes.
        /// </summary>
        [NotNull]
        IDictionary<TVertex, Size> InnerCanvasSizes { get; }
    }
}
