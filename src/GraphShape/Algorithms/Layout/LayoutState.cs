using System;
using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Layout iteration status.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class LayoutState<TVertex, TEdge>
    {
        /// <summary>
        /// Gets the positions of every vertex in this state of the layout process.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, Point> Positions { get; protected set; }

        /// <summary>
        /// Gets the positions of every vertex after overlap removal
        /// in this state of the layout process.
        /// </summary>
        [CanBeNull]
        public IDictionary<TVertex, Point> OverlapRemovedPositions { get; set; }

        /// <summary>
        /// Gets the edge routes in this state of the layout process.
        /// </summary>
        [CanBeNull]
        public IDictionary<TEdge, Point[]> RouteInfos { get; set; }

        /// <summary>
        /// Gets how much time did it take to compute the position of the vertices (till the end of this iteration).
        /// </summary>
        public TimeSpan ComputationTime { get; protected set; }

        /// <summary>
        /// Gets the index of the iteration.
        /// </summary>
        public int Iteration { get; protected set; }

        /// <summary>
        /// Gets the status message of this layout state.
        /// </summary>
        [NotNull]
        public string Message { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutState{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="positions">Vertices positions.</param>
        /// <param name="overlapRemovedPositions">Vertices position after overlap removal.</param>
        /// <param name="routeInfos">Edge routes information.</param>
        /// <param name="computationTime">Iteration computation time.</param>
        /// <param name="iteration">Iteration number.</param>
        /// <param name="message">Iteration message.</param>
        public LayoutState(
            [NotNull] IDictionary<TVertex, Point> positions,
            [CanBeNull] IDictionary<TVertex, Point> overlapRemovedPositions,
            [CanBeNull] IDictionary<TEdge, Point[]> routeInfos,
            TimeSpan computationTime,
            int iteration,
            [NotNull] string message)
        {
            if (iteration < 0)
                throw new ArgumentOutOfRangeException(nameof(iteration), $"{nameof(iteration)} must be positive or 0.");

            Positions = positions ?? throw new ArgumentNullException(nameof(positions));
            OverlapRemovedPositions = overlapRemovedPositions ?? positions;
            RouteInfos = routeInfos ?? new Dictionary<TEdge, Point[]>(0);
            ComputationTime = computationTime;
            Iteration = iteration;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}