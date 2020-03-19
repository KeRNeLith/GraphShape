using System;
using System.Collections.Generic;
using System.Windows;
using GraphShape.Utils;
using JetBrains.Annotations;
using QuikGraph;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.FDP
{
    /// <summary>
    /// Kamada-Kawai layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type</typeparam>
    public class KKLayoutAlgorithm<TVertex, TEdge, TGraph>
        : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, KKLayoutParameters>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        #region Variables needed for the layout

        /// <summary>
        /// Minimal distances between the vertices.
        /// </summary>
        private double[,] _distances;
        private double[,] _edgeLengths;
        private double[,] _springConstants;

        // Cache for speed-up
        private TVertex[] _vertices;

        /// <summary>
        /// Positions of the vertices, stored by indices.
        /// </summary>
        private Point[] _positions;

        private double _diameter;
        private double _idealEdgeLength;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="KKLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public KKLayoutAlgorithm([NotNull] TGraph visitedGraph, [CanBeNull] KKLayoutParameters oldParameters)
            : this(visitedGraph, null, oldParameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KKLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public KKLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] KKLayoutParameters oldParameters)
            : base(visitedGraph, verticesPositions, oldParameters)
        {
        }

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            _distances = new double[VisitedGraph.VertexCount, VisitedGraph.VertexCount];
            _edgeLengths = new double[VisitedGraph.VertexCount, VisitedGraph.VertexCount];
            _springConstants = new double[VisitedGraph.VertexCount, VisitedGraph.VertexCount];
            _vertices = new TVertex[VisitedGraph.VertexCount];
            _positions = new Point[VisitedGraph.VertexCount];

            // Initialize with random positions
            InitializeWithRandomPositions(Parameters.Width, Parameters.Height);

            // Copy positions into array (speed-up)
            int index = 0;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                _vertices[index] = vertex;
                _positions[index] = VerticesPositions[vertex];
                ++index;
            }

            // Calculate the diameter of the graph
            _diameter = VisitedGraph.GetDiameter<TVertex, TEdge, TGraph>(out _distances);

            // L0 is the length of a side of the display area
            double l0 = Math.Min(Parameters.Width, Parameters.Height);

            // Ideal length = L0 / max d_i,j
            _idealEdgeLength = l0 / _diameter * Parameters.LengthFactor;

            // Calculate the ideal distance between the nodes
            for (int i = 0; i < VisitedGraph.VertexCount - 1; ++i)
            {
                for (int j = i + 1; j < VisitedGraph.VertexCount; ++j)
                {
                    // Distance between non-adjacent vertices
                    double dist = _diameter * Parameters.DisconnectedMultiplier;

                    // Calculate the minimal distance between the vertices
                    if (!NearEqual(_distances[i, j], double.MaxValue))
                        dist = Math.Min(_distances[i, j], dist);
                    if (!NearEqual(_distances[j, i], double.MaxValue))
                        dist = Math.Min(_distances[j, i], dist);
                    _distances[i, j] = _distances[j, i] = dist;
                    _edgeLengths[i, j] = _edgeLengths[j, i] = _idealEdgeLength * dist;
                    _springConstants[i, j] = _springConstants[j, i] = Parameters.K / Math.Pow(dist, 2);
                }
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            int n = VisitedGraph.VertexCount;
            if (n == 0)
                return;

            for (int iteration = 0; iteration < Parameters.MaxIterations; ++iteration)
            {
                #region An iteration

                double maxDeltaM = double.NegativeInfinity;
                int pm = -1;

                // Get the 'p' with the max delta_m
                for (int i = 0; i < n; ++i)
                {
                    double deltaM = CalculateEnergyGradient(i);
                    if (maxDeltaM < deltaM)
                    {
                        maxDeltaM = deltaM;
                        pm = i;
                    }
                }

                if (pm == -1)
                    return;

                // Calculate the delta_x & delta_y with the Newton-Raphson method
                // There is an upper-bound for the while (deltaM > epsilon) {...} cycle (100)
                for (int i = 0; i < 100; ++i)
                {
                    _positions[pm] += CalculateDeltaXY(pm);

                    double deltaM = CalculateEnergyGradient(pm);
                    // Real stop condition
                    if (deltaM < double.Epsilon)
                        break;
                }

                // What if some of the vertices would be exchanged?
                if (Parameters.ExchangeVertices && maxDeltaM < double.Epsilon)
                {
                    double energy = CalculateEnergy();
                    for (int i = 0; i < n - 1; ++i)
                    {
                        for (int j = i + 1; j < n; ++j)
                        {
                            double exchangedEnergy = CalculateEnergyIfExchanged(i, j);
                            if (energy > exchangedEnergy)
                            {
                                Point p = _positions[i];
                                _positions[i] = _positions[j];
                                _positions[j] = p;
                                return;
                            }
                        }
                    }
                }

                #endregion

                if (ReportOnIterationEndNeeded)
                    Report(iteration);
            }

            Report(Parameters.MaxIterations);
        }

        #endregion

        /// <summary>
        /// Reports the end of the <paramref name="iteration"/>th iteration.
        /// </summary>
        protected void Report(int iteration)
        {
            // Copy positions to VerticesPositions
            for (int i = 0; i < _vertices.Length; ++i)
                VerticesPositions[_vertices[i]] = _positions[i];

            OnIterationEnded(
                iteration,
                iteration / (double)Parameters.MaxIterations,
                $"Iteration {iteration} finished.",
                true);
        }

        [Pure]
        // ReSharper disable InconsistentNaming
        private double ComputeEnergy(double l_ij, double k_ij, double dx, double dy)
        // ReSharper restore InconsistentNaming
        {
            return k_ij / 2 *
                   (dx * dx + dy * dy + l_ij * l_ij
                    -
                    2 * l_ij * Math.Sqrt(dx * dx + dy * dy));
        }

        /// <returns>
        /// Calculates the energy of the state where 
        /// the positions of the vertex 'p' and 'q' are exchanged.
        /// </returns>
        /// <param name="p">The index of a vertex.</param>
        /// <param name="q">The index of a vertex.</param>
        [Pure]
        private double CalculateEnergyIfExchanged(int p, int q)
        {
            double energy = 0;
            for (int i = 0; i < _vertices.Length - 1; ++i)
            {
                for (int j = i + 1; j < _vertices.Length; ++j)
                {
                    int ii = i == p ? q : i;
                    int jj = j == q ? p : j;

                    // ReSharper disable InconsistentNaming
                    double l_ij = _edgeLengths[i, j];
                    double k_ij = _springConstants[i, j];
                    // ReSharper restore InconsistentNaming
                    double dx = _positions[ii].X - _positions[jj].X;
                    double dy = _positions[ii].Y - _positions[jj].Y;

                    energy += ComputeEnergy(l_ij, k_ij, dx, dy);
                }
            }

            return energy;
        }

        /// <summary>
        /// Calculates the energy of the spring system.
        /// </summary>
        /// <returns>Returns with the energy of the spring system.</returns>
        [Pure]
        private double CalculateEnergy()
        {
            double energy = 0;
            for (int i = 0; i < _vertices.Length - 1; ++i)
            {
                for (int j = i + 1; j < _vertices.Length; ++j)
                {
                    // ReSharper disable InconsistentNaming
                    double l_ij = _edgeLengths[i, j];
                    double k_ij = _springConstants[i, j];
                    // ReSharper restore InconsistentNaming

                    double dx = _positions[i].X - _positions[j].X;
                    double dy = _positions[i].Y - _positions[j].Y;

                    energy += ComputeEnergy(l_ij, k_ij, dx, dy);
                }
            }

            return energy;
        }

        /// <summary>
        /// Determines a step to new position of the <paramref name="m"/>th vertex.
        /// </summary>
        /// <param name="m">The index of the vertex.</param>
        /// <returns>The delta XY of the <paramref name="m"/>th vertex.</returns>
        [Pure]
        private Vector CalculateDeltaXY(int m)
        {
            double dxm = 0;
            double dym = 0;
            double dxmdym = 0;
            double dymdxm;
            // ReSharper disable InconsistentNaming
            double d2xm = 0;
            double d2ym = 0;
            // ReSharper restore InconsistentNaming

            for (int i = 0; i < _vertices.Length; ++i)
            {
                if (i != m)
                {
                    // Common things
                    double l = _edgeLengths[m, i];
                    double k = _springConstants[m, i];
                    double dx = _positions[m].X - _positions[i].X;
                    double dy = _positions[m].Y - _positions[i].Y;

                    // Distance between the points
                    double d = Math.Sqrt(dx * dx + dy * dy);
                    double ddd = Math.Pow(d, 3);

                    dxm += k * (1 - l / d) * dx;
                    dym += k * (1 - l / d) * dy;
                    //TODO isn't it wrong?
                    d2xm += k * (1 - l * Math.Pow(dy, 2) / ddd);
                    //d2E_d2xm += k_mi * ( 1 - l_mi / d + l_mi * dx * dx / ddd );
                    dxmdym += k * l * dx * dy / ddd;
                    //d2E_d2ym += k_mi * ( 1 - l_mi / d + l_mi * dy * dy / ddd );
                    //TODO isn't it wrong?
                    d2ym += k * (1 - l * Math.Pow(dx, 2) / ddd);
                }
            }

            // d2E_dymdxm equals to d2E_dxmdym
            dymdxm = dxmdym;

            double denominator = d2xm * d2ym - dxmdym * dymdxm;
            double deltaX = (dxmdym * dym - d2ym * dxm) / denominator;
            double deltaY = (dymdxm * dxm - d2xm * dym) / denominator;

            return new Vector(deltaX, deltaY);
        }

        /// <summary>
        /// Calculates the gradient energy of <paramref name="m"/>th vertex.
        /// </summary>
        /// <param name="m">The index of the vertex.</param>
        /// <returns>The gradient energy of the <paramref name="m"/>th vertex.</returns>
        [Pure]
        private double CalculateEnergyGradient(int m)
        {
            double dxm = 0, dym = 0;
            //        {  1, if m < i
            // sign = { 
            //        { -1, if m > i
            for (int i = 0; i < _vertices.Length; i++)
            {
                if (i == m)
                    continue;

                // Differences of the positions
                double dx = _positions[m].X - _positions[i].X;
                double dy = _positions[m].Y - _positions[i].Y;

                // Distances of the two vertex (by positions)
                double d = Math.Sqrt(dx * dx + dy * dy);

                double common = _springConstants[m, i] * (1 - _edgeLengths[m, i] / d);
                dxm += common * dx;
                dym += common * dy;
            }

            // delta_m = sqrt((dE/dx)^2 + (dE/dy)^2)
            return Math.Sqrt(dxm * dxm + dym * dym);
        }
    }
}