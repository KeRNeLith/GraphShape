using System;
using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Simple.Circular
{
    /// <summary>
    /// Circular layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type</typeparam>
    public class CircularLayoutAlgorithm<TVertex, TEdge, TGraph>
        : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, CircularLayoutParameters>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        [NotNull]
        private readonly IDictionary<TVertex, Size> _verticesSizes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public CircularLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] CircularLayoutParameters oldParameters)
            : base(visitedGraph, verticesPositions, oldParameters)
        {
            _verticesSizes = verticesSizes;
        }

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            // Calculate the size of the circle
            double perimeter = 0;
            var halfSize = new double[VisitedGraph.VertexCount];
            int i = 0;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                Size size = _verticesSizes[vertex];
                halfSize[i] = Math.Sqrt(size.Width * size.Width + size.Height * size.Height) * 0.5;
                perimeter += halfSize[i] * 2;
                ++i;
            }

            double radius = perimeter / (2 * Math.PI);

            // Pre-calculation
            double angle = 0;
            double a;
            i = 0;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                a = Math.Sin(halfSize[i] * 0.5 / radius) * 2;
                angle += a;
                if (ReportOnIterationEndNeeded)
                    VerticesPositions[vertex] = new Point(Math.Cos(angle) * radius + radius, Math.Sin(angle) * radius + radius);
                angle += a;
            }

            if (ReportOnIterationEndNeeded)
                OnIterationEnded(0, 50, "Pre-calculation done.", false);

            // Recalculate radius
            radius = angle / (2 * Math.PI) * radius;

            // Calculation
            angle = 0;
            i = 0;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                a = Math.Sin(halfSize[i] * 0.5 / radius) * 2;
                angle += a;
                VerticesPositions[vertex] = new Point(Math.Cos(angle) * radius + radius, Math.Sin(angle) * radius + radius);
                angle += a;
            }
        }

        #endregion
    }
}