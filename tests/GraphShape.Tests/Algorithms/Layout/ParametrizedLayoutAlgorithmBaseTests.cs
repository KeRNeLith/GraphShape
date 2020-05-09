using System;
using System.Collections.Generic;
using GraphShape.Algorithms.Layout;
using GraphShape.Utils;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using QuikGraph.Algorithms;
using static GraphShape.Tests.Algorithms.AlgorithmTestHelpers;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="ParameterizedLayoutAlgorithmBase{TVertex,TEdge,TGraph,TParameters}"/>,
    /// <see cref="DefaultParameterizedLayoutAlgorithmBase{TVertex,TEdge,TGraph,TParameters}"/>
    /// and <see cref="ParameterizedLayoutAlgorithmBase{TVertex,TEdge,TGraph,TVertexInfo,TEdgeInfo,TParameters}"/>.
    /// </summary>
    [TestFixture]
    internal class ParameterizedLayoutAlgorithmBaseTests
    {
        #region Test classes

        private sealed class TestLayoutParameters : NotifierObject, ILayoutParameters, IEquatable<TestLayoutParameters>
        {
            private static int _counter;

            private readonly int _value;

            public TestLayoutParameters()
            {
                _value = ++_counter;
            }

            public object Clone()
            {
                return MemberwiseClone();
            }

            public bool Equals(TestLayoutParameters other)
            {
                if (other is null)
                    return false;
                return _value == other._value;
            }
        }

        private class TestSimpleParameterizedLayoutAlgorithm : ParameterizedLayoutAlgorithmBase<TestVertex, Edge<TestVertex>, AdjacencyGraph<TestVertex, Edge<TestVertex>>, TestLayoutParameters>
        {
            public TestSimpleParameterizedLayoutAlgorithm([NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph)
                : base(visitedGraph)
            {
            }

            public TestSimpleParameterizedLayoutAlgorithm(
                [NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph,
                [CanBeNull] IDictionary<TestVertex, Point> verticesPositions,
                [CanBeNull] TestLayoutParameters parameters)
                : base(visitedGraph, verticesPositions, parameters)
            {
            }

            protected override void InternalCompute()
            {
                OnProgressChanged(0);
                OnProgressChanged(50);
                OnIterationEnded(1, 50, "Test", false);
                OnIterationEnded(2, 60, "Test2", true);
                OnProgressChanged(100);
            }

            protected override TestLayoutParameters DefaultParameters { get; } = new TestLayoutParameters();
        }

        private class TestDefaultSimpleParameterizedLayoutAlgorithm : DefaultParameterizedLayoutAlgorithmBase<TestVertex, Edge<TestVertex>, AdjacencyGraph<TestVertex, Edge<TestVertex>>, TestLayoutParameters>
        {
            public TestDefaultSimpleParameterizedLayoutAlgorithm([NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph)
                : base(visitedGraph)
            {
            }

            public TestDefaultSimpleParameterizedLayoutAlgorithm(
                [NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph,
                [CanBeNull] IDictionary<TestVertex, Point> verticesPositions,
                [CanBeNull] TestLayoutParameters parameters)
                : base(visitedGraph, verticesPositions, parameters)
            {
            }

            protected override void InternalCompute()
            {
                OnProgressChanged(0);
                OnProgressChanged(50);
                OnIterationEnded(1, 50, "Test", false);
                OnIterationEnded(2, 60, "Test2", true);
                OnProgressChanged(100);
            }

            protected override TestLayoutParameters DefaultParameters { get; } = new TestLayoutParameters();
        }

        private class TestComplexParameterizedLayoutAlgorithm : ParameterizedLayoutAlgorithmBase<TestVertex, Edge<TestVertex>, AdjacencyGraph<TestVertex, Edge<TestVertex>>, int, double, TestLayoutParameters>
        {
            public TestComplexParameterizedLayoutAlgorithm([NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph)
                : base(visitedGraph)
            {
            }

            public TestComplexParameterizedLayoutAlgorithm(
                [NotNull] AdjacencyGraph<TestVertex, Edge<TestVertex>> visitedGraph,
                [CanBeNull] IDictionary<TestVertex, Point> verticesPositions,
                [CanBeNull] TestLayoutParameters parameters)
                : base(visitedGraph, verticesPositions, parameters)
            {
            }

            protected override void InternalCompute()
            {
                OnProgressChanged(0);
                OnProgressChanged(50);
                OnIterationEnded(1, 50, "Test", false);
                OnIterationEnded(2, 60, "Test2", true);
                OnProgressChanged(100);
            }

            protected override TestLayoutParameters DefaultParameters { get; } = new TestLayoutParameters();
        }

        #endregion

        [Test]
        public void Constructor1()
        {
            var verticesPositions = new Dictionary<TestVertex, Point>();
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true);

            algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, expectedReportProgress: true);

            algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true, true);

            algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph, null, null);
            AssertAlgorithmProperties(algorithm, graph);

            var parameters = new TestLayoutParameters();
            algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph, null, parameters);
            AssertAlgorithmProperties(algorithm, graph, parameters: parameters);
        }

        [Test]
        public void Constructor2()
        {
            var verticesPositions = new Dictionary<TestVertex, Point>();
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true);

            algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, expectedReportProgress: true);

            algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true, true);

            algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph, null, null);
            AssertAlgorithmProperties(algorithm, graph);

            var parameters = new TestLayoutParameters();
            algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph, null, parameters);
            AssertAlgorithmProperties(algorithm, graph, parameters: parameters);
        }

        [Test]
        public void Constructor3()
        {
            var verticesPositions = new Dictionary<TestVertex, Point>();
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestComplexParameterizedLayoutAlgorithm(graph);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new TestComplexParameterizedLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true);

            algorithm = new TestComplexParameterizedLayoutAlgorithm(graph);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportProgress: true);

            algorithm = new TestComplexParameterizedLayoutAlgorithm(graph);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, expectedReportIterationEnd: true, expectedReportProgress: true);

            algorithm = new TestComplexParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            AssertAlgorithmProperties(algorithm, graph, verticesPositions);

            algorithm = new TestComplexParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            algorithm.IterationEnded += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true);

            algorithm = new TestComplexParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, expectedReportProgress: true);

            algorithm = new TestComplexParameterizedLayoutAlgorithm(graph, verticesPositions, null);
            algorithm.IterationEnded += (sender, args) => { };
            algorithm.ProgressChanged += (sender, args) => { };
            AssertAlgorithmProperties(algorithm, graph, verticesPositions, true, true);

            algorithm = new TestComplexParameterizedLayoutAlgorithm(graph, null, null);
            AssertAlgorithmProperties(algorithm, graph);

            var parameters = new TestLayoutParameters();
            algorithm = new TestComplexParameterizedLayoutAlgorithm(graph, null, parameters);
            AssertAlgorithmProperties(algorithm, graph, parameters: parameters);
        }

        [Test]
        public void Constructor_Throws()
        {
            var verticesPositions = new Dictionary<TestVertex, Point>();
            var parameters = new TestLayoutParameters();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new TestSimpleParameterizedLayoutAlgorithm(null));
            Assert.Throws<ArgumentNullException>(() => new TestSimpleParameterizedLayoutAlgorithm(null, verticesPositions, parameters));
            Assert.Throws<ArgumentNullException>(() => new TestSimpleParameterizedLayoutAlgorithm(null, verticesPositions, null));
            Assert.Throws<ArgumentNullException>(() => new TestSimpleParameterizedLayoutAlgorithm(null, null, parameters));
            Assert.Throws<ArgumentNullException>(() => new TestSimpleParameterizedLayoutAlgorithm(null, null, null));

            Assert.Throws<ArgumentNullException>(() => new TestDefaultSimpleParameterizedLayoutAlgorithm(null));
            Assert.Throws<ArgumentNullException>(() => new TestDefaultSimpleParameterizedLayoutAlgorithm(null, verticesPositions, parameters));
            Assert.Throws<ArgumentNullException>(() => new TestDefaultSimpleParameterizedLayoutAlgorithm(null, verticesPositions, null));
            Assert.Throws<ArgumentNullException>(() => new TestDefaultSimpleParameterizedLayoutAlgorithm(null, null, parameters));
            Assert.Throws<ArgumentNullException>(() => new TestDefaultSimpleParameterizedLayoutAlgorithm(null, null, null));

            Assert.Throws<ArgumentNullException>(() => new TestComplexParameterizedLayoutAlgorithm(null));
            Assert.Throws<ArgumentNullException>(() => new TestComplexParameterizedLayoutAlgorithm(null, verticesPositions, parameters));
            Assert.Throws<ArgumentNullException>(() => new TestComplexParameterizedLayoutAlgorithm(null, verticesPositions, null));
            Assert.Throws<ArgumentNullException>(() => new TestComplexParameterizedLayoutAlgorithm(null, null, parameters));
            Assert.Throws<ArgumentNullException>(() => new TestComplexParameterizedLayoutAlgorithm(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void GetVertexInfo()
        {
            var vertex = new TestVertex("1");

            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm1 = new TestSimpleParameterizedLayoutAlgorithm(graph);
            Assert.IsNull(algorithm1.GetVertexInfo(vertex));

            var algorithm2 = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph);
            Assert.IsNull(algorithm2.GetVertexInfo(vertex));

            var algorithm3 = new TestComplexParameterizedLayoutAlgorithm(graph);
            Assert.IsNull(algorithm3.GetVertexInfo(vertex));

            graph.AddVertex(vertex);
            Assert.IsNull(algorithm3.GetVertexInfo(vertex));

            const int vertexInfo = 1;
            algorithm3.VerticesInfos.Add(vertex, vertexInfo);
            Assert.AreEqual(vertexInfo, algorithm3.GetVertexInfo(vertex));

            algorithm3.VerticesInfos.Remove(vertex);
            Assert.IsNull(algorithm3.GetVertexInfo(vertex));
        }

        [Test]
        public void GetVertexInfo_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm1 = new TestSimpleParameterizedLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm1.GetVertexInfo(null));

            var algorithm2 = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm2.GetVertexInfo(null));

            var algorithm3 = new TestComplexParameterizedLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm3.GetVertexInfo(null));
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
            var algorithm1 = new TestSimpleParameterizedLayoutAlgorithm(graph);
            Assert.IsNull(algorithm1.GetEdgeInfo(edge));

            var algorithm2 = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph);
            Assert.IsNull(algorithm2.GetEdgeInfo(edge));

            var algorithm3 = new TestComplexParameterizedLayoutAlgorithm(graph);
            Assert.IsNull(algorithm3.GetEdgeInfo(edge));

            graph.AddVerticesAndEdge(edge);
            Assert.IsNull(algorithm3.GetEdgeInfo(edge));

            const double edgeInfo = 1.2;
            algorithm3.EdgesInfos.Add(edge, edgeInfo);
            Assert.AreEqual(edgeInfo, algorithm3.GetEdgeInfo(edge));

            algorithm3.EdgesInfos.Remove(edge);
            Assert.IsNull(algorithm3.GetEdgeInfo(edge));
        }

        [Test]
        public void GetEdgeInfo_Throws()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm1 = new TestSimpleParameterizedLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm1.GetEdgeInfo(null));

            var algorithm2 = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm2.GetEdgeInfo(null));

            var algorithm3 = new TestComplexParameterizedLayoutAlgorithm(graph);
            Assert.Throws<ArgumentNullException>(() => algorithm3.GetEdgeInfo(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void ProgressChanged_Simple()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph);
            var progresses = new Stack<double>(new[] { 100.0, 50.0, 0.0 });
            algorithm.ProgressChanged += (sender, percent) => Assert.AreEqual(progresses.Pop(), percent);

            algorithm.Compute();

            CollectionAssert.IsEmpty(progresses);
        }

        [Test]
        public void ProgressChanged_DefaultSimple()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph);
            var progresses = new Stack<double>(new[] { 100.0, 50.0, 0.0 });
            algorithm.ProgressChanged += (sender, percent) => Assert.AreEqual(progresses.Pop(), percent);

            algorithm.Compute();

            CollectionAssert.IsEmpty(progresses);
        }

        [Test]
        public void ProgressChanged_Complex()
        {
            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new TestComplexParameterizedLayoutAlgorithm(graph);
            var progresses = new Stack<double>(new[] { 100.0, 50.0, 0.0 });
            algorithm.ProgressChanged += (sender, percent) => Assert.AreEqual(progresses.Pop(), percent);

            algorithm.Compute();

            CollectionAssert.IsEmpty(progresses);
        }

        private struct EventArgsContentChecker
        {
            public EventArgsContentChecker(
                int iteration,
                double percent,
                string message,
                [CanBeNull] IDictionary<TestVertex, Point> positions)
            {
                Iteration = iteration;
                Percent = percent;
                Message = message;
                Positions = positions;
            }

            public int Iteration { get; }
            public double Percent { get; }
            public string Message { get; }
            [CanBeNull]
            public IDictionary<TestVertex, Point> Positions { get; }
        }

        [Test]
        public void IterationEnded_Simple()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");

            var verticesPositions = new Dictionary<TestVertex, Point>
            {
                [vertex1] = new Point(12, 5),
                [vertex2] = new Point(5, 42)
            };

            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            graph.AddVertexRange(new[] { vertex1, vertex2 });

            var algorithm = new TestSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);

            var iteration1 = new EventArgsContentChecker(1, 50, "Test", verticesPositions);
            var normalizedVerticesPositions = new Dictionary<TestVertex, Point>(verticesPositions);
            LayoutUtils.NormalizePositions(normalizedVerticesPositions);
            var iteration2 = new EventArgsContentChecker(2, 60, "Test2", normalizedVerticesPositions);
            var iterations = new Stack<EventArgsContentChecker>(new[] { iteration2, iteration1 });
            algorithm.IterationEnded += (sender, args) =>
            {
                EventArgsContentChecker contentChecker = iterations.Pop();
                Assert.AreEqual(contentChecker.Iteration, args.Iteration);
                Assert.AreEqual(contentChecker.Percent, args.StatusInPercent);
                Assert.AreEqual(contentChecker.Message, args.Message);
                Assert.AreEqual(contentChecker.Positions, args.VerticesPositions);
            };

            algorithm.Compute();

            Assert.AreEqual(ComputationState.Finished, algorithm.State);
            CollectionAssert.IsEmpty(iterations);
        }

        [Test]
        public void IterationEnded_DefaultSimple()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");

            var verticesPositions = new Dictionary<TestVertex, Point>
            {
                [vertex1] = new Point(12, 5),
                [vertex2] = new Point(5, 42)
            };

            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            graph.AddVertexRange(new[] { vertex1, vertex2 });

            var algorithm = new TestDefaultSimpleParameterizedLayoutAlgorithm(graph, verticesPositions, null);

            var iteration1 = new EventArgsContentChecker(1, 50, "Test", verticesPositions);
            var normalizedVerticesPositions = new Dictionary<TestVertex, Point>(verticesPositions);
            LayoutUtils.NormalizePositions(normalizedVerticesPositions);
            var iteration2 = new EventArgsContentChecker(2, 60, "Test2", normalizedVerticesPositions);
            var iterations = new Stack<EventArgsContentChecker>(new[] { iteration2, iteration1 });
            algorithm.IterationEnded += (sender, args) =>
            {
                EventArgsContentChecker contentChecker = iterations.Pop();
                Assert.AreEqual(contentChecker.Iteration, args.Iteration);
                Assert.AreEqual(contentChecker.Percent, args.StatusInPercent);
                Assert.AreEqual(contentChecker.Message, args.Message);
                Assert.AreEqual(contentChecker.Positions, args.VerticesPositions);
            };

            algorithm.Compute();

            Assert.AreEqual(ComputationState.Finished, algorithm.State);
            CollectionAssert.IsEmpty(iterations);
        }

        [Test]
        public void IterationEnded_Complex()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");

            var verticesPositions = new Dictionary<TestVertex, Point>
            {
                [vertex1] = new Point(12, 5),
                [vertex2] = new Point(5, 42)
            };

            var graph = new AdjacencyGraph<TestVertex, Edge<TestVertex>>();
            graph.AddVertexRange(new[] { vertex1, vertex2 });

            var algorithm = new TestComplexParameterizedLayoutAlgorithm(graph, verticesPositions, null);

            var iteration1 = new EventArgsContentChecker(1, 50, "Test", verticesPositions);
            var normalizedVerticesPositions = new Dictionary<TestVertex, Point>(verticesPositions);
            LayoutUtils.NormalizePositions(normalizedVerticesPositions);
            var iteration2 = new EventArgsContentChecker(2, 60, "Test2", normalizedVerticesPositions);
            var iterations = new Stack<EventArgsContentChecker>(new[] { iteration2, iteration1 });
            var infoIterations = new Stack<EventArgsContentChecker>(new[] { iteration2, iteration1 });
            algorithm.IterationEnded += (sender, args) =>
            {
                EventArgsContentChecker contentChecker = iterations.Pop();
                Assert.AreEqual(contentChecker.Iteration, args.Iteration);
                Assert.AreEqual(contentChecker.Percent, args.StatusInPercent);
                Assert.AreEqual(contentChecker.Message, args.Message);
                Assert.AreEqual(contentChecker.Positions, args.VerticesPositions);
            };
            algorithm.InfoIterationEnded += (sender, args) =>
            {
                EventArgsContentChecker contentChecker = infoIterations.Pop();
                Assert.AreEqual(contentChecker.Iteration, args.Iteration);
                Assert.AreEqual(contentChecker.Percent, args.StatusInPercent);
                Assert.AreEqual(contentChecker.Message, args.Message);
                Assert.AreEqual(contentChecker.Positions, args.VerticesPositions);
            };

            algorithm.Compute();

            Assert.AreEqual(ComputationState.Finished, algorithm.State);
            CollectionAssert.IsEmpty(iterations);
            CollectionAssert.IsEmpty(infoIterations);
        }
    }
}