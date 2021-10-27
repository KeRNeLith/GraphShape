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
    /// <typeparam name="TVertexInfo">Vertex information type.</typeparam>
    /// <typeparam name="TEdgeInfo">Edge information type.</typeparam>
    public class TestingCompoundLayoutIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
        : CompoundLayoutIterationEventArgs<TVertex, TEdge>
        , ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Gravitation center position.
        /// </summary>
        public Point GravitationCenter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingCompoundLayoutIterationEventArgs{TVertex,TEdge,TVertexInfo,TEdgeInfo}"/> class.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="message">Message representing the status of the algorithm.</param>
        /// <param name="verticesPositions">Vertices positions associations.</param>
        /// <param name="innerCanvasSizes">Inner canvas vertices sizes associations.</param>
        /// <param name="verticesInfos">Extra vertices information.</param>
        /// <param name="gravitationCenter">Gravitation center.</param>
        public TestingCompoundLayoutIterationEventArgs(
            int iteration,
            double statusInPercent,
            [NotNull] string message,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> innerCanvasSizes,
            [NotNull] IDictionary<TVertex, TVertexInfo> verticesInfos,
            Point gravitationCenter)
            : base(iteration, statusInPercent, message, verticesPositions, innerCanvasSizes)
        {
            VerticesInfos = verticesInfos ?? throw new ArgumentNullException(nameof(verticesInfos));
            GravitationCenter = gravitationCenter;
        }

        /// <inheritdoc />
        [NotNull]
        public IDictionary<TVertex, TVertexInfo> VerticesInfos { get; }

        /// <inheritdoc />
        public IDictionary<TEdge, TEdgeInfo> EdgesInfos { get; } = new Dictionary<TEdge, TEdgeInfo>();

        /// <inheritdoc />
        public override object GetVertexInfo(TVertex vertex)
        {
            if (VerticesInfos.TryGetValue(vertex, out TVertexInfo info))
                return info;
            return null;
        }
    }
}