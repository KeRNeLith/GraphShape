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
    /// Tests for <see cref="HighlightAlgorithmBase{TVertex,TEdge,TGraph,TParameters}"/>.
    /// </summary>
    [TestFixture]
    internal class HighlightAlgorithmTests
    {
        #region Tests classes

        private class TestHighlightParameters : HighlightParameters
        {
            public int TestParam { [UsedImplicitly] get; set; }

            private int _raiseTestParam;

            public int RaiseTestParam
            {
                [UsedImplicitly] get => _raiseTestParam;
                set
                {
                    _raiseTestParam = value;
                    OnPropertyChanged();
                }
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

            private int _resetCount;

            public void AssertResetCount(int count)
            {
                Assert.AreEqual(count, _resetCount);
                _resetCount = 0;
            }

            /// <inheritdoc />
            public override void ResetHighlight()
            {
                ++_resetCount;
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
        public void Constructor()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var controller = new TestHighlightController(graph);
            var algorithm = new TestHighlightAlgorithm(controller, null);
            AssertAlgorithmProperties(algorithm, controller);

            var parameters = new HighlightParameters();
            algorithm = new TestHighlightAlgorithm(controller, parameters);
            AssertAlgorithmProperties(algorithm, controller, parameters);

            #region Local function

            void AssertAlgorithmProperties(
                TestHighlightAlgorithm algo,
                IHighlightController<int, Edge<int>, BidirectionalGraph<int, Edge<int>>> c,
                HighlightParameters p = null)
            {
                Assert.AreSame(c, algo.Controller);
                if (p is null)
                {
                    Assert.IsNull(((IHighlightAlgorithm<int, Edge<int>>)algo).Parameters);
                    Assert.IsNull(algo.Parameters);
                }
                else
                {
                    Assert.AreSame(p, ((IHighlightAlgorithm<int, Edge<int>>)algo).Parameters);
                    Assert.AreSame(p, algo.Parameters);
                }
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var parameters = new HighlightParameters();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new TestHighlightAlgorithm(null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new TestHighlightAlgorithm(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void ParameterChanged()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var controller = new TestHighlightController(graph);
            var algorithm = new TestHighlightAlgorithm(controller, null);

            algorithm.AssertResetCount(0);

            var parametersSet1 = new TestHighlightParameters();
            algorithm.TrySetParameters(parametersSet1);

            algorithm.AssertResetCount(0);

            parametersSet1.TestParam = 12;
            algorithm.AssertResetCount(0);

            parametersSet1.RaiseTestParam = 42;
            algorithm.AssertResetCount(1);

            var parametersSet2 = new TestHighlightParameters();
            algorithm.TrySetParameters(parametersSet2);

            algorithm.AssertResetCount(0);

            parametersSet2.TestParam = 25;
            algorithm.AssertResetCount(0);

            parametersSet2.RaiseTestParam = 50;
            algorithm.AssertResetCount(1);
        }
    }
}