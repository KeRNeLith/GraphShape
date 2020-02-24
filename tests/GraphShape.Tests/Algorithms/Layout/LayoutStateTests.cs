using System;
using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="LayoutState{TVertex, TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal class LayoutStateTests
    {
        [Test]
        public void Constructor()
        {
            var verticesPositions = new Dictionary<int, Point>();
            var overlapVerticesPositions = new Dictionary<int, Point>();
            var routeInfos = new Dictionary<Edge<int>, Point[]>();

            var state = new LayoutState<int, Edge<int>>(
                verticesPositions,
                null,
                null,
                TimeSpan.FromSeconds(10),
                1,
                string.Empty);
            CheckState(state, 1, string.Empty, TimeSpan.FromSeconds(10), verticesPositions, null, null);

            state.OverlapRemovedPositions = overlapVerticesPositions;
            state.RouteInfos = routeInfos;
            CheckState(state, 1, string.Empty, TimeSpan.FromSeconds(10), verticesPositions, overlapVerticesPositions, routeInfos);

            state = new LayoutState<int, Edge<int>>(
                verticesPositions,
                overlapVerticesPositions,
                null,
                TimeSpan.FromSeconds(11),
                12,
                "Test message");
            CheckState(state, 12, "Test message", TimeSpan.FromSeconds(11), verticesPositions, overlapVerticesPositions, null);

            state = new LayoutState<int, Edge<int>>(
                verticesPositions,
                null,
                routeInfos,
                TimeSpan.FromSeconds(5),
                15,
                "Test iteration");
            CheckState(state, 15, "Test iteration", TimeSpan.FromSeconds(5), verticesPositions, null, routeInfos);

            #region Local function

            void CheckState<TVertex, TEdge>(
                LayoutState<TVertex, TEdge> s,
                int iteration,
                string message,
                TimeSpan computeTime,
                IDictionary<TVertex, Point> positions,
                IDictionary<TVertex, Point> overlapPositions,
                IDictionary<TEdge, Point[]> routes)
                where TEdge : IEdge<TVertex>
            {
                Assert.AreSame(verticesPositions, s.Positions);
                Assert.AreSame(overlapPositions ?? positions, s.OverlapRemovedPositions);
                if (routes is null)
                    Assert.IsNotNull(s.RouteInfos);
                else
                    Assert.AreSame(routeInfos, s.RouteInfos);
                Assert.AreEqual(computeTime, s.ComputationTime);
                Assert.AreEqual(iteration, s.Iteration);
                Assert.AreEqual(message, s.Message);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var verticesPositions = new Dictionary<int, Point>();

            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutState<int, Edge<int>>(verticesPositions, null, null, TimeSpan.FromSeconds(10), -1, string.Empty));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LayoutState<int, Edge<int>>(verticesPositions, null, null, TimeSpan.FromSeconds(10), -10, string.Empty));

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new LayoutState<int, Edge<int>>(null, null, null, TimeSpan.FromSeconds(10), 1, string.Empty));
            Assert.Throws<ArgumentNullException>(
                () => new LayoutState<int, Edge<int>>(verticesPositions, null, null, TimeSpan.FromSeconds(10), 1, null));
            Assert.Throws<ArgumentNullException>(
                () => new LayoutState<int, Edge<int>>(null, null, null, TimeSpan.FromSeconds(10), 1, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}