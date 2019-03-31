using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using System.Windows;

namespace Palesz.QuickGraph.Test.Metrics
{
	public interface IMetricCalculator<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
		/*
		 * TGraph graph, IDictionary<TVertex, Point> vertexPositions, IDictionary<TVertex, Size> vertexSize, IDictionary<TEdge, Point[]> edgeRoutes
		 */
		void Calculate();
	}
}
