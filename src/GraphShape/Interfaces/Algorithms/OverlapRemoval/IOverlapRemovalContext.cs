using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Represents an overlap removal context.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public interface IOverlapRemovalContext<TVertex>
    {
        /// <summary>
        /// Overlap rectangles.
        /// </summary>
        [NotNull]
        IDictionary<TVertex, Rect> Rectangles { get; }
    }
}