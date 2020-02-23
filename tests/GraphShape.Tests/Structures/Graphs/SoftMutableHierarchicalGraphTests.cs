using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.GraphTestHelpers;

namespace GraphShape.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="SoftMutableHierarchicalGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal class SoftMutableHierarchicalGraphTests
    {
        [Test]
        public void Construction()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            AssertGraphProperties(graph);

            graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>(true);
            AssertGraphProperties(graph);

            graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>(false);
            AssertGraphProperties(graph, false);

            graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>(true, 12);
            AssertGraphProperties(graph);

            graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>(false, 12);
            AssertGraphProperties(graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                SoftMutableHierarchicalGraph<TVertex, TEdge> g,
                bool parallelEdges = true)
                where TEdge : TypedEdge<TVertex>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                AssertEmptyGraph(g);
                CollectionAssert.IsEmpty(g.HiddenVertices);
                Assert.AreEqual(0, g.HiddenVertexCount);
                CollectionAssert.IsEmpty(g.HiddenEdges);
                Assert.AreEqual(0, g.HiddenEdgeCount);
                CollectionAssert.IsEmpty(g.GeneralEdges);
                Assert.AreEqual(0, g.GeneralEdgeCount);
                CollectionAssert.IsEmpty(g.GeneralEdges);
                Assert.AreEqual(0, g.HierarchicalEdgeCount);
                CollectionAssert.IsEmpty(g.HierarchicalEdges);
            }

            #endregion
        }

        #region Add/Remove vertices

        [Test]
        public void AddVertex()
        {
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            int vertexAdded = 0;

            AssertNoVertex(graph);
            graph.VertexAdded += v =>
            {
                Assert.IsNotNull(v);
                ++vertexAdded;
            };

            // Vertex 1
            var vertex1 = new TestVertex("1");
            Assert.IsTrue(graph.AddVertex(vertex1));
            Assert.AreEqual(1, vertexAdded);
            AssertHasVertices(graph, new[] { vertex1 });

            // Vertex 2
            var vertex2 = new TestVertex("2");
            Assert.IsTrue(graph.AddVertex(vertex2));
            Assert.AreEqual(2, vertexAdded);
            AssertHasVertices(graph, new[] { vertex1, vertex2 });

            // Vertex 1 bis
            Assert.IsFalse(graph.AddVertex(vertex1));
            Assert.AreEqual(2, vertexAdded);
            AssertHasVertices(graph, new[] { vertex1, vertex2 });

            // Other "Vertex 1"
            var otherVertex1 = new TestVertex("1");
            Assert.IsTrue(graph.AddVertex(otherVertex1));
            Assert.AreEqual(3, vertexAdded);
            AssertHasVertices(graph, new[] { vertex1, vertex2, otherVertex1 });
        }

        [Test]
        public void AddVertex_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddVertex(null));
            AssertNoVertex(graph);
        }

        [Test]
        public void RemoveVertex()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();

            int verticesRemoved = 0;
            int edgesRemoved = 0;

            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                // ReSharper disable once AccessToModifiedClosure
                ++verticesRemoved;
            };
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge24 = new TypedEdge<int>(2, 4, EdgeTypes.Hierarchical);
            var edge31 = new TypedEdge<int>(3, 1, EdgeTypes.General);
            var edge33 = new TypedEdge<int>(3, 3, EdgeTypes.General);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge24, edge31, edge33, edge41
            });

            Assert.IsFalse(graph.RemoveVertex(5));
            CheckCounters(0, 0);

            Assert.IsTrue(graph.RemoveVertex(1));
            CheckCounters(1, 5);
            AssertHasVertices(graph, new[] { 2, 3, 4 });
            AssertHasEdges(graph, new[] { edge22, edge24, edge33 });

            Assert.IsTrue(graph.RemoveVertex(3));
            CheckCounters(1, 1);
            AssertHasVertices(graph, new[] { 2, 4 });
            AssertHasEdges(graph, new[] { edge22, edge24 });

            Assert.IsTrue(graph.RemoveVertex(2));
            CheckCounters(1, 2);
            AssertHasVertices(graph, new[] { 4 });
            AssertNoEdge(graph);

            Assert.IsTrue(graph.RemoveVertex(4));
            CheckCounters(1, 0);
            AssertEmptyGraph(graph);

            #region Local function

            void CheckCounters(int expectedRemovedVertices, int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedVertices, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                verticesRemoved = 0;
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void RemoveVertex_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveVertex(null));
        }

        #endregion

        #region Add/Remove edges

        [Test]
        public void AddEdge()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>(false);

            int edgeAdded = 0;

            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            AssertNoEdge(graph);
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            AssertHasEdges(graph, new[] { edge1 });

            // Edge 2
            var edge2 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            Assert.IsTrue(graph.AddEdge(edge2));
            Assert.AreEqual(2, edgeAdded);
            AssertHasEdges(graph, new[] { edge1, edge2 });

            // Edge 3
            var edge3 = new TypedEdge<int>(2, 1, EdgeTypes.General);
            Assert.IsTrue(graph.AddEdge(edge3));
            Assert.AreEqual(3, edgeAdded);
            AssertHasEdges(graph, new[] { edge1, edge2, edge3 });

            // Edge 1 bis
            Assert.IsFalse(graph.AddEdge(edge1));
            Assert.AreEqual(3, edgeAdded);
            AssertHasEdges(graph, new[] { edge1, edge2, edge3 });

            // Edge 4 self edge
            var edge4 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            Assert.IsTrue(graph.AddEdge(edge4));
            Assert.AreEqual(4, edgeAdded);
            AssertHasEdges(graph, new[] { edge1, edge2, edge3, edge4 });
        }

        [Test]
        public void AddEdge_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddEdge(null));
            AssertNoVertex(graph);
        }

        [Test]
        public void RemoveEdge()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();

            int verticesRemoved = 0;
            int edgesRemoved = 0;

            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                ++verticesRemoved;
            };
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge13Bis = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge24 = new TypedEdge<int>(2, 4, EdgeTypes.General);
            var edge31 = new TypedEdge<int>(3, 1, EdgeTypes.General);
            var edge33 = new TypedEdge<int>(3, 3, EdgeTypes.General);
            var edgeNotInGraph = new TypedEdge<int>(3, 4, EdgeTypes.Hierarchical);
            var edgeWithVertexNotInGraph1 = new TypedEdge<int>(2, 10, EdgeTypes.Hierarchical);
            var edgeWithVertexNotInGraph2 = new TypedEdge<int>(10, 2, EdgeTypes.Hierarchical);
            var edgeWithVerticesNotInGraph = new TypedEdge<int>(10, 11, EdgeTypes.General);
            var edgeNotEquatable = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            graph.AddVerticesAndEdgeRange(new[] { edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 });

            Assert.IsFalse(graph.RemoveEdge(edgeNotInGraph));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph1));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph2));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVerticesNotInGraph));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeNotEquatable));
            CheckCounters(0);

            Assert.IsTrue(graph.RemoveEdge(edge13Bis));
            CheckCounters(1);
            AssertHasVertices(graph, new[] { 1, 2, 3, 4 });
            AssertHasEdges(graph, new[] { edge12, edge13, edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge31));
            CheckCounters(1);
            AssertHasVertices(graph, new[] { 1, 2, 3, 4 });
            AssertHasEdges(graph, new[] { edge12, edge13, edge14, edge24, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge12));
            Assert.IsTrue(graph.RemoveEdge(edge13));
            Assert.IsTrue(graph.RemoveEdge(edge14));
            Assert.IsTrue(graph.RemoveEdge(edge24));
            Assert.IsTrue(graph.RemoveEdge(edge33));
            CheckCounters(5);
            AssertHasVertices(graph, new[] { 1, 2, 3, 4 });
            AssertNoEdge(graph);

            #region Local function

            void CheckCounters(int expectedRemovedEdges)
            {
                Assert.AreEqual(0, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void RemoveEdge_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveEdge(null));
        }

        #endregion

        #region Hierarchical & General edges

        [Test]
        public void HierarchicalAndGeneralEdges()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge33 = new TypedEdge<int>(2, 2, EdgeTypes.General);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVertexRange(new[] { 1, 2, 3, 4 });

            CollectionAssert.IsEmpty(graph.HierarchicalEdges);
            Assert.AreEqual(0, graph.HierarchicalEdgeCount);
            CollectionAssert.IsEmpty(graph.GeneralEdges);
            Assert.AreEqual(0, graph.GeneralEdgeCount);

            // Add edges
            graph.AddEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HierarchicalEdges);
            Assert.AreEqual(1, graph.HierarchicalEdgeCount);
            CollectionAssert.IsEmpty(graph.GeneralEdges);
            Assert.AreEqual(0, graph.GeneralEdgeCount);

            graph.AddEdge(edge13);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HierarchicalEdges);
            Assert.AreEqual(1, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge13 }, graph.GeneralEdges);
            Assert.AreEqual(1, graph.GeneralEdgeCount);

            graph.AddEdge(edge14);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.HierarchicalEdges);
            Assert.AreEqual(2, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge13 }, graph.GeneralEdges);
            Assert.AreEqual(1, graph.GeneralEdgeCount);

            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22 }, graph.HierarchicalEdges);
            Assert.AreEqual(3, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge13 }, graph.GeneralEdges);
            Assert.AreEqual(1, graph.GeneralEdgeCount);

            graph.AddEdge(edge33);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22 }, graph.HierarchicalEdges);
            Assert.AreEqual(3, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge13, edge33 }, graph.GeneralEdges);
            Assert.AreEqual(2, graph.GeneralEdgeCount);

            graph.AddEdge(edge41);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41 }, graph.HierarchicalEdges);
            Assert.AreEqual(4, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge13, edge33 }, graph.GeneralEdges);
            Assert.AreEqual(2, graph.GeneralEdgeCount);

            graph.AddEdge(edge42);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41 }, graph.HierarchicalEdges);
            Assert.AreEqual(4, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge13, edge33, edge42 }, graph.GeneralEdges);
            Assert.AreEqual(3, graph.GeneralEdgeCount);

            // Added twice
            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41, edge22 }, graph.HierarchicalEdges);
            Assert.AreEqual(5, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge13, edge33, edge42 }, graph.GeneralEdges);
            Assert.AreEqual(3, graph.GeneralEdgeCount);

            // Remove edges
            graph.RemoveEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41 }, graph.HierarchicalEdges);
            Assert.AreEqual(4, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge13, edge33, edge42 }, graph.GeneralEdges);
            Assert.AreEqual(3, graph.GeneralEdgeCount);

            graph.RemoveEdge(edge13);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41 }, graph.HierarchicalEdges);
            Assert.AreEqual(4, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge33, edge42 }, graph.GeneralEdges);
            Assert.AreEqual(2, graph.GeneralEdgeCount);

            // Remove vertex
            graph.RemoveVertex(4);
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.HierarchicalEdges);
            Assert.AreEqual(2, graph.HierarchicalEdgeCount);
            CollectionAssert.AreEquivalent(new[] { edge33 }, graph.GeneralEdges);
            Assert.AreEqual(1, graph.GeneralEdgeCount);
        }

        #endregion

        #region Hierarchical edges

        [Test]
        public void HierarchicalEdgesFor()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVertexRange(new[] { 1, 2, 3, 4 });

            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(4));

            // Add edges
            graph.AddEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(1, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(1, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(4));

            graph.AddEdge(edge13);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(1, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(1, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(4));

            graph.AddEdge(edge14);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(1, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(1, graph.HierarchicalEdgeCountFor(4));

            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(1, graph.HierarchicalEdgeCountFor(4));

            graph.AddEdge(edge41);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge41 }, graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(3, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(4));

            graph.AddEdge(edge42);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge41 }, graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(3, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(4));

            // Added twice
            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge41 }, graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(3, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge22 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(3, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(4));

            // Remove edges
            graph.RemoveEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge41 }, graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(3, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(4));

            graph.RemoveEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(1, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.HierarchicalEdgesFor(4));
            Assert.AreEqual(2, graph.HierarchicalEdgeCountFor(4));

            // Remove vertex
            graph.RemoveVertex(4);
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(1));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.HierarchicalEdgesFor(2));
            Assert.AreEqual(1, graph.HierarchicalEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.HierarchicalEdgesFor(3));
            Assert.AreEqual(0, graph.HierarchicalEdgeCountFor(3));
        }

        [Test]
        public void HierarchicalEdgesFor_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.HierarchicalEdgesFor(vertex));
            Assert.Throws<VertexNotFoundException>(() => graph.HierarchicalEdgeCountFor(vertex));
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HierarchicalEdgesFor(null));
            Assert.Throws<ArgumentNullException>(() => graph.HierarchicalEdgeCountFor(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void InHierarchicalEdges()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVertexRange(new[] { 1, 2, 3, 4 });

            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(1));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(2));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(4));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(4));

            // Add edges
            graph.AddEdge(edge12);
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(1));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(4));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(4));

            graph.AddEdge(edge13);
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(1));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(4));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(4));

            graph.AddEdge(edge14);
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(1));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InHierarchicalEdges(4));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(4));

            graph.AddEdge(edge22);
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(1));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(2, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InHierarchicalEdges(4));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(4));

            graph.AddEdge(edge41);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InHierarchicalEdges(1));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(2, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InHierarchicalEdges(4));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(4));

            graph.AddEdge(edge42);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InHierarchicalEdges(1));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(2, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InHierarchicalEdges(4));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(4));

            // Added twice
            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InHierarchicalEdges(1));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge22 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(3, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InHierarchicalEdges(4));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(4));

            // Remove edges
            graph.RemoveEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InHierarchicalEdges(1));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(2, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InHierarchicalEdges(4));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(4));

            graph.RemoveEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InHierarchicalEdges(1));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InHierarchicalEdges(4));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(4));

            // Remove vertex
            graph.RemoveVertex(4);
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(1));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.InHierarchicalEdges(2));
            Assert.AreEqual(1, graph.InHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InHierarchicalEdges(3));
            Assert.AreEqual(0, graph.InHierarchicalEdgeCount(3));
        }

        [Test]
        public void InHierarchicalEdges_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.InHierarchicalEdges(vertex));
            Assert.Throws<VertexNotFoundException>(() => graph.InHierarchicalEdgeCount(vertex));
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.InHierarchicalEdges(null));
            Assert.Throws<ArgumentNullException>(() => graph.InHierarchicalEdgeCount(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void OutHierarchicalEdges()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVertexRange(new[] { 1, 2, 3, 4 });

            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(1));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(2));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(4));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(4));

            // Add edges
            graph.AddEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.OutHierarchicalEdges(1));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(2));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(4));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(4));

            graph.AddEdge(edge13);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.OutHierarchicalEdges(1));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(2));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(4));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(4));

            graph.AddEdge(edge14);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutHierarchicalEdges(1));
            Assert.AreEqual(2, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(2));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(4));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(4));

            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutHierarchicalEdges(1));
            Assert.AreEqual(2, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutHierarchicalEdges(2));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(4));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(4));

            graph.AddEdge(edge41);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutHierarchicalEdges(1));
            Assert.AreEqual(2, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutHierarchicalEdges(2));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutHierarchicalEdges(4));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(4));

            graph.AddEdge(edge42);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutHierarchicalEdges(1));
            Assert.AreEqual(2, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutHierarchicalEdges(2));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutHierarchicalEdges(4));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(4));

            // Added twice
            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutHierarchicalEdges(1));
            Assert.AreEqual(2, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22, edge22 }, graph.OutHierarchicalEdges(2));
            Assert.AreEqual(2, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutHierarchicalEdges(4));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(4));

            // Remove edges
            graph.RemoveEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutHierarchicalEdges(1));
            Assert.AreEqual(2, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutHierarchicalEdges(2));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutHierarchicalEdges(4));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(4));

            graph.RemoveEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.OutHierarchicalEdges(1));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutHierarchicalEdges(2));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutHierarchicalEdges(4));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(4));

            // Remove vertex
            graph.RemoveVertex(4);
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(1));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutHierarchicalEdges(2));
            Assert.AreEqual(1, graph.OutHierarchicalEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutHierarchicalEdges(3));
            Assert.AreEqual(0, graph.OutHierarchicalEdgeCount(3));
        }

        [Test]
        public void OutHierarchicalEdges_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.OutHierarchicalEdges(vertex));
            Assert.Throws<VertexNotFoundException>(() => graph.OutHierarchicalEdgeCount(vertex));
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.OutHierarchicalEdges(null));
            Assert.Throws<ArgumentNullException>(() => graph.OutHierarchicalEdgeCount(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region General edges

        [Test]
        public void GeneralEdgesFor()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.General);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVertexRange(new[] { 1, 2, 3, 4 });

            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(1));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(1));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(2));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(4));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(4));

            // Add edges
            graph.AddEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.GeneralEdgesFor(1));
            Assert.AreEqual(1, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(1, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(4));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(4));

            graph.AddEdge(edge13);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.GeneralEdgesFor(1));
            Assert.AreEqual(1, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(1, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(4));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(4));

            graph.AddEdge(edge14);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.GeneralEdgesFor(1));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(1, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.GeneralEdgesFor(4));
            Assert.AreEqual(1, graph.GeneralEdgeCountFor(4));

            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.GeneralEdgesFor(1));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.GeneralEdgesFor(4));
            Assert.AreEqual(1, graph.GeneralEdgeCountFor(4));

            graph.AddEdge(edge41);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge41 }, graph.GeneralEdgesFor(1));
            Assert.AreEqual(3, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.GeneralEdgesFor(4));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(4));

            graph.AddEdge(edge42);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge41 }, graph.GeneralEdgesFor(1));
            Assert.AreEqual(3, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.GeneralEdgesFor(4));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(4));

            // Added twice
            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge41 }, graph.GeneralEdgesFor(1));
            Assert.AreEqual(3, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge22 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(3, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.GeneralEdgesFor(4));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(4));

            // Remove edges
            graph.RemoveEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge41 }, graph.GeneralEdgesFor(1));
            Assert.AreEqual(3, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.GeneralEdgesFor(4));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(4));

            graph.RemoveEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.GeneralEdgesFor(1));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(1, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41 }, graph.GeneralEdgesFor(4));
            Assert.AreEqual(2, graph.GeneralEdgeCountFor(4));

            // Remove vertex
            graph.RemoveVertex(4);
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(1));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.GeneralEdgesFor(2));
            Assert.AreEqual(1, graph.GeneralEdgeCountFor(2));
            CollectionAssert.IsEmpty(graph.GeneralEdgesFor(3));
            Assert.AreEqual(0, graph.GeneralEdgeCountFor(3));
        }

        [Test]
        public void GeneralEdgesFor_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.GeneralEdgesFor(vertex));
            Assert.Throws<VertexNotFoundException>(() => graph.GeneralEdgeCountFor(vertex));
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.GeneralEdgesFor(null));
            Assert.Throws<ArgumentNullException>(() => graph.GeneralEdgeCountFor(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void InGeneralEdges()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.General);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVertexRange(new[] { 1, 2, 3, 4 });

            CollectionAssert.IsEmpty(graph.InGeneralEdges(1));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(1));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(2));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(4));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(4));

            graph.AddEdge(edge12);
            CollectionAssert.IsEmpty(graph.InGeneralEdges(1));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.InGeneralEdges(2));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(4));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(4));

            graph.AddEdge(edge13);
            CollectionAssert.IsEmpty(graph.InGeneralEdges(1));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.InGeneralEdges(2));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(4));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(4));

            graph.AddEdge(edge14);
            CollectionAssert.IsEmpty(graph.InGeneralEdges(1));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.InGeneralEdges(2));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InGeneralEdges(4));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(4));

            graph.AddEdge(edge22);
            CollectionAssert.IsEmpty(graph.InGeneralEdges(1));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.InGeneralEdges(2));
            Assert.AreEqual(2, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InGeneralEdges(4));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(4));

            graph.AddEdge(edge41);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InGeneralEdges(1));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.InGeneralEdges(2));
            Assert.AreEqual(2, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InGeneralEdges(4));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(4));

            graph.AddEdge(edge42);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InGeneralEdges(1));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.InGeneralEdges(2));
            Assert.AreEqual(2, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InGeneralEdges(4));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(4));

            // Added twice
            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InGeneralEdges(1));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge22 }, graph.InGeneralEdges(2));
            Assert.AreEqual(3, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InGeneralEdges(4));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(4));

            // Remove edges
            graph.RemoveEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InGeneralEdges(1));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22 }, graph.InGeneralEdges(2));
            Assert.AreEqual(2, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InGeneralEdges(4));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(4));

            graph.RemoveEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.InGeneralEdges(1));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.InGeneralEdges(2));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.InGeneralEdges(4));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(4));

            // Remove vertex
            graph.RemoveVertex(4);
            CollectionAssert.IsEmpty(graph.InGeneralEdges(1));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.InGeneralEdges(2));
            Assert.AreEqual(1, graph.InGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.InGeneralEdges(3));
            Assert.AreEqual(0, graph.InGeneralEdgeCount(3));
        }

        [Test]
        public void InGeneralEdges_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.InGeneralEdges(vertex));
            Assert.Throws<VertexNotFoundException>(() => graph.InGeneralEdgeCount(vertex));
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.InGeneralEdges(null));
            Assert.Throws<ArgumentNullException>(() => graph.InGeneralEdgeCount(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void OutGeneralEdges()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.General);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVertexRange(new[] { 1, 2, 3, 4 });

            CollectionAssert.IsEmpty(graph.OutGeneralEdges(1));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(1));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(2));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(4));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(4));

            // Add edges
            graph.AddEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.OutGeneralEdges(1));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(1));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(2));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(4));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(4));

            graph.AddEdge(edge13);
            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.OutGeneralEdges(1));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(1));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(2));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(4));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(4));

            graph.AddEdge(edge14);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutGeneralEdges(1));
            Assert.AreEqual(2, graph.OutGeneralEdgeCount(1));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(2));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(4));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(4));

            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutGeneralEdges(1));
            Assert.AreEqual(2, graph.OutGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutGeneralEdges(2));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(4));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(4));

            graph.AddEdge(edge41);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutGeneralEdges(1));
            Assert.AreEqual(2, graph.OutGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutGeneralEdges(2));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutGeneralEdges(4));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(4));

            graph.AddEdge(edge42);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutGeneralEdges(1));
            Assert.AreEqual(2, graph.OutGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutGeneralEdges(2));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutGeneralEdges(4));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(4));

            // Added twice
            graph.AddEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutGeneralEdges(1));
            Assert.AreEqual(2, graph.OutGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22, edge22 }, graph.OutGeneralEdges(2));
            Assert.AreEqual(2, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutGeneralEdges(4));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(4));

            // Remove edges
            graph.RemoveEdge(edge22);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.OutGeneralEdges(1));
            Assert.AreEqual(2, graph.OutGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutGeneralEdges(2));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutGeneralEdges(4));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(4));

            graph.RemoveEdge(edge12);
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.OutGeneralEdges(1));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutGeneralEdges(2));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
            CollectionAssert.AreEquivalent(new[] { edge41 }, graph.OutGeneralEdges(4));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(4));

            // Remove vertex
            graph.RemoveVertex(4);
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(1));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(1));
            CollectionAssert.AreEquivalent(new[] { edge22 }, graph.OutGeneralEdges(2));
            Assert.AreEqual(1, graph.OutGeneralEdgeCount(2));
            CollectionAssert.IsEmpty(graph.OutGeneralEdges(3));
            Assert.AreEqual(0, graph.OutGeneralEdgeCount(3));
        }

        [Test]
        public void OutGeneralEdges_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.OutGeneralEdges(vertex));
            Assert.Throws<VertexNotFoundException>(() => graph.OutGeneralEdgeCount(vertex));
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.OutGeneralEdges(null));
            Assert.Throws<ArgumentNullException>(() => graph.OutGeneralEdgeCount(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region Vertex hide/unhide

        [Test]
        public void HideVertex()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });
            graph.AddVertex(5);

            var hiddenVertices = new Stack<int>(new[] { 3, 5, 4, 2 });
            var hiddenEdges = new Stack<Edge<int>>(new[] { edge13, edge41, edge14, edge42, edge22, edge12 });
            graph.VertexHidden += vertex => Assert.AreEqual(hiddenVertices.Pop(), vertex);
            graph.VertexUnhidden += vertex => Assert.Fail("Unhidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.AreSame(hiddenEdges.Pop(), edge);
            graph.EdgeUnhidden += edge => Assert.Fail("Unhidden edge event must not be called.");

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Vertices, 2);
            Assert.IsTrue(graph.HideVertex(2));
            CollectionAssert.DoesNotContain(graph.Vertices, 2);
            CollectionAssert.AreEquivalent(new[] { 2 }, graph.HiddenVertices);
            Assert.AreEqual(1, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(3, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Vertices, 4);
            Assert.IsTrue(graph.HideVertex(4));
            CollectionAssert.DoesNotContain(graph.Vertices, 4);
            CollectionAssert.AreEquivalent(new[] { 2, 4 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            Assert.IsFalse(graph.HideVertex(2));    // Already hidden
            CollectionAssert.AreEquivalent(new[] { 2, 4 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            Assert.IsFalse(graph.HideVertex(2, "TestTag"));    // Already hidden
            CollectionAssert.AreEquivalent(new[] { 2, 4 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Vertices, 5);
            Assert.IsTrue(graph.HideVertex(5));
            CollectionAssert.DoesNotContain(graph.Vertices, 5);
            CollectionAssert.AreEquivalent(new[] { 2, 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Vertices, 3);
            Assert.IsTrue(graph.HideVertex(3, "TestTag"));
            CollectionAssert.DoesNotContain(graph.Vertices, 3);
            CollectionAssert.AreEquivalent(new[] { 2, 3, 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(4, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(hiddenVertices);
            CollectionAssert.IsEmpty(hiddenEdges);
        }

        [Test]
        public void HideVertex_Throws()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();
            graph.AddVertex(vertex1);

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HideVertex(null));
            Assert.Throws<VertexNotFoundException>(() => graph.HideVertex(vertex2));

            Assert.Throws<ArgumentNullException>(() => graph.HideVertex(null, "TestTag"));
            Assert.Throws<VertexNotFoundException>(() => graph.HideVertex(vertex2, "TestTag"));

            Assert.Throws<ArgumentNullException>(() => graph.HideVertex(vertex1, null));

            Assert.Throws<ArgumentNullException>(() => graph.HideVertex(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void HideVertices()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);
            var edge45 = new TypedEdge<int>(4, 5, EdgeTypes.General);
            var edge56 = new TypedEdge<int>(5, 6, EdgeTypes.General);
            var edge57 = new TypedEdge<int>(5, 7, EdgeTypes.Hierarchical);
            var edge75 = new TypedEdge<int>(7, 5, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41,
                edge42, edge45, edge56, edge57, edge75
            });
            graph.AddVertex(8);

            var hiddenVertices = new Stack<int>(new[] { 3, 1, 8, 5, 4, 2 });
            var hiddenEdges = new Stack<Edge<int>>(new[] { edge13, edge57, edge56, edge75, edge45, edge41, edge14, edge42, edge22, edge12 });
            graph.VertexHidden += vertex => Assert.AreEqual(hiddenVertices.Pop(), vertex);
            graph.VertexUnhidden += vertex => Assert.Fail("Unhidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.AreSame(hiddenEdges.Pop(), edge);
            graph.EdgeUnhidden += edge => Assert.Fail("Unhidden edge event must not be called.");

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Vertices, 2);
            CollectionAssert.Contains(graph.Vertices, 4);
            graph.HideVertices(new[] { 2, 4 });
            CollectionAssert.DoesNotContain(graph.Vertices, 2);
            CollectionAssert.DoesNotContain(graph.Vertices, 4);
            CollectionAssert.AreEquivalent(new[] { 2, 4 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41, edge42, edge45 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Vertices, 5);
            CollectionAssert.Contains(graph.Vertices, 8);
            graph.HideVertices(new[] { 5, 8 });
            CollectionAssert.DoesNotContain(graph.Vertices, 5);
            CollectionAssert.DoesNotContain(graph.Vertices, 8);
            CollectionAssert.AreEquivalent(new[] { 2, 4, 5, 8 }, graph.HiddenVertices);
            Assert.AreEqual(4, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(
                new[] { edge12, edge14, edge22, edge41, edge42, edge45, edge56, edge57, edge75 },
                graph.HiddenEdges);
            Assert.AreEqual(9, graph.HiddenEdgeCount);

            graph.HideVertices(new[] { 2, 4 });    // Already hidden
            CollectionAssert.AreEquivalent(new[] { 2, 4, 5, 8 }, graph.HiddenVertices);
            Assert.AreEqual(4, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(
                new[] { edge12, edge14, edge22, edge41, edge42, edge45, edge56, edge57, edge75 },
                graph.HiddenEdges);
            Assert.AreEqual(9, graph.HiddenEdgeCount);

            graph.HideVertices(new[] { 2, 4 }, "TestTag");    // Already hidden
            CollectionAssert.AreEquivalent(new[] { 2, 4, 5, 8 }, graph.HiddenVertices);
            Assert.AreEqual(4, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(
                new[] { edge12, edge14, edge22, edge41, edge42, edge45, edge56, edge57, edge75 },
                graph.HiddenEdges);
            Assert.AreEqual(9, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Vertices, 1);
            CollectionAssert.Contains(graph.Vertices, 3);
            graph.HideVertices(new[] { 1, 3 }, "TestTag");
            CollectionAssert.DoesNotContain(graph.Vertices, 1);
            CollectionAssert.DoesNotContain(graph.Vertices, 3);
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5, 8 }, graph.HiddenVertices);
            Assert.AreEqual(6, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(
                new[] { edge12, edge13, edge14, edge22, edge41, edge42, edge45, edge56, edge57, edge75 },
                graph.HiddenEdges);
            Assert.AreEqual(10, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(hiddenVertices);
            CollectionAssert.IsEmpty(hiddenEdges);
        }

        [Test]
        public void HideVertices_Throws()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var vertex3 = new TestVertex("3");
            var vertex4 = new TestVertex("4");
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();
            graph.AddVertexRange(new[] { vertex1, vertex2, vertex3 });

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HideVertices(null));
            Assert.Throws<ArgumentNullException>(() => graph.HideVertices(new[] { vertex1, null }));
            Assert.Throws<VertexNotFoundException>(() => graph.HideVertices(new[] { vertex1, vertex4 }));

            Assert.Throws<ArgumentNullException>(() => graph.HideVertices(null, "TestTag"));
            Assert.Throws<ArgumentNullException>(() => graph.HideVertices(new[] { vertex2, null }, "TestTag"));
            Assert.Throws<VertexNotFoundException>(() => graph.HideVertices(new[] { vertex2, vertex4 }, "TestTag"));

            Assert.Throws<ArgumentNullException>(() => graph.HideVertices(new[] { vertex3, null }, null));
            Assert.Throws<ArgumentNullException>(() => graph.HideVertices(new[] { vertex3, vertex4 }, null));

            Assert.Throws<ArgumentNullException>(() => graph.HideVertices(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void HideVerticesIf()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });
            graph.AddVertex(5);

            var hiddenVertices = new Stack<int>(new[] { 5, 4, 2 });
            var hiddenEdges = new Stack<Edge<int>>(new[] { edge41, edge14, edge42, edge22, edge12 });
            graph.VertexHidden += vertex => Assert.AreEqual(hiddenVertices.Pop(), vertex);
            graph.VertexUnhidden += vertex => Assert.Fail("Unhidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.AreSame(hiddenEdges.Pop(), edge);
            graph.EdgeUnhidden += edge => Assert.Fail("Unhidden edge event must not be called.");

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            graph.HideVerticesIf(v => v == 100, "TestTag"); // Nothing hidden
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Vertices, 2);
            CollectionAssert.Contains(graph.Vertices, 4);
            graph.HideVerticesIf(v => v == 2 || v == 4, "TestTag");
            CollectionAssert.DoesNotContain(graph.Vertices, 2);
            CollectionAssert.DoesNotContain(graph.Vertices, 4);
            CollectionAssert.AreEquivalent(new[] { 2, 4 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            CollectionAssert.DoesNotContain(graph.Vertices, 2);
            CollectionAssert.DoesNotContain(graph.Vertices, 4);
            CollectionAssert.Contains(graph.Vertices, 5);
            graph.HideVerticesIf(v => v == 2 || v == 4 || v == 5, "TestTag");   // With some already hidden
            CollectionAssert.DoesNotContain(graph.Vertices, 2);
            CollectionAssert.DoesNotContain(graph.Vertices, 4);
            CollectionAssert.DoesNotContain(graph.Vertices, 5);
            CollectionAssert.AreEquivalent(new[] { 2, 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(hiddenVertices);
            CollectionAssert.IsEmpty(hiddenEdges);
        }

        [Test]
        public void HideVerticesIf_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HideVerticesIf(null, "TestTag"));
            Assert.Throws<ArgumentNullException>(() => graph.HideVerticesIf(vertex => true, null));
            Assert.Throws<ArgumentNullException>(() => graph.HideVerticesIf(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void IsHiddenVertex()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });
            graph.AddVertex(5);

            Assert.IsFalse(graph.IsHiddenVertex(1));
            Assert.IsFalse(graph.IsHiddenVertex(2));
            Assert.IsFalse(graph.IsHiddenVertex(3));
            Assert.IsFalse(graph.IsHiddenVertex(4));
            Assert.IsFalse(graph.IsHiddenVertex(5));

            Assert.IsTrue(graph.HideVertex(2));

            Assert.IsFalse(graph.IsHiddenVertex(1));
            Assert.IsTrue(graph.IsHiddenVertex(2));
            Assert.IsFalse(graph.IsHiddenVertex(3));
            Assert.IsFalse(graph.IsHiddenVertex(4));
            Assert.IsFalse(graph.IsHiddenVertex(5));

            Assert.IsTrue(graph.HideVertex(4));

            Assert.IsFalse(graph.IsHiddenVertex(1));
            Assert.IsTrue(graph.IsHiddenVertex(2));
            Assert.IsFalse(graph.IsHiddenVertex(3));
            Assert.IsTrue(graph.IsHiddenVertex(4));
            Assert.IsFalse(graph.IsHiddenVertex(5));

            Assert.IsFalse(graph.HideVertex(2));    // Already hidden

            Assert.IsFalse(graph.IsHiddenVertex(1));
            Assert.IsTrue(graph.IsHiddenVertex(2));
            Assert.IsFalse(graph.IsHiddenVertex(3));
            Assert.IsTrue(graph.IsHiddenVertex(4));
            Assert.IsFalse(graph.IsHiddenVertex(5));
        }

        [Test]
        public void IsHiddenVertex_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.IsHiddenVertex(null));
            Assert.Throws<VertexNotFoundException>(() => graph.IsHiddenVertex(vertex));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void UnhideVertex()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });
            graph.AddVertex(5);

            var unhiddenVertices = new Stack<int>(new[] { 2, 5, 4 });
            graph.VertexUnhidden += vertex => Assert.AreEqual(unhiddenVertices.Pop(), vertex);
            graph.EdgeUnhidden += edge => Assert.Fail("Unhidden edge event must not be called.");

            graph.HideVertices(new[] { 2, 4, 5, 3 });
            CollectionAssert.AreEquivalent(new[] { 2, 4, 5, 3 }, graph.HiddenVertices);
            Assert.AreEqual(4, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            graph.VertexHidden += vertex => Assert.Fail("Hidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.Fail("Hidden edge event must not be called.");

            CollectionAssert.DoesNotContain(graph.Vertices, 4);
            Assert.IsTrue(graph.UnhideVertex(4));
            CollectionAssert.Contains(graph.Vertices, 4);
            CollectionAssert.AreEquivalent(new[] { 2, 5, 3 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            CollectionAssert.DoesNotContain(graph.Vertices, 5);
            Assert.IsTrue(graph.UnhideVertex(5));
            CollectionAssert.Contains(graph.Vertices, 5);
            CollectionAssert.AreEquivalent(new[] { 2, 3 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            CollectionAssert.DoesNotContain(graph.Vertices, 2);
            Assert.IsTrue(graph.UnhideVertex(2));
            CollectionAssert.Contains(graph.Vertices, 2);
            CollectionAssert.AreEquivalent(new[] { 3 }, graph.HiddenVertices);
            Assert.AreEqual(1, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            Assert.IsFalse(graph.UnhideVertex(4));  // Already unhidden

            CollectionAssert.IsEmpty(unhiddenVertices);
        }

        [Test]
        public void UnhideVertex_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideVertex(null));
            Assert.Throws<VertexNotFoundException>(() => graph.UnhideVertex(vertex));
        }

        [Test]
        public void UnhideVertexAndEdges()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });
            graph.AddVertex(5);

            var unhiddenVertices = new Stack<int>(new[] { 3, 2, 5, 4 });
            var unhiddenEdges = new Stack<Edge<int>>(new[] { edge13, edge42, edge22, edge12, edge41, edge14 });
            graph.VertexUnhidden += vertex => Assert.AreEqual(unhiddenVertices.Pop(), vertex);
            graph.EdgeUnhidden += edge => Assert.AreEqual(unhiddenEdges.Pop(), edge);

            graph.HideVertices(new[] { 2, 4, 5, 3 });
            CollectionAssert.AreEquivalent(new[] { 2, 3, 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(4, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            graph.VertexHidden += vertex => Assert.Fail("Hidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.Fail("Hidden edge event must not be called.");

            CollectionAssert.DoesNotContain(graph.Vertices, 4);
            graph.UnhideVertexAndEdges(4);
            CollectionAssert.Contains(graph.Vertices, 4);
            CollectionAssert.AreEquivalent(new[] { 2, 3, 5 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge22, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(4, graph.HiddenEdgeCount);

            CollectionAssert.DoesNotContain(graph.Vertices, 5);
            graph.UnhideVertexAndEdges(5);
            CollectionAssert.Contains(graph.Vertices, 5);
            CollectionAssert.AreEquivalent(new[] { 2, 3 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge22, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(4, graph.HiddenEdgeCount);

            CollectionAssert.DoesNotContain(graph.Vertices, 2);
            graph.UnhideVertexAndEdges(2);
            CollectionAssert.Contains(graph.Vertices, 2);
            CollectionAssert.AreEquivalent(new[] { 3 }, graph.HiddenVertices);
            Assert.AreEqual(1, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge13 }, graph.HiddenEdges);
            Assert.AreEqual(1, graph.HiddenEdgeCount);

            graph.UnhideVertexAndEdges(4);  // Already unhidden
            CollectionAssert.AreEquivalent(new[] { 3 }, graph.HiddenVertices);
            Assert.AreEqual(1, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge13 }, graph.HiddenEdges);
            Assert.AreEqual(1, graph.HiddenEdgeCount);

            CollectionAssert.DoesNotContain(graph.Vertices, 3);
            graph.UnhideVertexAndEdges(3);
            CollectionAssert.Contains(graph.Vertices, 3);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(unhiddenVertices);
            CollectionAssert.IsEmpty(unhiddenEdges);
        }

        [Test]
        public void UnhideVertexAndEdges_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideVertexAndEdges(null));
            Assert.Throws<VertexNotFoundException>(() => graph.UnhideVertexAndEdges(vertex));
        }

        #endregion

        #region Edge hide/unhide

        [Test]
        public void HideEdge()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.General);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);
            var edgeNotInGraph = new TypedEdge<int>(2, 3, EdgeTypes.Hierarchical);
            var edgeVertexNotInGraph1 = new TypedEdge<int>(2, 10, EdgeTypes.General);
            var edgeVertexNotInGraph2 = new TypedEdge<int>(10, 2, EdgeTypes.Hierarchical);
            var edgeVerticesNotInGraph = new TypedEdge<int>(10, 11, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });

            var hiddenEdges = new Stack<Edge<int>>(new[] { edge13, edge22, edge14 });
            graph.VertexHidden += vertex => Assert.Fail("Hidden vertex event must not be called.");
            graph.VertexUnhidden += vertex => Assert.Fail("Unhidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.AreSame(hiddenEdges.Pop(), edge);
            graph.EdgeUnhidden += edge => Assert.Fail("Unhidden edge event must not be called.");

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            Assert.IsFalse(graph.HideEdge(edgeNotInGraph));
            Assert.IsFalse(graph.HideEdge(edgeVertexNotInGraph1));
            Assert.IsFalse(graph.HideEdge(edgeVertexNotInGraph2));
            Assert.IsFalse(graph.HideEdge(edgeVerticesNotInGraph));

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Edges, edge14);
            Assert.IsTrue(graph.HideEdge(edge14));
            CollectionAssert.DoesNotContain(graph.Edges, edge14);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge14 }, graph.HiddenEdges);
            Assert.AreEqual(1, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Edges, edge22);
            Assert.IsTrue(graph.HideEdge(edge22));
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge14, edge22 }, graph.HiddenEdges);
            Assert.AreEqual(2, graph.HiddenEdgeCount);

            Assert.IsFalse(graph.HideEdge(edge14)); // Already hidden
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge14, edge22 }, graph.HiddenEdges);
            Assert.AreEqual(2, graph.HiddenEdgeCount);

            Assert.IsFalse(graph.HideEdge(edge14, "TestTag")); // Already hidden
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge14, edge22 }, graph.HiddenEdges);
            Assert.AreEqual(2, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Edges, edge13);
            Assert.IsTrue(graph.HideEdge(edge13, "TestTag"));
            CollectionAssert.DoesNotContain(graph.Edges, edge13);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge13, edge14, edge22 }, graph.HiddenEdges);
            Assert.AreEqual(3, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(hiddenEdges);
        }

        [Test]
        public void HideEdge_Throws()
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var edgeNotInGraph = new TypedEdge<TestVertex>(vertex1, vertex2, EdgeTypes.General);
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();
            graph.AddVertexRange(new[] { vertex1, vertex2 });

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HideEdge(null));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdge(null, "TestTag"));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdge(edgeNotInGraph, null));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdge(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void HideEdges()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.General);
            var edge32 = new TypedEdge<int>(3, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);
            var edgeNotInGraph = new TypedEdge<int>(2, 3, EdgeTypes.General);
            var edgeVertexNotInGraph1 = new TypedEdge<int>(2, 10, EdgeTypes.General);
            var edgeVertexNotInGraph2 = new TypedEdge<int>(10, 2, EdgeTypes.Hierarchical);
            var edgeVerticesNotInGraph = new TypedEdge<int>(10, 11, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge32, edge41, edge42
            });

            var hiddenEdges = new Stack<Edge<int>>(new[] { edge32, edge42, edge12, edge41, edge13, edge22, edge14 });
            graph.VertexHidden += vertex => Assert.Fail("Hidden vertex event must not be called.");
            graph.VertexUnhidden += vertex => Assert.Fail("Unhidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.AreSame(hiddenEdges.Pop(), edge);
            graph.EdgeUnhidden += edge => Assert.Fail("Unhidden edge event must not be called.");

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            graph.HideEdges(new[]
            {
                edgeNotInGraph, edgeVertexNotInGraph1,
                edgeVertexNotInGraph2, edgeVerticesNotInGraph
            });

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Edges, edge14);
            CollectionAssert.Contains(graph.Edges, edge22);
            graph.HideEdges(new[] { edge14, edge22 });
            CollectionAssert.DoesNotContain(graph.Edges, edge14);
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge14, edge22 }, graph.HiddenEdges);
            Assert.AreEqual(2, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Edges, edge13);
            CollectionAssert.Contains(graph.Edges, edge41);
            graph.HideEdges(new[] { edge13, edge41 });
            CollectionAssert.DoesNotContain(graph.Edges, edge13);
            CollectionAssert.DoesNotContain(graph.Edges, edge41);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge13, edge14, edge22, edge41 }, graph.HiddenEdges);
            Assert.AreEqual(4, graph.HiddenEdgeCount);

            graph.HideEdges(new[] { edge14, edge41 });  // Already hidden
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge13, edge14, edge22, edge41 }, graph.HiddenEdges);
            Assert.AreEqual(4, graph.HiddenEdgeCount);

            graph.HideEdges(new[] { edge13, edge22 }, "TestTag");  // Already hidden
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge13, edge14, edge22, edge41 }, graph.HiddenEdges);
            Assert.AreEqual(4, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Edges, edge12);
            CollectionAssert.Contains(graph.Edges, edge32);
            CollectionAssert.Contains(graph.Edges, edge42);
            graph.HideEdges(new[] { edge12, edge42, edge32 }, "TestTag");
            CollectionAssert.DoesNotContain(graph.Edges, edge12);
            CollectionAssert.DoesNotContain(graph.Edges, edge32);
            CollectionAssert.DoesNotContain(graph.Edges, edge42);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge32, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(7, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(hiddenEdges);
        }

        [Test]
        public void HideEdges_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVertexRange(new[] { 1, 2, 3 });

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(null));
            Assert.Throws<ArgumentNullException>(
                () => graph.HideEdges(
                    new[] { new TypedEdge<int>(1, 2, EdgeTypes.General), null }));

            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(null, "TestTag"));
            Assert.Throws<ArgumentNullException>(
                () => graph.HideEdges(
                    new[] { new TypedEdge<int>(1, 2, EdgeTypes.General), null },
                    "TestTag"));

            Assert.Throws<ArgumentNullException>(
                () => graph.HideEdges(
                    new[] { new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical), null },
                    null));
            Assert.Throws<ArgumentNullException>(
                () => graph.HideEdges(
                    new[]
                    {
                        new TypedEdge<int>(1, 2, EdgeTypes.General),
                        new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical)
                    },
                    null));

            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void HideEdgesIf()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });

            var hiddenEdges = new Stack<Edge<int>>(new[] { edge22, edge42, edge41 });
            graph.VertexHidden += vertex => Assert.Fail("Hidden vertex event must not be called.");
            graph.VertexUnhidden += vertex => Assert.Fail("Unhidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.AreSame(hiddenEdges.Pop(), edge);
            graph.EdgeUnhidden += edge => Assert.Fail("Unhidden edge event must not be called.");

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Edges, edge41);
            CollectionAssert.Contains(graph.Edges, edge42);
            graph.HideEdgesIf(e => e.Source == 4, "TestTag");
            CollectionAssert.DoesNotContain(graph.Edges, edge41);
            CollectionAssert.DoesNotContain(graph.Edges, edge42);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(2, graph.HiddenEdgeCount);

            CollectionAssert.Contains(graph.Edges, edge22);
            graph.HideEdgesIf(e => e.Source == e.Target, "TestTag");
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(3, graph.HiddenEdgeCount);

            graph.HideEdgesIf(e => e.Source == 4 || e.Source == e.Target, "TestTag");  // Already hidden
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(3, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(hiddenEdges);
        }

        [Test]
        public void HideEdgesIf_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HideEdgesIf(null, "TestTag"));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdgesIf(e => true, null));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdgesIf(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void IsHiddenEdge()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);
            var edgeNotInGraph = new TypedEdge<int>(2, 3, EdgeTypes.General);
            var edgeVertexNotInGraph1 = new TypedEdge<int>(2, 10, EdgeTypes.General);
            var edgeVertexNotInGraph2 = new TypedEdge<int>(10, 2, EdgeTypes.Hierarchical);
            var edgeVerticesNotInGraph = new TypedEdge<int>(10, 11, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });

            Assert.IsFalse(graph.IsHiddenEdge(edge12));
            Assert.IsFalse(graph.IsHiddenEdge(edge13));
            Assert.IsFalse(graph.IsHiddenEdge(edge14));
            Assert.IsFalse(graph.IsHiddenEdge(edge22));
            Assert.IsFalse(graph.IsHiddenEdge(edge41));
            Assert.IsFalse(graph.IsHiddenEdge(edge42));
            Assert.IsFalse(graph.IsHiddenEdge(edgeNotInGraph));
            Assert.IsFalse(graph.IsHiddenEdge(edgeVertexNotInGraph1));
            Assert.IsFalse(graph.IsHiddenEdge(edgeVertexNotInGraph2));
            Assert.IsFalse(graph.IsHiddenEdge(edgeVerticesNotInGraph));

            Assert.IsTrue(graph.HideEdge(edge22));

            Assert.IsFalse(graph.IsHiddenEdge(edge12));
            Assert.IsFalse(graph.IsHiddenEdge(edge13));
            Assert.IsFalse(graph.IsHiddenEdge(edge14));
            Assert.IsTrue(graph.IsHiddenEdge(edge22));
            Assert.IsFalse(graph.IsHiddenEdge(edge41));
            Assert.IsFalse(graph.IsHiddenEdge(edge42));

            Assert.IsTrue(graph.HideEdge(edge42));

            Assert.IsFalse(graph.IsHiddenEdge(edge12));
            Assert.IsFalse(graph.IsHiddenEdge(edge13));
            Assert.IsFalse(graph.IsHiddenEdge(edge14));
            Assert.IsTrue(graph.IsHiddenEdge(edge22));
            Assert.IsFalse(graph.IsHiddenEdge(edge41));
            Assert.IsTrue(graph.IsHiddenEdge(edge42));

            Assert.IsFalse(graph.HideEdge(edge42));    // Already hidden

            Assert.IsFalse(graph.IsHiddenEdge(edge12));
            Assert.IsFalse(graph.IsHiddenEdge(edge13));
            Assert.IsFalse(graph.IsHiddenEdge(edge14));
            Assert.IsTrue(graph.IsHiddenEdge(edge22));
            Assert.IsFalse(graph.IsHiddenEdge(edge41));
            Assert.IsTrue(graph.IsHiddenEdge(edge42));
        }

        [Test]
        public void IsHiddenEdge_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.IsHiddenEdge(null));
        }

        [Test]
        public void UnhideEdge()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.General);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);
            var edgeNotInGraph = new TypedEdge<int>(2, 3, EdgeTypes.Hierarchical);
            var edgeVertexNotInGraph1 = new TypedEdge<int>(2, 10, EdgeTypes.General);
            var edgeVertexNotInGraph2 = new TypedEdge<int>(10, 2, EdgeTypes.General);
            var edgeVerticesNotInGraph = new TypedEdge<int>(10, 11, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });

            var unhiddenEdges = new Stack<Edge<int>>(new[] { edge22, edge41 });
            var unhideVertexHandler = new VertexAction<int>(v => Assert.Fail("Unhidden vertex event must not be called."));
            graph.VertexUnhidden += unhideVertexHandler;
            graph.EdgeUnhidden += edge => Assert.AreEqual(unhiddenEdges.Pop(), edge);

            graph.HideVertices(new[] { 2, 4, 3 });
            CollectionAssert.AreEquivalent(new[] { 2, 4, 3 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            graph.VertexHidden += vertex => Assert.Fail("Hidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.Fail("Hidden edge event must not be called.");

            Assert.IsFalse(graph.UnhideEdge(edgeNotInGraph));
            Assert.IsFalse(graph.UnhideEdge(edgeVertexNotInGraph1));
            Assert.IsFalse(graph.UnhideEdge(edgeVertexNotInGraph2));
            Assert.IsFalse(graph.UnhideEdge(edgeVerticesNotInGraph));

            CollectionAssert.AreEquivalent(new[] { 2, 4, 3 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            // Edge cannot be unhidden because at least one of its vertex is hidden
            CollectionAssert.DoesNotContain(graph.Edges, edge41);
            Assert.IsFalse(graph.UnhideEdge(edge41));
            CollectionAssert.DoesNotContain(graph.Edges, edge41);

            graph.VertexUnhidden -= unhideVertexHandler;
            graph.UnhideVertex(4);
            graph.VertexUnhidden += unhideVertexHandler;

            CollectionAssert.DoesNotContain(graph.Edges, edge41);
            Assert.IsTrue(graph.UnhideEdge(edge41));
            CollectionAssert.Contains(graph.Edges, edge41);
            CollectionAssert.AreEquivalent(new[] { 2, 3 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            // Edge cannot be unhidden because at least one of its vertex is hidden
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            Assert.IsFalse(graph.UnhideEdge(edge22));
            CollectionAssert.DoesNotContain(graph.Edges, edge22);

            graph.VertexUnhidden -= unhideVertexHandler;
            graph.UnhideVertex(2);
            graph.VertexUnhidden += unhideVertexHandler;

            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            Assert.IsTrue(graph.UnhideEdge(edge22));
            CollectionAssert.Contains(graph.Edges, edge22);
            CollectionAssert.AreEquivalent(new[] { 3 }, graph.HiddenVertices);
            Assert.AreEqual(1, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(4, graph.HiddenEdgeCount);

            Assert.IsFalse(graph.UnhideEdge(edge41));  // Already unhidden

            CollectionAssert.IsEmpty(unhiddenEdges);
        }

        [Test]
        public void UnhideEdge_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideEdge(null));
        }

        [Test]
        public void UnhideEdges()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);
            var edgeNotInGraph = new TypedEdge<int>(2, 3, EdgeTypes.Hierarchical);
            var edgeVertexNotInGraph1 = new TypedEdge<int>(2, 10, EdgeTypes.General);
            var edgeVertexNotInGraph2 = new TypedEdge<int>(10, 2, EdgeTypes.Hierarchical);
            var edgeVerticesNotInGraph = new TypedEdge<int>(10, 11, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });

            var unhiddenEdges = new Stack<Edge<int>>(new[] { edge13, edge22, edge41 });
            var unhideVertexHandler = new VertexAction<int>(v => Assert.Fail("Unhidden vertex event must not be called."));
            graph.VertexUnhidden += unhideVertexHandler;
            graph.EdgeUnhidden += edge => Assert.AreEqual(unhiddenEdges.Pop(), edge);

            graph.HideVertices(new[] { 2, 4, 3 });
            CollectionAssert.AreEquivalent(new[] { 2, 4, 3 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            graph.VertexHidden += vertex => Assert.Fail("Hidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.Fail("Hidden edge event must not be called.");

            graph.UnhideEdges(new[] { edgeNotInGraph, edgeVertexNotInGraph1, edgeVertexNotInGraph2, edgeVerticesNotInGraph });
            CollectionAssert.AreEquivalent(new[] { 2, 4, 3 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            // Edge cannot be unhidden because at least one of its vertex is hidden
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.DoesNotContain(graph.Edges, edge41);
            graph.UnhideEdges(new[] { edge41, edge22 });
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.DoesNotContain(graph.Edges, edge41);

            graph.VertexUnhidden -= unhideVertexHandler;
            graph.UnhideVertex(4);
            graph.VertexUnhidden += unhideVertexHandler;

            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.DoesNotContain(graph.Edges, edge41);
            graph.UnhideEdges(new[] { edge41, edge22 });
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.Contains(graph.Edges, edge41);
            CollectionAssert.AreEquivalent(new[] { 2, 3 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            graph.VertexUnhidden -= unhideVertexHandler;
            graph.UnhideVertex(2);
            graph.UnhideVertex(3);
            graph.VertexUnhidden += unhideVertexHandler;

            CollectionAssert.DoesNotContain(graph.Edges, edge13);
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            graph.UnhideEdges(new[] { edge22, edge13 });
            CollectionAssert.Contains(graph.Edges, edge13);
            CollectionAssert.Contains(graph.Edges, edge22);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(3, graph.HiddenEdgeCount);

            graph.UnhideEdges(new[] { edge13, edge41 });  // Already unhidden
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(3, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(unhiddenEdges);
        }

        [Test]
        public void UnhideEdges_Throws()
        {
            var edge = new TypedEdge<int>(1, 2, EdgeTypes.General);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVertexRange(new[] { 1, 2 });

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideEdges(null));
            Assert.Throws<ArgumentNullException>(() => graph.UnhideEdges(new[] { edge, null }));
        }

        [Test]
        public void UnhideEdgesIf()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.General);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });

            var unhiddenEdges = new Stack<Edge<int>>(new[] { edge42, edge13, edge22, edge41 });
            var unhideVertexHandler = new VertexAction<int>(v => Assert.Fail("Unhidden vertex event must not be called."));
            graph.VertexUnhidden += unhideVertexHandler;
            graph.EdgeUnhidden += edge => Assert.AreEqual(unhiddenEdges.Pop(), edge);

            graph.HideVertices(new[] { 2, 4, 3 });
            CollectionAssert.AreEquivalent(new[] { 2, 4, 3 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            graph.VertexHidden += vertex => Assert.Fail("Hidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.Fail("Hidden edge event must not be called.");

            // Edge cannot be unhidden because at least one of its vertex is hidden
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.DoesNotContain(graph.Edges, edge41);
            CollectionAssert.DoesNotContain(graph.Edges, edge42);
            graph.UnhideEdgesIf(e => e.Source == 4 || e.Source == e.Target);
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.DoesNotContain(graph.Edges, edge41);
            CollectionAssert.DoesNotContain(graph.Edges, edge42);

            graph.VertexUnhidden -= unhideVertexHandler;
            graph.UnhideVertex(4);
            graph.VertexUnhidden += unhideVertexHandler;

            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.DoesNotContain(graph.Edges, edge41);
            CollectionAssert.DoesNotContain(graph.Edges, edge42);
            graph.UnhideEdgesIf(e => e.Source == 4 || e.Source == e.Target);
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            CollectionAssert.Contains(graph.Edges, edge41);
            CollectionAssert.DoesNotContain(graph.Edges, edge42);   // 2 is hidden
            CollectionAssert.AreEquivalent(new[] { 2, 3 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            graph.VertexUnhidden -= unhideVertexHandler;
            graph.UnhideVertex(2);
            graph.UnhideVertex(3);
            graph.VertexUnhidden += unhideVertexHandler;

            CollectionAssert.DoesNotContain(graph.Edges, edge13);
            CollectionAssert.DoesNotContain(graph.Edges, edge22);
            graph.UnhideEdgesIf(e => e.Source == e.Target || e.Target == 3);
            CollectionAssert.Contains(graph.Edges, edge13);
            CollectionAssert.Contains(graph.Edges, edge22);
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(3, graph.HiddenEdgeCount);

            graph.UnhideEdgesIf(e => e.Source == 4 || e.Source == e.Target); // Some already unhidden
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14 }, graph.HiddenEdges);
            Assert.AreEqual(2, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(unhiddenEdges);
        }

        [Test]
        public void UnhideEdgesIf_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideEdgesIf(null));
        }

        [Test]
        public void HiddenEdgesOf()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.Hierarchical);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.General);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.General);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });

            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(1));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(1));
            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(2));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(2));
            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(3));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(3));
            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(4));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(4));

            graph.HideVertex(2);

            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HiddenEdgesOf(1));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge42 }, graph.HiddenEdgesOf(2));
            Assert.AreEqual(3, graph.HiddenEdgeCountOf(2));
            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(3));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(3));
            CollectionAssert.AreEquivalent(new[] { edge42 }, graph.HiddenEdgesOf(4));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(4));

            graph.HideVertex(2);    // Already hidden

            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HiddenEdgesOf(1));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge42 }, graph.HiddenEdgesOf(2));
            Assert.AreEqual(3, graph.HiddenEdgeCountOf(2));
            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(3));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(3));
            CollectionAssert.AreEquivalent(new[] { edge42 }, graph.HiddenEdgesOf(4));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(4));

            graph.HideEdge(edge13);

            CollectionAssert.AreEquivalent(new[] { edge12, edge13 }, graph.HiddenEdgesOf(1));
            Assert.AreEqual(2, graph.HiddenEdgeCountOf(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge42 }, graph.HiddenEdgesOf(2));
            Assert.AreEqual(3, graph.HiddenEdgeCountOf(2));
            CollectionAssert.AreEquivalent(new[] { edge13 }, graph.HiddenEdgesOf(3));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(3));
            CollectionAssert.AreEquivalent(new[] { edge42 }, graph.HiddenEdgesOf(4));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(4));

            graph.HideEdge(edge12); // Already hidden

            CollectionAssert.AreEquivalent(new[] { edge12, edge13 }, graph.HiddenEdgesOf(1));
            Assert.AreEqual(2, graph.HiddenEdgeCountOf(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge42 }, graph.HiddenEdgesOf(2));
            Assert.AreEqual(3, graph.HiddenEdgeCountOf(2));
            CollectionAssert.AreEquivalent(new[] { edge13 }, graph.HiddenEdgesOf(3));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(3));
            CollectionAssert.AreEquivalent(new[] { edge42 }, graph.HiddenEdgesOf(4));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(4));

            graph.HideVertex(1);

            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge41 }, graph.HiddenEdgesOf(1));
            Assert.AreEqual(4, graph.HiddenEdgeCountOf(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge42 }, graph.HiddenEdgesOf(2));
            Assert.AreEqual(3, graph.HiddenEdgeCountOf(2));
            CollectionAssert.AreEquivalent(new[] { edge13 }, graph.HiddenEdgesOf(3));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(3));
            CollectionAssert.AreEquivalent(new[] { edge14, edge41, edge42 }, graph.HiddenEdgesOf(4));
            Assert.AreEqual(3, graph.HiddenEdgeCountOf(4));

            graph.UnhideVertexAndEdges(1);

            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HiddenEdgesOf(1));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge42 }, graph.HiddenEdgesOf(2));
            Assert.AreEqual(3, graph.HiddenEdgeCountOf(2));
            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(3));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(3));
            CollectionAssert.AreEquivalent(new[] { edge42 }, graph.HiddenEdgesOf(4));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(4));

            graph.UnhideVertex(2);

            CollectionAssert.AreEquivalent(new[] { edge12 }, graph.HiddenEdgesOf(1));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(1));
            CollectionAssert.AreEquivalent(new[] { edge12, edge22, edge42 }, graph.HiddenEdgesOf(2));
            Assert.AreEqual(3, graph.HiddenEdgeCountOf(2));
            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(3));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(3));
            CollectionAssert.AreEquivalent(new[] { edge42 }, graph.HiddenEdgesOf(4));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(4));

            graph.UnhideVertexAndEdges(1);

            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(1));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(1));
            CollectionAssert.AreEquivalent(new[] { edge22, edge42 }, graph.HiddenEdgesOf(2));
            Assert.AreEqual(2, graph.HiddenEdgeCountOf(2));
            CollectionAssert.IsEmpty(graph.HiddenEdgesOf(3));
            Assert.AreEqual(0, graph.HiddenEdgeCountOf(3));
            CollectionAssert.AreEquivalent(new[] { edge42 }, graph.HiddenEdgesOf(4));
            Assert.AreEqual(1, graph.HiddenEdgeCountOf(4));
        }

        [Test]
        public void HiddenEdgesOf_Throws()
        {
            var vertex = new TestVertex();
            var graph = new SoftMutableHierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HiddenEdgesOf(null));
            Assert.Throws<ArgumentNullException>(() => graph.HiddenEdgeCountOf(null));

            Assert.Throws<VertexNotFoundException>(() => graph.HiddenEdgesOf(vertex));
            Assert.Throws<VertexNotFoundException>(() => graph.HiddenEdgeCountOf(vertex));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void Unhide()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.Hierarchical);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.Hierarchical);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);
            var edge56 = new TypedEdge<int>(5, 6, EdgeTypes.General);
            var edge57 = new TypedEdge<int>(5, 7, EdgeTypes.Hierarchical);

            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22,
                edge41, edge42, edge56, edge57
            });

            var unhiddenVertices = new Stack<int>(new[] { 5, 4, 1, 2 });
            var unhiddenEdges = new Stack<Edge<int>>(new[] { edge57, edge56, edge13, edge41, edge22 });
            graph.VertexUnhidden += vertex => Assert.AreEqual(unhiddenVertices.Pop(), vertex);
            graph.EdgeUnhidden += edge => Assert.AreEqual(unhiddenEdges.Pop(), edge);

            graph.HideVertex(2, "TestTag1");
            graph.HideEdge(edge14, "TestTag1");
            graph.HideVertex(4, "TestTag2");
            graph.HideEdge(edge13, "TestTag3");
            graph.HideVertex(1);    // NoTag
            graph.HideEdgesIf(e => e.Source == 5, "TestTag4");
            graph.HideVertex(5, "TestTag4");

            CollectionAssert.AreEquivalent(new[] { 1, 2, 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(4, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42, edge56, edge57 }, graph.HiddenEdges);
            Assert.AreEqual(8, graph.HiddenEdgeCount);

            graph.VertexHidden += vertex => Assert.Fail("Hidden vertex event must not be called.");
            graph.EdgeHidden += edge => Assert.Fail("Hidden edge event must not be called.");

            // Nothing unhidden
            graph.Unhide("NotExistingTag");
            CollectionAssert.AreEquivalent(new[] { 1, 2, 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(4, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42, edge56, edge57 }, graph.HiddenEdges);
            Assert.AreEqual(8, graph.HiddenEdgeCount);

            // Try with TestTag1 (Tag is removed after even if not fully cleaned)
            graph.Unhide("TestTag1");   // Do not unhide everything because some are hidden by other tags
            CollectionAssert.AreEquivalent(new[] { 1, 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(3, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge41, edge42, edge56, edge57 }, graph.HiddenEdges);
            Assert.AreEqual(7, graph.HiddenEdgeCount);

            graph.UnhideVertex(1);
            CollectionAssert.AreEquivalent(new[] { 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge41, edge42, edge56, edge57 }, graph.HiddenEdges);
            Assert.AreEqual(7, graph.HiddenEdgeCount);

            // TestTag1 no more exist so elements remain hidden
            graph.Unhide("TestTag1");
            CollectionAssert.AreEquivalent(new[] { 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(2, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge41, edge42, edge56, edge57 }, graph.HiddenEdges);
            Assert.AreEqual(7, graph.HiddenEdgeCount);

            // Try with TestTag2
            graph.Unhide("TestTag2");
            CollectionAssert.AreEquivalent(new[] { 5 }, graph.HiddenVertices);
            Assert.AreEqual(1, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge42, edge56, edge57 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);

            // Try with TestTag3
            graph.Unhide("TestTag3");
            CollectionAssert.AreEquivalent(new[] { 5 }, graph.HiddenVertices);
            Assert.AreEqual(1, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge42, edge56, edge57 }, graph.HiddenEdges);
            Assert.AreEqual(5, graph.HiddenEdgeCount);

            // Try with TestTag4
            graph.Unhide("TestTag4");
            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge14, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(3, graph.HiddenEdgeCount);

            CollectionAssert.IsEmpty(unhiddenVertices);
            CollectionAssert.IsEmpty(unhiddenEdges);
        }

        [Test]
        public void Unhide_Throws()
        {
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.Unhide(null));
        }

        [Test]
        public void UnhideAll()
        {
            var edge12 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            var edge13 = new TypedEdge<int>(1, 3, EdgeTypes.General);
            var edge14 = new TypedEdge<int>(1, 4, EdgeTypes.General);
            var edge22 = new TypedEdge<int>(2, 2, EdgeTypes.Hierarchical);
            var edge41 = new TypedEdge<int>(4, 1, EdgeTypes.Hierarchical);
            var edge42 = new TypedEdge<int>(4, 2, EdgeTypes.General);

            var hideVertexFailHandler = new VertexAction<int>(vertex => Assert.Fail("Hidden vertex event must not be called."));
            var unhideVertexFailHandler = new VertexAction<int>(vertex => Assert.Fail("Unhidden vertex event must not be called."));
            var hideEdgeFailHandler = new EdgeAction<int, Edge<int>>(edge => Assert.Fail("Hidden edge event must not be called."));
            var unhideEdgeFailHandler = new EdgeAction<int, Edge<int>>(edge => Assert.Fail("Unhidden edge event must not be called."));

            // Empty graph
            var graph = new SoftMutableHierarchicalGraph<int, TypedEdge<int>>();
            graph.VertexHidden += hideVertexFailHandler;
            graph.VertexUnhidden += unhideVertexFailHandler;
            graph.EdgeHidden += hideEdgeFailHandler;
            graph.EdgeUnhidden += unhideEdgeFailHandler;

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);

            Assert.IsTrue(graph.UnhideAll());

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);


            // Non empty graph
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });
            graph.AddVertex(5);

            graph.VertexHidden -= hideVertexFailHandler;
            graph.EdgeHidden -= hideEdgeFailHandler;
            graph.HideVerticesIf(v => true, "TesTag");
            CollectionAssert.DoesNotContain(graph.Vertices, 3);
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, graph.HiddenVertices);
            Assert.AreEqual(5, graph.HiddenVertexCount);
            CollectionAssert.AreEquivalent(new[] { edge12, edge13, edge14, edge22, edge41, edge42 }, graph.HiddenEdges);
            Assert.AreEqual(6, graph.HiddenEdgeCount);
            graph.VertexHidden += hideVertexFailHandler;
            graph.EdgeHidden += hideEdgeFailHandler;

            var unhiddenVertices = new List<int>();
            var unhideVertexHandler = new VertexAction<int>(vertex =>
            {
                Assert.IsNotNull(vertex);
                unhiddenVertices.Add(vertex);
            });
            var unhiddenEdges = new List<Edge<int>>();
            var unhideEdgeHandler = new EdgeAction<int, Edge<int>>(edge =>
            {
                Assert.IsNotNull(edge);
                unhiddenEdges.Add(edge);
            });

            graph.VertexUnhidden -= unhideVertexFailHandler;
            graph.VertexUnhidden += unhideVertexHandler;
            graph.EdgeUnhidden -= unhideEdgeFailHandler;
            graph.EdgeUnhidden += unhideEdgeHandler;

            Assert.IsTrue(graph.UnhideAll());

            CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, unhiddenVertices);
            CollectionAssert.AreEquivalent(unhiddenVertices, graph.Vertices);
            CollectionAssert.AreEquivalent(
                new[] { edge12, edge13, edge14, edge22, edge41, edge42 },
                unhiddenEdges);
            CollectionAssert.AreEquivalent(unhiddenEdges, graph.Edges);

            CollectionAssert.IsEmpty(graph.HiddenVertices);
            Assert.AreEqual(0, graph.HiddenVertexCount);
            CollectionAssert.IsEmpty(graph.HiddenEdges);
            Assert.AreEqual(0, graph.HiddenEdgeCount);
        }

        #endregion
    }
}