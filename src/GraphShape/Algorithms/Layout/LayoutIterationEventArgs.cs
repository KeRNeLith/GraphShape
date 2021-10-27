using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Information on a layout algorithm iteration.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class LayoutIterationEventArgs<TVertex, TEdge>
        : EventArgs
        , ILayoutInfoIterationEventArgs<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge}"/> class.
        /// </summary>
        public LayoutIterationEventArgs()
            : this(0, 0, string.Empty, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        public LayoutIterationEventArgs(int iteration, double statusInPercent)
            : this(iteration, statusInPercent, string.Empty, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="message">Message representing the status of the algorithm.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        public LayoutIterationEventArgs(int iteration, double statusInPercent, [NotNull] string message)
            : this(iteration, statusInPercent, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="verticesPositions">Vertices positions associations.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        public LayoutIterationEventArgs(
            int iteration,
            double statusInPercent,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions)
            : this(iteration, statusInPercent, string.Empty, verticesPositions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="message">Message representing the status of the algorithm.</param>
        /// <param name="verticesPositions">Vertices positions associations.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        public LayoutIterationEventArgs(
            int iteration,
            double statusInPercent,
            [NotNull] string message,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions)
        {
            if (statusInPercent < 0)
                throw new ArgumentOutOfRangeException(nameof(statusInPercent), $"{nameof(statusInPercent)} must be positive or 0.");
            if (iteration < 0)
                throw new ArgumentOutOfRangeException(nameof(iteration), $"{nameof(iteration)} must be positive or 0.");

            StatusInPercent = statusInPercent;
            Iteration = iteration;
            Abort = false;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            VerticesPositions = verticesPositions;
        }

        /// <inheritdoc />
        public double StatusInPercent { get; }

        /// <inheritdoc />
        public bool Abort { get; set; }

        /// <inheritdoc />
        public int Iteration { get; }

        /// <inheritdoc />
        public string Message { get; }

        /// <inheritdoc />
        public IDictionary<TVertex, Point> VerticesPositions { get; }

        /// <inheritdoc />
        public virtual object GetVertexInfo(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            return null;
        }

        /// <inheritdoc />
        public virtual object GetEdgeInfo(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            return null;
        }
    }

    /// <summary>
    /// Information on a layout algorithm iteration.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TVertexInfo">Vertex information type.</typeparam>
    /// <typeparam name="TEdgeInfo">Edge information type.</typeparam>
    public class LayoutIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
        : LayoutIterationEventArgs<TVertex, TEdge>
        , ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge,TVertexInfo,TEdgeInfo}"/> class.
        /// </summary>
        public LayoutIterationEventArgs()
            : this(0, 0, string.Empty, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge,TVertexInfo,TEdgeInfo}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        public LayoutIterationEventArgs(int iteration, double statusInPercent)
            : this(iteration, statusInPercent, string.Empty, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge,TVertexInfo,TEdgeInfo}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="message">Message representing the status of the algorithm.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        public LayoutIterationEventArgs(int iteration, double statusInPercent, [NotNull] string message)
            : this(iteration, statusInPercent, message, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge,TVertexInfo,TEdgeInfo}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="verticesPositions">Vertices positions associations.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        public LayoutIterationEventArgs(
            int iteration,
            double statusInPercent,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions)
            : this(iteration, statusInPercent, string.Empty, verticesPositions, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutIterationEventArgs{TVertex,TEdge,TVertexInfo,TEdgeInfo}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="message">Message representing the status of the algorithm.</param>
        /// <param name="verticesPositions">Vertices positions associations.</param>
        /// <param name="verticesInfos">Extra vertices information.</param>
        /// <param name="edgeInfos">Extra edges information.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        public LayoutIterationEventArgs(
            int iteration,
            double statusInPercent,
            [NotNull] string message,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] IDictionary<TVertex, TVertexInfo> verticesInfos,
            [CanBeNull] IDictionary<TEdge, TEdgeInfo> edgeInfos)
            : base(iteration, statusInPercent, message, verticesPositions)
        {
            VerticesInfos = verticesInfos;
            EdgesInfos = edgeInfos;
        }

        /// <inheritdoc />
        public IDictionary<TVertex, TVertexInfo> VerticesInfos { get; }

        /// <inheritdoc />
        public IDictionary<TEdge, TEdgeInfo> EdgesInfos { get; }

        /// <inheritdoc />
        public sealed override object GetVertexInfo(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            if (VerticesInfos != null && VerticesInfos.TryGetValue(vertex, out TVertexInfo info))
                return info;
            return null;
        }

        /// <inheritdoc />
        public sealed override object GetEdgeInfo(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            if (EdgesInfos != null && EdgesInfos.TryGetValue(edge, out TEdgeInfo info))
                return info;
            return null;
        }
    }
}