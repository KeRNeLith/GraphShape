using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Tests
{
    internal class PositionsMetricCalculator<TVertex, TEdge, TGraph> : MetricCalculatorBase<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        public PositionsMetricCalculator(
            [NotNull] TGraph graph,
            [NotNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [NotNull] IDictionary<TEdge, Point[]> edgeRoutes)
            : base(graph, verticesPositions, verticesSizes, edgeRoutes)
        {
        }

        public bool PositionsSet { get; private set; } = true;

        /// <inheritdoc />
        public override void Calculate()
        {
            foreach (TVertex vertex in Graph.Vertices)
            {
                if (!Positions.TryGetValue(vertex, out Point position))
                {
                    PositionsSet = false;
                    return;
                }

                if (double.IsNaN(position.X)
                    || double.IsInfinity(position.X)
                    || double.IsNaN(position.Y)
                    || double.IsInfinity(position.Y))
                {
                    PositionsSet = false;
                    return;
                }
            }
        }
    }
}
