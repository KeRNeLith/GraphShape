using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Compound.FDP
{
    /// <summary>
    /// Compound FDP layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public partial class CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>
        : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, CompoundFDPLayoutParameters>
        , ICompoundLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        private double _temperature;

        private const double TemperatureLambda = 0.99;

        /// <summary>
        /// Phase of the layout process.
        /// Values: 1,2,3.
        /// </summary>
        private int _phase = 1;

        /// <summary>
        /// The steps in the actual phase.
        /// </summary>
        private int _step;

        /// <summary>
        /// The maximum number of iteration in the phases.
        /// </summary>
        private int[] _maxIterationCounts;

        /// <summary>
        /// Indicates whether the removed tree-node
        /// has been grown back or not.
        /// </summary>
        private bool AllTreesGrown => _removedRootTreeNodeLevels.Count == 0;

        /// <summary>
        /// Grows back a tree-node level in every 'treeGrowingStep'th step.
        /// </summary>
        private const int TreeGrowingStep = 5;

        /// <summary>
        /// The magnitude of the gravity force calculated in the init phased.
        /// </summary>
        private double _gravityForceMagnitude;

        /// <summary>
        /// Has been the gravity center initiated or not.
        /// </summary>
        private bool _gravityCenterCalculated;

        private double _phaseDependentRepulsionMultiplier = 1.0;

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            Init(_verticesSizes, _verticesBorders, _layoutTypes);

            // Phases:
            // 1: Layout the skeleton graph without app. specific and gravitation forces.
            // 2: Add the removed tree nodes and apply app. specific and gravitation forces.
            // 3: Stabilization

            // For optimization purposes
            _maxIterationCounts = new[]
            {
                Parameters.Phase1Iterations,
                Parameters.Phase2Iterations,
                Parameters.Phase3Iterations
            };
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (VisitedGraph.VertexCount == 0)
                return;

            var temperatureMultipliers = new[]
            {
                1.0,
                Parameters.Phase2TemperatureInitialMultiplier,
                Parameters.Phase3TemperatureInitialMultiplier
            };

            double initialTemperature = Math.Sqrt(_compoundGraph.VertexCount) * Parameters.IdealEdgeLength;
            double minimalTemperature = initialTemperature * 0.1;
            _temperature = initialTemperature;

            _gravityCenterCalculated = false;

            for (_phase = 1; _phase <= 3; ++_phase)
            {
                _temperature = initialTemperature * temperatureMultipliers[_phase - 1];
                _phaseDependentRepulsionMultiplier = _phase < 2 ? 0.5 : 1.0;
                for (
                    _step = _maxIterationCounts[_phase - 1];
                    _step > 0 || _phase == 2 && !AllTreesGrown;
                    --_step)
                {
                    //error = 0;

                    ApplySpringForces();
                    ApplyRepulsionForces();

                    if (_phase > 1)
                    {
                        ApplyGravitationForces();
                        ApplyApplicationSpecificForces();
                    }

                    if (ReportOnIterationEndNeeded)
                        SavePositions();

                    CalculateNodePositionsAndSizes();

                    if (_phase == 2 && !AllTreesGrown && _step % TreeGrowingStep == 0)
                        GrowTreesOneLevel();

                    _temperature *= TemperatureLambda;
                    _temperature = Math.Max(_temperature, minimalTemperature);
                }

                if (!_gravityCenterCalculated)
                {
                    _rootCompoundVertex.RecalculateBounds();
                    _gravityCenterCalculated = true;
                }

                _temperature *= Parameters.TemperatureDecreasing;
            }

            SavePositions();
        }

        #endregion

        private void SavePositions()
        {
            foreach (TVertex vertex in _verticesData.Keys)
            {
                VertexData vertexData = _verticesData[vertex];
                VerticesPositions[vertex] = vertexData.Position;
            }

            // Build the test vertex infos
            Dictionary<TVertex, TestingCompoundVertexInfo> vertexInfos = _verticesData.ToDictionary(
                pair => pair.Key,
                pair => new TestingCompoundVertexInfo(
                    pair.Value.SpringForce,
                    pair.Value.RepulsionForce,
                    pair.Value.GravitationForce,
                    pair.Value.ApplicationForce));

            var iterationEndedArgs = new TestingCompoundLayoutIterationEventArgs<TVertex, TEdge, TestingCompoundVertexInfo, object>(
                0,
                0,
                $"Phase: {_phase}, Steps: {_step}",
                VerticesPositions,
                InnerCanvasSizes,
                vertexInfos,
                _rootCompoundVertex.InnerCanvasCenter);

            // Raise the event
            OnIterationEnded(iterationEndedArgs);
        }

        private void GrowTreesOneLevel()
        {
            if (_removedRootTreeNodeLevels.Count <= 0)
                return;

            IList<RemovedTreeNodeData> treeNodeData = _removedRootTreeNodeLevels.Pop();
            foreach (RemovedTreeNodeData data in treeNodeData)
            {
                _removedRootTreeNodes.Remove(data.Vertex);
                _removedRootTreeEdges.Remove(data.Edge);
                Levels[0].Add(data.Vertex);
                _compoundGraph.AddVertex(data.Vertex);
                _compoundGraph.AddEdge(data.Edge);

                TVertex otherVertex = data.Edge.GetOtherVertex(data.Vertex);
                _verticesData[data.Vertex].Position = _verticesData[otherVertex].Position;
            }
        }

        [Pure]
        private Vector GetSpringForce(double idealLength, Point uPos, Point vPos, Size uSize, Size vSize)
        {
            Vector positionVector = uPos - vPos;
            if (IsZero(positionVector.Length))
            {
                var compensationVector = new Vector(Rand.NextDouble(), Rand.NextDouble());
                positionVector = compensationVector * 2;
                uPos += compensationVector;
                vPos -= compensationVector;
            }
            positionVector.Normalize();

            // Get the clipping points
            Point clippingPointU = LayoutUtils.GetClippingPoint(uSize, uPos, vPos);
            Point clippingPointV = LayoutUtils.GetClippingPoint(vSize, vPos, uPos);

            Vector force = clippingPointU - clippingPointV;
            bool isSameDirection = LayoutUtils.IsSameDirection(positionVector, force);
            double length;
            if (isSameDirection)
                length = force.Length - idealLength;
            else
                length = force.Length + idealLength;

            if (IsZero(force.Length))
                force = -positionVector;
            force.Normalize();
            if (length > 0)
                force *= -1;

            Vector springForce = Math.Pow(length / idealLength, 2) / Parameters.ElasticConstant * force;
            return springForce;
        }

        private Vector GetRepulsionForce(Point uPos, Point vPos, Size uSize, Size vSize, double repulsionRange)
        {
            Vector positionVector = uPos - vPos;
            if (IsZero(positionVector.Length))
            {
                var compensationVector = new Vector(Rand.NextDouble(), Rand.NextDouble());
                positionVector = compensationVector * 2;
                uPos += compensationVector;
                vPos -= compensationVector;
            }
            positionVector.Normalize();

            Point clippingPointU = LayoutUtils.GetClippingPoint(uSize, uPos, vPos);
            Point clippingPointV = LayoutUtils.GetClippingPoint(vSize, vPos, uPos);

            Vector force = clippingPointU - clippingPointV;
            var isSameDirection = LayoutUtils.IsSameDirection(positionVector, force);

            if (isSameDirection && force.Length > repulsionRange)
                return new Vector();

            double length = Math.Max(1, force.Length);
            length = Math.Pow(isSameDirection ? length / (Parameters.IdealEdgeLength * 2.0) : 1 / length, 2);
            Vector repulsionForce = Parameters.RepulsionConstant / length * positionVector * _phaseDependentRepulsionMultiplier;
            return repulsionForce;
        }

        /// <summary>
        /// Applies the attraction forces (between the end nodes of the edges).
        /// </summary>
        private void ApplySpringForces()
        {
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                if (!AllTreesGrown && (_removedRootTreeNodes.Contains(edge.Source) || _removedRootTreeNodes.Contains(edge.Target)))
                    continue;

                // Get the ideal edge length
                double idealLength = Parameters.IdealEdgeLength;
                VertexData u = _verticesData[edge.Source];
                VertexData v = _verticesData[edge.Target];
                double multiplier = (u.Level + v.Level) / 2.0 + 1;
                if (IsInterGraphEdge(edge))
                {
                    idealLength *= 1 + (u.Level + v.Level + 1) * Parameters.NestingFactor;
                }

                Vector springForce = GetSpringForce(idealLength, u.Position, v.Position, u.Size, v.Size) * multiplier;

                // Aggregate the forces
                if ((u.IsFixedToParent && u.MovableParent is null) ^ (v.IsFixedToParent && v.MovableParent is null))
                    springForce *= 2;

                if (!u.IsFixedToParent)
                {
                    u.SpringForce += springForce;
                }
                else if (u.MovableParent != null)
                {
                    u.MovableParent.SpringForce += springForce;
                }

                if (!v.IsFixedToParent)
                {
                    v.SpringForce -= springForce;
                }
                else if (v.MovableParent != null)
                {
                    v.MovableParent.SpringForce -= springForce;
                }
            }
        }

        /// <summary>
        /// Applies the repulsion forces between every node-pair.
        /// </summary>
        private void ApplyRepulsionForces()
        {
            double repulsionRange = Parameters.IdealEdgeLength * Parameters.SeparationMultiplier;
            for (int i = Levels.Count - 1; i >= 0; --i)
            {
                var checkedVertices = new HashSet<TVertex>();
                foreach (TVertex uVertex in Levels[i])
                {
                    checkedVertices.Add(uVertex);
                    VertexData uData = _verticesData[uVertex];
                    foreach (TVertex vVertex in Levels[i])
                    {
                        if (checkedVertices.Contains(vVertex))
                            continue;

                        VertexData vData = _verticesData[vVertex];

                        if (uData.Parent != vData.Parent)
                            continue; // The two vertices are not in the same graph

                        Vector repulsionForce = GetRepulsionForce(
                            uData.Position,
                            vData.Position,
                            uData.Size,
                            vData.Size,
                            repulsionRange) * Math.Pow(uData.Level + 1, 2);

                        if (uData.IsFixedToParent ^ vData.IsFixedToParent)
                            repulsionForce *= 2;
                        if (!uData.IsFixedToParent)
                            uData.RepulsionForce += repulsionForce;
                        if (!vData.IsFixedToParent)
                            vData.RepulsionForce -= repulsionForce;
                    }
                }
            }
        }

        /// <summary>
        /// Applies the gravitation forces.
        /// </summary>
        private void ApplyGravitationForces()
        {
            for (int i = Levels.Count - 1; i >= 0; --i)
            {
                foreach (TVertex uVertex in Levels[i])
                {
                    VertexData uData = _verticesData[uVertex];
                    Point center = uData.Parent.InnerCanvasCenter;

                    Vector gravitationForce = center - uData.Position;
                    if (IsZero(gravitationForce.Length))
                        continue;

                    double length = Math.Max(1, gravitationForce.Length / (Parameters.IdealEdgeLength * 2.0));
                    gravitationForce.Normalize();
                    gravitationForce *= Parameters.GravitationFactor * _gravityForceMagnitude * Math.Pow(uData.Level + 1, 2) / Math.Pow(length, 0.25);
                    uData.GravitationForce += gravitationForce;
                }
            }
        }

        /// <summary>
        /// Applies the application specific forces to the vertices.
        /// </summary>
        protected virtual void ApplyApplicationSpecificForces()
        {
        }

        private void CalculateNodePositionsAndSizes()
        {
            for (int i = Levels.Count - 1; i >= 0; --i)
            {
                foreach (TVertex uVertex in Levels[i])
                {
                    VertexData uData = _verticesData[uVertex];
                    uData.ApplyForce(_temperature * Math.Max(1, _step) / 100.0 * Parameters.DisplacementLimitMultiplier);
                }
            }
        }

        [Pure]
        private bool IsInterGraphEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);
            return _verticesData[edge.Source].Parent != _verticesData[edge.Target].Parent;
        }
    }
}