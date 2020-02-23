using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests
{
    /// <summary>
    /// Assertion helpers for graphs.
    /// </summary>
    internal static class GraphTestHelpers
    {
        public static void AssertNoVertex<TVertex>([NotNull] IVertexSet<TVertex> graph)
        {
            Assert.IsTrue(graph.IsVerticesEmpty);
            Assert.AreEqual(0, graph.VertexCount);
            CollectionAssert.IsEmpty(graph.Vertices);
        }

        public static void AssertHasVertices<TVertex>(
            [NotNull] IVertexSet<TVertex> graph,
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            TVertex[] vertexArray = vertices.ToArray();
            CollectionAssert.IsNotEmpty(vertexArray);

            Assert.IsFalse(graph.IsVerticesEmpty);
            Assert.AreEqual(vertexArray.Length, graph.VertexCount);
            CollectionAssert.AreEquivalent(vertexArray, graph.Vertices);
        }

        public static void AssertNoEdge<TVertex, TEdge>([NotNull] IEdgeSet<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsTrue(graph.IsEdgesEmpty);
            Assert.AreEqual(0, graph.EdgeCount);
            CollectionAssert.IsEmpty(graph.Edges);
        }

        public static void AssertHasEdges<TVertex, TEdge>(
            [NotNull] IEdgeSet<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            TEdge[] edgeArray = edges.ToArray();
            CollectionAssert.IsNotEmpty(edgeArray);

            Assert.IsFalse(graph.IsEdgesEmpty);
            Assert.AreEqual(edgeArray.Length, graph.EdgeCount);
            CollectionAssert.AreEquivalent(edgeArray, graph.Edges);
        }

        public static void AssertHasEdges_DeepCheck<TVertex>(
            [NotNull] IEdgeSet<TVertex, Edge<TVertex>> graph,
            [NotNull, ItemNotNull] IEnumerable<Edge<TVertex>> edges)
        {
            List<Edge<TVertex>> edgeList = edges.ToList();
            CollectionAssert.IsNotEmpty(edgeList);

            Assert.IsFalse(graph.IsEdgesEmpty);
            Assert.AreEqual(edgeList.Count, graph.EdgeCount);
            foreach (Edge<TVertex> edge in graph.Edges)
            {
                Edge<TVertex> foundEdge = edgeList.FirstOrDefault(
                    e => Equals(e.Source, edge.Source)
                         && Equals(e.Target, edge.Target));

                Assert.IsNotNull(foundEdge);
                edgeList.Remove(foundEdge);
            }
        }

        public static void AssertEmptyGraph<TVertex, TEdge>(
            [NotNull] IEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            AssertNoVertex(graph);
            AssertNoEdge(graph);
        }
    }
}