using System;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.Layout.Contextual;
using GraphShape.Factory;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.Algorithms.AlgorithmTestHelpers;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests related to <see cref="DoubleTreeLayoutAlgorithm{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class DoubleTreeLayoutTests : LayoutAlgorithmTestBase
    {
        [Test]
        public void Constructor()
        {
            var verticesPositions = new Dictionary<string, Point>();
            var verticesSizes = new Dictionary<string, Size>();
            const string vertex = "0";
            var graph = new BidirectionalGraph<string, Edge<string>>();
            graph.AddVertex(vertex);
            var algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, vertex);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, vertex);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, vertex);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, vertex);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, verticesSizes, vertex);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, vertex);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            var parameters = new DoubleTreeLayoutParameters();
            algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, vertex, parameters);
            AssertAlgorithmProperties(algorithm, graph, parameters: parameters);

            algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, vertex, parameters);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, parameters: parameters);

            algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, verticesSizes, vertex, parameters);
            AssertAlgorithmProperties(algorithm, graph, parameters: parameters);
        }

        [Test]
        public void Constructor_Throws()
        {
            var verticesPositions = new Dictionary<string, Point>();
            var verticesSizes = new Dictionary<string, Size>();
            var emptyGraph = new BidirectionalGraph<string, Edge<string>>();
            const string vertex = "0";
            const string notInGraphVertex = "10";
            var graph = new BidirectionalGraph<string, Edge<string>>();
            graph.AddVertex(vertex);
            var parameters = new DoubleTreeLayoutParameters();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(emptyGraph, verticesSizes, vertex));
            Assert.Throws<ArgumentException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(emptyGraph, null, verticesSizes, vertex));
            Assert.Throws<ArgumentException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, notInGraphVertex));
            Assert.Throws<ArgumentException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, verticesSizes, notInGraphVertex));

            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, null));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesSizes, vertex));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, vertex));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesSizes, null));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, vertex));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesSizes, vertex, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, vertex, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesSizes, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, vertex, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, null, parameters));


            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, verticesSizes, null));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, verticesSizes, vertex));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, null, vertex));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, verticesSizes, null));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, null, vertex));

            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, verticesSizes, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, verticesSizes, vertex, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, null, vertex, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, verticesSizes, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, null, vertex, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, null, null, parameters));


            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, verticesSizes, null));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, vertex));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null, vertex));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, null));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null, vertex));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null, null));

            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, vertex, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null, vertex, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null, vertex, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null, null, parameters));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> DoubleTreeLayoutTestCases
        {
            [UsedImplicitly]
            get
            {
                var graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVertex("0");
                yield return new TestCaseData(graph)
                {
                    TestName = "Single vertex graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "0"));
                yield return new TestCaseData(graph)
                {
                    TestName = "Single vertex self loop graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVertexRange(new[] { "0", "1" });
                yield return new TestCaseData(graph)
                {
                    TestName = "Two vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                yield return new TestCaseData(graph)
                {
                    TestName = "Two linked vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVertex("2");
                yield return new TestCaseData(graph)
                {
                    TestName = "Three vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                yield return new TestCaseData(graph)
                {
                    TestName = "Four vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "1"));
                yield return new TestCaseData(graph)
                {
                    TestName = "Multiple vertices self loop graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "2"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "4"));
                graph.AddVerticesAndEdge(new Edge<string>("4", "5"));
                yield return new TestCaseData(graph)
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
                yield return new TestCaseData(graph)
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
                yield return new TestCaseData(graph)
                {
                    TestName = "Cycle graph"
                };

                IBidirectionalGraph<string, Edge<string>> completeGraph = GraphFactory.CreateCompleteGraph(
                    7,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t));
                yield return new TestCaseData(completeGraph)
                {
                    TestName = "Complete graph"
                };

                IBidirectionalGraph<string, Edge<string>> tree = GraphFactory.CreateTree(
                    20,
                    2,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(tree)
                {
                    TestName = "Tree graph 20 vertices/2 branches"
                };
                
                tree = GraphFactory.CreateTree(
                    25,
                    5,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(tree)
                {
                    TestName = "Tree graph 25 vertices/5 branches"
                };

                IBidirectionalGraph<string, Edge<string>> isolatedVerticesGraph = GraphFactory.CreateIsolatedVerticesGraph<string, Edge<string>>(
                    15,
                    i => i.ToString());
                yield return new TestCaseData(isolatedVerticesGraph)
                {
                    TestName = "Isolated vertices graph (15 vertices)"
                };
            }
        }

        [TestCaseSource(nameof(DoubleTreeLayoutTestCases))]
        public void DoubleTreeLayoutAlgorithm([NotNull] IBidirectionalGraph<string, Edge<string>> graph)
        {
            IDictionary<string, Size> verticesSizes = GetVerticesSizes(graph.Vertices);

            var parameters = new DoubleTreeLayoutParameters();

            foreach (LayoutDirection direction in Enum.GetValues(typeof(LayoutDirection)))
            {
                parameters.Direction = direction;

                var algorithm = new DoubleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(
                    graph,
                    verticesSizes,
                    graph.Vertices.First(),
                    parameters);

                Assert.DoesNotThrow(algorithm.Compute);
            }
        }
    }
}