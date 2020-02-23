using System;
using System.Collections.Generic;
using GraphShape.Algorithms.Highlight;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.Highlight
{
    /// <summary>
    /// Tests for <see cref="SimpleHighlightAlgorithm{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class SimpleHighLightAlgorithmTests
    {
        #region Tests classes

        private class TestHighlightController<TVertex> : IHighlightController<TVertex, Edge<TVertex>, BidirectionalGraph<TVertex, Edge<TVertex>>>
        {
            [NotNull]
            private readonly IDictionary<TVertex, object> _highlightedVertices = new Dictionary<TVertex, object>();

            [NotNull]
            private readonly IDictionary<TVertex, object> _semiHighlightedVertices = new Dictionary<TVertex, object>();

            [NotNull]
            private readonly IDictionary<Edge<TVertex>, object> _highlightedEdges = new Dictionary<Edge<TVertex>, object>();

            [NotNull]
            private readonly IDictionary<Edge<TVertex>, object> _semiHighlightedEdges = new Dictionary<Edge<TVertex>, object>();

            public TestHighlightController([NotNull] BidirectionalGraph<TVertex, Edge<TVertex>> graph)
            {
                Graph = graph;
            }

            /// <inheritdoc />
            public BidirectionalGraph<TVertex, Edge<TVertex>> Graph { get; }

            /// <inheritdoc />
            public IEnumerable<TVertex> HighlightedVertices => _highlightedVertices.Keys;

            /// <inheritdoc />
            public IEnumerable<TVertex> SemiHighlightedVertices => _semiHighlightedVertices.Keys;

            /// <inheritdoc />
            public IEnumerable<Edge<TVertex>> HighlightedEdges => _highlightedEdges.Keys;

            /// <inheritdoc />
            public IEnumerable<Edge<TVertex>> SemiHighlightedEdges => _semiHighlightedEdges.Keys;

            /// <inheritdoc />
            public bool IsHighlightedVertex(TVertex vertex)
            {
                return _highlightedVertices.ContainsKey(vertex);
            }

            /// <inheritdoc />
            public bool IsHighlightedVertex(TVertex vertex, out object highlightInfo)
            {
                return _highlightedVertices.TryGetValue(vertex, out highlightInfo);
            }

            /// <inheritdoc />
            public bool IsSemiHighlightedVertex(TVertex vertex)
            {
                return _semiHighlightedVertices.ContainsKey(vertex);
            }

            /// <inheritdoc />
            public bool IsSemiHighlightedVertex(TVertex vertex, out object semiHighlightInfo)
            {
                return _semiHighlightedVertices.TryGetValue(vertex, out semiHighlightInfo);
            }

            /// <inheritdoc />
            public bool IsHighlightedEdge(Edge<TVertex> edge)
            {
                return _highlightedEdges.ContainsKey(edge);
            }

            /// <inheritdoc />
            public bool IsHighlightedEdge(Edge<TVertex> edge, out object highlightInfo)
            {
                return _highlightedEdges.TryGetValue(edge, out highlightInfo);
            }

            /// <inheritdoc />
            public bool IsSemiHighlightedEdge(Edge<TVertex> edge)
            {
                return _semiHighlightedEdges.ContainsKey(edge);
            }

            /// <inheritdoc />
            public bool IsSemiHighlightedEdge(Edge<TVertex> edge, out object semiHighlightInfo)
            {
                return _semiHighlightedEdges.TryGetValue(edge, out semiHighlightInfo);
            }

            /// <inheritdoc />
            public void HighlightVertex(TVertex vertex, object highlightInfo)
            {
                _highlightedVertices[vertex] = highlightInfo;
            }

            /// <inheritdoc />
            public void SemiHighlightVertex(TVertex vertex, object semiHighlightInfo)
            {
                _semiHighlightedVertices[vertex] = semiHighlightInfo;
            }

            /// <inheritdoc />
            public void HighlightEdge(Edge<TVertex> edge, object highlightInfo)
            {
                _highlightedEdges[edge] = highlightInfo;
            }

            /// <inheritdoc />
            public void SemiHighlightEdge(Edge<TVertex> edge, object semiHighlightInfo)
            {
                _semiHighlightedEdges[edge] = semiHighlightInfo;
            }

            /// <inheritdoc />
            public void RemoveHighlightFromVertex(TVertex vertex)
            {
                _highlightedVertices.Remove(vertex);
            }

            /// <inheritdoc />
            public void RemoveSemiHighlightFromVertex(TVertex vertex)
            {
                _semiHighlightedVertices.Remove(vertex);
            }

            /// <inheritdoc />
            public void RemoveHighlightFromEdge(Edge<TVertex> edge)
            {
                _highlightedEdges.Remove(edge);
            }

            /// <inheritdoc />
            public void RemoveSemiHighlightFromEdge(Edge<TVertex> edge)
            {
                _semiHighlightedEdges.Remove(edge);
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var controller = new TestHighlightController<int>(graph);
            var algorithm = new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(controller, null);
            AssertAlgorithmProperties(algorithm, controller);

            var parameters = new HighlightParameters();
            algorithm = new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(controller, parameters);
            AssertAlgorithmProperties(algorithm, controller, parameters);

            #region Local function

            void AssertAlgorithmProperties(
                SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>> algo,
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
                () => new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(null, parameters));
            Assert.Throws<ArgumentNullException>(
                () => new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void OnVertexHighlighting()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var controller = new TestHighlightController<int>(graph);
            var algorithm = new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(controller, null);

            // Not in graph
            Assert.IsFalse(algorithm.OnVertexHighlighting(1));

            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge24 = new Edge<int>(2, 4);
            var edge25 = new Edge<int>(2, 5);
            var edge33 = new Edge<int>(3, 3);
            var edge35 = new Edge<int>(3, 5);
            var edge45 = new Edge<int>(4, 5);
            var edge64 = new Edge<int>(6, 4);
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge24, edge25,
                edge33, edge35, edge45, edge64
            });

            Assert.IsTrue(algorithm.OnVertexHighlighting(2));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsTrue(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge64));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge64));


            Assert.IsTrue(algorithm.OnVertexHighlighting(3));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsTrue(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge64));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge64));


            Assert.IsTrue(algorithm.OnVertexHighlighting(1));

            Assert.IsTrue(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge64));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge64));
        }

        [Test]
        public void OnVertexHighlightRemoving()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var controller = new TestHighlightController<int>(graph);
            var algorithm = new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(controller, null);

            // Not in graph
            Assert.IsFalse(algorithm.OnVertexHighlightRemoving(1));

            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge24 = new Edge<int>(2, 4);
            var edge25 = new Edge<int>(2, 5);
            var edge33 = new Edge<int>(3, 3);
            var edge35 = new Edge<int>(3, 5);
            var edge45 = new Edge<int>(4, 5);
            var edge64 = new Edge<int>(6, 4);
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge24, edge25,
                edge33, edge35, edge45, edge64
            });

            Assert.IsTrue(algorithm.OnVertexHighlightRemoving(2));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge64));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge64));


            algorithm.OnVertexHighlighting(3);
            Assert.IsTrue(algorithm.OnVertexHighlightRemoving(3));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge64));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge64));


            algorithm.OnVertexHighlighting(1);
            Assert.IsTrue(algorithm.OnVertexHighlightRemoving(1));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge64));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge64));
        }

        [Test]
        public void OnEdgeHighlighting()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var controller = new TestHighlightController<int>(graph);
            var algorithm = new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(controller, null);

            var edge12 = new Edge<int>(1, 2);

            // Not in graph
            Assert.IsFalse(algorithm.OnEdgeHighlighting(edge12));

            var edge13 = new Edge<int>(1, 3);
            var edge24 = new Edge<int>(2, 4);
            var edge33 = new Edge<int>(3, 3);
            var edge35 = new Edge<int>(3, 5);
            var edge45 = new Edge<int>(4, 5);
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge24,
                edge33, edge35, edge45
            });

            Assert.IsTrue(algorithm.OnEdgeHighlighting(edge24));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsTrue(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge45));


            Assert.IsTrue(algorithm.OnEdgeHighlighting(edge33));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsTrue(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge45));


            Assert.IsTrue(algorithm.OnEdgeHighlighting(edge45));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsTrue(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
            Assert.IsTrue(algorithm.Controller.IsHighlightedEdge(edge45));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge45));
        }

        [Test]
        public void OnEdgeHighlightRemoving()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var controller = new TestHighlightController<int>(graph);
            var algorithm = new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(controller, null);

            var edge12 = new Edge<int>(1, 2);

            // Not in graph
            Assert.IsFalse(algorithm.OnEdgeHighlightRemoving(edge12));

            var edge13 = new Edge<int>(1, 3);
            var edge24 = new Edge<int>(2, 4);
            var edge33 = new Edge<int>(3, 3);
            var edge35 = new Edge<int>(3, 5);
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge24, edge33, edge35
            });

            Assert.IsTrue(algorithm.OnEdgeHighlightRemoving(edge24));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));


            algorithm.OnEdgeHighlighting(edge33);
            Assert.IsTrue(algorithm.OnEdgeHighlightRemoving(edge33));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));


            algorithm.OnEdgeHighlighting(edge24);
            Assert.IsTrue(algorithm.OnEdgeHighlightRemoving(edge24));

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(6));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(6));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
        }

        [Test]
        public void ResetHighlight()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge23 = new Edge<int>(2, 3);
            var edge24 = new Edge<int>(2, 4);
            var edge25 = new Edge<int>(2, 5);
            var edge33 = new Edge<int>(3, 3);
            var edge35 = new Edge<int>(3, 5);
            var graph = new BidirectionalGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge23, edge24,
                edge25, edge33, edge35
            });

            var controller = new TestHighlightController<int>(graph);
            var algorithm = new SimpleHighlightAlgorithm<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(controller, null);

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge23));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge23));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));


            algorithm.ResetHighlight();

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge23));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge23));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));


            algorithm.OnVertexHighlighting(3);
            algorithm.ResetHighlight();

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge23));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge23));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));


            algorithm.OnEdgeHighlighting(edge35);
            algorithm.ResetHighlight();

            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(1));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(2));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(3));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(4));
            Assert.IsFalse(algorithm.Controller.IsHighlightedVertex(5));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedVertex(5));

            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge12));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge13));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge23));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge23));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge24));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge25));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge33));
            Assert.IsFalse(algorithm.Controller.IsHighlightedEdge(edge35));
            Assert.IsFalse(algorithm.Controller.IsSemiHighlightedEdge(edge35));
        }

        [Test]
        public void Highlight_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, Edge<TestVertex>>();
            var controller = new TestHighlightController<TestVertex>(graph);
            var algorithm = new SimpleHighlightAlgorithm<TestVertex, Edge<TestVertex>, BidirectionalGraph<TestVertex, Edge<TestVertex>>>(controller, null);

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.OnVertexHighlighting(null));
            Assert.Throws<ArgumentNullException>(() => algorithm.OnVertexHighlightRemoving(null));
            Assert.Throws<ArgumentNullException>(() => algorithm.OnEdgeHighlighting(null));
            Assert.Throws<ArgumentNullException>(() => algorithm.OnEdgeHighlightRemoving(null));
            // ReSharper restore AssignNullToNotNullAttribute
        }
    }
}