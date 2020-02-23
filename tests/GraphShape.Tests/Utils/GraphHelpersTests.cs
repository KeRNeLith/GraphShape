using System;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Utils;
using JetBrains.Annotations;
using QuikGraph;
using NUnit.Framework;
using static GraphShape.Tests.GraphTestHelpers;

namespace GraphShape.Tests
{
    /// <summary>
    /// Tests for <see cref="GraphHelpers"/>.
    /// </summary>
    [TestFixture]
    internal class GraphHelpersTests
    {
        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> GetNeighborsTestCases
        {
            [UsedImplicitly]
            get
            {
                var noEdgeGraph1 = new BidirectionalGraph<int, Edge<int>>();
                noEdgeGraph1.AddVertex(1);
                yield return new TestCaseData(noEdgeGraph1, 1)
                {
                    ExpectedResult = Enumerable.Empty<int>()
                };

                var noEdgeGraph2 = new BidirectionalGraph<int, Edge<int>>();
                noEdgeGraph2.AddVertexRange(new[] { 1, 2 });
                yield return new TestCaseData(noEdgeGraph2, 1)
                {
                    ExpectedResult = Enumerable.Empty<int>()
                };

                yield return new TestCaseData(noEdgeGraph2, 2)
                {
                    ExpectedResult = Enumerable.Empty<int>()
                };

                var graph = new BidirectionalGraph<int, Edge<int>>();
                graph.AddVerticesAndEdgeRange(new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(2, 3),
                    new Edge<int>(3, 1),
                    new Edge<int>(4, 1),
                    new Edge<int>(4, 5),
                    new Edge<int>(4, 5),
                    new Edge<int>(5, 5),
                    new Edge<int>(6, 6)
                });
                graph.AddVertex(7);
                yield return new TestCaseData(graph, 1)
                {
                    ExpectedResult = new[] { 2, 3, 4 }
                };

                yield return new TestCaseData(graph, 3)
                {
                    ExpectedResult = new[] { 1, 2 }
                };

                yield return new TestCaseData(graph, 5)
                {
                    ExpectedResult = new[] { 4, 5 }
                };

                yield return new TestCaseData(graph, 6)
                {
                    ExpectedResult = new[] { 6 }
                };

                yield return new TestCaseData(graph, 7)
                {
                    ExpectedResult = Enumerable.Empty<int>()
                };
            }
        }

        [TestCaseSource(nameof(GetNeighborsTestCases))]
        public IEnumerable<int> GetNeighbors(
            [NotNull] IBidirectionalGraph<int, Edge<int>> graph,
            int vertex)
        {
            return graph.GetNeighbors(vertex).OrderBy(x => x);
        }

