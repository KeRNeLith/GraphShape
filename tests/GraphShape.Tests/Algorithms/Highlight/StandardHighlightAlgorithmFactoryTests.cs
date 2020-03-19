using System;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.Highlight;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.Highlight
{
    /// <summary>
    /// Tests for <see cref="StandardHighlightAlgorithmFactory{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class StandardHighLightAlgorithmFactoryTests
    {
        #region Tests classes

        private class TestHighlightContext : HighlightContext<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>
        {
            public TestHighlightContext([NotNull] BidirectionalGraph<int, Edge<int>> graph)
                : base(graph)
            {
            }
        }

        private class TestHighlightController : IHighlightController<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>
        {
            public TestHighlightController([NotNull] BidirectionalGraph<int, Edge<int>> graph)
            {
                Graph = graph;
            }

            /// <inheritdoc />
            public BidirectionalGraph<int, Edge<int>> Graph { get; }

            /// <inheritdoc />
            public IEnumerable<int> HighlightedVertices => Enumerable.Empty<int>();

            /// <inheritdoc />
            public IEnumerable<int> SemiHighlightedVertices => Enumerable.Empty<int>();

            /// <inheritdoc />
            public IEnumerable<Edge<int>> HighlightedEdges => Enumerable.Empty<Edge<int>>();

            /// <inheritdoc />
            public IEnumerable<Edge<int>> SemiHighlightedEdges => Enumerable.Empty<Edge<int>>();

            /// <inheritdoc />
            public bool IsHighlightedVertex(int vertex)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public bool IsHighlightedVertex(int vertex, out object highlightInfo)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public bool IsSemiHighlightedVertex(int vertex)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public bool IsSemiHighlightedVertex(int vertex, out object semiHighlightInfo)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public bool IsHighlightedEdge(Edge<int> edge)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public bool IsHighlightedEdge(Edge<int> edge, out object highlightInfo)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public bool IsSemiHighlightedEdge(Edge<int> edge)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public bool IsSemiHighlightedEdge(Edge<int> edge, out object semiHighlightInfo)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void HighlightVertex(int vertex, object highlightInfo)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void SemiHighlightVertex(int vertex, object semiHighlightInfo)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void HighlightEdge(Edge<int> edge, object highlightInfo)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void SemiHighlightEdge(Edge<int> edge, object semiHighlightInfo)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void RemoveHighlightFromVertex(int vertex)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void RemoveSemiHighlightFromVertex(int vertex)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void RemoveHighlightFromEdge(Edge<int> edge)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void RemoveSemiHighlightFromEdge(Edge<int> edge)
            {
                throw new NotImplementedException();
            }
        }

        private class TestHighlightAlgorithm : HighlightAlgorithmBase<int, Edge<int>, BidirectionalGraph<int, Edge<int>>, HighlightParameters>
        {
            public TestHighlightAlgorithm(
                [NotNull] TestHighlightController controller,
                [CanBeNull] IHighlightParameters parameters)
                : base(controller, parameters)
            {
            }

            /// <inheritdoc />
            public override void ResetHighlight()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public override bool OnVertexHighlighting(int vertex)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public override bool OnVertexHighlightRemoving(int vertex)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public override bool OnEdgeHighlighting(Edge<int> edge)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public override bool OnEdgeHighlightRemoving(Edge<int> edge)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        [Test]
        public void StandardFactory()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var context = new TestHighlightContext(graph);
            var controller = new TestHighlightController(graph);

            var factory = new StandardHighlightAlgorithmFactory<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>();
            CollectionAssert.AreEqual(new[] { "Simple" }, factory.HighlightModes);


            Assert.IsNull(
                factory.CreateAlgorithm(
                    string.Empty,
                    context,
                    controller,
                    new HighlightParameters()));

            Assert.IsInstanceOf<SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>>(
                factory.CreateAlgorithm(
                    "Simple",
                    context,
                    controller,
                    new HighlightParameters()));

            Assert.IsInstanceOf<SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>>(
                factory.CreateAlgorithm(
                    "Simple",
                    context,
                    controller,
                    null));


            var parameters = new HighlightParameters();
            IHighlightParameters createdParameters = factory.CreateParameters(string.Empty, parameters);
            Assert.IsInstanceOf<HighlightParameters>(createdParameters);
            Assert.AreNotSame(parameters, createdParameters);

            createdParameters = factory.CreateParameters("Simple", parameters);
            Assert.IsInstanceOf<HighlightParameters>(createdParameters);
            Assert.AreNotSame(parameters, createdParameters);

            createdParameters = factory.CreateParameters("Simple", null);
            Assert.IsInstanceOf<HighlightParameters>(createdParameters);
            Assert.AreNotSame(parameters, createdParameters);

            createdParameters = factory.CreateParameters("NotExist", null);
            Assert.IsNull(createdParameters);


            Assert.IsFalse(factory.IsValidMode(null));
            Assert.IsTrue(factory.IsValidMode(string.Empty));
            Assert.IsTrue(factory.IsValidMode("Simple"));
            Assert.IsFalse(factory.IsValidMode("simple"));


            var algorithm1 = new TestHighlightAlgorithm(controller, new HighlightParameters());
            Assert.IsNull(factory.GetHighlightMode(algorithm1));

            var algorithm2 = new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(controller, new HighlightParameters());
            Assert.AreEqual("Simple", factory.GetHighlightMode(algorithm2));
        }

        [Test]
        public void StandardFactory_Throws()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var context = new TestHighlightContext(graph);
            var controller = new TestHighlightController(graph);

            var factory = new StandardHighlightAlgorithmFactory<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, context, controller, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(string.Empty, null, controller, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(string.Empty, context, null, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, null, controller, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, context, null, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(string.Empty, null, null, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, null, null, null));

            Assert.Throws<ArgumentNullException>(
                () => factory.CreateParameters(null, new HighlightParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateParameters(null, null));

            Assert.Throws<ArgumentNullException>(() => factory.GetHighlightMode(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}