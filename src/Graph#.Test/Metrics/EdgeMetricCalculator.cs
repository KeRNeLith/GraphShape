using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using System.Windows;

namespace Palesz.QuickGraph.Test.Metrics
{
	public class EdgeMetricCalculator<TVertex, TEdge, TGraph> : MetricCalculatorBase<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
		public EdgeMetricCalculator( TGraph graph, IDictionary<TVertex, Point> vertexPositions, IDictionary<TVertex, Size> vertexSizes, IDictionary<TEdge, Point[]> edgeRoutes )
			: base( graph, vertexPositions, vertexSizes, edgeRoutes ) { }

		public int CrossCount { get; private set; }

		public double MinimumEdgeLength { get; private set; }
		public double MaximumEdgeLength { get; private set; }
		public double AverageEdgeLength { get; private set; }

		public double MinimumAngle { get; private set; }
		public double MaximumAngle { get; private set; }
		public double AverageAngle { get; private set; }

		public override void Calculate()
		{
			var edges = Graph.Edges.ToArray();
			var edgePoints = new List<Point>[edges.Length];

			int segmentCount = 0;

			//create the points of the edges
			for ( int i = 0; i < edges.Length - 1; i++ )
			{
				var edge = edges[i];
				Point[] route = null;
				List<Point> points = null;
				if ( EdgeRoutes.TryGetValue( edge, out route ) && route != null && route.Length > 0 )
					points = new List<Point>( route.Length + 2 );
				else
					points = new List<Point>( 2 );
				points.Add( Positions[edge.Source] );
				if ( route != null && route.Length > 0 )
					points.AddRange( route );
				points.Add( Positions[edge.Target] );

				for ( int j = 0; j < points.Count-1; j++ )
				{
					double length = (points[j] - points[j - 1]).Length;

					MinimumEdgeLength = Math.Min( MinimumEdgeLength, length );
					MaximumEdgeLength = Math.Max( MaximumEdgeLength, length );
					AverageEdgeLength += length;
					segmentCount += 1;
				}
			}

			//check the crosses
			for ( int i = 0; i < edges.Length - 1; i++ )
			{
				for ( int j = i + 1; j < edges.Length; j++ )
				{
					List<Point> edgePoints1 = edgePoints[i];
					List<Point> edgePoints2 = edgePoints[j];

					for ( int ii = 0; ii < edgePoints1.Count - 1; ii++ )
					{
						var p11 = edgePoints1[ii];
						var p12 = edgePoints1[ii + 1];
						if ( p12.X < p11.X )
						{
							Point p = p12;
							p12 = p11;
							p11 = p;
						}
						for ( int jj = 0; jj < edgePoints2.Count - 1; jj++ )
						{
							var p21 = edgePoints2[jj];
							var p22 = edgePoints2[jj + 1];
							if ( p22.X < p21.X )
							{
								Point p = p22;
								p22 = p21;
								p21 = p22;
							}

							p11.X = p21.X = Math.Max( p11.X, p21.X );
							p12.X = p22.X = Math.Min( p12.X, p22.X );

							if ( ( p11.Y - p21.Y ) * ( p12.Y - p22.Y ) < 0 )
							{
								//the edges crosses each other
								CrossCount += 1;

								Vector v1 = p11 - p12;
								Vector v2 = p21 - p22;

								double angle = Math.Acos( Math.Abs( ( v1.X * v2.X + v1.Y * v2.Y ) / ( v1.Length * v2.Length ) ) );

								MinimumAngle = Math.Min( MinimumAngle, angle );
								MaximumAngle = Math.Max( MaximumAngle, angle );
								AverageAngle += angle;
							}
						}
					}
				}
			}

			AverageAngle /= segmentCount;
			AverageEdgeLength /= segmentCount;
		}
	}
}
