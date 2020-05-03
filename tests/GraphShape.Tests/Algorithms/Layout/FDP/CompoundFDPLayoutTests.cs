using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.Layout.Compound;
using GraphShape.Algorithms.Layout.Compound.FDP;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.Algorithms.AlgorithmTestHelpers;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests related to <see cref="CompoundFDPLayoutAlgorithm{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class CompoundFDPLayoutTests : LayoutAlgorithmTestBase
    {
        #region Test helpers

        private static void CheckCompoundFDPLayout<TVertex, TEdge, TGraph>(
            [NotNull] IBidirectionalGraph<TVertex, TEdge> graph,
            [NotNull] CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph> algorithm)
            where TVertex : class
            where TEdge : IEdge<TVertex>
            where TGraph : IBidirectionalGraph<TVertex, TEdge>
        {
            CollectionAssert.AreEquivalent(graph.Vertices, algorithm.Levels.SelectMany(level => level));

            if (graph is ICompoundGraph<TVertex, TEdge> compoundGraph)
            {
                CollectionAssert.AreEquivalent(compoundGraph.CompoundVertices, algorithm.InnerCanvasSizes.Keys);
            }
            else
            {
                CollectionAssert.IsEmpty(algorithm.InnerCanvasSizes.Keys);
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new BidirectionalGraph<string, Edge<string>>();
            var verticesPositions = new Dictionary<string, Point>();
            var verticesSizes = new Dictionary<string, Size>();
            var verticesThicknesses = new Dictionary<string, Thickness>();
            var verticesTypes = new Dictionary<string, CompoundVertexInnerLayoutType>();

            var algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, verticesThicknesses, verticesTypes);
            AssertFDPAlgorithmProperties(algorithm, graph);

            algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, verticesThicknesses, verticesTypes);
            algorithm.IterationEnded += (sender, args) => { };
            AssertFDPAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, verticesThicknesses, verticesTypes);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertFDPAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, verticesThicknesses, verticesTypes);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertFDPAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, verticesThicknesses, verticesTypes);
            AssertFDPAlgorithmProperties(algorithm, graph, verticesPositions);

            var parameters = new CompoundFDPLayoutParameters();
            algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, verticesThicknesses, verticesTypes, parameters);
            AssertFDPAlgorithmProperties(algorithm, graph, p: parameters);

            algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, verticesThicknesses, verticesTypes, parameters);
            AssertFDPAlgorithmProperties(algorithm, graph, verticesPositions, p: parameters);

            #region Local function

            void AssertFDPAlgorithmProperties<TVertex, TEdge, TGraph>(
                CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph> algo,
                TGraph g,
                IDictionary<TVertex, Point> pos = null,
                bool expectedReportIterationEnd = false,
                bool expectedReportProgress = false,
                CompoundFDPLayoutParameters p = null)
                where TVertex : class
                where TEdge : IEdge<TVertex>
                where TGraph : IBidirectionalGraph<TVertex, TEdge>
            {
                AssertAlgorithmProperties(algo, g, pos, expectedReportIterationEnd, expectedReportProgress, p);
                CollectionAssert.IsEmpty(algo.Levels);
                CollectionAssert.IsEmpty(algo.InnerCanvasSizes);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var graph = new BidirectionalGraph<string, Edge<string>>();
            var verticesPositions = new Dictionary<string, Point>();
            var verticesSizes = new Dictionary<string, Size>();
            var verticesThicknesses = new Dictionary<string, Thickness>();
            var verticesTypes = new Dictionary<string, CompoundVertexInnerLayoutType>();
            var parameters = new CompoundFDPLayoutParameters();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesSizes, verticesThicknesses, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, verticesThicknesses, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, null, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, verticesThicknesses, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, verticesThicknesses, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesSizes, null, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesSizes, verticesThicknesses, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, null, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, verticesThicknesses, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, null, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, verticesThicknesses, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, null, null, parameters));


            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, verticesThicknesses, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null, verticesThicknesses, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, null, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, verticesThicknesses, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null, verticesThicknesses, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, null, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, verticesThicknesses, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null, null, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null, verticesThicknesses, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null, null, verticesTypes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null, verticesThicknesses, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null, null, null, parameters));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> CompoundFDPLayoutTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(
                    new BidirectionalGraph<string, Edge<string>>(),
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Empty graph"
                };

                var graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVertex("0");
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Single vertex graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "0"));
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Single vertex self loop graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVertexRange(new[] { "0", "1" });
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Two vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Two linked vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVertex("2");
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Three vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Four vertices graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "1"));
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Multiple vertices self loop graph"
                };

                graph = new BidirectionalGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVerticesAndEdge(new Edge<string>("1", "2"));
                graph.AddVerticesAndEdge(new Edge<string>("2", "3"));
                graph.AddVerticesAndEdge(new Edge<string>("3", "4"));
                graph.AddVerticesAndEdge(new Edge<string>("4", "5"));
                yield return new TestCaseData(
                    graph,
                    1,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
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
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
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
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Cycle graph"
                };

                IBidirectionalGraph<string, Edge<string>> completeGraph = GraphFactory.CreateCompleteGraph(
                    7,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t));
                yield return new TestCaseData(
                    completeGraph,
                    110,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Complete graph"
                };

                IBidirectionalGraph<string, Edge<string>> tree = GraphFactory.CreateTree(
                    20,
                    2,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(
                    tree,
                    1,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Tree graph 20 vertices/2 branches"
                };

                tree = GraphFactory.CreateTree(
                    25,
                    5,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(
                    tree,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Tree graph 25 vertices/5 branches"
                };

                string[] vertices = tree.Vertices.ToArray();
                yield return new TestCaseData(
                    tree,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>
                    {
                        [vertices[2]] = CompoundVertexInnerLayoutType.Fixed,
                        [vertices[12]] = CompoundVertexInnerLayoutType.Fixed,
                        [vertices[19]] = CompoundVertexInnerLayoutType.Fixed
                    })
                {
                    TestName = "Tree graph 25 vertices/5 branches (with fixed)"
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
                yield return new TestCaseData(
                    dag,
                    5,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
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
                yield return new TestCaseData(
                    generalGraph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Generic graph 30 vertices/15 edges (Parallel edge)"
                };

                vertices = tree.Vertices.ToArray();
                yield return new TestCaseData(
                    generalGraph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>
                    {
                        [vertices[2]] = CompoundVertexInnerLayoutType.Fixed,
                        [vertices[21]] = CompoundVertexInnerLayoutType.Fixed
                    })
                {
                    TestName = "Generic graph 30 vertices/15 edges (Parallel edge) (with fixed)"
                };

                IBidirectionalGraph<string, Edge<string>> isolatedVerticesGraph = GraphFactory.CreateIsolatedVerticesGraph<string, Edge<string>>(
                    15,
                    i => i.ToString());
                yield return new TestCaseData(
                    isolatedVerticesGraph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Isolated vertices graph (15 vertices)"
                };
            }
        }

        [TestCaseSource(nameof(CompoundFDPLayoutTestCases))]
        public void CompoundFDPLayoutAlgorithm(
            [NotNull] IBidirectionalGraph<string, Edge<string>> graph,
            int maxCrossCount,
            [NotNull] IDictionary<string, CompoundVertexInnerLayoutType> verticesTypes)
        {
            IDictionary<string, Size> verticesSizes = GetVerticesSizes(graph.Vertices);
            var verticesThicknesses = new Dictionary<string, Thickness>();

            var parameters = new CompoundFDPLayoutParameters();

            var algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(
                graph,
                verticesSizes,
                verticesThicknesses,
                verticesTypes,
                parameters)
            {
                Rand = new Random(123456)
            };
            algorithm.IterationEnded += (sender, args) => { };

            LayoutResults results = ExecuteLayoutAlgorithm(algorithm, verticesSizes);
            results.CheckResult(maxCrossCount);
            CheckCompoundFDPLayout(graph, algorithm);
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> CompoundFDPLayoutCompoundGraphTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(
                    new CompoundGraph<string, Edge<string>>(),
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Empty graph (compound)"
                };

                var graph = new CompoundGraph<string, Edge<string>>();
                graph.AddVertex("0");
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Single vertex graph (compound)"
                };

                graph = new CompoundGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "0"));
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Single vertex self loop graph (compound)"
                };

                graph = new CompoundGraph<string, Edge<string>>();
                graph.AddVertexRange(new[] { "0", "1" });
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Two vertices graph (compound)"
                };

                graph = new CompoundGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Two linked vertices graph (compound)"
                };

                graph = new CompoundGraph<string, Edge<string>>();
                graph.AddVerticesAndEdge(new Edge<string>("0", "1"));
                graph.AddVertex("2");
                yield return new TestCaseData(
                    graph,
                    0,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "Three vertices graph (compound)"
                };

                ICompoundGraph<string, Edge<string>> compoundGraph = GraphFactory.CreateCompoundGraph(
                    20,
                    20,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(
                    compoundGraph,
                    5,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "General graph (20 vertices/20 edges) (compound)"
                };

                string[] vertices = compoundGraph.Vertices.ToArray();
                yield return new TestCaseData(
                    compoundGraph,
                    12,
                    new Dictionary<string, CompoundVertexInnerLayoutType>
                    {
                        [vertices[3]] = CompoundVertexInnerLayoutType.Fixed,
                        [vertices[6]] = CompoundVertexInnerLayoutType.Fixed,
                        [vertices[15]] = CompoundVertexInnerLayoutType.Fixed
                    })
                {
                    TestName = "General graph (20 vertices/20 edges) (compound with fixed)"
                };

                compoundGraph = GraphFactory.CreateCompoundGraph(
                    45,
                    30,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(
                    compoundGraph,
                    12,
                    new Dictionary<string, CompoundVertexInnerLayoutType>())
                {
                    TestName = "General graph (45 vertices/30 edges) (compound)"
                };

                vertices = compoundGraph.Vertices.ToArray();
                yield return new TestCaseData(
                    compoundGraph,
                    15,
                    new Dictionary<string, CompoundVertexInnerLayoutType>
                    {
                        [vertices[8]] = CompoundVertexInnerLayoutType.Fixed,
                        [vertices[12]] = CompoundVertexInnerLayoutType.Fixed,
                        [vertices[17]] = CompoundVertexInnerLayoutType.Fixed,
                        [vertices[25]] = CompoundVertexInnerLayoutType.Fixed,
                        [vertices[37]] = CompoundVertexInnerLayoutType.Fixed
                    })
                {
                    TestName = "General graph (45 vertices/30 edges) (compound with fixed)"
                };
            }
        }

        [TestCaseSource(nameof(CompoundFDPLayoutCompoundGraphTestCases))]
        public void CompoundFDPLayoutAlgorithm_CompoundGraph(
            [NotNull] ICompoundGraph<string, Edge<string>> graph,
            int maxCrossCount,
            [NotNull] IDictionary<string, CompoundVertexInnerLayoutType> verticesTypes)
        {
            IDictionary<string, Size> verticesSizes = GetVerticesSizes(graph.Vertices);
            var verticesThicknesses = new Dictionary<string, Thickness>();

            var parameters = new CompoundFDPLayoutParameters();

            var algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, ICompoundGraph<string, Edge<string>>>(
                graph,
                verticesSizes,
                verticesThicknesses,
                verticesTypes,
                parameters)
            {
                Rand = new Random(123456)
            };
            algorithm.IterationEnded += (sender, args) => { };

            LayoutResults results = ExecuteLayoutAlgorithm(algorithm, verticesSizes);
            results.CheckResult(maxCrossCount, int.MaxValue);
            CheckCompoundFDPLayout(graph, algorithm);
        }

        [Test]
        public void VertexLevels()
        {
            var compoundGraph = GraphFactory.CreateCompoundGraph(
                10,
                5,
                i => i.ToString(),
                (s, t) => new Edge<string>(s, t),
                new Random(123));

            IDictionary<string, Size> verticesSizes = GetVerticesSizes(compoundGraph.Vertices);
            var verticesThicknesses = new Dictionary<string, Thickness>();
            var verticesTypes = new Dictionary<string, CompoundVertexInnerLayoutType>();

            var parameters = new CompoundFDPLayoutParameters();
            var algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, ICompoundGraph<string, Edge<string>>>(
                compoundGraph,
                verticesSizes,
                verticesThicknesses, 
                verticesTypes,
                parameters)
            {
                Rand = new Random(123456)
            };

            Assert.DoesNotThrow(algorithm.Compute);

            foreach (string vertex in compoundGraph.Vertices)
            {
                int level = algorithm.LevelOfVertex(vertex);
                Assert.GreaterOrEqual(level, 0);
                Assert.Less(level, algorithm.Levels.Count);
            }
        }

        [Test]
        public void VertexLevels_Throws()
        {
            var compoundGraph = GraphFactory.CreateCompoundGraph(
                5,
                2,
                i => i.ToString(),
                (s, t) => new Edge<string>(s, t),
                new Random(123));

            IDictionary<string, Size> verticesSizes = GetVerticesSizes(compoundGraph.Vertices);
            var verticesThicknesses = new Dictionary<string, Thickness>();
            var verticesTypes = new Dictionary<string, CompoundVertexInnerLayoutType>();

            var parameters = new CompoundFDPLayoutParameters();
            var algorithm = new CompoundFDPLayoutAlgorithm<string, Edge<string>, ICompoundGraph<string, Edge<string>>>(
                compoundGraph,
                verticesSizes,
                verticesThicknesses,
                verticesTypes,
                parameters);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.LevelOfVertex(null));

            Assert.DoesNotThrow(algorithm.Compute);

            string vertexNotInGraph = "NotInGraph";
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => algorithm.LevelOfVertex(vertexNotInGraph));
        }
    }
}