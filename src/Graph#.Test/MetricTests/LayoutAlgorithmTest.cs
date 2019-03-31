using GraphSharp.Algorithms.EdgeRouting;
using GraphSharp.Algorithms.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph;
using System.Windows;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using Palesz.QuickGraph.Test.Metrics;
using System.Diagnostics;
using System.Xml;
using System.Linq;
using Palesz.QuickGraph.Test.Generators;

namespace GraphSharp.Test.MetricTests
{
	/*[DeploymentItem( "QuickGraph.dll" )]
	[TestClass()]
	public class LayoutAlgorithmTest
	{

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		[Serializable]
		[XmlInclude( typeof( CircularLayoutParameters ) )]
		[XmlInclude( typeof( SugiyamaLayoutParameters ) )]
		[XmlInclude( typeof( FreeFRLayoutParameters ) )]
        [XmlInclude( typeof( BoundedFRLayoutParameters) )]
		[XmlInclude( typeof( ISOMLayoutParameters ) )]
		[XmlInclude( typeof( KKLayoutParameters ) )]
		[XmlInclude( typeof( LinLogLayoutParameters ) )]
		[XmlInclude( typeof( SimpleTreeLayoutParameters ) )]
		public class LayoutResults
		{
			public string LayoutType;
			public string GraphType;
			public int VertexCount;
			public int EdgeCount;

			public long Milliseconds;
			public int OverlapCount;
			public double OverlappedArea;
			public double Area;
			public double Ratio;
			public int CrossCount;
			public double MinimumEdgeLength;
			public double MaximumEdgeLength;
			public double AverageEdgeLength;
			public double MinimumAngle;
			public double MaximumAngle;
			public double AverageAngle;
			public LayoutParametersBase Parameters;

			public void DivideBy( int n )
			{
				Milliseconds /= n;
				OverlapCount /= n;
				OverlappedArea /= (double)n;
				Area /= (double)n;
				Ratio /= (double)n;
				CrossCount /= n;
				MinimumEdgeLength /= (double)n;
				MaximumEdgeLength /= (double)n;
				AverageEdgeLength /= (double)n;
				MinimumAngle /= (double)n;
				MaximumAngle /= (double)n;
				AverageAngle /= (double)n;
			}
		}

		[DeploymentItem( "QuickGraph.dll" )]
		[TestMethod()]
		public void LayoutTests()
		{
			List<LayoutResults> results = new List<LayoutResults>();
			//tree teszteles

			//5 tree egy parameterhalmazhoz
			//mind az 5 tree-n vegigmegy az osszes algoritmus, es atlagokat szamol a metrikakra

			var tree = GraphGenerator.CreateTree<string, Edge<string>>( 20, 2, ( i ) => i.ToString(), ( s, t ) => new Edge<string>( s, t ) );
			var vertexSizes = tree.Vertices.ToDictionary<string, string, Size>(
				( s ) => s, ( s ) => new Size( 20, 20 ) );
			LayoutResults res = new LayoutResults();
			TestLayout<string, Edge<string>>( "Tree", new CircularLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>( tree, null, vertexSizes, new CircularLayoutParameters() ), vertexSizes, res );

			results.Add( res );

			XmlSerializer serializer = new XmlSerializer( typeof( List<LayoutResults> ) );
			XmlWriter writer = new XmlTextWriter( string.Format( "C:\\Users\\Palesz\\Desktop\\{0}_results.xml", "tree" ), null );
			serializer.Serialize( writer, results );

			//dag teszteles

			//graph teszteles
		}

		[DeploymentItem( "QuickGraph.dll" )]
		public void TestLayout<TVertex, TEdge>( string graphType, ILayoutAlgorithm<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> algo, IDictionary<TVertex, Size> vertexSizes, LayoutResults results )
			where TVertex : class
			where TEdge : IEdge<TVertex>
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			algo.Compute();

			results.LayoutType = algo.GetType().Name;
			results.GraphType = graphType;
			results.VertexCount = algo.VisitedGraph.VertexCount;
			results.EdgeCount = algo.VisitedGraph.EdgeCount;

			results.Milliseconds += watch.ElapsedMilliseconds;
			watch.Stop();

			var edgeRoutes = ( algo is IEdgeRoutingAlgorithm<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> ? ( algo as IEdgeRoutingAlgorithm<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> ).EdgeRoutes : new Dictionary<TEdge, Point[]>() );

			//do the metrics
			OverlapMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> overlapMetric = new OverlapMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>( algo.VisitedGraph, algo.VertexPositions, vertexSizes, edgeRoutes );
			results.OverlapCount += overlapMetric.OverlapCount;
			results.OverlappedArea += overlapMetric.OverlappedArea;

			LayoutAreaMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> areaMetric = new LayoutAreaMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>( algo.VisitedGraph, algo.VertexPositions, vertexSizes, edgeRoutes );
			results.Area += areaMetric.Area;
			results.Ratio += areaMetric.Ratio;

			EdgeMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>> edgeMetric = new EdgeMetricCalculator<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>( algo.VisitedGraph, algo.VertexPositions, vertexSizes, edgeRoutes );
			results.CrossCount += edgeMetric.CrossCount;
			results.MinimumEdgeLength += edgeMetric.MinimumEdgeLength;
			results.MaximumEdgeLength += edgeMetric.MaximumEdgeLength;
			results.AverageEdgeLength += edgeMetric.AverageEdgeLength;
			results.MinimumAngle += edgeMetric.MinimumAngle;
			results.MaximumAngle += edgeMetric.MaximumAngle;
			results.AverageAngle += edgeMetric.AverageAngle;
		}
	}*/
}