using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph;
using static GraphShape.Tests.GraphTestHelpers;

namespace GraphShape.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="SoftMutableBidirectionalGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal class SoftMutableBidirectionalGraphTests
    {
        [Test]
        public void Construction()
        {
            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
            AssertGraphProperties(graph);

            graph = new SoftMutableBidirectionalGraph<int, Edge<int>>(true);
            AssertGraphProperties(graph);

            graph = new SoftMutableBidirectionalGraph<int, Edge<int>>(false);
            AssertGraphProperties(graph, false);

            graph = new SoftMutableBidirectionalGraph<int, Edge<int>>(true, 12);
            AssertGraphProperties(graph);

            graph = new SoftMutableBidirectionalGraph<int, Edge<int>>(false, 12);
            AssertGraphProperties(graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                SoftMutableBidirectionalGraph<TVertex, TEdge> g,
                bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                AssertEmptyGraph(g);
                CollectionAssert.IsEmpty(g.HiddenVertices);
                Assert.AreEqual(0, g.HiddenVertexCount);
                CollectionAssert.IsEmpty(g.HiddenEdges);
                Assert.AreEqual(0, g.HiddenEdgeCount);
            }

            #endregion
        }

        #region Vertex hide/unhide

        [Test]
        public void HideVertex()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<TestVertex, Edge<TestVertex>>();
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
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);
            var edge45 = new Edge<int>(4, 5);
            var edge56 = new Edge<int>(5, 6);
            var edge57 = new Edge<int>(5, 7);
            var edge75 = new Edge<int>(7, 5);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<TestVertex, Edge<TestVertex>>();
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
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<TestVertex, Edge<TestVertex>>();

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HideVerticesIf(null, "TestTag"));
            Assert.Throws<ArgumentNullException>(() => graph.HideVerticesIf(vertex => true, null));
            Assert.Throws<ArgumentNullException>(() => graph.HideVerticesIf(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void IsHiddenVertex()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<TestVertex, Edge<TestVertex>>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.IsHiddenVertex(null));
            Assert.Throws<VertexNotFoundException>(() => graph.IsHiddenVertex(vertex));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void UnhideVertex()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<TestVertex, Edge<TestVertex>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideVertex(null));
            Assert.Throws<VertexNotFoundException>(() => graph.UnhideVertex(vertex));
        }

        [Test]
        public void UnhideVertexAndEdges()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<TestVertex, Edge<TestVertex>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideVertexAndEdges(null));
            Assert.Throws<VertexNotFoundException>(() => graph.UnhideVertexAndEdges(vertex));
        }

        #endregion

        #region Edge hide/unhide

        [Test]
        public void HideEdge()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);
            var edgeNotInGraph = new Edge<int>(2, 3);
            var edgeVertexNotInGraph1 = new Edge<int>(2, 10);
            var edgeVertexNotInGraph2 = new Edge<int>(10, 2);
            var edgeVerticesNotInGraph = new Edge<int>(10, 11);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                edge12, edge13, edge14, edge22, edge41, edge42
            });

            var hiddenEdges = new Stack<Edge<int>>(new[] { edge13, edge22 , edge14 });
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
            var edgeNotInGraph = new Edge<TestVertex>(vertex1, vertex2);
            var graph = new SoftMutableBidirectionalGraph<TestVertex, Edge<TestVertex>>();
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
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge32 = new Edge<int>(3, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);
            var edgeNotInGraph = new Edge<int>(2, 3);
            var edgeVertexNotInGraph1 = new Edge<int>(2, 10);
            var edgeVertexNotInGraph2 = new Edge<int>(10, 2);
            var edgeVerticesNotInGraph = new Edge<int>(10, 11);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
            graph.AddVertexRange(new[] { 1, 2, 3 });

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(null));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(new[] { new Edge<int>(1, 2), null }));

            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(null, "TestTag"));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(new[] { new Edge<int>(1, 2), null }, "TestTag"));

            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(new[] { new Edge<int>(1, 2), null }, null));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(new[] { new Edge<int>(1, 2), new Edge<int>(1, 3) }, null));

            Assert.Throws<ArgumentNullException>(() => graph.HideEdges(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void HideEdgesIf()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.HideEdgesIf(null, "TestTag"));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdgesIf(e => true, null));
            Assert.Throws<ArgumentNullException>(() => graph.HideEdgesIf(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void IsHiddenEdge()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);
            var edgeNotInGraph = new Edge<int>(2, 3);
            var edgeVertexNotInGraph1 = new Edge<int>(2, 10);
            var edgeVertexNotInGraph2 = new Edge<int>(10, 2);
            var edgeVerticesNotInGraph = new Edge<int>(10, 11);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.IsHiddenEdge(null));
        }

        [Test]
        public void UnhideEdge()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);
            var edgeNotInGraph = new Edge<int>(2, 3);
            var edgeVertexNotInGraph1 = new Edge<int>(2, 10);
            var edgeVertexNotInGraph2 = new Edge<int>(10, 2);
            var edgeVerticesNotInGraph = new Edge<int>(10, 11);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideEdge(null));
        }

        [Test]
        public void UnhideEdges()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);
            var edgeNotInGraph = new Edge<int>(2, 3);
            var edgeVertexNotInGraph1 = new Edge<int>(2, 10);
            var edgeVertexNotInGraph2 = new Edge<int>(10, 2);
            var edgeVerticesNotInGraph = new Edge<int>(10, 11);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var edge = new Edge<int>(1, 2);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
            graph.AddVertexRange(new[] { 1, 2 });

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideEdges(null));
            Assert.Throws<ArgumentNullException>(() => graph.UnhideEdges(new[] { edge, null }));
        }

        [Test]
        public void UnhideEdgesIf()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.UnhideEdgesIf(null));
        }

        [Test]
        public void HiddenEdgesOf()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<TestVertex, Edge<TestVertex>>();

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
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);
            var edge56 = new Edge<int>(5, 6);
            var edge57 = new Edge<int>(5, 7);

            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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
            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.Unhide(null));
        }

        [Test]
        public void UnhideAll()
        {
            var edge12 = new Edge<int>(1, 2);
            var edge13 = new Edge<int>(1, 3);
            var edge14 = new Edge<int>(1, 4);
            var edge22 = new Edge<int>(2, 2);
            var edge41 = new Edge<int>(4, 1);
            var edge42 = new Edge<int>(4, 2);

            var hideVertexFailHandler = new VertexAction<int>(vertex => Assert.Fail("Hidden vertex event must not be called."));
            var unhideVertexFailHandler = new VertexAction<int>(vertex => Assert.Fail("Unhidden vertex event must not be called."));
            var hideEdgeFailHandler = new EdgeAction<int, Edge<int>>(edge => Assert.Fail("Hidden edge event must not be called."));
            var unhideEdgeFailHandler = new EdgeAction<int, Edge<int>>(edge => Assert.Fail("Unhidden edge event must not be called."));

            // Empty graph
            var graph = new SoftMutableBidirectionalGraph<int, Edge<int>>();
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