using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.Layout.Simple.Tree;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.Algorithms.AlgorithmTestHelpers;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests related to <see cref="SimpleTreeLayoutAlgorithm{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class TreeLayoutTests : LayoutAlgorithmTestBase
    {
        #region Test helpers

        private static void CheckTreeLayout<TVertex, TEdge>(
            [NotNull] SimpleTreeLayoutAlgorithm<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> algorithm,
            [NotNull] IDictionary<TVertex, Size> verticesSizes)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            var slices = new Dictionary<double, HashSet<double>>();

            TVertex[] vertices = algorithm.VisitedGraph.Vertices.ToArray();
            for (int i = 0; i < vertices.Length - 1; ++i)
            {
                TVertex vertexI = vertices[i];
                Point posI = algorithm.VerticesPositions[vertexI];
                for (int j = i + 1; j < vertices.Length; ++j)
                {
                    TVertex vertexJ = vertices[j];
                    Point posJ = algorithm.VerticesPositions[vertexJ];
                    if (algorithm.Parameters.Direction.IsHorizontal())
                    {
                        CheckSpacingAndAddSlice(posI.Y, verticesSizes[vertexI].Height / 2);
                        CheckSpacingAndAddSlice(posJ.Y, verticesSizes[vertexJ].Height / 2);

                        CheckSpacingAndAddLayerToSlice(posI.Y, posI.X, verticesSizes[vertexI].Width / 2);
                        CheckSpacingAndAddLayerToSlice(posJ.Y, posJ.X, verticesSizes[vertexJ].Width / 2);
                    }
                    else if (algorithm.Parameters.Direction.IsVertical())
                    {
                        CheckSpacingAndAddSlice(posI.X, verticesSizes[vertexI].Width / 2);
                        CheckSpacingAndAddSlice(posJ.X, verticesSizes[vertexJ].Width / 2);

                        CheckSpacingAndAddLayerToSlice(posI.X, posI.Y, verticesSizes[vertexI].Height / 2);
                        CheckSpacingAndAddLayerToSlice(posJ.X, posJ.Y, verticesSizes[vertexJ].Height / 2);
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported layout direction.");
                    }
                }
            }

            #region Local functions

            // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
            void CheckSpacingAndAddSlice(double slicePos, double size)
            {
                if (!slices.ContainsKey(slicePos))
                {
                    Assert.IsTrue(slices.All(pair => Math.Abs(slicePos - pair.Key) + size >= algorithm.Parameters.VertexGap));
                    slices.Add(slicePos, new HashSet<double>());
                }
            }

            void CheckSpacingAndAddLayerToSlice(double slicePos, double layerPos, double size)
            {
                Assert.IsTrue(slices.TryGetValue(slicePos, out HashSet<double> layerPositions));
                Assert.IsTrue(layerPositions.All(lPos => Math.Abs(layerPos - lPos) + size >= algorithm.Parameters.LayerGap));
            }
            // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local

            #endregion
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var verticesPositions = new Dictionary<string, Point>();
            var verticesSizes = new Dictionary<string, Size>();
            var graph = new BidirectionalGraph<string, Edge<string>>();
            var algorithm = new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, verticesSizes);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            var parameters = new SimpleTreeLayoutParameters();
            algorithm = new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesSizes, parameters);
            AssertAlgorithmProperties(algorithm, graph, parameters: parameters);

            algorithm = new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, verticesSizes, parameters);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, parameters: parameters);
        }

        [Test]
        public void Constructor_Throws()
        {
            var verticesPositions = new Dictionary<string, Point>();
            var verticesSizes = new Dictionary<string, Size>();
            var graph = new BidirectionalGraph<string, Edge<string>>();
            var parameters = new SimpleTreeLayoutParameters();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesSizes));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null));

            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesSizes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, parameters));


            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, (IDictionary<string, Size>)null));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, verticesSizes));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, (IDictionary<string, Size>)null));

            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null));

            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, null, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, verticesSizes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, null, null, parameters));

            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph, verticesPositions, null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, verticesSizes, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(null, verticesPositions, null, parameters));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> SimpleTreeLayoutTestCases
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

                IBidirectionalGraph<string, Edge<string>> dag = GraphFactory.CreateDAG(
                    25,
                    25,
                    10,
                    10,
                    true,
                    i => i.ToString(),
                    (s, t) => new Edge<string>(s, t),
                    new Random(123));
                yield return new TestCaseData(dag, 60)
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
                yield return new TestCaseData(generalGraph, 25)
                {
                    TestName = "Generic graph 30 vertices/15 edges (Parallel edge)"
                };
            }
        }

        [TestCaseSource(nameof(SimpleTreeLayoutTestCases))]
        public void SimpleTreeLayoutAlgorithm(
            [NotNull] IBidirectionalGraph<string, Edge<string>> graph,
            int maxCrossCount)
        {
            IDictionary<string, Size> verticesSizes = GetVerticesSizes(graph.Vertices);

            var parameters = new SimpleTreeLayoutParameters
            {
                LayerGap = 15,
                VertexGap = 20
            };

            foreach (LayoutDirection direction in Enum.GetValues(typeof(LayoutDirection)))
            {
                parameters.Direction = direction;

                foreach (SpanningTreeGeneration treeGen in Enum.GetValues(typeof(SpanningTreeGeneration)))
                {
                    parameters.SpanningTreeGeneration = treeGen;

                    var algorithm = new SimpleTreeLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(
                        graph,
                        verticesSizes,
                        parameters);

                    LayoutResults results = ExecuteLayoutAlgorithm(algorithm, verticesSizes);
                    results.CheckResult(maxCrossCount);
                    CheckTreeLayout(algorithm, verticesSizes);
                }
            }
        }
    }
}