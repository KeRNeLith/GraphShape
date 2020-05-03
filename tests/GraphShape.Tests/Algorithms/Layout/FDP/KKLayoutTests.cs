using System;
using System.Collections.Generic;
using GraphShape.Algorithms.Layout.Simple.FDP;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.Algorithms.AlgorithmTestHelpers;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests related to <see cref="KKLayoutAlgorithm{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class KKLayoutTests : LayoutAlgorithmTestBase
    {
        [Test]
        public void Constructor()
        {
            var verticesPositions = new Dictionary<string, Point>();
            var graph = new BidirectionalGraph<string, Edge<string>>();
            var algorithm = new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            var parameters = new KKLayoutParameters();
            algorithm = new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, parameters);
            AssertAlgorithmProperties(algorithm, graph, parameters: parameters);

            algorithm = new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, parameters);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, parameters: parameters);
        }

        [Test]
        public void Constructor_Throws()
        {
            var verticesPositions = new Dictionary<string, Point>();
            var parameters = new KKLayoutParameters();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, parameters));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> KKLayoutTestCases
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
                graph.AddVertexRange(new[] { "0", "1" });
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Two vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                yield return new TestCaseData(graph, 0)
                {
                    TestName = "Two linked vertices graph"
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
                yield return new TestCaseData(completeGraph, 110)
                {
                    TestName = "Complete graph"
                };

                IBidirectionalGraph<string, Edge<string>> tree = GraphFactory.CreateTree(
                    20,
                    2,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(tree, 0)
                {
                    TestName = "Tree graph 20 vertices/2 branches"
                };
                
                tree = GraphFactory.CreateTree(
                    25,
                    5,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(tree, 0)
                {
                    TestName = "Tree graph 25 vertices/5 branches"
                };

                IBidirectionalGraph<string, Edge<string>> dag = GraphFactory.CreateDAG(
                    25,
                    25,
                    10,
                    10,
                    true,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(dag, 10)
                {
                    TestName = "DAG graph 25 vertices/25 edges (Parallel edge)"
                };

                IBidirectionalGraph<string, Edge<string>> generalGraph = GraphFactory.CreateGeneralGraph(
                    30,
                    15,
                    10,
                    true,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(generalGraph, 10)
                {
                    TestName = "Generic graph 30 vertices/15 edges (Parallel edge)"
                };

                IBidirectionalGraph<string, Edge<string>> isolatedVerticesGraph = GraphFactory.CreateIsolatedVerticesGraph<string, Edge<string>>(
                    15,
                    i => i.ToString());
                yield return new TestCaseData(isolatedVerticesGraph, 0)
                {
                    TestName = "Isolated vertices graph (15 vertices)"
                };
            }
        }

        [TestCaseSource(nameof(KKLayoutTestCases))]
        public void KKLayoutAlgorithm(
            [NotNull] IBidirectionalGraph<string, Edge<string>> graph,
            int maxCrossCount)
        {
            IDictionary<string, Size> verticesSizes = GetVerticesSizes(graph.Vertices);

            var parameters = new KKLayoutParameters
            {
                Width = 1000,
                Height = 1000
            };

            foreach (bool exchange in new[] { true, false })
            {
                parameters.ExchangeVertices = exchange;

                int iteration = 0;
                var algorithm = new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(
                    graph,
                    parameters)
                {
                    Rand = new Random(12345)
                };
                algorithm.IterationEnded += (sender, args) =>
                {
                    Assert.LessOrEqual(args.Iteration, parameters.MaxIterations);
                    Assert.AreEqual(iteration++, args.Iteration);
                };

                LayoutResults results = ExecuteLayoutAlgorithm(algorithm, verticesSizes, true);
                results.CheckResult(maxCrossCount);
            }
        }
    }
}