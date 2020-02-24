using System.Collections.Generic;
using QuikGraph;
using System.Windows;

namespace GraphShape.Algorithms.Layout.Compound
{
    public class CompoundLayoutIterationEventArgs<TVertex, TEdge>
        : LayoutIterationEventArgs<TVertex, TEdge>, ICompoundLayoutIterationEventArgs<TVertex>
        where TVertex : class
        where TEdge : IEdge<TVertex>
    {
        public CompoundLayoutIterationEventArgs(
            int iteration, 
            double statusInPercent, 
            string message,
            IDictionary<TVertex, Point> verticesPositions,
            IDictionary<TVertex, Size> innerCanvasSizes)
            : base(iteration, statusInPercent, message, verticesPositions)
        {
            InnerCanvasSizes = innerCanvasSizes;
        }

        #region ICompoundLayoutIterationEventArgs<TVertex> Members

        public IDictionary<TVertex, Size> InnerCanvasSizes
        {
            get; private set;
        }

        #endregion
    }
}
