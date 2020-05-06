using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;
using QuikGraph.Algorithms.Search;
using QuikGraph.Collections;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.Tree
{
    /// <summary>
    /// Simple tree layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public partial class SimpleTreeLayoutAlgorithm<TVertex, TEdge, TGraph> : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, SimpleTreeLayoutParameters>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        private BidirectionalGraph<TVertex, Edge<TVertex>> _spanningTree;

        [NotNull]
        private readonly IDictionary<TVertex, Size> _verticesSizes;

        [NotNull]
        private readonly IDictionary<TVertex, VertexData> _data = new Dictionary<TVertex, VertexData>();

        [NotNull, ItemNotNull]
        private readonly IList<Layer> _layers = new List<Layer>();

        private int _direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTreeLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public SimpleTreeLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] SimpleTreeLayoutParameters oldParameters = null)
            : this(visitedGraph, null, verticesSizes, oldParameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTreeLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public SimpleTreeLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] SimpleTreeLayoutParameters oldParameters = null)
            : base(visitedGraph, verticesPositions, oldParameters)
        {
            _verticesSizes = new Dictionary<TVertex, Size>(verticesSizes);
        }

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (VisitedGraph.VertexCount == 0)
                return;

            if (Parameters.Direction == LayoutDirection.LeftToRight
                || Parameters.Direction == LayoutDirection.RightToLeft)
            {
                // Change the sizes
                foreach (KeyValuePair<TVertex, Size> sizePair in _verticesSizes.ToArray())
                {
                    _verticesSizes[sizePair.Key] = new Size(sizePair.Value.Height, sizePair.Value.Width);
                }
            }

            _direction = Parameters.Direction == LayoutDirection.RightToLeft
                        || Parameters.Direction == LayoutDirection.BottomToTop
                ? -1
                : 1;

            GenerateSpanningTree();

            // First layout the vertices with 0 in-edge
            foreach (TVertex source in _spanningTree.Vertices.Where(v => _spanningTree.InDegree(v) == 0))
            {
                CalculatePosition(source, null, 0);
            }

            // Then the others
            foreach (TVertex source in _spanningTree.Vertices)
            {
                CalculatePosition(source, null, 0);
            }

            AssignPositions();
        }

        #endregion

        private void GenerateSpanningTree()
        {
            _spanningTree = new BidirectionalGraph<TVertex, Edge<TVertex>>(false);
            _spanningTree.AddVertexRange(VisitedGraph.Vertices);
            IQueue<TVertex> vb = new QuikGraph.Collections.Queue<TVertex>();
            vb.Enqueue(VisitedGraph.Vertices.OrderBy(v => VisitedGraph.InDegree(v)).First());

            switch (Parameters.SpanningTreeGeneration)
            {
                case SpanningTreeGeneration.BFS:
                    var bfs = new BreadthFirstSearchAlgorithm<TVertex, TEdge>(VisitedGraph, vb, new Dictionary<TVertex, GraphColor>());
                    bfs.TreeEdge += e => _spanningTree.AddEdge(new Edge<TVertex>(e.Source, e.Target));
                    bfs.Compute();
                    break;
                case SpanningTreeGeneration.DFS:
                    var dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(VisitedGraph);
                    dfs.TreeEdge += e => _spanningTree.AddEdge(new Edge<TVertex>(e.Source, e.Target));
                    dfs.Compute();
                    break;
            }
        }

        private double CalculatePosition([NotNull] TVertex vertex, [CanBeNull] TVertex parent, int l)
        {
            if (_data.ContainsKey(vertex))
                return -1; // This vertex is already layed out

            while (l >= _layers.Count)
            {
                _layers.Add(new Layer());
            }

            Layer layer = _layers[l];
            Size size = _verticesSizes[vertex];
            var d = new VertexData { Parent = parent };
            _data[vertex] = d;

            layer.NextPosition += size.Width / 2.0;
            if (l > 0)
            {
                layer.NextPosition += _layers[l - 1].LastTranslate;
                _layers[l - 1].LastTranslate = 0;
            }

            layer.Size = Math.Max(layer.Size, size.Height + Parameters.LayerGap);
            layer.Vertices.Add(vertex);

            if (_spanningTree.OutDegree(vertex) == 0)
            {
                d.Position = layer.NextPosition;
            }
            else
            {
                double minPos = double.MaxValue;
                double maxPos = -double.MaxValue;
                // First put the children
                foreach (TVertex child in _spanningTree.OutEdges(vertex).Select(e => e.Target))
                {
                    double childPos = CalculatePosition(child, vertex, l + 1);
                    if (childPos >= 0)
                    {
                        minPos = Math.Min(minPos, childPos);
                        maxPos = Math.Max(maxPos, childPos);
                    }
                }

                if (NearEqual(minPos, double.MaxValue))
                    d.Position = layer.NextPosition;
                else
                    d.Position = (minPos + maxPos) / 2.0;
                
                d.Translate = Math.Max(layer.NextPosition - d.Position, 0);

                layer.LastTranslate = d.Translate;
                d.Position += d.Translate;
                layer.NextPosition = d.Position;
            }

            layer.NextPosition += size.Width / 2.0 + Parameters.VertexGap;

            return d.Position;
        }

        private void AssignPositions()
        {
            double layerSize = 0;
            bool changeCoordinates = Parameters.Direction == LayoutDirection.LeftToRight
                                     || Parameters.Direction == LayoutDirection.RightToLeft;

            foreach (Layer layer in _layers)
            {
                foreach (TVertex vertex in layer.Vertices)
                {
                    Size size = _verticesSizes[vertex];
                    VertexData d = _data[vertex];
                    if (d.Parent != null)
                    {
                        d.Position += _data[d.Parent].Translate;
                        d.Translate += _data[d.Parent].Translate;
                    }

                    VerticesPositions[vertex] = changeCoordinates
                        ? new Point(_direction * (layerSize + size.Height / 2.0), d.Position)
                        : new Point(d.Position, _direction * (layerSize + size.Height / 2.0));
                }

                layerSize += layer.Size;
            }

            if (_direction < 0)
            {
                NormalizePositions();
            }
        }
    }
}