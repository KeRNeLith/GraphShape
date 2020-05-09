using System;
using System.Collections.Generic;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.Layout.Contextual;
using GraphShape.Utils;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="ContextualLayoutAlgorithmFactory{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class ContextualLayoutAlgorithmFactoryTests
    {
        #region Test classes

        private class TestLayoutParameters : NotifierObject, ILayoutParameters
        {
            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        private class TestLayoutAlgorithm : LayoutAlgorithmBase<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>
        {
            public TestLayoutAlgorithm()
                : base(new BidirectionalGraph<TestVertex, Edge<TestVertex>>())
            {
            }

            protected override void InternalCompute()
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        [Test]
        public void StandardFactory()
        {
            var vertex = new TestVertex("10");
            var positions = new Dictionary<TestVertex, Point>();
            var sizes = new Dictionary<TestVertex, Size>();
            var graph = new BidirectionalGraph<TestVertex, Edge<TestVertex>>();
            graph.AddVertex(vertex);
            var context = new ContextualLayoutContext<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>(
                graph,
                vertex,
                positions,
                sizes);

            var factory = new ContextualLayoutAlgorithmFactory<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>();
            CollectionAssert.AreEqual(
                new[] { "DoubleTree", "BalloonTree" },
                factory.AlgorithmTypes);


            Assert.IsNull(
                factory.CreateAlgorithm(
                    string.Empty,
                    context,
                    new DoubleTreeLayoutParameters()));

            Assert.IsNull(
                factory.CreateAlgorithm(
                    "NotExist",
                    context,
                    new DoubleTreeLayoutParameters()));

            Assert.IsNull(
                factory.CreateAlgorithm(
                    "doubletree",
                    context,
                    new DoubleTreeLayoutParameters()));

            Assert.IsInstanceOf<DoubleTreeLayoutAlgorithm<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>>(
                factory.CreateAlgorithm(
                    "DoubleTree",
                    context,
                    new DoubleTreeLayoutParameters()));

            Assert.IsInstanceOf<BalloonTreeLayoutAlgorithm<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>>(
                factory.CreateAlgorithm(
                    "BalloonTree",
                    context,
                    new BalloonTreeLayoutParameters()));


            var testParameters = new TestLayoutParameters();
            var doubleTreeParameters = new DoubleTreeLayoutParameters();
            ILayoutParameters createdParameters = factory.CreateParameters(string.Empty, doubleTreeParameters);
            Assert.IsNull(createdParameters);

            createdParameters = factory.CreateParameters("NotExist", doubleTreeParameters);
            Assert.IsNull(createdParameters);

            createdParameters = factory.CreateParameters("DoubleTree", null);
            Assert.IsInstanceOf<DoubleTreeLayoutParameters>(createdParameters);
            Assert.AreNotSame(doubleTreeParameters, createdParameters);

            createdParameters = factory.CreateParameters("DoubleTree", testParameters);
            Assert.IsInstanceOf<DoubleTreeLayoutParameters>(createdParameters);
            Assert.AreNotSame(testParameters, createdParameters);

            createdParameters = factory.CreateParameters("DoubleTree", doubleTreeParameters);
            Assert.IsInstanceOf<DoubleTreeLayoutParameters>(createdParameters);
            Assert.AreNotSame(doubleTreeParameters, createdParameters);

            var balloonTreeParameters = new BalloonTreeLayoutParameters();
            createdParameters = factory.CreateParameters("BalloonTree", null);
            Assert.IsInstanceOf<BalloonTreeLayoutParameters>(createdParameters);
            Assert.AreNotSame(balloonTreeParameters, createdParameters);

            createdParameters = factory.CreateParameters("BalloonTree", testParameters);
            Assert.IsInstanceOf<BalloonTreeLayoutParameters>(createdParameters);
            Assert.AreNotSame(testParameters, createdParameters);

            createdParameters = factory.CreateParameters("BalloonTree", balloonTreeParameters);
            Assert.IsInstanceOf<BalloonTreeLayoutParameters>(createdParameters);
            Assert.AreNotSame(balloonTreeParameters, createdParameters);


            Assert.IsFalse(factory.IsValidAlgorithm(null));
            Assert.IsFalse(factory.IsValidAlgorithm(string.Empty));
            Assert.IsTrue(factory.IsValidAlgorithm("DoubleTree"));
            Assert.IsFalse(factory.IsValidAlgorithm("doubletree"));
            Assert.IsTrue(factory.IsValidAlgorithm("BalloonTree"));


            var algorithm1 = new TestLayoutAlgorithm();
            Assert.IsEmpty(factory.GetAlgorithmType(algorithm1));

            var algorithm2 = new DoubleTreeLayoutAlgorithm<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>(graph, positions, sizes, vertex, doubleTreeParameters);
            Assert.AreEqual("DoubleTree", factory.GetAlgorithmType(algorithm2));

            var algorithm3 = new BalloonTreeLayoutAlgorithm<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>(graph, positions, vertex, balloonTreeParameters);
            Assert.AreEqual("BalloonTree", factory.GetAlgorithmType(algorithm3));


            Assert.IsFalse(factory.NeedEdgeRouting(string.Empty));
            Assert.IsTrue(factory.NeedEdgeRouting("DoubleTree"));
            Assert.IsTrue(factory.NeedEdgeRouting("BalloonTree"));


            Assert.IsFalse(factory.NeedOverlapRemoval(string.Empty));
            Assert.IsFalse(factory.NeedOverlapRemoval("DoubleTree"));
            Assert.IsFalse(factory.NeedOverlapRemoval("BalloonTree"));
        }

    [Test]
        public void StandardFactory_Throws()
        {
            var positions = new Dictionary<TestVertex, Point>();
            var sizes = new Dictionary<TestVertex, Size>();
            var graph = new BidirectionalGraph<TestVertex, Edge<TestVertex>>();
            var context = new LayoutContext<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>(
                graph,
                positions,
                sizes,
                LayoutMode.Simple);

            var factory = new ContextualLayoutAlgorithmFactory<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, context, new BalloonTreeLayoutParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, null, new BalloonTreeLayoutParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, context, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(string.Empty, null, new BalloonTreeLayoutParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(string.Empty, null, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, null, null));
            Assert.Throws<ArgumentException>(
                () => factory.CreateAlgorithm(string.Empty, context, new BalloonTreeLayoutParameters()));

            Assert.Throws<ArgumentNullException>(
                () => factory.CreateParameters(null, new BalloonTreeLayoutParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateParameters(null, null));

            Assert.Throws<ArgumentNullException>(() => factory.GetAlgorithmType(null));

            Assert.Throws<ArgumentNullException>(() => factory.NeedEdgeRouting(null));

            Assert.Throws<ArgumentNullException>(() => factory.NeedOverlapRemoval(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}