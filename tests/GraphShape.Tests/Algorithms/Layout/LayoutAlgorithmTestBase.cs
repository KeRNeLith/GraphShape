﻿using System.Windows;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.EdgeRouting;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.OverlapRemoval;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Base class for tests related to layout algorithms.
    /// </summary>
    internal abstract class LayoutAlgorithmTestBase
    {
        #region Test helpers

        protected class LayoutResults
        {
            public bool PositionsSet { get; set; } = true;

            public int OverlapCount { get; set; }
            public double OverlappedArea { get; set; }

            public Point TopLeft { get; set; }
            public Point BottomRight { get; set; }
            public double Area { get; set; }
            public double Ratio { get; set; }

            public int CrossCount { get; set; }
            public double MinimumEdgeLength { get; set; }
            public double MaximumEdgeLength { get; set; }
            public double AverageEdgeLength { get; set; }

            public void CheckResult(int maxCrossCount, int maxOverlapped = 0)
            {
                Assert.IsTrue(PositionsSet);
                if (maxOverlapped == 0)
                {
                    Assert.AreEqual(0, OverlapCount);
                    Assert.AreEqual(0, OverlappedArea);
                }
                else
                {
                    Assert.LessOrEqual(OverlapCount, maxOverlapped);
                }
                Assert.LessOrEqual(CrossCount, maxCrossCount);
            }
        }

        [Pure]
        [NotNull]
        protected static IDictionary<TVertex, Size> GetVerticesSizes<TVertex>(
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            return vertices.ToDictionary(
                vertex => vertex,
                vertex => new Size(20, 20));
        }

        [Pure]
        [NotNull]
        protected static LayoutResults ExecuteLayoutAlgorithm<TVertex, TEdge>(
            [NotNull] ILayoutAlgorithm<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> algorithm,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            bool requireOverlapRemoval = false)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            var results = new LayoutResults();

            Assert.DoesNotThrow(algorithm.Compute);
            IDictionary<TVertex, Point> verticesPositions = algorithm.VerticesPositions;

            if (requireOverlapRemoval)
            {
                var rectangles = new Dictionary<TVertex, Rect>();
                foreach (TVertex vertex in algorithm.VisitedGraph.Vertices)
                {
                    Point position = algorithm.VerticesPositions[vertex];
                    Size size = verticesSizes[vertex];
                    rectangles[vertex] = new Rect(
                        position.X - size.Width * (float)0.5,
                        position.Y - size.Height * (float)0.5,
                        size.Width,
                        size.Height);
                }

                var overlapRemoval = new FSAAlgorithm<TVertex>(
                    rectangles,
                    new OverlapRemovalParameters());
                Assert.DoesNotThrow(overlapRemoval.Compute);

                foreach (KeyValuePair<TVertex, Rect> pair in overlapRemoval.Rectangles)
                {
                    verticesPositions[pair.Key] = new Point(
                        pair.Value.Left + pair.Value.Size.Width * 0.5,
                        pair.Value.Top + pair.Value.Size.Height * 0.5);
                }
            }

            IDictionary<TEdge, Point[]> edgeRoutes =
                algorithm is IEdgeRoutingAlgorithm<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> routingAlgorithm
                    ? routingAlgorithm.EdgeRoutes
                    : new Dictionary<TEdge, Point[]>();

            // Compute metrics
            var positionsMetric = new PositionsMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                algorithm.VisitedGraph,
                verticesPositions,
                verticesSizes,
                edgeRoutes);
            positionsMetric.Calculate();
            results.PositionsSet = positionsMetric.PositionsSet;


            var overlapMetric = new OverlapMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                algorithm.VisitedGraph,
                verticesPositions,
                verticesSizes,
                edgeRoutes);
            overlapMetric.Calculate();

            results.OverlapCount = overlapMetric.OverlapCount;
            results.OverlappedArea = overlapMetric.OverlappedArea;


            var areaMetric = new LayoutAreaMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                algorithm.VisitedGraph,
                verticesPositions,
                verticesSizes,
                edgeRoutes);
            areaMetric.Calculate();
            results.TopLeft = areaMetric.TopLeft;
            results.BottomRight = areaMetric.BottomRight;
            results.Area = areaMetric.Area;
            results.Ratio = areaMetric.Ratio;

            var edgeMetric = new EdgeCrossingCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                algorithm.VisitedGraph,
                verticesPositions,
                verticesSizes,
                edgeRoutes);
            edgeMetric.Calculate();
            results.CrossCount = edgeMetric.CrossCount;
            results.MaximumEdgeLength = edgeMetric.MaximumEdgeLength;
            results.MinimumEdgeLength = edgeMetric.MinimumEdgeLength;
            results.AverageEdgeLength = edgeMetric.AverageEdgeLength;

            return results;
        }

        #endregion
    }
}