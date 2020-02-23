using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.GraphTestHelpers;

namespace GraphShape.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="CompoundGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal class CompoundGraphTests
    {
        [Test]
        public void Construction()
        {
            var graph = new CompoundGraph<int, Edge<int>>();
            AssertGraphProperties(graph);

            graph = new CompoundGraph<int, Edge<int>>(true);
            AssertGraphProperties(graph);

            graph = new CompoundGraph<int, Edge<int>>(false);
            AssertGraphProperties(graph, false);

            graph = new CompoundGraph<int, Edge<int>>(true, 12);
            AssertGraphProperties(graph);

            graph = new CompoundGraph<int, Edge<int>>(false, 12);
            AssertGraphProperties(graph, false);


            // From graph
            var otherGraph1 = new AdjacencyGraph<int, Edge<int>>();
            graph = new CompoundGraph<int, Edge<int>>(otherGraph1);
            AssertGraphProperties(graph);

            var otherGraph2 = new AdjacencyGraph<int, Edge<int>>(false);
            graph = new CompoundGraph<int, Edge<int>>(otherGraph2);
            AssertGraphProperties(graph, false);

            var otherNonEmptyGraph1 = new AdjacencyGraph<int, Edge<int>>();
            otherNonEmptyGraph1.AddVertexRange(new[] { 1, 2 });
            graph = new CompoundGraph<int, Edge<int>>(otherNonEmptyGraph1);
            AssertGraphProperties(graph, vertices: new[] { 1, 2 });

            var graphEdges = new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(1, 3),
                new Edge<int>(2, 3)
            };
            var otherNonEmptyGraph2 = new AdjacencyGraph<int, Edge<int>>();
            otherNonEmptyGraph2.AddVerticesAndEdgeRange(graphEdges);
            graph = new CompoundGraph<int, Edge<int>>(otherNonEmptyGraph2);
            AssertGraphProperties(graph, vertices: new[] { 1, 2, 3 }, edges: graphEdges);


            // From CompoundGraph
            var otherCompoundGraph = new CompoundGraph<int, Edge<int>>();
            graph = new CompoundGraph<int, Edge<int>>(otherCompoundGraph);
            AssertGraphProperties(graph);

            var otherNonEmptyCompoundGraph1 = new CompoundGraph<int, Edge<int>>();
            otherNonEmptyCompoundGraph1.AddVerticesAndEdgeRange(graphEdges);
            graph = new CompoundGraph<int, Edge<int>>(otherNonEmptyCompoundGraph1);
            AssertGraphProperties(graph, vertices: new[] { 1, 2, 3 }, edges: graphEdges);

            var otherNonEmptyCompoundGraph2 = new CompoundGraph<int, Edge<int>>();
            otherNonEmptyCompoundGraph2.AddVerticesAndEdgeRange(graphEdges);
            otherNonEmptyCompoundGraph2.AddChildVertex(2, 3);
            graph = new CompoundGraph<int, Edge<int>>(otherNonEmptyCompoundGraph2);
            AssertGraphProperties(graph, vertices: new[] { 1, 2, 3 }, edges: graphEdges, compoundVertices: new[] { 2 });

            var otherNonEmptyCompoundGraph3 = new CompoundGraph<int, Edge<int>>();
            otherNonEmptyCompoundGraph3.AddVerticesAndEdgeRange(graphEdges);
            otherNonEmptyCompoundGraph3.AddChildVertex(1, 2);
            otherNonEmptyCompoundGraph3.AddChildVertex(2, 3);
            graph = new CompoundGraph<int, Edge<int>>(otherNonEmptyCompoundGraph3);
            AssertGraphProperties(graph, vertices: new[] { 1, 2, 3 }, edges: graphEdges, compoundVertices: new[] { 1, 2 });

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                CompoundGraph<TVertex, TEdge> g,
                bool parallelEdges = true,
                IEnumerable<TVertex> vertices = null,
                IEnumerable<TEdge> edges = null,
                IEnumerable<TVertex> compoundVertices = null)
                where TEdge : IEdge<TVertex>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);

                TVertex[] verticesArray = vertices?.ToArray();
                if (vertices is null && edges is null)
                    AssertEmptyGraph(g);
                else if (verticesArray is null)  // && edges != null
                    throw new InvalidOperationException("Cannot have edges without vertices.");
                else // vertices != null
                {
                    AssertHasVertices(g, verticesArray);
                    if (edges is null)
                        AssertNoEdge(g);
                    else
                        AssertHasEdges(g, edges);
                }

                TVertex[] compoundVerticesArray = compoundVertices?.ToArray();
                if (verticesArray is null)
                    CollectionAssert.IsEmpty(g.SimpleVertices);
                else if (compoundVertices is null)
                    CollectionAssert.AreEquivalent(verticesArray, g.SimpleVertices);
                else
                    CollectionAssert.AreEquivalent(verticesArray.Except(compoundVerticesArray), g.SimpleVertices);

                if (compoundVerticesArray is null)
                    CollectionAssert.IsEmpty(g.CompoundVertices);
                else
                    CollectionAssert.AreEquivalent(compoundVerticesArray, g.CompoundVertices);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new CompoundGraph<int, Edge<int>>((IEdgeListGraph<int, Edge<int>>)null));
            Assert.Throws<ArgumentNullException>(() => new CompoundGraph<int, Edge<int>>(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void SimpleAndCompoundVertices()
        {
            var graph = new CompoundGraph<int, Edge<int>>();
            CollectionAssert.IsEmpty(graph.SimpleVertices);
            CollectionAssert.IsEmpty(graph.CompoundVertices);

            graph.AddVertex(1);
            CollectionAssert.AreEquivalent(new[] { 1 }, graph.SimpleVertices);
            CollectionAssert.IsEmpty(graph.CompoundVertices);

            graph.AddVerticesAndEdge(new Edge<int>(2, 3));
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, graph.SimpleVertices);
            CollectionAssert.IsEmpty(graph.CompoundVertices);

            graph.AddChildVertex(1, 4);
            CollectionAssert.AreEquivalent(new[] { 2, 3, 4 }, graph.SimpleVertices);
            CollectionAssert.AreEquivalent(new[] { 1 }, graph.CompoundVertices);

            graph.AddChildVertex(4, 2);
            CollectionAssert.AreEquivalent(new[] { 2, 3 }, graph.SimpleVertices);
            CollectionAssert.AreEquivalent(new[] { 1, 4 }, graph.CompoundVertices);

            graph.AddVertex(5);
            graph.AddChildVertex(5, 3);
            CollectionAssert.AreEquivalent(new[] { 2, 3 }, graph.SimpleVertices);
            CollectionAssert.AreEquivalent(new[] { 1, 4, 5 }, graph.CompoundVertices);

            graph.RemoveVertex(1);
            CollectionAssert.AreEquivalent(new[] { 2, 3 }, graph.SimpleVertices);
            CollectionAssert.AreEquivalent(new[] { 4, 5 }, graph.CompoundVertices);

            graph.RemoveVertex(3);
            CollectionAssert.AreEquivalent(new[] { 2, 5 }, graph.SimpleVertices);
            CollectionAssert.AreEquivalent(new[] { 4 }, graph.CompoundVertices);

            graph.RemoveVertex(2);
            CollectionAssert.AreEquivalent(new[] { 4, 5 }, graph.SimpleVertices);
            CollectionAssert.IsEmpty(graph.CompoundVertices);
        }

        [Test]
        public void AddChildVertex()
        {
            var graph = new CompoundGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 1)
            });
            graph.AddVertexRange(new[] { 5, 6 });

            // 4 child of 2
            Assert.IsFalse(graph.IsChildVertex(4));
            Assert.IsFalse(graph.IsCompoundVertex(2));

            Assert.IsTrue(graph.AddChildVertex(2, 4));

            Assert.IsTrue(graph.IsChildVertex(4));
            Assert.IsTrue(graph.IsCompoundVertex(2));

            // 6 child of 5
            Assert.IsFalse(graph.IsChildVertex(6));
            Assert.IsFalse(graph.IsCompoundVertex(5));

            Assert.IsTrue(graph.AddChildVertex(5, 6));

            Assert.IsTrue(graph.IsChildVertex(6));
            Assert.IsTrue(graph.IsCompoundVertex(5));

            // 7 (not in graph) child of 5
            Assert.IsFalse(graph.ContainsVertex(7));
            Assert.IsTrue(graph.IsCompoundVertex(5));

            Assert.IsTrue(graph.AddChildVertex(5, 7));

            Assert.IsTrue(graph.IsChildVertex(7));
            CollectionAssert.Contains(graph.Vertices, 7);
            Assert.IsTrue(graph.IsCompoundVertex(5));

            // 4 child of 2 (again)
            Assert.IsTrue(graph.IsChildVertex(4));
            Assert.IsTrue(graph.IsCompoundVertex(2));

            Assert.IsTrue(graph.AddChildVertex(2, 4));

            Assert.IsTrue(graph.IsChildVertex(4));
            Assert.IsTrue(graph.IsCompoundVertex(2));
        }

        [Test]
        public void AddChildVertex_Throws()
        {
            var parent = new TestVertex("1");
            var child = new TestVertex("2");
            var graph = new CompoundGraph<TestVertex, Edge<TestVertex>>();

            Assert.Throws<VertexNotFoundException>(() => graph.AddChildVertex(parent, child));

            graph.AddVertex(parent);
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddChildVertex(null, child));
            Assert.Throws<ArgumentNullException>(() => graph.AddChildVertex(parent, null));
            Assert.Throws<ArgumentNullException>(() => graph.AddChildVertex(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void AddChildVertexRange()
        {
            var graph = new CompoundGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 1)
            });
            graph.AddVertexRange(new[] { 5, 6 });

            // 3 & 4 children of 2
            Assert.IsFalse(graph.IsChildVertex(3));
            Assert.IsFalse(graph.IsChildVertex(4));
            Assert.IsFalse(graph.IsCompoundVertex(2));

            Assert.AreEqual(0 /* Already in */, graph.AddChildVertexRange(2, new[] { 3, 4 }));

            Assert.IsTrue(graph.IsChildVertex(3));
            Assert.IsTrue(graph.IsChildVertex(4));
            Assert.IsTrue(graph.IsCompoundVertex(2));

            // 1 & 6 children of 5
            Assert.IsFalse(graph.IsChildVertex(1));
            Assert.IsFalse(graph.IsChildVertex(6));
            Assert.IsFalse(graph.IsCompoundVertex(5));

            Assert.AreEqual(0 /* Already in */, graph.AddChildVertexRange(5, new[] { 1, 6 }));

            Assert.IsTrue(graph.IsChildVertex(1));
            Assert.IsTrue(graph.IsChildVertex(6));
            Assert.IsTrue(graph.IsCompoundVertex(5));

            // 2 & 7 (not in graph) children of 5
            Assert.IsFalse(graph.ContainsVertex(7));
            Assert.IsFalse(graph.IsChildVertex(2));
            Assert.IsTrue(graph.IsCompoundVertex(2));
            Assert.IsTrue(graph.IsCompoundVertex(5));

            Assert.AreEqual(1 /* 7 was added */, graph.AddChildVertexRange(5, new[] { 2, 7 }));

            Assert.IsTrue(graph.IsChildVertex(2));
            Assert.IsTrue(graph.IsChildVertex(7));
            CollectionAssert.Contains(graph.Vertices, 7);
            Assert.IsTrue(graph.IsCompoundVertex(2));
            Assert.IsTrue(graph.IsCompoundVertex(5));

            // 1 & 4 (again) children of 2
            Assert.IsTrue(graph.IsChildVertex(1));  // Child of another vertex also
            Assert.IsTrue(graph.IsChildVertex(4));
            Assert.IsTrue(graph.IsCompoundVertex(2));

            Assert.AreEqual(0 /* Already in */, graph.AddChildVertexRange(2, new[] { 1, 4 }));

            Assert.IsTrue(graph.IsChildVertex(1));
            Assert.IsTrue(graph.IsChildVertex(4));
            Assert.IsTrue(graph.IsCompoundVertex(2));
        }

        [Test]
        public void AddChildVertexRange_Throws()
        {
            var parent = new TestVertex("1");
            var child1 = new TestVertex("2");
            var child2 = new TestVertex("3");
            var graph = new CompoundGraph<TestVertex, Edge<TestVertex>>();

            Assert.Throws<VertexNotFoundException>(() => graph.AddChildVertexRange(parent, new[] { child1, child2 }));

            graph.AddVertex(parent);
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddChildVertexRange(null, new[] { child1, child2 }));
            Assert.Throws<ArgumentNullException>(() => graph.AddChildVertexRange(parent, null));
            Assert.Throws<ArgumentNullException>(() => graph.AddChildVertexRange(null, null));
            Assert.Throws<ArgumentNullException>(() => graph.AddChildVertexRange(parent, new[] { child1, null, child2 }));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void GetParent()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var vertex3 = new TestVertex("3");
            var vertex4 = new TestVertex("4");
            var graph = new CompoundGraph<TestVertex, Edge<TestVertex>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<TestVertex>(vertex1, vertex2),
                new Edge<TestVertex>(vertex2, vertex2),
                new Edge<TestVertex>(vertex2, vertex3),
                new Edge<TestVertex>(vertex3, vertex4),
                new Edge<TestVertex>(vertex4, vertex1)
            });

            Assert.IsNull(graph.GetParent(vertex1));
            Assert.IsNull(graph.GetParent(vertex2));
            Assert.IsNull(graph.GetParent(vertex3));
            Assert.IsNull(graph.GetParent(vertex4));

            graph.AddChildVertex(vertex1, vertex2);

            Assert.IsNull(graph.GetParent(vertex1));
            Assert.AreSame(vertex1, graph.GetParent(vertex2));
            Assert.IsNull(graph.GetParent(vertex3));
            Assert.IsNull(graph.GetParent(vertex4));

            graph.AddChildVertex(vertex1, vertex4);

            Assert.IsNull(graph.GetParent(vertex1));
            Assert.AreSame(vertex1, graph.GetParent(vertex2));
            Assert.IsNull(graph.GetParent(vertex3));
            Assert.AreSame(vertex1, graph.GetParent(vertex4));

            graph.AddChildVertex(vertex3, vertex1);

            Assert.AreSame(vertex3, graph.GetParent(vertex1));
            Assert.AreSame(vertex1, graph.GetParent(vertex2));
            Assert.IsNull(graph.GetParent(vertex3));
            Assert.AreSame(vertex1, graph.GetParent(vertex4));
        }

        [Test]
        public void GetParent_Throws()
        {
            var vertex = new TestVertex("1");
            var graph = new CompoundGraph<TestVertex, Edge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.GetParent(vertex));
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.GetParent(null));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void IsChildVertex()
        {
            var graph = new CompoundGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 1)
            });

            Assert.IsFalse(graph.IsChildVertex(1));
            Assert.IsFalse(graph.IsChildVertex(2));
            Assert.IsFalse(graph.IsChildVertex(3));
            Assert.IsFalse(graph.IsChildVertex(4));

            graph.AddChildVertex(1, 2);

            Assert.IsFalse(graph.IsChildVertex(1));
            Assert.IsTrue(graph.IsChildVertex(2));
            Assert.IsFalse(graph.IsChildVertex(3));
            Assert.IsFalse(graph.IsChildVertex(4));

            graph.AddChildVertex(1, 4);

            Assert.IsFalse(graph.IsChildVertex(1));
            Assert.IsTrue(graph.IsChildVertex(2));
            Assert.IsFalse(graph.IsChildVertex(3));
            Assert.IsTrue(graph.IsChildVertex(4));

            graph.AddChildVertex(3, 1);

            Assert.IsTrue(graph.IsChildVertex(1));
            Assert.IsTrue(graph.IsChildVertex(2));
            Assert.IsFalse(graph.IsChildVertex(3));
            Assert.IsTrue(graph.IsChildVertex(4));
        }

        [Test]
        public void IsChildVertex_Throws()
        {
            var vertex = new TestVertex("1");
            var graph = new CompoundGraph<TestVertex, Edge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.IsChildVertex(vertex));
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.IsChildVertex(null));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void GetChildrenVertices()
        {
            var graph = new CompoundGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 1)
            });
            graph.AddVertexRange(new[] { 5, 6 });

            CheckChildren(1);
            CheckChildren(2);
            CheckChildren(3);
            CheckChildren(4);
            CheckChildren(5);
            CheckChildren(6);

            // 4 child of 2
            graph.AddChildVertex(2, 4);

            CheckChildren(1);
            CheckChildren(2, new[] { 4 });
            CheckChildren(3);
            CheckChildren(4);
            CheckChildren(5);
            CheckChildren(6);

            // 6 child of 5
            graph.AddChildVertex(5, 6);

            CheckChildren(1);
            CheckChildren(2, new[] { 4 });
            CheckChildren(3);
            CheckChildren(4);
            CheckChildren(5, new[] { 6 });
            CheckChildren(6);

            // 7 (not in graph) child of 5
            graph.AddChildVertex(5, 7);

            CheckChildren(1);
            CheckChildren(2, new[] { 4 });
            CheckChildren(3);
            CheckChildren(4);
            CheckChildren(5, new[] { 6, 7 });
            CheckChildren(6);

            // 4 child of 2 (again)
            graph.AddChildVertex(2, 4);

            CheckChildren(1);
            CheckChildren(2, new[] { 4, 4 });
            CheckChildren(3);
            CheckChildren(4);
            CheckChildren(5, new[] { 6, 7 });
            CheckChildren(6);

            #region Local function

            void CheckChildren(int vertex, int[] expectedChildren = null)
            {
                if (expectedChildren is null || expectedChildren.Length == 0)
                {
                    CollectionAssert.IsEmpty(graph.GetChildrenVertices(vertex));
                    Assert.AreEqual(0, graph.GetChildrenCount(vertex));
                }
                else
                {
                    CollectionAssert.AreEquivalent(expectedChildren, graph.GetChildrenVertices(vertex));
                    Assert.AreEqual(expectedChildren.Length, graph.GetChildrenCount(vertex));
                }
            }

            #endregion
        }

        [Test]
        public void GetChildrenVertices_Throws()
        {
            var vertex = new TestVertex("1");
            var graph = new CompoundGraph<TestVertex, Edge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.GetChildrenVertices(vertex));
            Assert.Throws<VertexNotFoundException>(() => graph.GetChildrenCount(vertex));
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.GetChildrenVertices(null));
            Assert.Throws<ArgumentNullException>(() => graph.GetChildrenCount(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void IsCompoundVertex()
        {
            var graph = new CompoundGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 1)
            });

            Assert.IsFalse(graph.IsCompoundVertex(1));
            Assert.IsFalse(graph.IsCompoundVertex(2));
            Assert.IsFalse(graph.IsCompoundVertex(3));
            Assert.IsFalse(graph.IsCompoundVertex(4));

            graph.AddChildVertex(1, 2);

            Assert.IsTrue(graph.IsCompoundVertex(1));
            Assert.IsFalse(graph.IsCompoundVertex(2));
            Assert.IsFalse(graph.IsCompoundVertex(3));
            Assert.IsFalse(graph.IsCompoundVertex(4));

            graph.AddChildVertex(1, 4);

            Assert.IsTrue(graph.IsCompoundVertex(1));
            Assert.IsFalse(graph.IsCompoundVertex(2));
            Assert.IsFalse(graph.IsCompoundVertex(3));
            Assert.IsFalse(graph.IsCompoundVertex(4));

            graph.AddChildVertex(3, 1);

            Assert.IsTrue(graph.IsCompoundVertex(1));
            Assert.IsFalse(graph.IsCompoundVertex(2));
            Assert.IsTrue(graph.IsCompoundVertex(3));
            Assert.IsFalse(graph.IsCompoundVertex(4));
        }

        [Test]
        public void IsCompoundVertex_Throws()
        {
            var vertex = new TestVertex("1");
            var graph = new CompoundGraph<TestVertex, Edge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.IsCompoundVertex(vertex));
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.IsCompoundVertex(null));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void RemoveVertex()
        {
            var graph = new CompoundGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(2, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 4),
                new Edge<int>(4, 1)
            });
            graph.AddVertexRange(new[] { 5, 6, 7 });
            graph.AddChildVertex(2, 4);
            graph.AddChildVertex(5, 6);
            graph.AddChildVertex(5, 7);

            AssertHasVertices(graph, new[] { 1, 2, 3, 4, 5, 6, 7 });

            graph.RemoveVertex(10);
            AssertHasVertices(graph, new[] { 1, 2, 3, 4, 5, 6, 7 });

            graph.RemoveVertex(1);
            AssertHasVertices(graph, new[] { 2, 3, 4, 5, 6, 7 });

            graph.RemoveVertex(5);
            AssertHasVertices(graph, new[] { 2, 3, 4, 6, 7 });

            graph.RemoveVertex(4);
            AssertHasVertices(graph, new[] { 2, 3, 6, 7 });

            graph.RemoveVertex(6);
            AssertHasVertices(graph, new[] { 2, 3, 7 });

            foreach (int vertex in graph.Vertices.ToArray())
            {
                graph.RemoveVertex(vertex);
            }
            AssertEmptyGraph(graph);
        }

        [Test]
        public void RemoveVertex_Throws()
        {
            var graph = new CompoundGraph<TestVertex, Edge<TestVertex>>();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveVertex(null));
        }
    }
}