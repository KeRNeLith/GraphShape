using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using QuickGraph;

namespace Palesz.QuickGraph.Test.Metrics
{
	public abstract class MetricCalculatorBase<TVertex, TEdge, TGraph> : IMetricCalculator<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
		public TGraph Graph { get; private set; }
		public IDictionary<TVertex, Point> Positions { get; private set; }
		public IDictionary<TVertex, Size> Sizes { get; private set; }
		public IDictionary<TEdge, Point[]> EdgeRoutes { get; private set; }

		public MetricCalculatorBase( TGraph graph, IDictionary<TVertex, Point> vertexPositions, IDictionary<TVertex, Size> vertexSizes, IDictionary<TEdge, Point[]> edgeRoutes )
		{
			Graph = graph;
			Positions = vertexPositions;
			Sizes = vertexSizes;
			EdgeRoutes = edgeRoutes;
		}

		public abstract void Calculate();
	}
}
