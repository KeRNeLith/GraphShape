using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using System.Diagnostics;
using JetBrains.Annotations;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        private class VertexLayer : List<SugiVertex>
        {
            /// <summary>
            /// Index of the layer.
            /// </summary>
            private readonly int _layerIndex;

            [NotNull]
            private readonly SoftMutableHierarchicalGraph<SugiVertex, SugiEdge> _graph;

            /// <summary>
            /// Height of the layer. (Equals with the height of the hightest vertex.)
            /// </summary>
            public double Height => ComputeHeight();

            /// <summary>
            /// List of the hierarchical edges comes into this layer.
            /// </summary>
            private IEnumerable<SugiEdge> UpEdges => this.SelectMany(vertex => _graph.InHierarchicalEdges(vertex));

            /// <summary>
            /// List of the hierarchical edges goes out from this layer.
            /// </summary>
            private IEnumerable<SugiEdge> DownEdges => this.SelectMany(vertex => _graph.OutHierarchicalEdges(vertex));

            public VertexLayer(
                [NotNull] SoftMutableHierarchicalGraph<SugiVertex, SugiEdge> graph,
                int layerIndex,
                [NotNull, ItemNotNull] IEnumerable<SugiVertex> vertices)
            {
                Debug.Assert(graph != null);
                Debug.Assert(vertices != null);

                _graph = graph;
                _layerIndex = layerIndex;
                AddRange(vertices);
            }

            #region Crosscounting

            [Pure]
            public int CalculateCrossCount(
                CrossCount crossCountDirection,
                bool sourcesByMeasure = false,
                bool targetsByMeasure = false)
            {
                int crossCount = 0;

                bool calculateUpCrossings = (crossCountDirection & CrossCount.Up) == CrossCount.Up;
                bool calculateDownCrossings = (crossCountDirection & CrossCount.Down) == CrossCount.Down;

                if (calculateUpCrossings)
                    crossCount += CalculateCrossings(UpEdges, sourcesByMeasure, targetsByMeasure);

                if (calculateDownCrossings)
                    crossCount += CalculateCrossings(DownEdges, sourcesByMeasure, targetsByMeasure);

                return crossCount;
            }

            [Pure]
            private static int CalculateCrossings(
                [NotNull, ItemNotNull] IEnumerable<SugiEdge> edges,
                bool sourcesByMeasure,
                bool targetsByMeasure)
            {
                SugiEdge[] edgeArray = edges.ToArray();
                int count = edgeArray.Length;
                int crossings = 0;
                for (int i = 0; i < count; ++i)
                {
                    SugiEdge edge1 = edgeArray[i];
                    for (int j = i + 1; j < count; ++j)
                    {
                        SugiEdge edge2 = edgeArray[j];
                        Debug.Assert(
                            edge1.Source.LayerIndex == edge2.Source.LayerIndex
                            && edge1.Target.LayerIndex == edge2.Target.LayerIndex,
                            "Bad edge at crossing computing",
                            $"{edge1}{Environment.NewLine}{edge2}");

                        // Get the position of the sources
                        double source1Pos;
                        double source2Pos;
                        if (sourcesByMeasure)
                        {
                            source1Pos = edge1.Source.Measure;
                            source2Pos = edge2.Source.Measure;
                        }
                        else
                        {
                            source1Pos = edge1.Source.Position;
                            source2Pos = edge2.Source.Position;
                        }

                        // Get the position of the targets
                        double target1Pos;
                        double target2Pos;
                        if (targetsByMeasure)
                        {
                            target1Pos = edge1.Target.Measure;
                            target2Pos = edge2.Target.Measure;
                        }
                        else
                        {
                            target1Pos = edge1.Target.Position;
                            target2Pos = edge2.Target.Position;
                        }

                        if ((source1Pos - source2Pos) * (target1Pos - target2Pos) < 0)
                            ++crossings;
                    }
                }

                return crossings;
            }

            #endregion

            #region Insert & Remove

            public new void Add([NotNull] SugiVertex vertex)
            {
                Debug.Assert(vertex != null);

                base.Add(vertex);
                vertex.LayerIndex = _layerIndex;
                ReassignPositions();
            }

            public new void AddRange([NotNull, ItemNotNull] IEnumerable<SugiVertex> vertices)
            {
                Debug.Assert(vertices != null);

                SugiVertex[] verticesArray = vertices as SugiVertex[] ?? vertices.ToArray();
                base.AddRange(verticesArray);
                foreach (SugiVertex vertex in verticesArray)
                    vertex.LayerIndex = _layerIndex;
                ReassignPositions();
            }

            public new void Remove([NotNull] SugiVertex vertex)
            {
                Debug.Assert(vertex != null);

                base.Remove(vertex);
                vertex.LayerIndex = SugiVertex.UndefinedLayerIndex;
            }

            #endregion

            #region Measuring

            /// <summary>
            /// Computes the measures for every vertex in the layer by the given barycenters.
            /// </summary>
            /// <param name="barycenters">The barycenters.</param>
            /// <param name="byRealPosition">If true, the barycenters will be computed based on the RealPosition.X value of the vertices. Otherwise the barycenter will be computed based on the value of the Position field (which is basically the index of the vertex inside the layer).</param>
            public void Measure(Barycenter barycenters, bool byRealPosition)
            {
                bool computeUpBarycenter = (barycenters & Barycenter.Up) == Barycenter.Up;
                bool computeDownBarycenter = (barycenters & Barycenter.Down) == Barycenter.Down;
                bool computeSubBarycenter = (barycenters & Barycenter.Sub) == Barycenter.Sub;

                int divCount = 0;
                if (computeUpBarycenter)
                    ++divCount;
                if (computeDownBarycenter)
                    ++divCount;
                if (computeSubBarycenter)
                    ++divCount;

                // Compute the measures for every vertex in the layer
                foreach (SugiVertex vertex in this)
                    Measure(vertex, computeUpBarycenter, computeDownBarycenter, computeSubBarycenter, divCount, byRealPosition);
            }

            /// <summary>
            /// Computes the measure for the given <paramref name="vertex"/>.
            /// </summary>
            private void Measure(
                [NotNull] SugiVertex vertex,
                bool computeUpBarycenter,
                bool computeDownBarycenter,
                bool computeSubBarycenter,
                int divCount,
                bool byRealPosition)
            {
                Debug.Assert(vertex != null);

                vertex.Measure = 0;

                if (computeUpBarycenter)
                    vertex.Measure += ComputeBarycenter(vertex, _graph.InHierarchicalEdges(vertex), byRealPosition);
                if (computeDownBarycenter)
                    vertex.Measure += ComputeBarycenter(vertex, _graph.OutHierarchicalEdges(vertex), byRealPosition);
                if (computeSubBarycenter)
                    vertex.Measure += ComputeBarycenter(vertex, _graph.GeneralEdgesFor(vertex), byRealPosition);

                vertex.Measure /= divCount;
            }

            /// <summary>
            /// Computes the barycenter of the given <paramref name="vertex"/>
            /// based on positions of the vertices on other side of the given <paramref name="edges"/>.
            /// </summary>
            /// <param name="vertex">The vertex which barycenter will be computed.</param>
            /// <param name="edges">The edges used for the computation.</param>
            /// <param name="byRealPosition"></param>
            /// <returns>The computed barycenter.</returns>
            private static double ComputeBarycenter(
                [NotNull] SugiVertex vertex,
                [NotNull, ItemNotNull] IEnumerable<SugiEdge> edges,
                bool byRealPosition)
            {
                Debug.Assert(vertex != null);
                Debug.Assert(edges != null);

                double barycenter = 0;
                int number = 0;

                foreach (SugiEdge edge in edges)
                {
                    if (byRealPosition)
                        barycenter += edge.GetOtherVertex(vertex).RealPosition.X;
                    else
                        barycenter += edge.GetOtherVertex(vertex).Position;
                    ++number;
                }

                if (number != 0)
                    return barycenter / number;
                return byRealPosition ? vertex.RealPosition.X : vertex.Position;
            }

            /// <summary>
            /// Computes the height of the vertexlayer (which is the maximum height of the vertices 
            /// in this layer).
            /// </summary>
            /// <returns>Returns with the computed height of the layer.</returns>
            private double ComputeHeight()
            {
                return this.Max(v => v.Size.Height);
            }

            #endregion

            #region Sort

            /// <returns>
            /// Returns with true if the vertices in this layer ordered by the given <paramref name="barycenters"/>.
            /// </returns>
            public bool IsOrderedByBarycenters(Barycenter barycenters, bool byRealPosition)
            {
                if (Count == 0)
                    return true;

                // Fill the measure by the given barycenters
                Measure(barycenters, byRealPosition);

                // Check that the ordering is valid
                for (int i = 1; i < Count; ++i)
                {
                    if (this[i].Measure < this[i - 1].Measure)
                        return false; // Invalid ordering
                }

                // The ordering is valid
                return true;
            }

            /// <summary>
            /// Sorts the vertices in the layer by it's measures.
            /// </summary>
            public void SortByMeasure()
            {
                // Sort the vertices by the measure
                Sort(MeasureComparer.Instance);

                // Reassign the positions of the vertices
                ReassignPositions();
            }

            private void SavePositionsToTemp()
            {
                foreach (SugiVertex vertex in this)
                    vertex.SavePositionToTemp();
            }

            private void LoadPositionsFromTemp()
            {
                foreach (SugiVertex vertex in this)
                    vertex.LoadPositionFromTemp();
            }

            /// <returns>
            /// Returns true if the vertices have been swapped, 
            /// otherwise (no more permutation) returns with false.</returns>
            [Pure]
            private static bool Swap([NotNull, ItemNotNull] IList<SugiVertex> vertices)
            {
                Debug.Assert(vertices != null);

                // Do the initial ordering
                int n = vertices.Count;
                int i;
                int j;

                // Find place to start
                for (i = n - 1;
                    i > 0 && vertices[i - 1].PermutationIndex >= vertices[i].PermutationIndex;
                    --i)
                {
                }

                // All in reverse order
                if (i < 1)
                    return false; // No more permutation

                // Do next permutation
                for (j = n;
                    j > 1 && vertices[j - 1].PermutationIndex <= vertices[i - 1].PermutationIndex;
                    --j)
                {
                }

                // Swap values i-1, j-1
                int c = vertices[i - 1].PermutationIndex;
                vertices[i - 1].PermutationIndex = vertices[j - 1].PermutationIndex;
                vertices[j - 1].PermutationIndex = c;

                // Need more swaps
                for (i++, j = n; i < j; ++i, --j)
                {
                    c = vertices[i - 1].PermutationIndex;
                    vertices[i - 1].PermutationIndex = vertices[j - 1].PermutationIndex;
                    vertices[j - 1].PermutationIndex = c;
                }

                return true; // New permutation generated
            }

            /// <summary>
            /// Changes the order of the vertices with the same measure.
            /// It does that in the brute-force way (every permutation will be analyzed).
            /// Vertices should be sorted by it's measures.
            /// </summary>
            public void FindBestPermutation(CrossCount crossCounting)
            {
                int bestKnownCrossCount = CalculateCrossCount(crossCounting);

                // Get the vertices with the same index
                var verticesWithSameMeasure = new List<SugiVertex>();
                int startIndex, endIndex;
                for (startIndex = 0; startIndex < Count; startIndex = endIndex + 1)
                {
                    for (endIndex = startIndex + 1;
                        endIndex < Count && NearEqual(this[startIndex].Measure, this[endIndex].Measure);
                        ++endIndex)
                    {
                    }
                    --endIndex;

                    if (endIndex > startIndex)
                    {
                        for (int i = startIndex; i <= endIndex; ++i)
                            verticesWithSameMeasure.Add(this[i]);
                    }
                }

                // Save the original positions
                SavePositionsToTemp();

                // Null PermutationIndex
                foreach (SugiVertex vertex in this)
                    vertex.PermutationIndex = 0;

                // Create initial permutation
                foreach (SugiVertex vertex in verticesWithSameMeasure)
                    vertex.PermutationIndex = 0;

                while (Swap(verticesWithSameMeasure))
                {
                    // Sort the vertices with the same measure by barycenter
                    Sort(MeasureAndPermutationIndexComparer.Instance);
                    ReassignPositions();

                    int newCrossCount = CalculateCrossCount(crossCounting);
                    if (newCrossCount < bestKnownCrossCount)
                    {
                        SavePositionsToTemp();
                        bestKnownCrossCount = newCrossCount;
                    }
                }

                // The best solution is in the temp
                LoadPositionsFromTemp();

                Sort(PositionComparer.Instance);
                ReassignPositions();
            }

            /// <summary>
            /// Reassigns the position of vertices to it's indexes in the vertexlayer.
            /// </summary>
            private void ReassignPositions()
            {
                int index = 0;
                foreach (SugiVertex vertex in this)
                    vertex.Position = index++;
            }

            #endregion

            #region Comparers

            private class MeasureComparer : IComparer<SugiVertex>
            {
                #region Singleton management

                private MeasureComparer()
                {
                }

                /// <summary>
                /// Gets the cache instance.
                /// </summary>
                public static MeasureComparer Instance { get; } = InstanceHandler.InternalInstance;

                private static class InstanceHandler
                {
                    // Explicit static constructor to tell C# compiler
                    // not to mark type as beforefieldinit
                    static InstanceHandler()
                    {
                    }

                    internal static readonly MeasureComparer InternalInstance = new MeasureComparer();
                }

                #endregion

                /// <inheritdoc />
                public int Compare(SugiVertex x, SugiVertex y)
                {
                    // ReSharper disable PossibleNullReferenceException
                    // Comparing only non null vertices
                    return Math.Sign((sbyte)(x.Measure - y.Measure));
                    // ReSharper restore PossibleNullReferenceException
                }
            }

            private class PositionComparer : IComparer<SugiVertex>
            {
                #region Singleton management

                private PositionComparer()
                {
                }

                /// <summary>
                /// Gets the cache instance.
                /// </summary>
                public static PositionComparer Instance { get; } = InstanceHandler.InternalInstance;

                private static class InstanceHandler
                {
                    // Explicit static constructor to tell C# compiler
                    // not to mark type as beforefieldinit
                    static InstanceHandler()
                    {
                    }

                    internal static readonly PositionComparer InternalInstance = new PositionComparer();
                }

                #endregion

                /// <inheritdoc />
                public int Compare(SugiVertex x, SugiVertex y)
                {
                    // ReSharper disable PossibleNullReferenceException
                    // Comparing only non null vertices
                    return Math.Sign((sbyte)(x.Position - y.Position));
                    // ReSharper restore PossibleNullReferenceException
                }
            }

            private class MeasureAndPermutationIndexComparer : IComparer<SugiVertex>
            {
                #region Singleton management

                private MeasureAndPermutationIndexComparer()
                {
                }

                /// <summary>
                /// Gets the cache instance.
                /// </summary>
                public static MeasureAndPermutationIndexComparer Instance { get; } = InstanceHandler.InternalInstance;

                private static class InstanceHandler
                {
                    // Explicit static constructor to tell C# compiler
                    // not to mark type as beforefieldinit
                    static InstanceHandler()
                    {
                    }

                    internal static readonly MeasureAndPermutationIndexComparer InternalInstance = new MeasureAndPermutationIndexComparer();
                }

                #endregion

                /// <inheritdoc />
                public int Compare(SugiVertex x, SugiVertex y)
                {
                    // ReSharper disable PossibleNullReferenceException
                    // Comparing only non null vertices
                    int sign = Math.Sign((sbyte)(x.Measure - y.Measure));
                    if (sign == 0)
                        return Math.Sign((sbyte)(x.PermutationIndex - y.PermutationIndex));
                    return sign;
                    // ReSharper restore PossibleNullReferenceException
                }
            }

            #endregion

            #region Priorities

            public void CalculateSubPriorities()
            {
                SugiVertex[] orderedVertices =
                    (from v in this orderby v.Priority, v.Measure, v.Position select v).ToArray();

                // Calculate sub-priorities
                int startIndex = 0;
                while (startIndex < orderedVertices.Length)
                {
                    int endIndex = startIndex + 1;

                    // Get the vertices with the same priorities and measure
                    while (endIndex < orderedVertices.Length
                           && orderedVertices[startIndex].Priority == orderedVertices[endIndex].Priority
                           && NearEqual(orderedVertices[startIndex].Measure, orderedVertices[endIndex].Measure))
                    {
                        ++endIndex;
                    }
                    --endIndex;

                    // Set the sub-priorities
                    int count = endIndex - startIndex + 1;
                    var border = (int)Math.Ceiling(count / (float)2.0);
                    int subPriority = count - border;
                    for (int i = 0; i < count; ++i)
                    {
                        orderedVertices[startIndex + i].SubPriority = count - Math.Abs(subPriority);
                        --subPriority;
                    }

                    // Go to the next group of vertices with the same priorities
                    startIndex = endIndex + 1;
                }
            }

            #endregion
        }
    }
}