using System;
using System.Collections.Generic;
using GraphShape.Algorithms.Layout;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="LayoutIterationEventArgs{TVertex, TEdge}"/>
    /// and <see cref="LayoutIterationEventArgs{TVertex, TEdge, TVertexInfo, TEdgeInfo}"/>.
    /// </summary>
    [TestFixture]
    internal class LayoutIterationEventArgsTests
    {
        #region Test helpers

        private static void CheckArgs<TVertex, TEdge>(
            [NotNull] ILayoutInfoIterationEventArgs<TVertex, TEdge> args,
            double status,
            bool isAborted,
            int iteration,
            [NotNull] string message,
            [CanBeNull] IDictionary<TVertex, Point> positions)
            where TEdge : IEdge<TVertex>
        {
            Assert.AreEqual(status, args.StatusInPercent);
            Assert.AreEqual(isAborted, args.Abort);
            Assert.AreEqual(iteration, args.Iteration);
            Assert.AreEqual(message, args.Message);
            Assert.AreSame(positions, args.VerticesPositions);
        }

        private static void CheckArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>(
            [NotNull] ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo> args,
            double status,
            bool isAborted,
            int iteration,
            [NotNull] string message,
            [CanBeNull] IDictionary<TVertex, Point> positions,
            [CanBeNull] IDictionary<TVertex, TVertexInfo> verticesInfos,
            [CanBeNull] IDictionary<TEdge, TEdgeInfo> edgeInfos)
            where TEdge : IEdge<TVertex>
        {
            CheckArgs(args, status, isAborted, iteration, message, positions);
            Assert.AreSame(verticesInfos, args.VerticesInfos);
            Assert.AreSame(edgeInfos, args.EdgesInfos);
        }

        #endregion

        [Test]
        public void Constructor1()
        {
            var verticesPositions = new Dictionary<int, Point>();
            
            var args = new LayoutIterationEventArgs<int, Edge<int>>();
            CheckArgs(args, 0, false, 0, string.Empty, null);

            args.Abort = true;
            CheckArgs(args, 0, true, 0, string.Empty, null);

            args = new LayoutIterationEventArgs<int, Edge<int>>(12, 55.4);
            CheckArgs(args, 55.4, false, 12, string.Empty, null);

            args = new LayoutIterationEventArgs<int, Edge<int>>(42, 15.6, "Test message");
            CheckArgs(args, 15.6, false, 42, "Test message", null);

            args = new LayoutIterationEventArgs<int, Edge<int>>(1, 1.6, verticesPositions);
            CheckArgs(args, 1.6, false, 1, string.Empty, verticesPositions);

            args = new LayoutIterationEventArgs<int, Edge<int>>(1, 1.6, (IDictionary<int, Point>)null);
            CheckArgs(args, 1.6, false, 1, string.Empty, null);

            args = new LayoutIterationEventArgs<int, Edge<int>>(1, 1.6, "Test iteration", null);
            CheckArgs(args, 1.6, false, 1, "Test iteration", null);

            args = new LayoutIterationEventArgs<int, Edge<int>>(1, 1.6, "Test iteration", verticesPositions);
            CheckArgs(args, 1.6, false, 1, "Test iteration", verticesPositions);
        }

        [Test]
        public void Constructor1_Throws()
        {
            var verticesPositions = new Dictionary<int, Point>();

            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(-1, 11.0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(0, -1.0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(-1, -10.0));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(-1, 11.0, "Test message"));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(0, -1.0, "Test message"));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(-1, -10.0, "Test message"));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(-1, 11.0, verticesPositions));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(0, -1.0, verticesPositions));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(-1, -10.0, verticesPositions));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(-1, 11.0, "Test message", verticesPositions));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(0, -1.0, "Test message", verticesPositions));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(-1, -10.0, "Test message", verticesPositions));

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(12, 55.4, (string)null));
            Assert.Throws<ArgumentNullException>(
                () => new LayoutIterationEventArgs<int, Edge<int>>(12, 55.4, null, verticesPositions));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Constructor2()
        {
            var verticesPositions = new Dictionary<int, Point>();
            var verticesInfos = new Dictionary<int, int>();
            var edgeInfos = new Dictionary<Edge<int>, double>();

            var args = new LayoutIterationEventArgs<int, Edge<int>, int, double>();
            CheckArgs(args, 0, false, 0, string.Empty, null, null, null);

            args.Abort = true;
            CheckArgs(args, 0, true, 0, string.Empty, null, null, null);

            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(12, 55.4);
            CheckArgs(args, 55.4, false, 12, string.Empty, null, null, null);

            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(42, 15.6, "Test message");
            CheckArgs(args, 15.6, false, 42, "Test message", null, null, null);

            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(1, 1.6, verticesPositions);
            CheckArgs(args, 1.6, false, 1, string.Empty, verticesPositions, null, null);

            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(1, 1.6, (IDictionary<int, Point>)null);
            CheckArgs(args, 1.6, false, 1, string.Empty, null, null, null);

            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(1, 1.6, "Test iteration", null, null, null);
            CheckArgs(args, 1.6, false, 1, "Test iteration", null, null, null);

            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(1, 1.6, "Test iteration", verticesPositions, null, null);
            CheckArgs(args, 1.6, false, 1, "Test iteration", verticesPositions, null, null);

            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(1, 1.6, "Test iteration", null, verticesInfos, null);
            CheckArgs(args, 1.6, false, 1, "Test iteration", null, verticesInfos, null);

            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(1, 1.6, "Test iteration", null, null, edgeInfos);
            CheckArgs(args, 1.6, false, 1, "Test iteration", null, null, edgeInfos);
        }

        [Test]
        public void Constructor2_Throws()
        {
            var verticesPositions = new Dictionary<int, Point>();
            var verticesInfos = new Dictionary<int, int>();
            var edgeInfos = new Dictionary<Edge<int>, double>();

            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(-1, 11.0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(0, -1.0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(-1, -10.0));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(-1, 11.0, "Test message"));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(0, -1.0, "Test message"));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(-1, -10.0, "Test message"));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(-1, 11.0, verticesPositions));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(0, -1.0, verticesPositions));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(-1, -10.0, verticesPositions));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(-1, 11.0, "Test message", verticesPositions, verticesInfos, edgeInfos));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(0, -1.0, "Test message", verticesPositions, verticesInfos, edgeInfos));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(-1, -10.0, "Test message", verticesPositions, verticesInfos, edgeInfos));

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(12, 55.4, (string)null));
            Assert.Throws<ArgumentNullException>(
                () => new LayoutIterationEventArgs<int, Edge<int>, int, double>(12, 55.4, null, verticesPositions, verticesInfos, edgeInfos));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void GetVertexInfo1()
        {
            var args = new LayoutIterationEventArgs<int, Edge<int>>();
            Assert.IsNull(args.GetVertexInfo(1));
        }

        [Test]
        public void GetVertexInfo1_Throws()
        {
            var args = new LayoutIterationEventArgs<TestVertex, Edge<TestVertex>>();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => args.GetVertexInfo(null));
        }

        [Test]
        public void GetVertexInfo2()
        {
            var args = new LayoutIterationEventArgs<int, Edge<int>, int, double>();
            Assert.IsNull(args.GetVertexInfo(1));

            var verticesInfos = new Dictionary<int, int>
            {
                [2] = 12
            };
            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(1, 1.0, string.Empty, null, verticesInfos, null);
            Assert.IsNull(args.GetVertexInfo(1));
            Assert.AreEqual(12, args.GetVertexInfo(2));
        }

        [Test]
        public void GetVertexInfo2_Throws()
        {
            var args = new LayoutIterationEventArgs<TestVertex, Edge<TestVertex>, int, double>();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => args.GetVertexInfo(null));
        }

        [Test]
        public void GetEdgeInfo1()
        {
            var args = new LayoutIterationEventArgs<int, Edge<int>>();
            Assert.IsNull(args.GetEdgeInfo(new Edge<int>(1, 2)));
        }

        [Test]
        public void GetEdgeInfo1_Throws()
        {
            var args = new LayoutIterationEventArgs<TestVertex, Edge<TestVertex>>();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => args.GetEdgeInfo(null));
        }

        [Test]
        public void GetEdgeInfo2()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge21 = new Edge<int>(2, 1);

            var args = new LayoutIterationEventArgs<int, Edge<int>, int, double>();
            Assert.IsNull(args.GetEdgeInfo(edge12));

            var edgeInfos = new Dictionary<Edge<int>, double>
            {
                [edge21] = 42.2
            };
            args = new LayoutIterationEventArgs<int, Edge<int>, int, double>(1, 1.0, string.Empty, null, null, edgeInfos);
            Assert.IsNull(args.GetEdgeInfo(edge12));
            Assert.AreEqual(42.2, args.GetEdgeInfo(edge21));
        }

        [Test]
        public void GetEdgeInfo2_Throws()
        {
            var args = new LayoutIterationEventArgs<TestVertex, Edge<TestVertex>, int, double>();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => args.GetEdgeInfo(null));
        }
    }
}