using System;
using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Tests
{
    /// <summary>
    /// Layout metrics.
    /// </summary>
    internal class LayoutAreaMetricCalculator<TVertex, TEdge, TGraph> : MetricCalculatorBase<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        public LayoutAreaMetricCalculator(
            [NotNull] TGraph graph,
            [NotNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [NotNull] IDictionary<TEdge, Point[]> edgeRoutes)
            : base(graph, verticesPositions, verticesSizes, edgeRoutes)
        {
        }

        public double Area { get; private set; }
        public double Ratio { get; private set; }

        /// <inheritdoc />
        public override void Calculate()
        {
            var topLeft = new Point(double.PositiveInfinity, double.PositiveInfinity);
            var bottomRight = new Point(double.NegativeInfinity, double.NegativeInfinity);

            foreach (TVertex vertex in Graph.Vertices)
            {
                Point pos = Positions[vertex];
                Size size = Sizes[vertex];
                topLeft.X = Math.Min(pos.X - size.Width / 2.0, topLeft.X);
                topLeft.Y = Math.Min(pos.Y - size.Height / 2.0, topLeft.Y);

                bottomRight.X = Math.Max(pos.X + size.Width / 2.0, bottomRight.X);
                bottomRight.Y = Math.Max(pos.Y + size.Height / 2.0, bottomRight.Y);
            }

            foreach (TEdge edge in Graph.Edges)
            {
                if (!EdgeRoutes.TryGetValue(edge, out Point[] routePoints)
                    || routePoints is null
                    || routePoints.Length == 0)
                    continue;

                foreach (Point point in routePoints)
                {
                    topLeft.X = Math.Min(point.X, topLeft.X);
                    topLeft.Y = Math.Min(point.Y, topLeft.Y);

                    bottomRight.X = Math.Max(point.X, bottomRight.X);
                    bottomRight.Y = Math.Max(point.Y, bottomRight.Y);
                }
            }

            Vector layoutAreaSize = bottomRight - topLeft;

            Area = layoutAreaSize.LengthSquared;
            Ratio = layoutAreaSize.X / layoutAreaSize.Y;
        }
    }
}
