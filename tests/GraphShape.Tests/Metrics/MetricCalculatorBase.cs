using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Tests
{
    /// <summary>
    /// Base class for metrics.
    /// </summary>
    public abstract class MetricCalculatorBase<TVertex, TEdge, TGraph> : IMetricCalculator
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        [NotNull]
        public TGraph Graph { get; }

        [NotNull]
        public IDictionary<TVertex, Point> Positions { get; }

        [NotNull]
        public IDictionary<TVertex, Size> Sizes { get; }

        [NotNull]
        public IDictionary<TEdge, Point[]> EdgeRoutes { get; }

        protected MetricCalculatorBase(
            [NotNull] TGraph graph,
            [NotNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [NotNull] IDictionary<TEdge, Point[]> edgeRoutes)
        {
            Graph = graph;
            Positions = verticesPositions;
            Sizes = verticesSizes;
            EdgeRoutes = edgeRoutes;
        }

        /// <inheritdoc />
        public abstract void Calculate();
    }
}
