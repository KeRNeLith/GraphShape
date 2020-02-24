using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Represents a layout iteration event arguments.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public interface ILayoutIterationEventArgs<TVertex>
    {
        /// <summary>
        /// Represents the status of the layout algorithm in percent.
        /// </summary>
        double StatusInPercent { get; }

        /// <summary>
        /// If the user sets this value to true, the algorithm aborts as soon as possible.
        /// </summary>
        bool Abort { get; set; }

        /// <summary>
        /// Number of the current iteration.
        /// </summary>
        int Iteration { get; }

        /// <summary>
        /// Message representing the status of the algorithm.
        /// </summary>
        [NotNull]
        string Message { get; }

        /// <summary>
        /// Vertices positions associations.
        /// </summary>
        [CanBeNull]
        IDictionary<TVertex, Point> VerticesPositions { get; }
    }
}
