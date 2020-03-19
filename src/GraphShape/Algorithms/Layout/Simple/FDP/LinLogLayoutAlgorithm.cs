using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.FDP
{
    /// <summary>
    /// LinLog layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type</typeparam>
    public partial class LinLogLayoutAlgorithm<TVertex, TEdge, TGraph>
        : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, LinLogLayoutParameters>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        private class LinLogVertex
        {
            public int Index { get; }

            [NotNull]
            public TVertex OriginalVertex { get; }

            [NotNull, ItemNotNull]
            public LinLogEdge[] Attractions { get; }

            public double RepulsionWeight { get; set; }

            public Point Position { get; set; }

            public LinLogVertex(
                int index,
                [NotNull] TVertex vertex,
                [NotNull, ItemNotNull] LinLogEdge[] attractions)
            {
                Index = index;
                OriginalVertex = vertex;
                Attractions = attractions;
            }
        }

        private class LinLogEdge
        {
            [NotNull]
            public LinLogVertex Target { get; }
            public double AttractionWeight { get; }

            public LinLogEdge([NotNull] LinLogVertex target, double weight)
            {
                Target = target;
                AttractionWeight = weight;
            }
        }

        [ItemNotNull]
        private LinLogVertex[] _vertices;
        private Point _barycenter;
        private double _repulsionMultiplier;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinLogLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        public LinLogLayoutAlgorithm([NotNull] TGraph visitedGraph)
            : base(visitedGraph)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinLogLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public LinLogLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] LinLogLayoutParameters oldParameters)
            : base(visitedGraph, verticesPositions, oldParameters)
        {
        }

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (VisitedGraph.VertexCount <= 1)
                return;

            InitializeWithRandomPositions(1, 1, -0.5, -0.5);

            InitAlgorithm();

            double finalRepulsiveExponent = Parameters.RepulsiveExponent;
            double finalAttractionExponent = Parameters.AttractionExponent;

            for (int step = 1; step <= Parameters.IterationCount; ++step)
            {
                ComputeBarycenter();
                QuadTree quadTree = BuildQuadTree();

                #region Define cooling function

                if (Parameters.IterationCount >= 50 && finalRepulsiveExponent < 1.0)
                {
                    Parameters.AttractionExponent = finalAttractionExponent;
                    Parameters.RepulsiveExponent = finalRepulsiveExponent;
                    if (step <= 0.6 * Parameters.IterationCount)
                    {
                        // Use energy model with few local minimum
                        Parameters.AttractionExponent += 1.1 * (1.0 - finalRepulsiveExponent);
                        Parameters.RepulsiveExponent += 0.9 * (1.0 - finalRepulsiveExponent);
                    }
                    else if (step <= 0.9 * Parameters.IterationCount)
                    {
                        // Gradually move to final energy model
                        Parameters.AttractionExponent +=
                            1.1 * (1.0 - finalRepulsiveExponent) * (0.9 - step / (double)Parameters.IterationCount) / 0.3;
                        Parameters.RepulsiveExponent +=
                            0.9 * (1.0 - finalRepulsiveExponent) * (0.9 - step / (double)Parameters.IterationCount) / 0.3;
                    }
                }

                #endregion

                #region Move each node

                for (int i = 0; i < _vertices.Length; ++i)
                {
                    LinLogVertex vertex = _vertices[i];
                    double oldEnergy = GetEnergy(i, quadTree);

                    // Compute direction of the move of the node
                    GetDirection(i, quadTree, out Vector bestDirection);

                    // Line search: compute length of the move
                    Point oldPosition = vertex.Position;

                    double bestEnergy = oldEnergy;
                    int bestMultiple = 0;
                    bestDirection /= 32;
                    // Determine the best multiple (for little moves)
                    for (int multiple = 32;
                        multiple >= 1 && (bestMultiple == 0 || bestMultiple / 2 == multiple);
                        multiple /= 2)
                    {
                        vertex.Position = oldPosition + bestDirection * multiple;
                        double curEnergy = GetEnergy(i, quadTree);
                        if (curEnergy < bestEnergy)
                        {
                            bestEnergy = curEnergy;
                            bestMultiple = multiple;
                        }
                    }

                    // Try to determine a better multiple (for larger moves)
                    for (int multiple = 64;
                        multiple <= 128 && bestMultiple == multiple / 2;
                        multiple *= 2)
                    {
                        vertex.Position = oldPosition + bestDirection * multiple;
                        double curEnergy = GetEnergy(i, quadTree);
                        if (curEnergy < bestEnergy)
                        {
                            bestEnergy = curEnergy;
                            bestMultiple = multiple;
                        }
                    }

                    // Best move
                    vertex.Position = oldPosition + bestDirection * bestMultiple;
                    if (bestMultiple > 0)
                    {
                        quadTree.MoveNode(oldPosition, vertex.Position, vertex.RepulsionWeight);
                    }
                }

                #endregion

                if (ReportOnIterationEndNeeded)
                    Report(step);
            }

            CopyPositions();
            NormalizePositions();
        }

        #endregion

        private void InitAlgorithm()
        {
            _vertices = new LinLogVertex[VisitedGraph.VertexCount];

            var verticesMap = new Dictionary<TVertex, LinLogVertex>();

            // Index vertices
            int i = 0;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                _vertices[i] = new LinLogVertex(i, vertex, new LinLogEdge[VisitedGraph.Degree(vertex)])
                {
                    RepulsionWeight = 0,
                    Position = VerticesPositions[vertex]
                };
                verticesMap[vertex] = _vertices[i];
                ++i;
            }

            // Compute best attraction weight and attraction index for each vertex
            foreach (LinLogVertex vertex in _vertices)
            {
                int attractionIndex = 0;
                foreach (TEdge edge in VisitedGraph.InEdges(vertex.OriginalVertex))
                {
                    var weightedEdge = edge as WeightedEdge<TVertex>;
                    double weight = weightedEdge?.Weight ?? 1;
                    vertex.Attractions[attractionIndex] = new LinLogEdge(verticesMap[edge.Source], weight);
                    //TODO look at this line below
                    //vertex.RepulsionWeight += weight;
                    ++vertex.RepulsionWeight;
                    ++attractionIndex;
                }

                foreach (TEdge edge in VisitedGraph.OutEdges(vertex.OriginalVertex))
                {
                    var weightedEdge = edge as WeightedEdge<TVertex>;
                    double weight = weightedEdge?.Weight ?? 1;
                    vertex.Attractions[attractionIndex] = new LinLogEdge(verticesMap[edge.Target], weight);
                    //vertex.RepulsionWeight += weight;
                    ++vertex.RepulsionWeight;
                    ++attractionIndex;
                }

                vertex.RepulsionWeight = Math.Max(vertex.RepulsionWeight, Parameters.GravitationMultiplier);
            }

            _repulsionMultiplier = ComputeRepulsionMultiplier();
        }

        [Pure]
        private double ComputeRepulsionMultiplier()
        {
            double attractionSum = _vertices.Sum(v => v.Attractions.Sum(e => e.AttractionWeight));
            double repulsionSum = _vertices.Sum(v => v.RepulsionWeight);

            if (repulsionSum > 0 && attractionSum > 0)
                return attractionSum / Math.Pow(repulsionSum, 2) * Math.Pow(repulsionSum, 0.5 * (Parameters.AttractionExponent - Parameters.RepulsiveExponent));
            return 1;
        }

        /// <summary>
        /// Copies positions from <see cref="_vertices"/> to <see cref="LayoutAlgorithmBase{TVertex,TEdge,TGraph}.VerticesPositions"/>.
        /// </summary>
        protected void CopyPositions()
        {
            // Copy positions
            foreach (LinLogVertex vertex in _vertices)
                VerticesPositions[vertex.OriginalVertex] = vertex.Position;
        }

        /// <summary>
        /// Reports the end of iteration <paramref name="step"/>.
        /// </summary>
        protected void Report(int step)
        {
            CopyPositions();
            OnIterationEnded(
                step,
                step / (double)Parameters.IterationCount * 100,
                $"Iteration {step} finished.",
                true);
        }

        private void GetDirection(int index, QuadTree quadTree, out Vector direction)
        {
            direction = default(Vector);

            double direction2 = AddRepulsionDirection(index, quadTree, ref direction);
            direction2 += AddAttractionDirection(index, ref direction);
            direction2 += AddGravitationDirection(index, ref direction);

            if (!IsZero(direction2))
            {
                direction /= direction2;

                double length = direction.Length;
                if (length > quadTree.Width / 8)
                {
                    length /= quadTree.Width / 8;
                    direction /= length;
                }
            }
            else
            {
                direction = default(Vector);
            }
        }

        /// <summary>
        /// Computes the repulsion force for vertex at given <paramref name="index"/> wit <paramref name="quadTree"/>.
        /// </summary>
        /// <param name="index">Index of the vertex for which adding repulsion force.</param>
        /// <param name="quadTree">Quad tree.</param>
        /// <param name="direction">Vector on which adding the computed repulsive force.</param>
        /// <returns>The second derivative of the repulsive energy.</returns>
        [Pure]
        private double AddRepulsionDirection(int index, [CanBeNull] QuadTree quadTree, ref Vector direction)
        {
            LinLogVertex vertex = _vertices[index];

            if (quadTree is null || quadTree.Index == index || vertex.RepulsionWeight <= 0)
                return 0.0;

            Vector repulsionVector = quadTree.Position - vertex.Position;
            double distance = repulsionVector.Length;
            if (quadTree.Index < 0 && distance < 2.0 * quadTree.Width)
            {
                double direction2 = 0.0;
                foreach (QuadTree childTree in quadTree.Children)
                    direction2 += AddRepulsionDirection(index, childTree, ref direction);
                return direction2;
            }

            if (!IsZero(distance))
            {
                double tmp = _repulsionMultiplier
                             * vertex.RepulsionWeight
                             * quadTree.Weight
                             * Math.Pow(distance, Parameters.RepulsiveExponent - 2);
                direction -= repulsionVector * tmp;
                return tmp * Math.Abs(Parameters.RepulsiveExponent - 1);
            }

            return 0.0;
        }

        [Pure]
        private double AddGravitationDirection(int index, ref Vector direction)
        {
            LinLogVertex vertex = _vertices[index];
            Vector gravitationVector = _barycenter - vertex.Position;
            double distance = gravitationVector.Length;
            double tmp = Parameters.GravitationMultiplier
                         * _repulsionMultiplier
                         * Math.Max(vertex.RepulsionWeight, 1)
                         * Math.Pow(distance, Parameters.AttractionExponent - 2);
            direction += gravitationVector * tmp;

            return tmp * Math.Abs(Parameters.AttractionExponent - 1);
        }

        [Pure]
        private double AddAttractionDirection(int index, ref Vector direction)
        {
            double direction2 = 0.0;
            LinLogVertex vertex = _vertices[index];
            foreach (LinLogEdge edge in vertex.Attractions)
            {
                // Avoid loop
                if (edge.Target == vertex)
                    continue;

                Vector attractionVector = edge.Target.Position - vertex.Position;
                double distance = attractionVector.Length;
                if (distance <= 0)
                    continue;

                double tmp = edge.AttractionWeight * Math.Pow(distance, Parameters.AttractionExponent - 2);
                direction2 += tmp * Math.Abs(Parameters.AttractionExponent - 1);

                direction += (edge.Target.Position - vertex.Position) * tmp;
            }

            return direction2;
        }

        [Pure]
        private double GetEnergy(int index, [CanBeNull] QuadTree quadTree)
        {
            return GetRepulsionEnergy(index, quadTree)
                   + GetAttractionEnergy(index)
                   + GetGravitationEnergy(index);
        }

        [Pure]
        private double GetRepulsionEnergy(int index, [CanBeNull] QuadTree quadTree)
        {
            if (quadTree is null || quadTree.Index == index || index >= _vertices.Length)
                return 0.0;

            LinLogVertex vertex = _vertices[index];

            double dist = (vertex.Position - quadTree.Position).Length;
            if (quadTree.Index < 0 && dist < 2 * quadTree.Width)
            {
                double energy = 0.0;
                foreach (QuadTree childTree in quadTree.Children)
                    energy += GetRepulsionEnergy(index, childTree);
                return energy;
            }

            if (IsZero(Parameters.RepulsiveExponent))
                return -_repulsionMultiplier * vertex.RepulsionWeight * quadTree.Weight * Math.Log(dist);

            return -_repulsionMultiplier
                * vertex.RepulsionWeight
                * quadTree.Weight
                * Math.Pow(dist, Parameters.RepulsiveExponent) / Parameters.RepulsiveExponent;
        }

        [Pure]
        private double GetAttractionEnergy(int index)
        {
            double energy = 0.0;
            LinLogVertex vertex = _vertices[index];
            foreach (LinLogEdge edge in vertex.Attractions)
            {
                if (edge.Target == vertex)
                    continue;

                double distance = (edge.Target.Position - vertex.Position).Length;
                energy += edge.AttractionWeight * Math.Pow(distance, Parameters.AttractionExponent) / Parameters.AttractionExponent;
            }

            return energy;
        }

        [Pure]
        private double GetGravitationEnergy(int index)
        {
            LinLogVertex vertex = _vertices[index];

            double distance = (vertex.Position - _barycenter).Length;
            return Parameters.GravitationMultiplier
                * _repulsionMultiplier
                * Math.Max(vertex.RepulsionWeight, 1)
                * Math.Pow(distance, Parameters.AttractionExponent) / Parameters.AttractionExponent;
        }

        private void ComputeBarycenter()
        {
            _barycenter = new Point(0, 0);
            double repWeightSum = 0.0;
            foreach (LinLogVertex vertex in _vertices)
            {
                repWeightSum += vertex.RepulsionWeight;
                _barycenter.X += vertex.Position.X * vertex.RepulsionWeight;
                _barycenter.Y += vertex.Position.Y * vertex.RepulsionWeight;
            }

            if (repWeightSum > 0.0)
            {
                _barycenter.X /= repWeightSum;
                _barycenter.Y /= repWeightSum;
            }
        }

        /// <summary>
        /// Build a <see cref="QuadTree"/> (similar to OctTree but in 2D).
        /// </summary>
        [Pure]
        [NotNull]
        private QuadTree BuildQuadTree()
        {
            // Compute minimal and maximal positions
            Point minPos = new Point(double.MaxValue, double.MaxValue);
            Point maxPos = new Point(-double.MaxValue, -double.MaxValue);

            foreach (LinLogVertex vertex in _vertices)
            {
                if (vertex.RepulsionWeight <= 0)
                    continue;

                minPos.X = Math.Min(minPos.X, vertex.Position.X);
                minPos.Y = Math.Min(minPos.Y, vertex.Position.Y);
                maxPos.X = Math.Max(maxPos.X, vertex.Position.X);
                maxPos.Y = Math.Max(maxPos.Y, vertex.Position.Y);
            }

            // Add node with non negative repulsive weight to the tree
            QuadTree result = null;
            foreach (LinLogVertex vertex in _vertices)
            {
                if (vertex.RepulsionWeight <= 0)
                    continue;

                if (result is null)
                    result = new QuadTree(vertex.Index, vertex.Position, vertex.RepulsionWeight, minPos, maxPos);
                else
                    result.AddNode(vertex.Index, vertex.Position, vertex.RepulsionWeight, 0);
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            // Justification: There is at least one vertex in the graph to run algorithm
            return result;
        }
    }
}