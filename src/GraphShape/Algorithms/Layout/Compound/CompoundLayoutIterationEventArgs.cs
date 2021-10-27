using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Information on a compound layout algorithm iteration.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class CompoundLayoutIterationEventArgs<TVertex, TEdge>
        : LayoutIterationEventArgs<TVertex, TEdge>
        , ICompoundLayoutIterationEventArgs<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundLayoutIterationEventArgs{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="message">Message representing the status of the algorithm.</param>
        /// <param name="verticesPositions">Vertices positions associations.</param>
        /// <param name="innerCanvasSizes">Inner canvas vertices sizes associations.</param>
        public CompoundLayoutIterationEventArgs(
            int iteration,
            double statusInPercent,
            [NotNull] string message,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> innerCanvasSizes)
            : base(iteration, statusInPercent, message, verticesPositions)
        {
            InnerCanvasSizes = innerCanvasSizes ?? throw new ArgumentNullException(nameof(innerCanvasSizes));
        }

        #region ICompoundLayoutIterationEventArgs<TVertex>

        /// <inheritdoc />
        public IDictionary<TVertex, Size> InnerCanvasSizes { get; }

        #endregion
    }
}