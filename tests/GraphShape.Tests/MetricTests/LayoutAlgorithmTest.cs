using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.EdgeRouting;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.Layout.Simple.Circular;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests
{
    /// <summary>
    /// Tests related to layout algorithms.
    /// </summary>
    [TestFixture]
    public class LayoutAlgorithmTest
    {
        #region Test helpers

        public class LayoutResults
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

            public void CheckResult(int maxCrossCount)
            {
                Assert.IsTrue(PositionsSet);
                Assert.AreEqual(0, OverlapCount);
                Assert.AreEqual(0, OverlappedArea);
                Assert.LessOrEqual(CrossCount, maxCrossCount);
            }
        }

        [Pure]
        [NotNull]
        private static IDictionary<TVertex, Size> GetVerticesSizes<TVertex>(
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            return vertices.ToDictionary(
                vertex => vertex,
                vertex => new Size(20, 20));
        }

        [Pure]
        [NotNull]
        private static LayoutResults ExecuteLayoutAlgorithm<TVertex, TEdge>(
            [NotNull] ILayoutAlgorithm<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> algorithm,
            [NotNull] IDictionary<TVertex, Size> vertexSizes)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            var results = new LayoutResults();

            Assert.DoesNotThrow(algorithm.Compute);

            IDictionary<TEdge, Point[]> edgeRoutes =
                algorithm is IEdgeRoutingAlgorithm<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> routingAlgorithm
                    ? routingAlgorithm.EdgeRoutes
                    : new Dictionary<TEdge, Point[]>();

            // Compute metrics
            var positionsMetric = new PositionsMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                algorithm.VisitedGraph,
                algorithm.VerticesPositions,
                vertexSizes,
                edgeRoutes);
            positionsMetric.Calculate();
            results.PositionsSet = positionsMetric.PositionsSet;


            var overlapMetric = new OverlapMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                algorithm.VisitedGraph,
                algorithm.VerticesPositions,
                vertexSizes,
                edgeRoutes);
            overlapMetric.Calculate();

            results.OverlapCount = overlapMetric.OverlapCount;
            results.OverlappedArea = overlapMetric.OverlappedArea;


            var areaMetric = new LayoutAreaMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                algorithm.VisitedGraph,
                algorithm.VerticesPositions,
                vertexSizes,
                edgeRoutes);
            areaMetric.Calculate();
            results.TopLeft = areaMetric.TopLeft;
            results.BottomRight = areaMetric.BottomRight;
            results.Area = areaMetric.Area;
            results.Ratio = areaMetric.Ratio;

            var edgeMetric = new EdgeCrossingCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                algorithm.VisitedGraph,
                algorithm.VerticesPositions,
                vertexSizes,
                edgeRoutes);
            edgeMetric.Calculate();
            results.CrossCount = edgeMetric.CrossCount;
            results.MaximumEdgeLength = edgeMetric.MaximumEdgeLength;
            results.MinimumEdgeLength = edgeMetric.MinimumEdgeLength;
            results.AverageEdgeLength = edgeMetric.AverageEdgeLength;

            return results;
        }

        private static void CheckCircularLayout<TVertex, TEdge>(
            [NotNull] CircularLayoutAlgorithm<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> algorithm)
            where TEdge : IEdge<TVertex>
        {
            IDictionary<TVertex, Point> verticesPositions = algorithm.VerticesPositions;
            if (verticesPositions.Count < 3)
                return;

            const double epsilon = 1.0;
            Point[] positions = algorithm.VerticesPositions.Values.ToArray();
            FindCircle(positions[0], positions[1], positions[2], out Point center, out double radius);

            foreach (Point position in positions)
            {
                Assert.AreEqual(radius, (center - position).Length, epsilon);
            }

            #region Local function

            void FindCircle(Point p1, Point p2, Point p3, out Point middle, out double r)
            {
                double x1 = p1.X;
                double y1 = p1.Y;
                double x2 = p2.X;
                double y2 = p2.Y;
                double x3 = p3.X;
                double y3 = p3.Y;

                double x12 = x1 - x2;
                double x13 = x1 - x3;

                double y12 = y1 - y2;
                double y13 = y1 - y3;

                double y31 = y3 - y1;
                double y21 = y2 - y1;

                double x31 = x3 - x1;
                double x21 = x2 - x1;

                // x1^2 - x3^2 
                double sx13 = Math.Pow(x1, 2) - Math.Pow(x3, 2);

                // y1^2 - y3^2 
                double sy13 = Math.Pow(y1, 2) - Math.Pow(y3, 2);

                double sx21 = Math.Pow(x2, 2) - Math.Pow(x1, 2);

                double sy21 = Math.Pow(y2, 2) - Math.Pow(y1, 2);

                double f = (sx13 * x12 + sy13 * x12 + sx21 * x13 + sy21 * x13)
                           / (2 * (y31 * x12 - y21 * x13));
                double g = (sx13 * y12 + sy13 * y12 + sx21 * y13 + sy21 * y13)
                           / (2 * (x31 * y12 - x21 * y13));

                double c = -Math.Pow(x1, 2) - Math.Pow(y1, 2) - 2 * g * x1 - 2 * f * y1;

                // Equation of circle be x^2 + y^2 + 2*g*x + 2*f*y + c = 0 
                // where centre is (h = -g, k = -f) and radius r 
                // as r^2 = h^2 + k^2 - c
                double h = -g;
                double k = -f;
                double squareRadius = h * h + k * k - c;

                r = Math.Round(Math.Sqrt(squareRadius), 5);
                middle = new Point(h, k);
            }

            #endregion
        }

        #endregion

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> CircularLayoutTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new BidirectionalGraph<string, Edge<string>>(), 0)
                {
                    TestName = "Empty graph"
                };

                var graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVertex("0");
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Single vertex graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "0"));
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Single vertex self loop graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Two vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVertex("2");
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Three vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Four vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "1"));
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Multiple vertices self loop graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "2"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "4"));
                graph.AddVerticesAndEdge(new Edge<string>("4", "5"));
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Line graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "2"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "4"));
                graph.AddVerticesAndEdge(new Edge<string>("4", "5"));
                graph.AddVerticesAndEdge(new Edge<string>("5", "1"));
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Cycle line graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "2"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "4"));
                graph.AddVerticesAndEdge(new Edge<string>("4", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("4", "5"));
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Cycle graph"
                };

                IBidirectionalGraph<string, Edge<string>> completeGraph = GraphFactory.CreateCompleteGraph(
                    7,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t));
                yield return new TestCaseData(completeGraph, 140)
                {
                    TestName = "Complete graph"
                };

                IBidirectionalGraph<string, Edge<string>> tree = GraphFactory.CreateTree(
                    20,
                    2,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(tree, 40)
                {
                    TestName = "Tree graph 20 vertices/2 branches"
                };
                
                tree = GraphFactory.CreateTree(
                    25,
                    5,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(tree, 50)
                {
                    TestName = "Tree graph 25 vertices/5 branches"
                };
            }
        }

        [TestCaseSource(nameof(CircularLayoutTestCases))]
        public void CircularLayoutAlgorithm(
            [NotNull] IBidirectionalGraph<string, Edge<string>> graph,
            int maxCrossCount)
        {
            IDictionary<string, Size> verticesSizes = GetVerticesSizes(graph.Vertices);
            var algorithm = new CircularLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(
                graph,
                null,
                verticesSizes,
                new CircularLayoutParameters());

            LayoutResults results = ExecuteLayoutAlgorithm(algorithm, verticesSizes);
            results.CheckResult(maxCrossCount);
            CheckCircularLayout(algorithm);
        }
    }
}