using System;
using System.Collections.Generic;
using System.Diagnostics;
using GraphShape.Utils;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Inverted Self-Organizing Map (ISOM) layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class ISOMLayoutAlgorithm<TVertex, TEdge, TGraph>
        : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, ISOMLayoutParameters>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        private sealed class ISOMData
        {
            public bool Visited { get; set; }
            public double Distance { get; set; }
        }

        [NotNull, ItemNotNull]
        private readonly Queue<TVertex> _queue;
        
        [NotNull]
        private readonly Dictionary<TVertex, ISOMData> _isomDataDict;
        
        private Point _tempPos;
        
        private double _adaptation;

        private int _radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="ISOMLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public ISOMLayoutAlgorithm([NotNull] TGraph visitedGraph, [CanBeNull] ISOMLayoutParameters parameters = null)
            : this(visitedGraph, null, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISOMLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public ISOMLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] ISOMLayoutParameters parameters = null)
            : base(visitedGraph, verticesPositions, parameters)
        {
            _queue = new Queue<TVertex>();
            _isomDataDict = new Dictionary<TVertex, ISOMData>();
            _adaptation = Parameters.InitialAdaptation;
        }

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            // Initialize vertices positions
            InitializeWithRandomPositions(Parameters.Width, Parameters.Height);

            // Initialize ISOM data
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                if (!_isomDataDict.ContainsKey(vertex))
                {
                    _isomDataDict[vertex] = new ISOMData();
                }
            }

            _radius = Parameters.InitialRadius;
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (VisitedGraph.VertexCount < 2)
                return;

            for (int epoch = 0; epoch < Parameters.MaxEpochs; ++epoch)
            {
                ThrowIfCancellationRequested();

                Adjust();

                // Update Parameters
                double factor = Math.Exp(-1 * Parameters.CoolingFactor * (1.0 * epoch / Parameters.MaxEpochs));
                _adaptation = Math.Max(Parameters.MinAdaptation, factor * Parameters.InitialAdaptation);
                if (_radius > Parameters.MinRadius && epoch % Parameters.RadiusConstantTime == 0)
                {
                    --_radius;
                }

                // Report
                if (ReportOnIterationEndNeeded)
                {
                    OnIterationEnded(
                        epoch,
                        epoch / (double)Parameters.MaxEpochs,
                        $"Iteration {epoch} finished.",
                        true);
                }
                if (ReportOnProgressChangedNeeded)
                {
                    OnProgressChanged(epoch / (double)Parameters.MaxEpochs * 100);
                }
            }
        }

        #endregion

        /// <summary>
        /// Adjust all vertices to a random chosen one.
        /// </summary>
        protected void Adjust()
        {
            // Get a random point in the container
            _tempPos = new Point
            {
                X = 0.1 * Parameters.Width + Rand.NextDouble() * 0.8 * Parameters.Width,
                Y = 0.1 * Parameters.Height + Rand.NextDouble() * 0.8 * Parameters.Height
            };

            // Find the closest vertex to this random point
            TVertex closest = GetClosest(_tempPos);

            // Adjust the vertices to the selected vertex
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                ISOMData data = _isomDataDict[vertex];
                data.Distance = 0;
                data.Visited = false;
            }

            AdjustVertex(closest);
        }

        private void AdjustVertex([NotNull] TVertex closest)
        {
            Debug.Assert(closest != null);

            _queue.Clear();
            ISOMData vid = _isomDataDict[closest];
            vid.Distance = 0;
            vid.Visited = true;
            _queue.Enqueue(closest);

            while (_queue.Count > 0)
            {
                ThrowIfCancellationRequested();

                TVertex current = _queue.Dequeue();
                ISOMData currentData = _isomDataDict[current];
                Point position = VerticesPositions[current];

                Vector force = _tempPos - position;
                double factor = _adaptation / Math.Pow(2, currentData.Distance);

                position += factor * force;
                VerticesPositions[current] = position;

                // If it is in the radius
                if (currentData.Distance < _radius)
                {
                    // Also consider neighbors
                    foreach (TVertex neighbor in VisitedGraph.GetNeighbors(current))
                    {
                        ThrowIfCancellationRequested();

                        ISOMData neighborData = _isomDataDict[neighbor];
                        if (!neighborData.Visited)
                        {
                            neighborData.Visited = true;
                            neighborData.Distance = currentData.Distance + 1;
                            _queue.Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the the closest vertex to the given position.
        /// </summary>
        /// <param name="tempPos">The position.</param>
        /// <returns>Returns with the reference of the closest vertex.</returns>
        [Pure]
        [NotNull]
        private TVertex GetClosest(Point tempPos)
        {
            var closest = default(TVertex);
            double distance = double.MaxValue;

            // Find the closest vertex
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                double d = (tempPos - VerticesPositions[vertex]).Length;
                if (d < distance)
                {
                    closest = vertex;
                    distance = d;
                }
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            // Justification: algorithm run only if graph has more that one vertex
            return closest;
        }
    }
}