using System;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.Layout;
using GraphShape.Factory;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.Algorithms.AlgorithmTestHelpers;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests related to <see cref="RandomLayoutAlgorithm{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class RandomLayoutTests : LayoutAlgorithmTestBase
    {
        [Test]
        public void Constructor()
        {
            var verticesPositions = new Dictionary<string, Point>();
            var verticesSizes = new Dictionary<string, Size>();
            var verticesTypes = new Dictionary<string, RandomVertexType>();
            var graph = new AdjacencyGraph<string, Edge<string>>();
            var algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesSizes, null);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesSizes, null);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesSizes, null);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesSizes, null);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, null, verticesSizes, null);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, null);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            var parameters = new RandomLayoutParameters();
            algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesSizes, null, parameters);
            AssertAlgorithmProperties(algorithm, graph, parameters: parameters);

            algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, null, parameters);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, parameters: parameters);

            algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, verticesTypes, parameters);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, parameters: parameters);
        }

        [Test]
        public void Constructor_Throws()
        {
            var verticesPositions = new Dictionary<string, Point>();
            var verticesSizes = new Dictionary<string, Size>();
            var graph = new AdjacencyGraph<string, Edge<string>>();
            var parameters = new RandomLayoutParameters();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, verticesSizes, null));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, verticesSizes, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, null, null, parameters));


            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, null, (IDictionary<string, Size>)null, null));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, null, verticesSizes, null));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, null, (IDictionary<string, Size>)null, null));

            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesPositions, null, null));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, null));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, verticesPositions, null, null));

            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, null, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, null, verticesSizes, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, null, null, null, parameters));

            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(graph, verticesPositions, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(null, verticesPositions, null, null, parameters));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> RandomLayoutTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new AdjacencyGraph<string, Edge<string>>())
                {
                    TestName = "Empty graph"
                };

                var graph = new AdjacencyGraph<string, Edge<string>>();
                graph.AddVertex("0");
                yield return new TestCaseData(graph)
                {
                    TestName = "Single vertex graph"
                };

                graph = new AdjacencyGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "0"));
                yield return new TestCaseData(graph)
                {
                    TestName = "Single vertex self loop graph"
                };

                graph = new AdjacencyGraph<string, Edge<string>>();
                graph.AddVertexRange(new[] { "0", "1" });
                yield return new TestCaseData(graph)
                {
                    TestName = "Two vertices graph"
                };

                graph = new AdjacencyGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                yield return new TestCaseData(graph)
                {
                    TestName = "Two linked vertices graph"
                };

                graph = new AdjacencyGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVertex("2");
                yield return new TestCaseData(graph)
                {
                    TestName = "Three vertices graph"
                };

                graph = new AdjacencyGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                yield return new TestCaseData(graph)
                {
                    TestName = "Four vertices graph"
                };

                graph = new AdjacencyGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "1"));
                yield return new TestCaseData(graph)
                {
                    TestName = "Multiple vertices self loop graph"
                };

                graph = new AdjacencyGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "2"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "4"));
                graph.AddVerticesAndEdge(new Edge<string>("4", "5"));
                yield return new TestCaseData(graph)
                {
                    TestName = "Line graph"
                };

                graph = new AdjacencyGraph<string, Edge<string>>();
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

                graph = new AdjacencyGraph<string, Edge<string>>();
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

                IVertexAndEdgeListGraph<string, Edge<string>> completeGraph = GraphFactory.CreateCompleteGraph(
                    7,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t));
                yield return new TestCaseData(completeGraph)
                {
                    TestName = "Complete graph"
                };

                IVertexAndEdgeListGraph<string, Edge<string>> tree = GraphFactory.CreateTree(
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

                IVertexAndEdgeListGraph<string, Edge<string>> dag = GraphFactory.CreateDAG(
                    25,
                    25,
                    10,
                    10,
                    true,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(dag)
                {
                    TestName = "DAG graph 25 vertices/25 edges (Parallel edge)"
                };
                
                IVertexAndEdgeListGraph<string, Edge<string>> generalGraph = GraphFactory.CreateGeneralGraph(
                    30,
                    15,
                    10,
                    true,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(generalGraph)
                {
                    TestName = "Generic graph 30 vertices/15 edges (Parallel edge)"
                };

                IVertexAndEdgeListGraph<string, Edge<string>> isolatedVerticesGraph = GraphFactory.CreateIsolatedVerticesGraph<string, Edge<string>>(
                    15,
                    i => i.ToString());
                yield return new TestCaseData(isolatedVerticesGraph)
                {
                    TestName = "Isolated vertices graph (15 vertices)"
                };
            }
        }

        [TestCaseSource(nameof(RandomLayoutTestCases))]
        public void RandomLayoutAlgorithm([NotNull] IVertexAndEdgeListGraph<string, Edge<string>> graph)
        {
            IDictionary<string, Size> verticesSizes = GetVerticesSizes(graph.Vertices);
            var algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(
                graph,
                verticesSizes,
                null,
                new RandomLayoutParameters())
            {
                Rand = new Random(12345)
            };

            LayoutResults results = ExecuteLayoutAlgorithm(algorithm, verticesSizes, true);
            results.CheckPositions();
        }

        [Test]
        public void RandomLayoutAlgorithm_WithFixedVertices()
        {
            IVertexAndEdgeListGraph<string, Edge<string>> graph = GraphFactory.CreateGeneralGraph(
                30,
                15,
                10,
                false,
                i => i.ToString(),
                (s, t) => new Edge<string>(s, t),
                new Random(123));
            string[] vertices = graph.Vertices.ToArray();

            string fixedVertexNoPos = vertices[2];
            string fixedVertexWithPos = vertices[16];
            var fixedVertexPosition = new Point(50.0, 50.0);
            IDictionary<string, Point> verticesPositions = new Dictionary<string, Point>
            {
                [fixedVertexWithPos] = fixedVertexPosition
            };
            IDictionary<string, Size> verticesSizes = GetVerticesSizes(vertices);
            var verticesTypes = new Dictionary<string, RandomVertexType>
            {
                [fixedVertexNoPos] = RandomVertexType.Fixed,
                [vertices[8]] = RandomVertexType.Free,
                [fixedVertexWithPos] = RandomVertexType.Fixed
            };

            var algorithm = new RandomLayoutAlgorithm<string, Edge<string>, IVertexAndEdgeListGraph<string, Edge<string>>>(
                graph,
                verticesPositions,
                verticesSizes,
                verticesTypes,
                new RandomLayoutParameters())
            {
                Rand = new Random(12345)
            };

            // Run without overlap removal otherwise fixed vertex may change their positions after algorithm run
            LayoutResults results = ExecuteLayoutAlgorithm(algorithm, verticesSizes);
            results.CheckPositions();

            Assert.AreEqual(fixedVertexPosition, algorithm.VerticesPositions[fixedVertexWithPos]);
        }
    }
}