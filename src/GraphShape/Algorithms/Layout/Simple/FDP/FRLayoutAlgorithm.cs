using System;
using System.Collections.Generic;
using QuikGraph;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.Layout.Simple.FDP
{
    /// <summary>
    /// Fruchterman-Reingold layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type</typeparam>
    public class FRLayoutAlgorithm<TVertex, TEdge, TGraph>
        : ParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, FRLayoutParametersBase>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Actual temperature of the 'mass'.
        /// </summary>
        private double _temperature;

        private double _maxWidth = double.PositiveInfinity;
        private double _maxHeight = double.PositiveInfinity;

        /// <summary>
        /// Initializes a new instance of the <see cref="FRLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public FRLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] FRLayoutParametersBase oldParameters = null)
            : this(visitedGraph, null, oldParameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FRLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public FRLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] FRLayoutParametersBase oldParameters = null)
            : base(visitedGraph, verticesPositions, oldParameters)
        {
        }

        /// <inheritdoc />
        protected override FRLayoutParametersBase DefaultParameters { get; } = new FreeFRLayoutParameters();

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            // Initializing the positions
            if (Parameters is BoundedFRLayoutParameters boundedFRParams)
            {
                InitializeWithRandomPositions(boundedFRParams.Width, boundedFRParams.Height);
                _maxWidth = boundedFRParams.Width;
                _maxHeight = boundedFRParams.Height;
            }
            else
            {
                InitializeWithRandomPositions(10.0, 10.0);
            }

            Parameters.VertexCount = VisitedGraph.VertexCount;
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            // Actual temperature of the 'mass'. Used for cooling.
            double minimalTemperature = Parameters.InitialTemperature * 0.01;
            _temperature = Parameters.InitialTemperature;
            for (int i = 0;
                i < Parameters.MaxIterations && _temperature > minimalTemperature;
                ++i)
            {
                ThrowIfCancellationRequested();

                IterateOne();

                // Make some cooling
                switch (Parameters.CoolingFunction)
                {
                    case FRCoolingFunction.Linear:
                        _temperature *= 1.0 - i / (double)Parameters.MaxIterations;
                        break;
                    case FRCoolingFunction.Exponential:
                        _temperature *= Parameters.Lambda;
                        break;
                }

                // Iteration ended, do some report
                if (ReportOnIterationEndNeeded)
                {
                    double statusInPercent = i / (double)Parameters.MaxIterations;
                    OnIterationEnded(i, statusInPercent, string.Empty, true);
                }
            }
        }

        #endregion

        /// <summary>
        /// Compute one force application iteration.
        /// </summary>
        protected void IterateOne()
        {
            // Create the forces (zero forces)
            var forces = new Dictionary<TVertex, Vector>();

            #region Repulsive forces

            foreach (TVertex v in VisitedGraph.Vertices)
            {
                var force = default(Vector);

                Point posV = VerticesPositions[v];
                foreach (TVertex u in VisitedGraph.Vertices)
                {
                    ThrowIfCancellationRequested();

                    // Doesn't repulse itself
                    if (EqualityComparer<TVertex>.Default.Equals(u, v))
                        continue;

                    // Calculate repulsive force
                    Vector delta = posV - VerticesPositions[u];
                    double length = Math.Max(delta.Length, double.Epsilon);
                    delta = delta / length * Parameters.ConstantOfRepulsion / length;

                    force += delta;
                }
                
                forces[v] = force;
            }

            #endregion

            #region Attractive forces

            foreach (TEdge edge in VisitedGraph.Edges)
            {
                TVertex source = edge.Source;
                TVertex target = edge.Target;

                if (edge.IsSelfEdge())
                    continue;

                // Compute attraction point between 2 vertices
                Vector delta = VerticesPositions[source] - VerticesPositions[target];
                double length = Math.Max(delta.Length, double.Epsilon);
                delta = delta / length * Math.Pow(length, 2) / Parameters.ConstantOfAttraction;

                forces[source] -= delta;
                forces[target] += delta;
            }

            #endregion

            #region Limit displacement

            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                Point position = VerticesPositions[vertex];

                Vector delta = forces[vertex];
                if (delta != default(Vector))
                {
                    double length = Math.Max(delta.Length, double.Epsilon);
                    delta = delta / length * Math.Min(length, _temperature);

                    position += delta;

                    // Ensure bounds
                    position.X = Math.Min(_maxWidth, Math.Max(0, position.X));
                    position.Y = Math.Min(_maxHeight, Math.Max(0, position.Y));
                    VerticesPositions[vertex] = position;
                }
            }

            #endregion
        }
    }
}