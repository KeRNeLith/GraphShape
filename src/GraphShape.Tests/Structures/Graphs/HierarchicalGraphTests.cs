using System;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.GraphTestHelpers;

namespace GraphShape.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="HierarchicalGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal class HierarchicalGraphTests
    {
        [Test]
        public void Construction()
        {
            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
            AssertGraphProperties(graph);

            graph = new HierarchicalGraph<int, TypedEdge<int>>(true);
            AssertGraphProperties(graph);

            graph = new HierarchicalGraph<int, TypedEdge<int>>(false);
            AssertGraphProperties(graph, false);

            graph = new HierarchicalGraph<int, TypedEdge<int>>(true, 12);
            AssertGraphProperties(graph);

            graph = new HierarchicalGraph<int, TypedEdge<int>>(false, 12);
            AssertGraphProperties(graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                HierarchicalGraph<TVertex, TEdge> g,
                bool parallelEdges = true)
                where TEdge : TypedEdge<TVertex>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                AssertEmptyGraph(g);
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
            var graph = new HierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

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
            var graph = new HierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddVertex(null));
            AssertNoVertex(graph);
        }

        [Test]
        public void RemoveVertex()
        {
            var graph = new HierarchicalGraph<int, TypedEdge<int>>();

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
            var graph = new HierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveVertex(null));
        }

        #endregion

        #region Add/Remove edges

        [Test]
        public void AddEdge()
        {
            var graph = new HierarchicalGraph<int, TypedEdge<int>>(false);

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
            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddEdge(null));
            AssertNoVertex(graph);
        }

        [Test]
        public void RemoveEdge()
        {
            var graph = new HierarchicalGraph<int, TypedEdge<int>>();

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
            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
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

            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
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

            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
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
            var graph = new HierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

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

            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
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
            var graph = new HierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

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

            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
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
            var graph = new HierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

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

            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
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
            var graph = new HierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

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

            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
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
            var graph = new HierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

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

            var graph = new HierarchicalGraph<int, TypedEdge<int>>();
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
            var graph = new HierarchicalGraph<TestVertex, TypedEdge<TestVertex>>();

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
    }
}