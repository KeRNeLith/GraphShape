using System;
using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.EdgeRouting;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.Algorithms.AlgorithmTestHelpers;

namespace GraphShape.Tests.Algorithms.EdgeRouting
{
    /// <summary>
    /// Tests for <see cref="EdgeRoutingAlgorithmBase{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class EdgeRoutingAlgorithmTests
    {
        #region Tests classes

        private class TestEdgeRoutingAlgorithm : EdgeRoutingAlgorithmBase<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>
        {
            public TestEdgeRoutingAlgorithm(
                [NotNull] AdjacencyGraph<int, Edge<int>> visitedGraph,
                [NotNull] IDictionary<int, Point> verticesPositions)
                : base(visitedGraph, verticesPositions)
            {
            }

            /// <inheritdoc />
            protected override void InternalCompute()
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            var verticesPositions = new Dictionary<int, Point>();
            var algorithm = new TestEdgeRoutingAlgorithm(graph, verticesPositions);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties(
                TestEdgeRoutingAlgorithm algo,
                AdjacencyGraph<int, Edge<int>> g)
            {
                AssertAlgorithmState(algo, g);
                Assert.IsNotNull(algo.EdgeRoutes);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            var verticesPositions = new Dictionary<int, Point>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new TestEdgeRoutingAlgorithm(graph, null));
            Assert.Throws<ArgumentNullException>(
                () => new TestEdgeRoutingAlgorithm(null, verticesPositions));
            Assert.Throws<ArgumentNullException>(
                () => new TestEdgeRoutingAlgorithm(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}