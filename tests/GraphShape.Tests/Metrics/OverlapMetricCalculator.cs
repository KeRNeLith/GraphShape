using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Tests
{
    /// <summary>
    /// Overlap metric.
    /// </summary>
    internal class OverlapMetricCalculator<TVertex, TEdge, TGraph> : MetricCalculatorBase<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        public OverlapMetricCalculator(
            [NotNull] TGraph graph,
            [NotNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [NotNull] IDictionary<TEdge, Point[]> edgeRoutes)
            : base(graph, verticesPositions, verticesSizes, edgeRoutes)
        {
        }

        public int OverlapCount { get; private set; }

        public double OverlappedArea { get; private set; }

        /// <inheritdoc />
        public override void Calculate()
        {
            TVertex[] vertices = Graph.Vertices.ToArray();
            for (int i = 0; i < vertices.Length - 1; ++i)
            {
                for (int j = i + 1; j < vertices.Length; ++j)
                {
                    TVertex vertex1 = vertices[i];
                    TVertex vertex2 = vertices[j];

                    Point pos1 = Positions[vertex1];
                    Point pos2 = Positions[vertex2];

                    Size size1 = Sizes[vertex1];
                    Size size2 = Sizes[vertex2];

                    var zone1 = new Rect(
                        pos1.X - size1.Width / 2,
                        pos1.Y - size1.Height / 2,
                        size1.Width,
                        size1.Height);
                    var zone2 = new Rect(
                        pos2.X - size2.Width / 2,
                        pos2.Y - size2.Height / 2,
                        size2.Width,
                        size2.Height);

                    // Check whether the vertices overlaps or not
                    zone1.Intersect(zone2);

                    if (zone1.Width > 0 && zone1.Height > 0)
                    {
                        ++OverlapCount;
                        OverlappedArea += zone1.Width * zone1.Height;
                    }
                }
            }
        }
    }
}
