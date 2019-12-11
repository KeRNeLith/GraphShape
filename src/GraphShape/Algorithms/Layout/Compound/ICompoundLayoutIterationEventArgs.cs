using System.Collections.Generic;
using System.Windows;

namespace GraphShape.Algorithms.Layout.Compound
{
    public interface ICompoundLayoutIterationEventArgs<TVertex> 
        : ILayoutIterationEventArgs<TVertex>
        where TVertex : class
    {
        IDictionary<TVertex, Size> InnerCanvasSizes { get; }
    }
}
