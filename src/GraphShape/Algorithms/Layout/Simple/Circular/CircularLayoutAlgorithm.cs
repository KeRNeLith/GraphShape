using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public CircularLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [CanBeNull] CircularLayoutParameters oldParameters = null)
            : this(visitedGraph, null, verticesSizes, oldParameters)
        {
        }

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
            [CanBeNull] CircularLayoutParameters oldParameters = null)
            : base(visitedGraph, verticesPositions, oldParameters)
        {
            _verticesSizes = verticesSizes ?? throw new ArgumentNullException(nameof(verticesSizes));
        }

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            switch (VisitedGraph.VertexCount)
            {
                case 2:
                    TwoVerticesCircularLayout();
                    break;
                case 3:
                    ThreeVerticesCircularLayout();
                    break;
                default:
                    CircularLayout();
                    break;
            }

            #region Local functions

            void TwoVerticesCircularLayout()
            {
                TVertex[] vertices = VisitedGraph.Vertices.ToArray();
                VerticesPositions[vertices[0]] = default(Point);
                VerticesPositions[vertices[1]] = new Point(_verticesSizes[vertices[0]].Width * 1.3, 0);
            }

            void ThreeVerticesCircularLayout()
            {
                double maxWidth = _verticesSizes.Values.Max(size => size.Width);
                double maxHeight = _verticesSizes.Values.Max(size => size.Height);
                double radius = Math.Max(maxWidth, maxHeight) * 1.3;

                double angle = 0;
                foreach (TVertex vertex in VisitedGraph.Vertices)
                {
                    VerticesPositions[vertex] = new Point(
                        Math.Cos(angle) * radius + radius,
                        Math.Sin(angle) * radius + radius);
                    angle += 2 * Math.PI / 3;   // 120°
                }
            }

            void CircularLayout()
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
                    {
                        VerticesPositions[vertex] = new Point(
                            Math.Cos(angle) * radius + radius,
                            Math.Sin(angle) * radius + radius);
                    }
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
                    VerticesPositions[vertex] = new Point(
                        Math.Cos(angle) * radius + radius,
                        Math.Sin(angle) * radius + radius);
                    angle += a;
                }
            }

            #endregion
        }

        #endregion
    }
}