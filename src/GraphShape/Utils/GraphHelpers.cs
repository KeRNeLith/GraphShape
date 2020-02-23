using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;
using QuikGraph.Algorithms;

namespace GraphShape.Utils
{
    /// <summary>
    /// Helpers to deal with graphs.
    /// </summary>
    public static class GraphHelpers
    {
        /// <summary>
        /// Gets the neighbors (adjacent vertices) of the <paramref name="vertex"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="vertex">The vertex of which we want to get neighbors.</param>
        /// <returns>Adjacent vertices of the <paramref name="vertex"/>.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> GetNeighbors<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph,
            [NotNull] TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            return graph.InEdges(vertex).Select(e => e.Source)
                .Concat(graph.OutEdges(vertex).Select(e => e.Target))
                .Distinct();
        }

        /// <summary>
        /// Gets the out neighbors (only adjacent vertices from out-edges) of the <paramref name="vertex"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="vertex">The vertex of which we want to get neighbors.</param>
        /// <returns>Adjacent vertices of the <paramref name="vertex"/>.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> GetOutNeighbors<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            return graph.OutEdges(vertex).Select(e => e.Target).Distinct();
        }

        /// <summary>
        /// Returns every edges which source is one of the vertices in the <paramref name="set1"/>
        /// and the target is one of the vertices in the <paramref name="set2"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="set1">Source vertices set.</param>
        /// <param name="set2">Target vertices set</param>
        /// <returns>Edges with a source in <paramref name="set1"/> and a target in <paramref name="set2"/>.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TEdge> GetEdgesBetween<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] TVertex[] set1,
            [NotNull, ItemNotNull] TVertex[] set2)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (set1 is null)
                throw new ArgumentNullException(nameof(set1));
            if (set2 is null)
                throw new ArgumentNullException(nameof(set2));

            foreach (TVertex vertex in set1)
            {
                foreach (TEdge edge in graph.OutEdges(vertex))
                {
                    if (set2.Contains(edge.Target))
                        yield return edge;
                }
            }
        }

        /// <summary>
        /// Gets the distances between the vertices of the <paramref name="graph"/>.
        /// Note: The distance is the number of edges between 2 vertices.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <returns>The distances between every vertex-pair.</returns>
        [Pure]
        [NotNull]
        public static double[,] GetDistances<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph)
            where TEdge : IEdge<TVertex>
            where TGraph : IBidirectionalGraph<TVertex, TEdge>
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            var distances = new double[graph.VertexCount, graph.VertexCount];
            for (int k = 0; k < graph.VertexCount; ++k)
            {
                for (int j = 0; j < graph.VertexCount; ++j)
                {
                    distances[k, j] = double.PositiveInfinity;
                }
            }

            var undirected = new UndirectedBidirectionalGraph<TVertex, TEdge>(graph);
            // Compute the distances from every vertex: O(n(n^2 + e)) complexity
            int i = 0;
            foreach (TVertex source in graph.Vertices)
            {
                // Compute the distances from the 'source'
                // Each edge is taken into account as 1 (unweighted)
                var dijkstra = new UndirectedDijkstraShortestPathAlgorithm<TVertex, TEdge>(
                    undirected,
                    edge => 1.0,
                    DistanceRelaxers.ShortestDistance);
                dijkstra.Compute(source);

                int j = 0;
                foreach (TVertex vertex in undirected.Vertices)
                {
                    double distance = dijkstra.Distances[vertex];
                    distances[i, j] = Math.Min(distances[i, j], distance);
                    distances[i, j] = Math.Min(distances[i, j], distances[j, i]);
                    distances[j, i] = Math.Min(distances[i, j], distances[j, i]);
                    ++j;
                }
                ++i;
            }

            return distances;
        }

        /// <summary>
        /// Gets the diameter of the <paramref name="graph"/>.
        /// Note: The diameter is the greatest distance between two vertices.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <returns>The diameter of the <paramref name="graph"/>.</returns>
        [Pure]
        public static double GetDiameter<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph)
            where TEdge : IEdge<TVertex>
            where TGraph : IBidirectionalGraph<TVertex, TEdge>
        {
            return graph.GetDiameter<TVertex, TEdge, TGraph>(out _);
        }

        /// <summary>
        /// Gets the diameter of the <paramref name="graph"/>.
        /// Note: The diameter is the greatest distance between two vertices.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="distances">Will give distances between every vertex-pair.</param>
        /// <returns>The diameter of the <paramref name="graph"/>.</returns>
        [Pure]
        public static double GetDiameter<TVertex, TEdge, TGraph>(
            [NotNull] this TGraph graph,
            [NotNull] out double[,] distances)
            where TEdge : IEdge<TVertex>
            where TGraph : IBidirectionalGraph<TVertex, TEdge>
        {
            distances = GetDistances<TVertex, TEdge, TGraph>(graph);

            int n = graph.VertexCount;
            double distance = double.NegativeInfinity;
            for (int i = 0; i < n - 1; ++i)
            {
                for (int j = i + 1; j < n; ++j)
                {
                    if (Math.Abs(double.MaxValue - distances[i, j]) < double.Epsilon)
                        continue;

                    distance = Math.Max(distance, distances[i, j]);
                }
            }

            return distance;
        }

        #region Graph manipulations

        /// <summary>
        /// Creates a <see cref="BidirectionalGraph{TVertex,TEdge}"/> with the given <paramref name="vertices"/>
        /// and edges constructed by getting values of properties <paramref name="sourcePropertyName"/>
        /// and <paramref name="targetPropertyName"/> on type <typeparamref name="TEdgeData"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdgeData">Type of the object used to construct edges.</typeparam>
        /// <param name="vertices">The set of the vertices.</param>
        /// <param name="edgesData">The set of data used to construct graph edges.</param>
        /// <param name="sourcePropertyName">
        /// Name of the property to get value from <typeparamref name="TEdgeData"/> to construct edge source.
        /// </param>
        /// <param name="targetPropertyName">
        /// Name of the property to get value from <typeparamref name="TEdgeData"/> to construct edge target.
        /// </param>
        /// <returns>A <see cref="BidirectionalGraph{TVertex,TEdge}"/>.</returns>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, Edge<TVertex>> CreateGraph<TVertex, TEdgeData>(
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices,
            [NotNull, ItemNotNull] IEnumerable<TEdgeData> edgesData,
            [NotNull] string sourcePropertyName,
            [NotNull] string targetPropertyName)
        {
            if (edgesData is null)
                throw new ArgumentNullException(nameof(edgesData));

            var graph = new BidirectionalGraph<TVertex, Edge<TVertex>>();
            graph.AddVertexRange(vertices);

            // Get the property infos
            PropertyInfo sourceProperty = typeof(TEdgeData).GetProperty(sourcePropertyName)
                ?? throw new ArgumentException(
                    $"No source property named {sourcePropertyName} on type {typeof(TEdgeData).Name}.",
                    nameof(sourcePropertyName));
            if (!typeof(TVertex).IsAssignableFrom(sourceProperty.PropertyType))
            {
                throw new ArgumentException(
                    $"Type of property {sourcePropertyName} is not assignable to type {typeof(TVertex).Name}.",
                    nameof(sourcePropertyName));
            }

            PropertyInfo targetProperty = typeof(TEdgeData).GetProperty(targetPropertyName)
                ?? throw new ArgumentException(
                    $"No source property named {targetPropertyName} on type {typeof(TEdgeData).Name}.",
                    nameof(targetPropertyName));
            if (!typeof(TVertex).IsAssignableFrom(targetProperty.PropertyType))
            {
                throw new ArgumentException(
                    $"Type of property {targetPropertyName} is not assignable to type {typeof(TVertex).Name}.",
                    nameof(targetPropertyName));
            }

            // Create the new edges
            foreach (TEdgeData data in edgesData)
            {
                var edge = new Edge<TVertex>(
                    (TVertex)sourceProperty.GetValue(data, null),
                    (TVertex)targetProperty.GetValue(data, null));
                graph.AddEdge(edge);
            }

            return graph;
        }

        /// <summary>
        /// Creates a <see cref="BidirectionalGraph{TVertex,TEdgeTo}"/> with the given <paramref name="vertices"/>
        /// and edges constructed using <paramref name="edgeFactory"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TEdgeData">Type of the object used to construct edges.</typeparam>
        /// <param name="vertices">The set of the vertices.</param>
        /// <param name="edgesData">The set of data used to construct graph edges.</param>
        /// <param name="edgeFactory">Factory method to convert an edge data into an edge.</param>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <returns>A <see cref="BidirectionalGraph{TVertex,TEdge}"/>.</returns>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> CreateGraph<TVertex, TEdge, TEdgeData>(
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices,
            [NotNull, ItemNotNull] IEnumerable<TEdgeData> edgesData,
            [NotNull, InstantHandle] Func<TEdgeData, TEdge> edgeFactory,
            bool allowParallelEdges)
            where TEdge : IEdge<TVertex>
        {
            if (edgesData is null)
                throw new ArgumentNullException(nameof(edgesData));
            if (edgeFactory is null)
                throw new ArgumentNullException(nameof(edgeFactory));

            var graph = new BidirectionalGraph<TVertex, TEdge>(allowParallelEdges);
            graph.AddVertexRange(vertices);

            // Create the edges
            foreach (TEdgeData data in edgesData)
            {
                TEdge edge = edgeFactory(data);
                graph.AddEdge(edge);
            }

            return graph;
        }

        /// <summary>
        /// Creates a <see cref="BidirectionalGraph{TVertex,TEdgeTo}"/> with the given <paramref name="vertices"/>
        /// and edges constructed using <paramref name="edgeFactory"/>.
        /// Note: The graph will allow parallel edges.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TEdgeData">Type of the object used to construct edges.</typeparam>
        /// <param name="vertices">The set of the vertices.</param>
        /// <param name="edgesData">The set of data used to construct graph edges.</param>
        /// <param name="edgeFactory">Factory method to convert an edge data into an edge.</param>
        /// <returns>A <see cref="BidirectionalGraph{TVertex,TEdge}"/>.</returns>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> CreateGraph<TVertex, TEdge, TEdgeData>(
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices,
            [NotNull, ItemNotNull] IEnumerable<TEdgeData> edgesData,
            [NotNull, InstantHandle] Func<TEdgeData, TEdge> edgeFactory)
            where TEdge : IEdge<TVertex>
        {
            return CreateGraph(vertices, edgesData, edgeFactory, true);
        }

        /// <summary>
        /// Converts the <paramref name="oldGraph"/> into the <paramref name="newGraph"/>.
        /// Uses <paramref name="vertexConverter"/> and <paramref name="edgeConverter"/> to
        /// convert vertices and edges if provided. Performs a direct cast otherwise.
        /// </summary>
        /// <typeparam name="TOldVertex">Old vertex type.</typeparam>
        /// <typeparam name="TNewVertex">New vertex type.</typeparam>
        /// <typeparam name="TOldEdge">Old edge type.</typeparam>
        /// <typeparam name="TNewEdge">New edge type.</typeparam>
        /// <typeparam name="TNewGraph">Converted graph type.</typeparam>
        /// <param name="oldGraph">Graph to convert.</param>
        /// <param name="newGraph">Graph that will be filled with converted content.</param>
        /// <param name="vertexConverter">Function to convert vertices from <typeparamref name="TOldVertex"/> to <typeparamref name="TNewVertex"/>.</param>
        /// <param name="edgeConverter">Function to convert edges from <typeparamref name="TOldEdge"/> to <typeparamref name="TNewEdge"/>.</param>
        /// <returns>The converted graph.</returns>
        [Pure]
        [NotNull]
        public static TNewGraph Convert<TOldVertex, TOldEdge, TNewVertex, TNewEdge, TNewGraph>(
            [NotNull] this IVertexAndEdgeListGraph<TOldVertex, TOldEdge> oldGraph,
            [NotNull] TNewGraph newGraph,
            [CanBeNull, InstantHandle] Func<TOldVertex, TNewVertex> vertexConverter,
            [CanBeNull, InstantHandle] Func<TOldEdge, TNewEdge> edgeConverter)
            where TOldEdge : IEdge<TOldVertex>
            where TNewEdge : IEdge<TNewVertex>
            where TNewGraph : IMutableVertexAndEdgeListGraph<TNewVertex, TNewEdge>
        {
            if (oldGraph is null)
                throw new ArgumentNullException(nameof(oldGraph));
            if (newGraph == null)
                throw new ArgumentNullException(nameof(newGraph));

            // Vertices
            newGraph.AddVertexRange(vertexConverter != null
                ? oldGraph.Vertices.Select(vertexConverter)
                : oldGraph.Vertices.Cast<TNewVertex>());

            // Edges
            newGraph.AddEdgeRange(edgeConverter != null
                ? oldGraph.Edges.Select(edgeConverter)
                : oldGraph.Edges.Cast<TNewEdge>());

            return newGraph;
        }

        /// <summary>
        /// Converts the <paramref name="oldGraph"/> into the <paramref name="newGraph"/>.
        /// Uses <paramref name="edgeConverter"/> to convert edges if provided. Performs a direct cast otherwise.
        /// </summary>
        /// <typeparam name="TVertex">Old vertex type.</typeparam>
        /// <typeparam name="TOldEdge">Old edge type.</typeparam>
        /// <typeparam name="TNewEdge">New edge type.</typeparam>
        /// <typeparam name="TNewGraph">Converted graph type.</typeparam>
        /// <param name="oldGraph">Graph to convert.</param>
        /// <param name="newGraph">Graph that will be filled with converted content.</param>
        /// <param name="edgeConverter">Function to convert edges from <typeparamref name="TOldEdge"/> to <typeparamref name="TNewEdge"/>.</param>
        /// <returns>The converted graph.</returns>
        [Pure]
        [NotNull]
        public static TNewGraph Convert<TVertex, TOldEdge, TNewEdge, TNewGraph>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TOldEdge> oldGraph,
            [NotNull] TNewGraph newGraph,
            [CanBeNull, InstantHandle] Func<TOldEdge, TNewEdge> edgeConverter)
            where TOldEdge : IEdge<TVertex>
            where TNewEdge : IEdge<TVertex>
            where TNewGraph : IMutableVertexAndEdgeListGraph<TVertex, TNewEdge>
        {
            return oldGraph.Convert<TVertex, TOldEdge, TVertex, TNewEdge, TNewGraph>(
                newGraph,
                null,
                edgeConverter);
        }

        /// <summary>
        /// Converts the <paramref name="oldGraph"/> into the <paramref name="newGraph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Old vertex type.</typeparam>
        /// <typeparam name="TEdge">Old edge type.</typeparam>
        /// <typeparam name="TNewGraph">Converted graph type.</typeparam>
        /// <param name="oldGraph">Graph to convert.</param>
        /// <param name="newGraph">Graph that will be filled with converted content.</param>
        /// <returns>The converted graph.</returns>
        [Pure]
        [NotNull]
        public static TNewGraph Convert<TVertex, TEdge, TNewGraph>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> oldGraph,
            [NotNull] TNewGraph newGraph)
            where TEdge : IEdge<TVertex>
            where TNewGraph : IMutableVertexAndEdgeListGraph<TVertex, TEdge>
        {
            return oldGraph.Convert<TVertex, TEdge, TVertex, TEdge, TNewGraph>(
                newGraph,
                null,
                null);
        }

        /// <summary>
        /// Converts the <paramref name="oldGraph"/> into a <see cref="BidirectionalGraph{TVertex,TEdge}"/>.
        /// Uses <paramref name="vertexConverter"/> and <paramref name="edgeConverter"/> to
        /// convert vertices and edges if provided. Performs a direct cast otherwise.
        /// </summary>
        /// <typeparam name="TOldVertex">Old vertex type.</typeparam>
        /// <typeparam name="TNewVertex">New vertex type.</typeparam>
        /// <typeparam name="TOldEdge">Old edge type.</typeparam>
        /// <typeparam name="TNewEdge">New edge type.</typeparam>
        /// <param name="oldGraph">Graph to convert.</param>
        /// <param name="vertexConverter">Function to convert vertices from <typeparamref name="TOldVertex"/> to <typeparamref name="TNewVertex"/>.</param>
        /// <param name="edgeConverter">Function to convert edges from <typeparamref name="TOldEdge"/> to <typeparamref name="TNewEdge"/>.</param>
        /// <returns>The converted graph.</returns>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TNewVertex, TNewEdge> Convert<TOldVertex, TOldEdge, TNewVertex, TNewEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TOldVertex, TOldEdge> oldGraph,
            [CanBeNull, InstantHandle] Func<TOldVertex, TNewVertex> vertexConverter,
            [CanBeNull, InstantHandle] Func<TOldEdge, TNewEdge> edgeConverter)
            where TOldEdge : IEdge<TOldVertex>
            where TNewEdge : IEdge<TNewVertex>
        {
            if (oldGraph is null)
                throw new ArgumentNullException(nameof(oldGraph));
            return oldGraph.Convert(
                new BidirectionalGraph<TNewVertex, TNewEdge>(oldGraph.AllowParallelEdges, oldGraph.VertexCount),
                vertexConverter,
                edgeConverter);
        }

        /// <summary>
        /// Converts the <paramref name="oldGraph"/> into a <see cref="BidirectionalGraph{TVertex,TEdge}"/>.
        /// Uses <paramref name="edgeConverter"/> to convert edges if provided. Performs a direct cast otherwise.
        /// </summary>
        /// <typeparam name="TVertex">Old vertex type.</typeparam>
        /// <typeparam name="TOldEdge">Old edge type.</typeparam>
        /// <typeparam name="TNewEdge">New edge type.</typeparam>
        /// <param name="oldGraph">Graph to convert.</param>
        /// <param name="edgeConverter">Function to convert edges from <typeparamref name="TOldEdge"/> to <typeparamref name="TNewEdge"/>.</param>
        /// <returns>The converted graph.</returns>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TNewEdge> Convert<TVertex, TOldEdge, TNewEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TOldEdge> oldGraph,
            [CanBeNull, InstantHandle] Func<TOldEdge, TNewEdge> edgeConverter)
            where TOldEdge : IEdge<TVertex>
            where TNewEdge : IEdge<TVertex>
        {
            return oldGraph.Convert<TVertex, TOldEdge, TVertex, TNewEdge>(
                null,
                edgeConverter);
        }

        /// <summary>
        /// Copies this graph into a <see cref="BidirectionalGraph{TVertex,TEdge}"/> one.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to copy.</param>
        /// <returns><see cref="BidirectionalGraph{TVertex,TEdge}"/> initialized from <paramref name="graph"/>.</returns>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> CopyToBidirectionalGraph<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            var newGraph = new BidirectionalGraph<TVertex, TEdge>();

            newGraph.AddVertexRange(graph.Vertices);
            newGraph.AddEdgeRange(graph.Edges);

            return newGraph;
        }

        #endregion
    }
}