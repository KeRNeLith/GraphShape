using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Tests
{
    /// <summary>
    /// Graph Factory.
    /// </summary>
    internal static class GraphFactory
    {
        /// <summary>
        /// Creates a graph fully composed of isolated vertices.
        /// </summary>
        /// <param name="vertexCount">Total number of vertices.</param>
        /// <param name="vertexFactory">Vertex factory.</param>
        [Pure]
        [NotNull]
        public static IBidirectionalGraph<TVertex, TEdge> CreateIsolatedVerticesGraph<TVertex, TEdge>(
            int vertexCount,
            [NotNull, InstantHandle] Func<int, TVertex> vertexFactory)
            where TEdge : IEdge<TVertex>
        {
            var graph = new BidirectionalGraph<TVertex, TEdge>(false, vertexCount);

            for (int i = 0; i < vertexCount; ++i)
            {
                graph.AddVertex(vertexFactory(i));
            }

            return graph;
        }

        /// <summary>
        /// Creates a Tree graph.
        /// </summary>
        /// <param name="vertexCount">Total number of vertices.</param>
        /// <param name="componentCount">Number of tree branches.</param>
        /// <param name="vertexFactory">Vertex factory.</param>
        /// <param name="edgeFactory">Edge factory.</param>
        /// <param name="random">Random number generator.</param>
        [Pure]
        [NotNull]
        public static IBidirectionalGraph<TVertex, TEdge> CreateTree<TVertex, TEdge>(
            int vertexCount,
            int componentCount,
            [NotNull, InstantHandle] Func<int, TVertex> vertexFactory,
            [NotNull, InstantHandle] Func<TVertex, TVertex, TEdge> edgeFactory,
            [NotNull] Random random)
            where TEdge : IEdge<TVertex>
        {
            var treeGraph = new BidirectionalGraph<TVertex, TEdge>(false, vertexCount);
            for (int i = 0; i < componentCount; ++i)
            {
                treeGraph.AddVertex(vertexFactory(i));
            }

            for (int n = treeGraph.VertexCount; n < vertexCount; ++n)
            {
                int parentIndex = random.Next(treeGraph.VertexCount);
                TVertex parent = treeGraph.Vertices.ElementAt(parentIndex);

                TVertex child = vertexFactory(n);
                treeGraph.AddVertex(child);
                treeGraph.AddEdge(edgeFactory(parent, child));
            }

            return treeGraph;
        }

        /// <summary>
        /// Creates a DAG graph.
        /// </summary>
        /// <param name="vertexCount">Total number of vertices.</param>
        /// <param name="edgeCount">Total number of edges.</param>
        /// <param name="maxParent">Maximum number of parent per vertex.</param>
        /// <param name="maxChild">Maximum number of child per vertex.</param>
        /// <param name="parallelEdgeAllowed">Allows parallel edges or not.</param>
        /// <param name="vertexFactory">Vertex factory.</param>
        /// <param name="edgeFactory">Edge factory.</param>
        /// <param name="random">Random number generator.</param>
        [Pure]
        [NotNull]
        public static IBidirectionalGraph<TVertex, TEdge> CreateDAG<TVertex, TEdge>(
            int vertexCount,
            int edgeCount,
            int maxParent,
            int maxChild,
            bool parallelEdgeAllowed,
            [NotNull, InstantHandle] Func<int, TVertex> vertexFactory,
            [NotNull, InstantHandle] Func<TVertex, TVertex, TEdge> edgeFactory,
            [NotNull] Random random)
            where TEdge : IEdge<TVertex>
        {
            var dagGraph = new BidirectionalGraph<TVertex, TEdge>(parallelEdgeAllowed, vertexCount);

            var verticesMap = new Dictionary<int, TVertex>();

            for (int i = 0; i < vertexCount; ++i)
            {
                TVertex vertex = vertexFactory(i);
                verticesMap[i] = vertex;
                dagGraph.AddVertex(vertex);
            }

            for (int i = 0; i < edgeCount; ++i)
            {
                TVertex parent;
                TVertex child;
                do
                {
                    int childIndex = random.Next(vertexCount - 1) + 1;
                    int parentIndex = random.Next(childIndex);
                    child = verticesMap[childIndex];
                    parent = verticesMap[parentIndex];
                } while (!parallelEdgeAllowed && dagGraph.ContainsEdge(parent, child)
                         || dagGraph.OutDegree(parent) >= maxChild
                         || dagGraph.InDegree(child) >= maxParent);

                // Create the edge between the 2 vertex
                dagGraph.AddEdge(edgeFactory(parent, child));
            }

            return dagGraph;
        }

        /// <summary>
        /// Creates a general graph.
        /// </summary>
        /// <param name="vertexCount">Total number of vertices.</param>
        /// <param name="edgeCount">Total number of edges.</param>
        /// <param name="maxDegree">Maximum degree per vertex.</param>
        /// <param name="parallelEdgeAllowed">Allows parallel edges or not.</param>
        /// <param name="vertexFactory">Vertex factory.</param>
        /// <param name="edgeFactory">Edge factory.</param>
        /// <param name="random">Random number generator.</param>
        [Pure]
        [NotNull]
        public static IBidirectionalGraph<TVertex, TEdge> CreateGeneralGraph<TVertex, TEdge>(
            int vertexCount,
            int edgeCount,
            int maxDegree,
            bool parallelEdgeAllowed,
            [NotNull, InstantHandle] Func<int, TVertex> vertexFactory,
            [NotNull, InstantHandle] Func<TVertex, TVertex, TEdge> edgeFactory,
            [NotNull] Random random)
            where TEdge : IEdge<TVertex>
        {
            var graph = new BidirectionalGraph<TVertex, TEdge>(parallelEdgeAllowed, vertexCount);

            var verticesMap = new Dictionary<int, TVertex>();

            for (int i = 0; i < vertexCount; ++i)
            {
                TVertex vertex = vertexFactory(i);
                verticesMap[i] = vertex;
                graph.AddVertex(vertex);
            }

            for (int i = 0; i < edgeCount; ++i)
            {
                int childIndex;
                int parentIndex;
                TVertex child;
                TVertex parent;
                do
                {
                    childIndex = random.Next(vertexCount);
                    parentIndex = random.Next(vertexCount);
                    child = verticesMap[childIndex];
                    parent = verticesMap[parentIndex];
                } while (childIndex == parentIndex
                         || !parallelEdgeAllowed && graph.ContainsEdge(parent, child)
                         || graph.Degree(parent) >= maxDegree
                         || graph.Degree(child) >= maxDegree);

                // Create the edge between the 2 vertex
                graph.AddEdge(edgeFactory(parent, child));
            }

            return graph;
        }

        /// <summary>
        /// Creates a <paramref name="vertexCount"/> complete graph.
        /// </summary>
        /// <param name="vertexCount">Total number of vertices.</param>
        /// <param name="vertexFactory">Vertex factory.</param>
        /// <param name="edgeFactory">Edge factory.</param>
        [Pure]
        [NotNull]
        public static IBidirectionalGraph<TVertex, TEdge> CreateCompleteGraph<TVertex, TEdge>(
            int vertexCount,
            [NotNull, InstantHandle] Func<int, TVertex> vertexFactory,
            [NotNull, InstantHandle] Func<TVertex, TVertex, TEdge> edgeFactory)
            where TEdge : IEdge<TVertex>
        {
            var graph = new BidirectionalGraph<TVertex, TEdge>(false, vertexCount);

            var verticesMap = new Dictionary<int, TVertex>();
            for (int i = 0; i < vertexCount; ++i)
            {
                TVertex vertex = vertexFactory(i);
                verticesMap[i] = vertex;
                graph.AddVertex(vertex);
            }

            for (int i = 0; i < vertexCount; ++i)
            {
                for (int j = 0; j < vertexCount; ++j)
                {
                    if (i != j)
                    {
                        graph.AddEdge(edgeFactory(verticesMap[i], verticesMap[j]));
                    }
                }
            }

            return graph;
        }
    }
}
