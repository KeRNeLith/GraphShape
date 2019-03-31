using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using QuickGraph;

namespace Palesz.QuickGraph.Test.Metrics
{
	public class OverlapMetricCalculator<TVertex, TEdge, TGraph> : MetricCalculatorBase<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
		public OverlapMetricCalculator( TGraph graph, IDictionary<TVertex, Point> vertexPositions, IDictionary<TVertex, Size> vertexSizes, IDictionary<TEdge, Point[]> edgeRoutes )
			: base( graph, vertexPositions, vertexSizes, edgeRoutes ) { }

		public int OverlapCount { get; private set; }
		public double OverlappedArea { get; private set; }

		public override void Calculate()
		{
			var vertices = Graph.Vertices.ToArray();
			for ( int i = 0; i < vertices.Length - 1; i++ )
			{
				for ( int j = i + 1; j < vertices.Length; j++ )
				{
					var v1 = vertices[i];
					var v2 = vertices[j];

					var p1 = Positions[v1];
					var p2 = Positions[v2];
					
					var s1 = Sizes[v1];
					var s2 = Sizes[v2];

					Rect r1 = new Rect( p1.X - s1.Width / 2, p1.Y - s1.Height / 2, p1.X + s1.Width / 2, p1.Y + s1.Height / 2 );
					Rect r2 = new Rect( p2.X - s1.Width / 2, p2.Y - s1.Height / 2, p2.X + s1.Width / 2, p2.Y + s1.Height / 2 );

					//check whether the vertices overlaps or not
					r1.Intersect( r2 );

					if ( r1.Width > 0 && r1.Height > 0 )
					{
						OverlapCount++;
						OverlappedArea += r1.Width * r1.Height;
					}
				}
			}
		}
	}
}