        [Test]
        public void GetNeighbors_Throws()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.GetNeighbors(1));
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> GetOutNeighborsTestCases
        {
            [UsedImplicitly]
            get
            {
                var noEdgeGraph1 = new AdjacencyGraph<int, Edge<int>>();
                noEdgeGraph1.AddVertex(1);
                yield return new TestCaseData(noEdgeGraph1, 1)
                {
                    ExpectedResult = Enumerable.Empty<int>()
                };

                var noEdgeGraph2 = new AdjacencyGraph<int, Edge<int>>();
                noEdgeGraph2.AddVertexRange(new[] { 1, 2 });
                yield return new TestCaseData(noEdgeGraph2, 1)
                {
                    ExpectedResult = Enumerable.Empty<int>()
                };

                yield return new TestCaseData(noEdgeGraph2, 2)
                {
                    ExpectedResult = Enumerable.Empty<int>()
                };

                var graph = new AdjacencyGraph<int, Edge<int>>();
                graph.AddVerticesAndEdgeRange(new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(2, 3),
                    new Edge<int>(3, 1),
                    new Edge<int>(4, 1),
                    new Edge<int>(4, 5),
                    new Edge<int>(4, 5),
                    new Edge<int>(5, 5),
                    new Edge<int>(6, 6)
                });
                graph.AddVertex(7);
                yield return new TestCaseData(graph, 1)
                {
                    ExpectedResult = new[] { 2, 3 }
                };

                yield return new TestCaseData(graph, 3)
                {
                    ExpectedResult = new[] { 1 }
                };

                yield return new TestCaseData(graph, 4)
                {
                    ExpectedResult = new[] { 1, 5 }
                };

                yield return new TestCaseData(graph, 5)
                {
                    ExpectedResult = new[] { 5 }
                };

                yield return new TestCaseData(graph, 6)
                {
                    ExpectedResult = new[] { 6 }
                };

                yield return new TestCaseData(graph, 7)
                {
                    ExpectedResult = Enumerable.Empty<int>()
                };
            }
        }

        [TestCaseSource(nameof(GetOutNeighborsTestCases))]
        public IEnumerable<int> GetOutNeighbors(
            [NotNull] IVertexAndEdgeListGraph<int, Edge<int>> graph,
            int vertex)
        {
            return graph.GetOutNeighbors(vertex).OrderBy(x => x);
        }

        [Test]
        public void GetOutNeighbors_Throws()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.GetOutNeighbors(1));
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> GetEdgesBetweenTestCases
        {
            [UsedImplicitly]
            get
            {
                var noEdgeGraph = new AdjacencyGraph<int, Edge<int>>();
                noEdgeGraph.AddVertexRange(new[] { 1, 2 });
                yield return new TestCaseData(
                    noEdgeGraph,
                    new[] { 1 },
                    new[] { 2 },
                    Enumerable.Empty<Edge<int>>());

                var edge12 = new Edge<int>(1, 2);
                var edge21 = new Edge<int>(2, 1);
                var graph1 = new AdjacencyGraph<int, Edge<int>>();
                graph1.AddVerticesAndEdgeRange(new[]
                {
                    edge12, edge21
                });
                yield return new TestCaseData(
                    graph1,
                    new[] { 1 },
                    new[] { 2 },
                    new[] { edge12 });
                yield return new TestCaseData(
                    graph1,
                    new[] { 2 },
                    new[] { 1 },
                    new[] { edge21 });
                yield return new TestCaseData(
                    graph1,
                    new[] { 1, 2 },
                    new[] { 1, 2 },
                    new[] { edge12, edge21 });

                var edge13 = new Edge<int>(1, 3);
                var edge23 = new Edge<int>(2, 3);
                var edge31 = new Edge<int>(3, 1);
                var edge41 = new Edge<int>(4, 1);
                var edge45 = new Edge<int>(4, 5);
                var edge45Bis = new Edge<int>(4, 5);
                var edge55 = new Edge<int>(5, 5);
                var edge66 = new Edge<int>(6, 6);
                var graph2 = new AdjacencyGraph<int, Edge<int>>();
                graph2.AddVerticesAndEdgeRange(new[]
                {
                    edge12, edge13, edge23, edge31, edge41,
                    edge45, edge45Bis, edge55, edge66
                });
                graph2.AddVertex(7);
                yield return new TestCaseData(
                    graph2,
                    new[] { 1, 2, 4 },
                    new[] { 1, 2, 3 },
                    new[] { edge12, edge13, edge23, edge41 });

                yield return new TestCaseData(
                    graph2,
                    new[] { 1, 4, 5 },
                    new[] { 4, 5 },
                    new[] { edge45, edge45Bis, edge55 });

                yield return new TestCaseData(
                    graph2,
                    new[] { 1, 6, 7 },
                    new[] { 4, 5 },
                    Enumerable.Empty<Edge<int>>());
            }
        }

        [TestCaseSource(nameof(GetEdgesBetweenTestCases))]
        public void GetEdgesBetween(
            [NotNull] IVertexAndEdgeListGraph<int, Edge<int>> graph,
            [NotNull] IEnumerable<int> set1,
            [NotNull] IEnumerable<int> set2,
            [NotNull] IEnumerable<Edge<int>> expectedEdges)
        {
            CollectionAssert.AreEquivalent(
                expectedEdges,
                graph.GetEdgesBetween(set1.ToArray(), set2.ToArray()));
        }

        [Test]
        public void GetEdgesBetween_Throws()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVertex(1);
            var set1 = new[] { 2 };
            var set2 = new[] { 1 };

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.GetEdgesBetween(null, set1).ToArray());
            Assert.Throws<ArgumentNullException>(() => graph.GetEdgesBetween(set1, null).ToArray());
            Assert.Throws<ArgumentNullException>(() => graph.GetEdgesBetween(null, null).ToArray());
            Assert.Throws<ArgumentNullException>(() => ((IVertexAndEdgeListGraph<int, Edge<int>>)null).GetEdgesBetween(null, set1).ToArray());
            Assert.Throws<ArgumentNullException>(() => ((IVertexAndEdgeListGraph<int, Edge<int>>)null).GetEdgesBetween(set1, null).ToArray());
            Assert.Throws<ArgumentNullException>(() => ((IVertexAndEdgeListGraph<int, Edge<int>>)null).GetEdgesBetween(null, null).ToArray());
            // ReSharper restore AssignNullToNotNullAttribute

            Assert.Throws<VertexNotFoundException>(() => graph.GetEdgesBetween(set1, set2).ToArray());
            Assert.DoesNotThrow(() => graph.GetEdgesBetween(set2, set1).ToArray()); // Not throw when set1 only has vertices in graph
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void GetDistances()
        {
            var emptyGraph = new BidirectionalGraph<string, Edge<string>>();
            CollectionAssert.IsEmpty(
                emptyGraph.GetDistances<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>());

            var graph1 = new BidirectionalGraph<string, Edge<string>>();
            graph1.AddVertexRange(new[] { "1", "2" });
            CollectionAssert.AreEqual(
                new[,]
                {
                    { 0.0, double.MaxValue },
                    { double.MaxValue, 0.0 }
                },
                graph1.GetDistances<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>());

            var graph2 = new BidirectionalGraph<string, Edge<string>>();
            graph2.AddVerticesAndEdge(new Edge<string>("1", "2"));
            CollectionAssert.AreEqual(
                new[,]
                {
                    { 0.0, 1.0 },
                    { 1.0, 0.0 }
                },
                graph2.GetDistances<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>());

            var graph3 = new BidirectionalGraph<string, Edge<string>>();
            graph3.AddVertexRange(new[] { "1", "2", "3", "4", "5", "6", "7" });
            graph3.AddEdgeRange(new[]
            {
                new Edge<string>("1", "2"),
                new Edge<string>("2", "1"),
                new Edge<string>("2", "3"),
                new Edge<string>("3", "4"),
                new Edge<string>("4", "6"),
                new Edge<string>("5", "5")
            });
            CollectionAssert.AreEqual(
                new[,]
                {
                    { 0.0, 1.0, 2.0, 3.0, double.MaxValue, 4.0, double.MaxValue },
                    { 1.0, 0.0, 1.0, 2.0, double.MaxValue, 3.0, double.MaxValue },
                    { 2.0, 1.0, 0.0, 1.0, double.MaxValue, 2.0, double.MaxValue },
                    { 3.0, 2.0, 1.0, 0.0, double.MaxValue, 1.0, double.MaxValue },
                    { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, 0.0, double.MaxValue, double.MaxValue },
                    { 4.0, 3.0, 2.0, 1.0, double.MaxValue, 0.0, double.MaxValue },
                    { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, 0.0 }
                },
                graph3.GetDistances<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>());
        }

        [Test]
        public void GetDistances_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<int, Edge<int>>)null).GetDistances<int, Edge<int>, IBidirectionalGraph<int, Edge<int>>>());
        }

        [Test]
        public void GetDiameter()
        {
            var emptyGraph = new BidirectionalGraph<string, Edge<string>>();
            double diameter = emptyGraph.GetDiameter<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>();
            Assert.AreEqual(double.NegativeInfinity, diameter);

            var graph1 = new BidirectionalGraph<string, Edge<string>>();
            graph1.AddVertexRange(new[] { "1", "2" });
            diameter = graph1.GetDiameter<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>();
            Assert.AreEqual(double.NegativeInfinity, diameter);

            var graph2 = new BidirectionalGraph<string, Edge<string>>();
            graph2.AddVerticesAndEdge(new Edge<string>("1", "2"));
            diameter = graph2.GetDiameter<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>();
            Assert.AreEqual(1.0, diameter);

            var graph3 = new BidirectionalGraph<string, Edge<string>>();
            graph3.AddVertexRange(new[] { "1", "2", "3", "4", "5", "6", "7" });
            graph3.AddEdgeRange(new[]
            {
                new Edge<string>("1", "2"),
                new Edge<string>("2", "1"),
                new Edge<string>("2", "3"),
                new Edge<string>("3", "4"),
                new Edge<string>("4", "6"),
                new Edge<string>("5", "5")
            });
            diameter = graph3.GetDiameter<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>();
            Assert.AreEqual(4.0, diameter);
        }

        [Test]
        public void GetDiameter_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<int, Edge<int>>)null).GetDiameter<int, Edge<int>, IBidirectionalGraph<int, Edge<int>>>());
        }

        [Test]
        public void GetDiameterWithDistance()
        {
            var emptyGraph = new BidirectionalGraph<string, Edge<string>>();
            double diameter = emptyGraph.GetDiameter<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>(out double[,] distances);
            Assert.AreEqual(double.NegativeInfinity, diameter);
            CollectionAssert.IsEmpty(distances);

            var graph1 = new BidirectionalGraph<string, Edge<string>>();
            graph1.AddVertexRange(new[] { "1", "2" });
            diameter = graph1.GetDiameter<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>(out distances);
            Assert.AreEqual(double.NegativeInfinity, diameter);
            CollectionAssert.AreEqual(
                new[,]
                {
                    { 0.0, double.MaxValue },
                    { double.MaxValue, 0.0 }
                },
                distances);

            var graph2 = new BidirectionalGraph<string, Edge<string>>();
            graph2.AddVerticesAndEdge(new Edge<string>("1", "2"));
            diameter = graph2.GetDiameter<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>(out distances);
            Assert.AreEqual(1.0, diameter);
            CollectionAssert.AreEqual(
                new[,]
                {
                    { 0.0, 1.0 },
                    { 1.0, 0.0 }
                },
                distances);

            var graph3 = new BidirectionalGraph<string, Edge<string>>();
            graph3.AddVertexRange(new[] { "1", "2", "3", "4", "5", "6", "7" });
            graph3.AddEdgeRange(new[]
            {
                new Edge<string>("1", "2"),
                new Edge<string>("2", "1"),
                new Edge<string>("2", "3"),
                new Edge<string>("3", "4"),
                new Edge<string>("4", "6"),
                new Edge<string>("5", "5")
            });
            diameter = graph3.GetDiameter<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>(out distances);
            Assert.AreEqual(4.0, diameter);
            CollectionAssert.AreEqual(
                new[,]
                {
                    { 0.0, 1.0, 2.0, 3.0, double.MaxValue, 4.0, double.MaxValue },
                    { 1.0, 0.0, 1.0, 2.0, double.MaxValue, 3.0, double.MaxValue },
                    { 2.0, 1.0, 0.0, 1.0, double.MaxValue, 2.0, double.MaxValue },
                    { 3.0, 2.0, 1.0, 0.0, double.MaxValue, 1.0, double.MaxValue },
                    { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, 0.0, double.MaxValue, double.MaxValue },
                    { 4.0, 3.0, 2.0, 1.0, double.MaxValue, 0.0, double.MaxValue },
                    { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, 0.0 }
                },
                distances);
        }

        [Test]
        public void GetDiameterAndDistances_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IBidirectionalGraph<int, Edge<int>>)null).GetDiameter<int, Edge<int>, IBidirectionalGraph<int, Edge<int>>>(out _));
        }

        #region Graph manipulations

        #region Test classes

        private class TestEdgeData
        {
            public int Src { [UsedImplicitly] get; set; }

            [UsedImplicitly]
            public double FakeSrc { get; }

            public int Dst { [UsedImplicitly] get; set; }

            [UsedImplicitly]
            public double FakeDst { get; private set; }
        }

        #endregion

        [Test]
        public void CreateGraphByReflection()
        {
            // Test 1
            var vertices = new int[] { };
            var edgeData = new TestEdgeData[] { };

            BidirectionalGraph<int, Edge<int>> createdGraph = GraphHelpers.CreateGraph(
                vertices,
                edgeData,
                nameof(TestEdgeData.Src),
                nameof(TestEdgeData.Dst));
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertEmptyGraph(createdGraph);

            // Test 2
            vertices = new[] { 1, 2 };
            edgeData = new[] { new TestEdgeData { Src = 1, Dst = 2 } };
            createdGraph = GraphHelpers.CreateGraph(
                vertices,
                edgeData,
                nameof(TestEdgeData.Src),
                nameof(TestEdgeData.Dst));
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(createdGraph, new[] { new Edge<int>(1, 2) });

            // Test 3
            vertices = new[] { 1, 2, 3, 4, 5 };
            edgeData = new[]
            {
                new TestEdgeData { Src = 1, Dst = 2 },
                new TestEdgeData { Src = 1, Dst = 3 },
                new TestEdgeData { Src = 3, Dst = 4 },
                new TestEdgeData { Src = 4, Dst = 2 }
            };
            createdGraph = GraphHelpers.CreateGraph(
                vertices,
                edgeData,
                nameof(TestEdgeData.Src),
                nameof(TestEdgeData.Dst));
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(
                createdGraph,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(3, 4),
                    new Edge<int>(4, 2)
                });
        }

        [Test]
        public void CreateGraphByReflection_Throws()
        {
            int[] vertices = { 1, 2 };
            TestEdgeData[] edgeData = { new TestEdgeData { Src = 2, Dst = 1 } };

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    null,
                    edgeData,
                    nameof(TestEdgeData.Src),
                    nameof(TestEdgeData.Dst)));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    vertices,
                    null,
                    nameof(TestEdgeData.Src),
                    nameof(TestEdgeData.Dst)));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph(
                    vertices,
                    edgeData,
                    null,
                    nameof(TestEdgeData.Dst)));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph(
                    vertices,
                    edgeData,
                    nameof(TestEdgeData.Src),
                    null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    null,
                    null,
                    nameof(TestEdgeData.Src),
                    nameof(TestEdgeData.Dst)));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    null,
                    edgeData,
                    null,
                    nameof(TestEdgeData.Dst)));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    null,
                    edgeData,
                    nameof(TestEdgeData.Src),
                    null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    vertices,
                    null,
                    null,
                    nameof(TestEdgeData.Dst)));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    vertices,
                    null,
                    nameof(TestEdgeData.Src),
                    null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph(
                    vertices,
                    edgeData,
                    null,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    null,
                    null,
                    null,
                    nameof(TestEdgeData.Dst)));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    null,
                    null,
                    nameof(TestEdgeData.Src),
                    null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    null,
                    edgeData,
                    null,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    vertices,
                    null,
                    null,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, TestEdgeData>(
                    null,
                    null,
                    null,
                    null));
            // ReSharper restore AssignNullToNotNullAttribute

            Assert.Throws<ArgumentException>(
                () => GraphHelpers.CreateGraph(
                    vertices,
                    edgeData,
                    "NotExists",
                    nameof(TestEdgeData.Dst)));

            Assert.Throws<ArgumentException>(
                () => GraphHelpers.CreateGraph(
                    vertices,
                    edgeData,
                    nameof(TestEdgeData.FakeSrc),
                    nameof(TestEdgeData.Dst)));

            Assert.Throws<ArgumentException>(
                () => GraphHelpers.CreateGraph(
                    vertices,
                    edgeData,
                    nameof(TestEdgeData.Src),
                    "NotExists"));

            Assert.Throws<ArgumentException>(
                () => GraphHelpers.CreateGraph(
                    vertices,
                    edgeData,
                    nameof(TestEdgeData.Src),
                    nameof(TestEdgeData.FakeDst)));

            var edgeDataNotInGraph = new[] { new TestEdgeData { Src = 3, Dst = 4 } };
            Assert.Throws<VertexNotFoundException>(
                () => GraphHelpers.CreateGraph(
                    vertices,
                    edgeDataNotInGraph,
                    nameof(TestEdgeData.Src),
                    nameof(TestEdgeData.Dst)));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void CreateGraph()
        {
            // Test 1
            var vertices = new int[] { };
            var edgeData = new TestEdgeData[] { };
            Func<TestEdgeData, Edge<int>> factory = data => new Edge<int>(data.Src, data.Dst);

            BidirectionalGraph<int, Edge<int>> createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertEmptyGraph(createdGraph);

            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory, true);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertEmptyGraph(createdGraph);

            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory, false);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsFalse(createdGraph.AllowParallelEdges);
            AssertEmptyGraph(createdGraph);


            // Test 2
            vertices = new[] { 1, 2 };
            edgeData = new[] { new TestEdgeData { Src = 1, Dst = 2 } };
            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(createdGraph, new[] { new Edge<int>(1, 2) });

            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory, true);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(createdGraph, new[] { new Edge<int>(1, 2) });

            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory, false);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsFalse(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(createdGraph, new[] { new Edge<int>(1, 2) });


            // Test 3
            vertices = new[] { 1, 2, 3, 4, 5 };
            edgeData = new[]
            {
                new TestEdgeData { Src = 1, Dst = 2 },
                new TestEdgeData { Src = 1, Dst = 3 },
                new TestEdgeData { Src = 3, Dst = 4 },
                new TestEdgeData { Src = 4, Dst = 2 }
            };
            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(
                createdGraph,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(3, 4),
                    new Edge<int>(4, 2)
                });

            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory, true);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(
                createdGraph,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(3, 4),
                    new Edge<int>(4, 2)
                });

            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory, false);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsFalse(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(
                createdGraph,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(3, 4),
                    new Edge<int>(4, 2)
                });


            // Test 4
            vertices = new[] { 1, 2, 3 };
            edgeData = new[]
            {
                new TestEdgeData { Src = 1, Dst = 2 },
                new TestEdgeData { Src = 1, Dst = 2 },
                new TestEdgeData { Src = 2, Dst = 3 }
            };
            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(
                createdGraph,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 2),
                    new Edge<int>(2, 3)
                });

            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory, true);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsTrue(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(
                createdGraph,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 2),
                    new Edge<int>(2, 3)
                });

            createdGraph = GraphHelpers.CreateGraph(vertices, edgeData, factory, false);
            Assert.IsTrue(createdGraph.IsDirected);
            Assert.IsFalse(createdGraph.AllowParallelEdges);
            AssertHasVertices(createdGraph, vertices);
            AssertHasEdges_DeepCheck(
                createdGraph,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(2, 3)
                });
        }

        [Test]
        public void CreateGraph_Throws()
        {
            int[] vertices = { 1, 2 };
            TestEdgeData[] edgeData = { new TestEdgeData { Src = 2, Dst = 1 } };
            Func<TestEdgeData, Edge<int>> factory = data => new Edge<int>(data.Src, data.Dst);

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, edgeData, factory));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, edgeData, factory, true));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, edgeData, factory, false));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph(vertices, null, factory));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph(vertices, null, factory, true));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph(vertices, null, factory, false));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(vertices, edgeData, null));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(vertices, edgeData, null, true));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(vertices, edgeData, null, false));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, null, factory));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, null, factory, true));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, null, factory, false));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, edgeData, null));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, edgeData, null, true));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, edgeData, null, false));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(vertices, null, null));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(vertices, null, null, true));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(vertices, null, null, false));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, null, null, true));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.CreateGraph<int, Edge<int>, TestEdgeData>(null, null, null, false));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void Convert1()
        {
            var inGraph = new AdjacencyGraph<int, Edge<int>>();
            inGraph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(1, 3),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 2)
            });
            inGraph.AddVertex(5);

            // Test 1
            var outGraph1 = new BidirectionalGraph<float, EquatableEdge<float>>();
            Assert.AreSame(
                outGraph1,
                inGraph.Convert(
                    outGraph1,
                    v => (float)v,
                    e => new EquatableEdge<float>(e.Source, e.Target)));
            AssertHasVertices(outGraph1, new[] { 1f, 2f, 3f, 4f, 5f });
            AssertHasEdges(
                outGraph1,
                new[]
                {
                    new EquatableEdge<float>(1f, 2f),
                    new EquatableEdge<float>(1f, 3f),
                    new EquatableEdge<float>(2f, 2f),
                    new EquatableEdge<float>(2f, 3f),
                    new EquatableEdge<float>(3f, 4f),
                    new EquatableEdge<float>(4f, 2f)
                });

            // Test 2
            var outGraph2 = new BidirectionalGraph<int, EquatableEdge<int>>();
            Assert.AreSame(
                outGraph2,
                inGraph.Convert(
                    outGraph2,
                    (Func<int, int>)null,
                    e => new EquatableEdge<int>(e.Source, e.Target)));
            AssertHasVertices(outGraph2, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges(
                outGraph2,
                new[]
                {
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(1, 3),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(2, 3),
                    new EquatableEdge<int>(3, 4),
                    new EquatableEdge<int>(4, 2)
                });

            // Test 3
            var outGraph3 = new BidirectionalGraph<int, Edge<int>>();
            Assert.AreSame(
                outGraph3,
                inGraph.Convert(
                    outGraph3,
                    (Func<int, int>)null,
                    (Func<Edge<int>, Edge<int>>)null));
            AssertHasVertices(outGraph3, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges_DeepCheck(
                outGraph3,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(2, 2),
                    new Edge<int>(2, 3),
                    new Edge<int>(3, 4),
                    new Edge<int>(4, 2)
                });
        }

        [Test]
        public void Convert2()
        {
            var inGraph = new AdjacencyGraph<int, Edge<int>>();
            inGraph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(1, 3),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 2)
            });
            inGraph.AddVertex(5);

            // Test 1
            var outGraph1 = new BidirectionalGraph<int, EquatableEdge<int>>();
            Assert.AreSame(
                outGraph1,
                inGraph.Convert(
                    outGraph1,
                    e => new EquatableEdge<int>(e.Source, e.Target)));
            AssertHasVertices(outGraph1, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges(
                outGraph1,
                new[]
                {
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(1, 3),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(2, 3),
                    new EquatableEdge<int>(3, 4),
                    new EquatableEdge<int>(4, 2)
                });

            // Test 2
            var outGraph2 = new BidirectionalGraph<int, Edge<int>>();
            Assert.AreSame(
                outGraph2,
                inGraph.Convert(outGraph2, (Func<Edge<int>, Edge<int>>)null));
            AssertHasVertices(outGraph2, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges_DeepCheck(
                outGraph2,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(2, 2),
                    new Edge<int>(2, 3),
                    new Edge<int>(3, 4),
                    new Edge<int>(4, 2)
                });
        }

        [Test]
        public void Convert3()
        {
            var inGraph = new AdjacencyGraph<int, Edge<int>>();
            inGraph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(1, 3),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 2)
            });
            inGraph.AddVertex(5);

            var outGraph1 = new BidirectionalGraph<int, Edge<int>>();
            Assert.AreSame(
                outGraph1,
                inGraph.Convert(outGraph1));
            AssertHasVertices(outGraph1, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges_DeepCheck(
                outGraph1,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(2, 2),
                    new Edge<int>(2, 3),
                    new Edge<int>(3, 4),
                    new Edge<int>(4, 2)
                });
        }

        [Test]
        public void Convert4()
        {
            var inGraph = new AdjacencyGraph<int, Edge<int>>();
            inGraph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(1, 3),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 2)
            });
            inGraph.AddVertex(5);

            // Test 1
            BidirectionalGraph<float, EquatableEdge<float>> outGraph1 = inGraph.Convert(
                v => (float)v,
                e => new EquatableEdge<float>(e.Source, e.Target));
            AssertHasVertices(outGraph1, new[] { 1f, 2f, 3f, 4f, 5f });
            AssertHasEdges(
                outGraph1,
                new[]
                {
                    new EquatableEdge<float>(1f, 2f),
                    new EquatableEdge<float>(1f, 3f),
                    new EquatableEdge<float>(2f, 2f),
                    new EquatableEdge<float>(2f, 3f),
                    new EquatableEdge<float>(3f, 4f),
                    new EquatableEdge<float>(4f, 2f)
                });

            // Test 2
            BidirectionalGraph<int, EquatableEdge<int>> outGraph2 = inGraph.Convert(
                (Func<int, int>)null,
                e => new EquatableEdge<int>(e.Source, e.Target));
            AssertHasVertices(outGraph2, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges(
                outGraph2,
                new[]
                {
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(1, 3),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(2, 3),
                    new EquatableEdge<int>(3, 4),
                    new EquatableEdge<int>(4, 2)
                });

            // Test 3
            BidirectionalGraph<int, Edge<int>> outGraph3 = inGraph.Convert(
                (Func<int, int>)null,
                (Func<Edge<int>, Edge<int>>)null);
            AssertHasVertices(outGraph3, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges_DeepCheck(
                outGraph3,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(2, 2),
                    new Edge<int>(2, 3),
                    new Edge<int>(3, 4),
                    new Edge<int>(4, 2)
                });
        }

        [Test]
        public void Convert5()
        {
            var inGraph = new AdjacencyGraph<int, Edge<int>>();
            inGraph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(1, 3),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 2)
            });
            inGraph.AddVertex(5);

            // Test 1
            BidirectionalGraph<int, EquatableEdge<int>> outGraph1 = inGraph.Convert(
                e => new EquatableEdge<int>(e.Source, e.Target));
            AssertHasVertices(outGraph1, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges(
                outGraph1,
                new[]
                {
                    new EquatableEdge<int>(1, 2),
                    new EquatableEdge<int>(1, 3),
                    new EquatableEdge<int>(2, 2),
                    new EquatableEdge<int>(2, 3),
                    new EquatableEdge<int>(3, 4),
                    new EquatableEdge<int>(4, 2)
                });

            // Test 2
            BidirectionalGraph<int, Edge<int>> outGraph2 = inGraph.Convert(
                (Func<Edge<int>, Edge<int>>)null);
            AssertHasVertices(outGraph2, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges_DeepCheck(
                outGraph2,
                new[]
                {
                    new Edge<int>(1, 2),
                    new Edge<int>(1, 3),
                    new Edge<int>(2, 2),
                    new Edge<int>(2, 3),
                    new Edge<int>(3, 4),
                    new Edge<int>(4, 2)
                });
        }

        [Test]
        public void Convert_Throws()
        {
            var inGraph = new AdjacencyGraph<int, Edge<int>>();
            var outGraph1 = new AdjacencyGraph<float, Edge<float>>();
            var outGraph2 = new AdjacencyGraph<int, EquatableEdge<int>>();
            var outGraph3 = new BidirectionalGraph<int, Edge<int>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, float, Edge<float>, AdjacencyGraph<float, Edge<float>>>(null, outGraph1, null, null));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, float, Edge<float>, AdjacencyGraph<float, Edge<float>>>(inGraph, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, float, Edge<float>, AdjacencyGraph<float, Edge<float>>>(null, null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, EquatableEdge<int>, AdjacencyGraph<int, EquatableEdge<int>>>(null, outGraph2, null));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, EquatableEdge<int>, AdjacencyGraph<int, EquatableEdge<int>>>(inGraph, null, null));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, EquatableEdge<int>, AdjacencyGraph<int, EquatableEdge<int>>>(null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(null, outGraph3));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(inGraph, null));
            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(null, null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, float, Edge<float>>(null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => GraphHelpers.Convert<int, Edge<int>, EquatableEdge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void CopyToBidirectionalGraph()
        {
            // Test 1
            var graph = new AdjacencyGraph<int, Edge<int>>();

            BidirectionalGraph<int, Edge<int>> createdGraph = graph.CopyToBidirectionalGraph();
            AssertEmptyGraph(createdGraph);

            // Test 2
            var edge12 = new Edge<int>(1, 2);
            graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVerticesAndEdge(edge12);
            createdGraph = graph.CopyToBidirectionalGraph();
            AssertHasVertices(createdGraph, new[] { 1, 2 });
            AssertHasEdges(createdGraph, new[] { edge12 });

            // Test 3
            var edge13 = new Edge<int>(1, 3);
            var edge34 = new Edge<int>(3, 4);
            var edge42 = new Edge<int>(4, 2);
            graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge34, edge42
            });
            createdGraph = graph.CopyToBidirectionalGraph();
            AssertHasVertices(createdGraph, new[] { 1, 2, 3, 4 });
            AssertHasEdges(createdGraph, new[] { edge12, edge13, edge34, edge42 });

            // Test 4 (isolated vertex)
            graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge34, edge42
            });
            graph.AddVertex(5);
            createdGraph = graph.CopyToBidirectionalGraph();
            AssertHasVertices(createdGraph, new[] { 1, 2, 3, 4, 5 });
            AssertHasEdges(createdGraph, new[] { edge12, edge13, edge34, edge42 });
        }

        [Test]
        public void CopyToBidirectionalGraph_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => GraphHelpers.CopyToBidirectionalGraph<int, Edge<int>>(null));
        }

        #endregion
    }
}