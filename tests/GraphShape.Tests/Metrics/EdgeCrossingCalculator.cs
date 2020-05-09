using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Tests
{
    /// <summary>
    /// Edge crossing metric.
    /// </summary>
    internal class EdgeCrossingCalculator<TVertex, TEdge, TGraph> : MetricCalculatorBase<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        public EdgeCrossingCalculator(
            [NotNull] TGraph graph,
            [NotNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [NotNull] IDictionary<TEdge, Point[]> edgeRoutes)
            : base(graph, verticesPositions, verticesSizes, edgeRoutes)
        {
        }

        public int CrossCount { get; private set; }

        public double MinimumEdgeLength { get; private set; }
        public double MaximumEdgeLength { get; private set; }
        public double AverageEdgeLength { get; private set; }

        /// <inheritdoc />
        public override void Calculate()
        {
            TEdge[] edges = Graph.Edges.ToArray();
            var edgePoints = new List<Point>[edges.Length];

            // Create the points of the edges
            for (int i = 0; i < edges.Length; ++i)
            {
                TEdge edge = edges[i];
                List<Point> points = EdgeRoutes.TryGetValue(edge, out Point[] route) && route != null && route.Length > 0
                    ? new List<Point>(route.Length + 2)
                    : new List<Point>(2);

                points.Add(Positions[edge.Source]);
                if (route != null && route.Length > 0)
                    points.AddRange(route);
                points.Add(Positions[edge.Target]);

                edgePoints[i] = points;

                for (int j = 1; j < points.Count; ++j)
                {
                    double length = (points[j] - points[j - 1]).Length;

                    MinimumEdgeLength = Math.Min(MinimumEdgeLength, length);
                    MaximumEdgeLength = Math.Max(MaximumEdgeLength, length);
                    AverageEdgeLength += length;
                }
            }

            // Check the crosses
            for (int i = 0; i < edges.Length - 1; ++i)
            {
                for (int j = i + 1; j < edges.Length; ++j)
                {
                    List<Point> edgePoints1 = edgePoints[i];
                    List<Point> edgePoints2 = edgePoints[j];

                    for (int ii = 0; ii < edgePoints1.Count - 1; ++ii)
                    {
                        Point pA = edgePoints1[ii];
                        Point pB = edgePoints1[ii + 1];

                        for (int jj = 0; jj < edgePoints2.Count - 1; ++jj)
                        {
                            Point pC = edgePoints2[jj];
                            Point pD = edgePoints2[jj + 1];

                            if (pB.Equals(pC) || pA.Equals(pC) || pA.Equals(pD) || pB.Equals(pD))
                                continue;   // Ignore if source and/or target are the same

                            // [AB]
                            double xA = pA.X;
                            double yA = pA.Y;
                            double xB = pB.X;
                            double yB = pB.Y;

                            // [CD]
                            double xC = pC.X;
                            double yC = pC.Y;
                            double xD = pD.X;
                            double yD = pD.Y;

                            // The edges crosses each other
                            bool segmentCrossing =
                                ((xC - xA) * (yB - yA) + (yC - yA) * (xA - xB) < 0) ^
                                ((xD - xA) * (yB - yA) + (yD - yA) * (xA - xB) < 0)
                                &&
                                ((xA - xC) * (yD - yC) + (yA - yC) * (xC - xD) < 0) ^
                                ((xB - xC) * (yD - yC) + (yB - yC) * (xC - xD) < 0);

                            if (segmentCrossing)
                                ++CrossCount;
                        }
                    }
                }
            }
        }
    }
}
