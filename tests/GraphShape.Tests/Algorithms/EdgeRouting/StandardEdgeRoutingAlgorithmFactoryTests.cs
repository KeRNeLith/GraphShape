using System;
using System.Collections.Generic;
using GraphShape.Algorithms.EdgeRouting;
using GraphShape.Algorithms.Layout;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.EdgeRouting
{
    /// <summary>
    /// Tests for <see cref="StandardEdgeRoutingAlgorithmFactory{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class StandardEdgeRoutingAlgorithmFactoryTests
    {
        #region Tests classes

        private class TestEdgeRoutingAlgorithm : EdgeRoutingAlgorithmBase<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>
        {
            public TestEdgeRoutingAlgorithm(
                [NotNull] BidirectionalGraph<int, Edge<int>> visitedGraph,
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
        public void StandardFactory()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var verticesPositions = new Dictionary<int, Point>();
            var verticesSizes = new Dictionary<int, Size>();
            var context = new LayoutContext<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(
                graph,
                verticesPositions,
                verticesSizes,
                LayoutMode.Simple);

            var factory = new StandardEdgeRoutingAlgorithmFactory<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>();
            CollectionAssert.IsEmpty(factory.AlgorithmTypes);


            Assert.IsNull(factory.CreateAlgorithm(
                string.Empty,
                context,
                null));

            Assert.IsNull(factory.CreateAlgorithm(
                string.Empty,
                context,
                new EdgeRoutingParameters()));

            Assert.IsNull(factory.CreateAlgorithm(
                "TestType",
                context,
                new EdgeRoutingParameters()));


            var parameters = new EdgeRoutingParameters();
            IEdgeRoutingParameters createdParameters = factory.CreateParameters(string.Empty, null);
            Assert.IsInstanceOf<EdgeRoutingParameters>(createdParameters);
            Assert.AreNotSame(parameters, createdParameters);

            createdParameters = factory.CreateParameters(string.Empty, null);
            Assert.IsInstanceOf<EdgeRoutingParameters>(createdParameters);
            Assert.AreNotSame(parameters, createdParameters);

            createdParameters = factory.CreateParameters("TestType", null);
            Assert.IsInstanceOf<EdgeRoutingParameters>(createdParameters);
            Assert.AreNotSame(parameters, createdParameters);

            createdParameters = factory.CreateParameters("TestType", null);
            Assert.IsInstanceOf<EdgeRoutingParameters>(createdParameters);
            Assert.AreNotSame(parameters, createdParameters);


            Assert.IsFalse(factory.IsValidAlgorithm(null));
            Assert.IsFalse(factory.IsValidAlgorithm(string.Empty));
            Assert.IsFalse(factory.IsValidAlgorithm("TestType"));


            var algorithm = new TestEdgeRoutingAlgorithm(graph, verticesPositions);
            Assert.IsEmpty(factory.GetAlgorithmType(algorithm));
        }

        [Test]
        public void StandardFactory_Throws()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var verticesPositions = new Dictionary<int, Point>();
            var verticesSizes = new Dictionary<int, Size>();
            var context = new LayoutContext<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(
                graph,
                verticesPositions,
                verticesSizes,
                LayoutMode.Simple);

            var factory = new StandardEdgeRoutingAlgorithmFactory<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, context, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(string.Empty, null, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => factory.CreateParameters(null, new EdgeRoutingParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateParameters(null, null));

            Assert.Throws<ArgumentNullException>(() => factory.GetAlgorithmType(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}