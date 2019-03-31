using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using System.Windows;

namespace Palesz.QuickGraph.Test.Metrics
{
	public class LayoutAreaMetricCalculator<TVertex, TEdge, TGraph> : MetricCalculatorBase<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
		public LayoutAreaMetricCalculator( TGraph graph, IDictionary<TVertex, Point> vertexPositions, IDictionary<TVertex, Size> vertexSizes, IDictionary<TEdge, Point[]> edgeRoutes )
			: base( graph, vertexPositions, vertexSizes, edgeRoutes ) { }

		public double Area { get; private set; }
		public double Ratio { get; private set; }

		public override void Calculate()
		{
			Point topLeft = new Point( double.PositiveInfinity, double.PositiveInfinity );
			Point bottomRight = new Point( double.NegativeInfinity, double.NegativeInfinity );

			foreach ( var v in Graph.Vertices )
			{
				Point p = Positions[v];
				Size s = Sizes[v];
				topLeft.X = Math.Min( p.X - s.Width / 2.0, topLeft.X );
				topLeft.Y = Math.Min( p.Y - s.Height / 2.0, topLeft.Y );

				bottomRight.X = Math.Max( p.X + s.Width / 2.0, bottomRight.X );
				bottomRight.Y = Math.Max( p.Y + s.Height / 2.0, bottomRight.Y );
			}

			foreach ( var e in Graph.Edges )
			{
				Point[] routePoints = null;
				if ( !EdgeRoutes.TryGetValue( e, out routePoints ) || routePoints == null || routePoints.Length == 0)
					continue;

				for ( int i = 0; i < routePoints.Length; i++ )
				{
					Point p = routePoints[i];
					topLeft.X = Math.Min( p.X, topLeft.X );
					topLeft.Y = Math.Min( p.Y, topLeft.Y );

					bottomRight.X = Math.Max( p.X, bottomRight.X );
					bottomRight.Y = Math.Max( p.Y, bottomRight.Y );
				}
			}

			Vector layoutAreaSize = bottomRight - topLeft;

			Area = layoutAreaSize.LengthSquared;
			Ratio = layoutAreaSize.X / layoutAreaSize.Y;
		}
	}
}
