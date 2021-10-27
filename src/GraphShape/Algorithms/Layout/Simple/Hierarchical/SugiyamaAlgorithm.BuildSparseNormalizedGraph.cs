using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        private sealed class WHOptimizationLayerInfo
        {
            public double LayerWidth { get; set; }
            public double LayerHeight { get; set; }

            [NotNull, ItemNotNull]
            public readonly Queue<WHOptimizationVertexInfo> Vertices = new Queue<WHOptimizationVertexInfo>();
        }

        private sealed class WHOptimizationVertexInfo
        {
            [NotNull]
            public readonly SugiVertex Vertex;
            public readonly double Value;
            public readonly double Cost;

            public double ValuePerCost
            {
                get
                {
                    if (Value < 0)
                        return double.NaN;
                    if (Cost <= 0)
                        return double.PositiveInfinity;
                    return Value / Cost;
                }
            }

            public WHOptimizationVertexInfo(
                [NotNull] SugiVertex vertex,
                double value,
                double cost)
            {
                Vertex = vertex;
                Value = value;
                Cost = cost;
            }
        }

        private double _actualWidth;
        private double _actualHeight;

        private double ActualWidthPerHeight => _actualWidth / _actualHeight;

        [NotNull, ItemNotNull]
        private readonly IList<WHOptimizationLayerInfo> _whOptLayerInfos =
            new List<WHOptimizationLayerInfo>();

        [NotNull]
        private readonly IDictionary<SugiVertex, WHOptimizationVertexInfo> _whOptVertexInfos =
            new Dictionary<SugiVertex, WHOptimizationVertexInfo>();

        /// <summary>
        /// From the original graph it creates a sparse normalized graph
        /// with segments and dummy vertices (p-vertex, q-vertex, s-vertex).
        /// </summary>
        private void BuildSparseNormalizedGraph()
        {
            CreateInitialLayering();
            if (Parameters.OptimizeWidth)
                DoWidthAndHeightOptimization();
            CreateDummyVerticesAndSegments();
        }

        private void CreateInitialLayering()
        {
            var layerTopologicalSort = new LayeredTopologicalSortAlgorithm<SugiVertex, SugiEdge>(_graph);
            layerTopologicalSort.Compute();

            for (int i = 0; i < layerTopologicalSort.LayerCount; ++i)
            {
                // Set the layer
                _layers.Add(layerTopologicalSort.Layers[i].ToList());

                // Assign the layer indexes
                foreach (SugiVertex vertex in _layers[i])
                {
                    ThrowIfCancellationRequested();

                    vertex.LayerIndex = i;
                }
            }

            // Minimize edge length
            if (Parameters.MinimizeEdgeLength)
            {
                MinimizeInitialLayersEdgeLength();
            }
        }

        private void MinimizeInitialLayersEdgeLength()
        {
            for (int i = _layers.Count - 1; i >= 0; --i)
            {
                IList<SugiVertex> layer = _layers[i];
                foreach (SugiVertex vertex in layer.ToArray())
                {
                    ThrowIfCancellationRequested();
                    if (_graph.OutDegree(vertex) == 0)
                        continue;

                    // Put the vertex above the descendant on the highest layer
                    int newLayerIndex = _graph.OutEdges(vertex).Min(edge => edge.Target.LayerIndex - 1);

                    if (newLayerIndex != vertex.LayerIndex)
                    {
                        // We're changing layer
                        layer.Remove(vertex);
                        _layers[newLayerIndex].Add(vertex);
                        vertex.LayerIndex = newLayerIndex;
                    }
                }
            }
        }

        private void DoWidthAndHeightOptimization()
        {
            CreateVertexWHOptimizationInfos();
            CreateLayerWHOptimizationInfos();

            if (ActualWidthPerHeight <= Parameters.WidthPerHeight)
                return;

            bool optimized;
            do
            {
                optimized = DoWHOptimizationStep();
            } while (optimized);
            RewriteLayerIndexes();
        }

        /// <summary>
        /// Replaces long edge (span(edge) > 1) with dummy vertices
        /// or segments (span(edge) > 2).
        /// </summary>
        private void CreateDummyVerticesAndSegments()
        {
            foreach (SugiEdge edge in _graph.Edges.ToArray())
            {
                int sourceLayerIndex = edge.Source.LayerIndex;
                int targetLayerIndex = edge.Target.LayerIndex;
                int span = targetLayerIndex - sourceLayerIndex;
                if (span < 1)
                    throw new InvalidOperationException("Span cannot be lower than 1.");

                if (span == 1)
                    continue;

                _graph.RemoveEdge(edge);
                bool notReversed = edge.Source.OriginalVertex == edge.OriginalEdge.Source && edge.Target.OriginalVertex == edge.OriginalEdge.Target;
                var dummyVertexList = new List<SugiVertex>();
                _dummyVerticesOfEdges[edge.OriginalEdge] = dummyVertexList;
                if (span == 2)
                {
                    // Insert an R-vertex
                    SugiVertex rVertex = AddDummyVertex(VertexTypes.RVertex, sourceLayerIndex + 1);

                    _graph.AddEdge(new SugiEdge(edge.OriginalEdge, edge.Source, rVertex));
                    _graph.AddEdge(new SugiEdge(edge.OriginalEdge, rVertex, edge.Target));

                    dummyVertexList.Add(rVertex);
                }
                else
                {
                    // Insert a P-vertex, a Q-vertex
                    SugiVertex pVertex = AddDummyVertex(VertexTypes.PVertex, sourceLayerIndex + 1);
                    SugiVertex qVertex = AddDummyVertex(VertexTypes.QVertex, targetLayerIndex - 1);

                    if (notReversed)
                    {
                        dummyVertexList.Add(pVertex);
                        dummyVertexList.Add(qVertex);
                    }
                    else
                    {
                        dummyVertexList.Add(qVertex);
                        dummyVertexList.Add(pVertex);
                    }

                    _graph.AddEdge(new SugiEdge(edge.OriginalEdge, edge.Source, pVertex));
                    _graph.AddEdge(new SugiEdge(edge.OriginalEdge, qVertex, edge.Target));
                    AddSegment(pVertex, qVertex);
                }
            }
        }

        /// <summary>
        /// Adds a new segment to the sparse compaction graph.
        /// </summary>
        /// <param name="pVertex">The source vertex of the segment.</param>
        /// <param name="qVertex">The target vertex of the segment.</param>
        /// <returns>The newly created segment.</returns>
        private static void AddSegment([NotNull] SugiVertex pVertex, [NotNull] SugiVertex qVertex)
        {
            Debug.Assert(pVertex != null);
            Debug.Assert(qVertex != null);

            var segment = new Segment(pVertex, qVertex);

            pVertex.Segment = segment;
            qVertex.Segment = segment;
        }

        /// <summary>
        /// Adds a dummy vertex to the sparse compaction graph.
        /// </summary>
        /// <param name="type">The type of the dummy vertex (p,q,r).</param>
        /// <param name="layerIndex">The index of the layer of the vertex.</param>
        /// <returns>The new vertex which has been added to the graph and the layers.</returns>
        [NotNull]
        private SugiVertex AddDummyVertex(VertexTypes type, int layerIndex)
        {
            var vertex = new SugiVertex(type)
            {
                LayerIndex = layerIndex
            };
            _layers[layerIndex].Add(vertex);
            _graph.AddVertex(vertex);

            return vertex;
        }

        private void RewriteLayerIndexes()
        {
            int i = 0;
            foreach (IList<SugiVertex> layer in _layers)
            {
                foreach (SugiVertex vertex in layer)
                {
                    ThrowIfCancellationRequested();

                    vertex.LayerIndex = i;
                }
                ++i;
            }
        }

        private bool DoWHOptimizationStep()
        {
            double desiredWidth = _actualHeight * Parameters.WidthPerHeight;

            int maxWidthLayerIndex = 0;
            var maxWidthLayer = _whOptLayerInfos[0];
            for (int i = 0; i < _whOptLayerInfos.Count; ++i)
            {
                if (_whOptLayerInfos[i].LayerWidth > maxWidthLayer.LayerWidth
                    && _whOptLayerInfos[i].Vertices.Count > 0
                    && _whOptLayerInfos[i].LayerWidth > desiredWidth)
                {
                    maxWidthLayer = _whOptLayerInfos[i];
                    maxWidthLayerIndex = i;
                }
            }

            if (maxWidthLayer.LayerWidth <= desiredWidth || maxWidthLayer.Vertices.Count <= 0)
                return false;

            // Insert a new layer
            int insertedLayerIndex = maxWidthLayerIndex + 1;
            double width = 0;
            double c = 0;
            if (insertedLayerIndex > 0)
            {
                foreach (SugiVertex vertex in _layers[insertedLayerIndex - 1])
                {
                    width += Math.Max(0, _graph.OutDegree(vertex) - 1) * Parameters.LayerGap;
                }

                ++c;
            }
            if (insertedLayerIndex < _layers.Count - 1)
            {
                foreach (SugiVertex vertex in _layers[insertedLayerIndex])
                {
                    width += Math.Max(0, _graph.OutDegree(vertex) - 1) * Parameters.LayerGap;
                }

                ++c;
            }

            if (c > 0)
                width /= c;

            if (width >= desiredWidth - _whOptLayerInfos[insertedLayerIndex - 1].Vertices.Peek().Cost)
                return false;

            var insertedLayerInfo = new WHOptimizationLayerInfo();
            var insertedLayer = new List<SugiVertex>();
            _whOptLayerInfos.Insert(insertedLayerIndex, insertedLayerInfo);
            _layers.Insert(insertedLayerIndex, insertedLayer);

            double height = 0.0;
            while (insertedLayerInfo.LayerWidth < _whOptLayerInfos[insertedLayerIndex - 1].LayerWidth
                   && _whOptLayerInfos[insertedLayerIndex - 1].Vertices.Count > 0
                   && insertedLayerInfo.LayerWidth <= desiredWidth - _whOptLayerInfos[insertedLayerIndex - 1].Vertices.Peek().Cost)
            {
                WHOptimizationVertexInfo repositionedVertex = _whOptLayerInfos[insertedLayerIndex - 1].Vertices.Dequeue();
                insertedLayerInfo.LayerWidth += repositionedVertex.Cost;
                _whOptLayerInfos[insertedLayerIndex - 1].LayerWidth -= repositionedVertex.Value;
                _layers[insertedLayerIndex - 1].Remove(repositionedVertex.Vertex);
                insertedLayer.Add(repositionedVertex.Vertex);
                height = Math.Max(height, repositionedVertex.Vertex.Size.Height);
            }

            _actualHeight += height + Parameters.LayerGap;
            _actualWidth = _whOptLayerInfos.Max(li => li.LayerWidth);

            return true;
        }

        private void CreateLayerWHOptimizationInfos()
        {
            _actualHeight = 0;
            _actualWidth = 0;
            foreach (IList<SugiVertex> layer in _layers)
            {
                var layerInfo = new WHOptimizationLayerInfo();

                foreach (SugiVertex vertex in layer)
                {
                    ThrowIfCancellationRequested();

                    layerInfo.LayerHeight = Math.Max(vertex.Size.Height, layerInfo.LayerHeight);
                    layerInfo.LayerWidth += vertex.Size.Width;
                    if (_whOptVertexInfos.TryGetValue(vertex, out WHOptimizationVertexInfo vertexInfo) && vertexInfo.ValuePerCost >= 0)
                    {
                        layerInfo.Vertices.Enqueue(vertexInfo);
                    }
                }

                layerInfo.LayerWidth += Math.Max(0, layer.Count - 1) * Parameters.SliceGap;
                _actualWidth = Math.Max(layerInfo.LayerWidth, _actualWidth);

                var verticesList = new List<WHOptimizationVertexInfo>();
                foreach (WHOptimizationVertexInfo vertexInfo in layerInfo.Vertices)
                {
                    ThrowIfCancellationRequested();

                    if (!double.IsNaN(vertexInfo.ValuePerCost)
                        && !double.IsPositiveInfinity(vertexInfo.ValuePerCost)
                        && !double.IsNegativeInfinity(vertexInfo.ValuePerCost))
                    {
                        verticesList.Add(vertexInfo);
                    }
                }

                verticesList.Sort((v1, v2) => Math.Sign(v2.ValuePerCost - v1.ValuePerCost));

                _actualHeight += layerInfo.LayerHeight + Parameters.LayerGap;
                layerInfo.Vertices.Clear();

                foreach (WHOptimizationVertexInfo vertexInfo in verticesList)
                {
                    layerInfo.Vertices.Enqueue(vertexInfo);
                }

                _whOptLayerInfos.Add(layerInfo);
            }

            _actualHeight -= Parameters.LayerGap;
            _actualWidth -= Parameters.SliceGap;
        }

        private void CreateVertexWHOptimizationInfos()
        {
            foreach (SugiVertex vertex in _graph.Vertices)
            {
                if (vertex.Type != VertexTypes.Original)
                    continue;

                var whOptInfo = new WHOptimizationVertexInfo(
                    vertex,
                    vertex.Size.Width - Math.Max(0, _graph.InDegree(vertex) - 1) * Parameters.SliceGap,
                    vertex.Size.Width - Math.Max(0, _graph.OutDegree(vertex) - 1) * Parameters.SliceGap);

                _whOptVertexInfos[vertex] = whOptInfo;
            }
        }
    }
}
