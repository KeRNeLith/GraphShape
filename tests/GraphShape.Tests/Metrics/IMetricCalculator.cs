using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuikGraph;
using System.Windows;

namespace Palesz.QuickGraph.Test.Metrics
{
	public interface IMetricCalculator<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
		/*
		 * TGraph graph, IDictionary<TVertex, Point> verticesPositions, IDictionary<TVertex, Size> vertexSize, IDictionary<TEdge, Point[]> edgeRoutes
		 */
		void Calculate();
	}
}
