using System;
using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.Layout;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using QuikGraph.Algorithms;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="LayoutAlgorithm{TVertex, TEdge, TGraph}"/>
    /// and <see cref="LayoutAlgorithm{TVertex,TEdge,TGraph,TVertexInfo,TEdgeInfo}"/>.
    /// </summary>
    [TestFixture]
    internal class LayoutAlgorithmBaseTests : AlgorithmTestsBase
    {
        #region Test classes

        private class TestSimpleLayoutAlgorithm : LayoutAlgorithmBase<TestVertex, Edge<TestVertex>, AdjacencyGraph<TestVertex, Edge<TestVertex>>>
        {
            public TestSimpleLayoutAlgorithm(
                [NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph)
                : base(visitedGraph)
            {
            }

            public TestSimpleLayoutAlgorithm(
                [NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph,
                [CanBeNull] IDictionary<TestVertex, Point> verticesPositions)
                : base(visitedGraph, verticesPositions)
            {
            }

            public LayoutIterationEventArgs<TestVertex, Edge<TestVertex>> Args { get; set; }

            protected override void InternalCompute()
            {
                OnProgressChanged(0);
                OnProgressChanged(50);
                if (Args != null)
                    OnIterationEnded(Args);
                OnProgressChanged(100);
            }
        }

        private class TestComplexLayoutAlgorithm : LayoutAlgorithmBase<TestVertex, Edge<TestVertex>, AdjacencyGraph<TestVertex, Edge<TestVertex>>, int, double>
        {
            public TestComplexLayoutAlgorithm(
                [NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph)
                : base(visitedGraph)
            {
            }

            public TestComplexLayoutAlgorithm(
                [NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph,
                [CanBeNull] IDictionary<TestVertex, Point> verticesPositions)
                : base(visitedGraph, verticesPositions)
            {
            }

            public LayoutIterationEventArgs<TestVertex, Edge<TestVertex>, int, double> Args { get; set; }

            protected override void InternalCompute()
            {
                OnProgressChanged(0);
                OnProgressChanged(50);
                if (Args != null)
                    OnIterationEnded(Args);
                OnProgressChanged(100);
            }
        }

        #endregion

        [Test]
        public void Constructor1()
        {
            var verticesPositions = new Dictionary<TestVertex, Point>();
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestSimpleLayoutAlgorithm(graph);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            algorithm = new TestSimpleLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new TestSimpleLayoutAlgorithm(graph);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new TestSimpleLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new TestSimpleLayoutAlgorithm(graph, verticesPositions);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            algorithm = new TestSimpleLayoutAlgorithm(graph, verticesPositions);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true);

            algorithm = new TestSimpleLayoutAlgorithm(graph, verticesPositions);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, expectedReportProgress: true);

            algorithm = new TestSimpleLayoutAlgorithm(graph, verticesPositions);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true, true);

            algorithm = new TestSimpleLayoutAlgorithm(graph, null);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge, TGraph>(
                LayoutAlgorithmBase<TVertex, TEdge, TGraph> algo,
                TGraph g,
                IDictionary<TVertex, Point> pos = null,
                bool expectedReportIterationEnd = false,
                bool expectedReportProgress = false)
                where TEdge : IEdge<TVertex>
                where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
            {
                AssertAlgorithmState(algo);
                Assert.AreSame(g, algo.VisitedGraph);
                if (pos is null)
                    Assert.IsNotNull(algo.VerticesPositions);
                else
                    CollectionAssert.AreEqual(pos, algo.VerticesPositions);
                Assert.AreEqual(expectedReportIterationEnd, algo.ReportOnIterationEndNeeded);
                Assert.AreEqual(expectedReportProgress, algo.ReportOnProgressChangedNeeded);
            }

            #endregion
        }

        [Test]
        public void Constructor2()
        {
            var verticesPositions = new Dictionary<TestVertex, Point>();
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestComplexLayoutAlgorithm(graph);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            algorithm = new TestComplexLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new TestComplexLayoutAlgorithm(graph);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new TestComplexLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new TestComplexLayoutAlgorithm(graph, verticesPositions);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            algorithm = new TestComplexLayoutAlgorithm(graph, verticesPositions);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true);

            algorithm = new TestComplexLayoutAlgorithm(graph, verticesPositions);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, expectedReportProgress: true);

            algorithm = new TestComplexLayoutAlgorithm(graph, verticesPositions);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true, true);

            algorithm = new TestComplexLayoutAlgorithm(graph, null);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo>(
                LayoutAlgorithmBase<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo> algo,
                TGraph g,
                IDictionary<TVertex, Point> pos = null,
                bool expectedReportIterationEnd = false,
                bool expectedReportProgress = false)
                where TEdge : IEdge<TVertex>
                where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
            {
                AssertAlgorithmState(algo);
                Assert.AreSame(g, algo.VisitedGraph);
                if (pos is null)
                    Assert.IsNotNull(algo.VerticesPositions);
                else
                    CollectionAssert.AreEqual(pos, algo.VerticesPositions);
                Assert.AreEqual(expectedReportIterationEnd, algo.ReportOnIterationEndNeeded);
                Assert.AreEqual(expectedReportProgress, algo.ReportOnProgressChangedNeeded);
                CollectionAssert.IsEmpty(algo.VerticesInfos);
                CollectionAssert.IsEmpty(algo.EdgesInfos);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var verticesPositions = new Dictionary<TestVertex, Point>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new TestSimpleLayoutAlgorithm(null));
            Assert.Throws<ArgumentNullException>(() => new TestSimpleLayoutAlgorithm(null, verticesPositions));
            Assert.Throws<ArgumentNullException>(() => new TestSimpleLayoutAlgorithm(null, null));

            Assert.Throws<ArgumentNullException>(() => new TestComplexLayoutAlgorithm(null));
            Assert.Throws<ArgumentNullException>(() => new TestComplexLayoutAlgorithm(null, verticesPositions));
            Assert.Throws<ArgumentNullException>(() => new TestComplexLayoutAlgorithm(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void GetVertexInfo()
        {
            var vertex = new TestVertex("1");

            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm1 = new TestSimpleLayoutAlgorithm(graph);
            Assert.IsNull(algorithm1.GetVertexInfo(vertex));

            var algorithm2 = new TestComplexLayoutAlgorithm(graph);
            Assert.IsNull(algorithm2.GetVertexInfo(vertex));

            graph.AddVertex(vertex);
            Assert.IsNull(algorithm2.GetVertexInfo(vertex));

            const int vertexInfo = 1;
            algorithm2.VerticesInfos.Add(vertex, vertexInfo);
            Assert.AreEqual(vertexInfo, algorithm2.GetVertexInfo(vertex));

            algorithm2.VerticesInfos.Remove(vertex);
            Assert.IsNull(algorithm2.GetVertexInfo(vertex));
        }

        [Test]
        public void GetVertexInfo_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm1 = new TestSimpleLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm1.GetVertexInfo(null));

            var algorithm2 = new TestComplexLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm2.GetVertexInfo(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void GetEdgeInfo()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var edge = new Edge<TestVertex>(vertex1, vertex2);

            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm1 = new TestSimpleLayoutAlgorithm(graph);
            Assert.IsNull(algorithm1.GetEdgeInfo(edge));

            var algorithm2 = new TestComplexLayoutAlgorithm(graph);
            Assert.IsNull(algorithm2.GetEdgeInfo(edge));

            graph.AddVerticesAndEdge(edge);
            Assert.IsNull(algorithm2.GetEdgeInfo(edge));

            const double edgeInfo = 1.2;
            algorithm2.EdgesInfos.Add(edge, edgeInfo);
            Assert.AreEqual(edgeInfo, algorithm2.GetEdgeInfo(edge));

            algorithm2.EdgesInfos.Remove(edge);
            Assert.IsNull(algorithm2.GetEdgeInfo(edge));
        }

        [Test]
        public void GetEdgeInfo_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm1 = new TestSimpleLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm1.GetEdgeInfo(null));

            var algorithm2 = new TestComplexLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm2.GetEdgeInfo(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void ProgressChanged_Simple()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestSimpleLayoutAlgorithm(graph);
            var progresses = new Stack<double>(new[] { 100.0, 50.0, 0.0 });
            algorithm.ProgressChanged += (sender, percent) => Assert.AreEqual(progresses.Pop(), percent);

            algorithm.Compute();

            CollectionAssert.IsEmpty(progresses);
        }

        [Test]
        public void ProgressChanged_Complex()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestComplexLayoutAlgorithm(graph);
            var progresses = new Stack<double>(new[] { 100.0, 50.0, 0.0 });
            algorithm.ProgressChanged += (sender, percent) => Assert.AreEqual(progresses.Pop(), percent);

            algorithm.Compute();

            CollectionAssert.IsEmpty(progresses);
        }

        [Test]
        public void IterationEnded_Simple()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestSimpleLayoutAlgorithm(graph);
            var arguments = new LayoutIterationEventArgs<TestVertex, Edge<TestVertex>>(1, 100.0);
            algorithm.Args = arguments;

            var iterations = new Stack<ILayoutIterationEventArgs<TestVertex>>(new[] { arguments });
            algorithm.IterationEnded += (sender, args) => Assert.AreSame(iterations.Pop(), args);

            algorithm.Compute();

            Assert.AreEqual(ComputationState.Finished, algorithm.State);
            CollectionAssert.IsEmpty(iterations);
        }

        [Test]
        public void IterationEndedAbort_Simple()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestSimpleLayoutAlgorithm(graph);
            var arguments = new LayoutIterationEventArgs<TestVertex, Edge<TestVertex>>(1, 100.0)
            {
                Abort = true
            };
            algorithm.Args = arguments;

            var iterations = new Stack<ILayoutIterationEventArgs<TestVertex>>(new[] { arguments });
            algorithm.IterationEnded += (sender, args) => Assert.AreSame(iterations.Pop(), args);

            algorithm.Compute();

            Assert.AreEqual(ComputationState.Aborted, algorithm.State);
            CollectionAssert.IsEmpty(iterations);
        }

        [Test]
        public void IterationEnded_Complex()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestComplexLayoutAlgorithm(graph);
            var arguments = new LayoutIterationEventArgs<TestVertex, Edge<TestVertex>, int, double>(1, 100.0);
            algorithm.Args = arguments;

            var iterations = new Stack<ILayoutIterationEventArgs<TestVertex>>(new[] { arguments });
            var infoIterations = new Stack<ILayoutIterationEventArgs<TestVertex>>(new[] { arguments });
            algorithm.IterationEnded += (sender, args) => Assert.AreSame(iterations.Pop(), args);
            algorithm.InfoIterationEnded += (sender, args) => Assert.AreSame(infoIterations.Pop(), args);

            algorithm.Compute();

            Assert.AreEqual(ComputationState.Finished, algorithm.State);
            CollectionAssert.IsEmpty(iterations);
            CollectionAssert.IsEmpty(infoIterations);
        }

        [Test]
        public void IterationEndedAbort_Complex()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestComplexLayoutAlgorithm(graph);
            var arguments = new LayoutIterationEventArgs<TestVertex, Edge<TestVertex>, int, double>(1, 100.0)
            {
                Abort = true
            };
            algorithm.Args = arguments;

            var iterations = new Stack<ILayoutIterationEventArgs<TestVertex>>(new[] { arguments });
            var infoIterations = new Stack<ILayoutIterationEventArgs<TestVertex>>(new[] { arguments });
            algorithm.IterationEnded += (sender, args) => Assert.AreSame(iterations.Pop(), args);
            algorithm.InfoIterationEnded += (sender, args) => Assert.AreSame(infoIterations.Pop(), args);

            algorithm.Compute();

            Assert.AreEqual(ComputationState.Aborted, algorithm.State);
            CollectionAssert.IsEmpty(iterations);
            CollectionAssert.IsEmpty(infoIterations);
        }
    }
}