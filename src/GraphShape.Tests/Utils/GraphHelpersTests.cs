using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using GraphShape;
using NUnit.Framework;

namespace GraphShape.Tests
{
	/// <summary>
	/// Summary description for GraphHelpersTests
	/// </summary>
	[TestFixture]
	public class GraphHelpersTests
	{
		BidirectionalGraph<string, Edge<string>> directedGraph;
		UndirectedBidirectionalGraph<string, Edge<string>> undirectedGraph;
		string one = "one";
		string two = "two";
		string three = "three";
		string four = "four";


		public GraphHelpersTests()
		{
			#region create directedGraph
			directedGraph = new BidirectionalGraph<string, Edge<string>>( );

			directedGraph.AddVertex( one ); directedGraph.AddVertex( two ); directedGraph.AddVertex( three ); directedGraph.AddVertex( four );
			directedGraph.AddEdge( new Edge<string>( one, four ) );
			directedGraph.AddEdge( new Edge<string>( one, three ) );
			directedGraph.AddEdge( new Edge<string>( four, two ) );
			directedGraph.AddEdge( new Edge<string>( one, two ) );
			#endregion

			#region create undirected graph
			undirectedGraph = new UndirectedBidirectionalGraph<string, Edge<string>>( directedGraph );
			#endregion
		}

		[Test]
		public void EdgesBetweenDirectedTestOne()
		{
			List<string> set1 = new List<string>( );
			set1.Add( one ); set1.Add( two );

			List<string> set2 = new List<string>( );
			set2.Add( three ); set2.Add( four );

			List<Edge<string>> result = directedGraph.GetEdgesBetween( set1, set2 ).ToList();

			Assert.AreEqual( 2, result.Count );
			Assert.AreEqual( one, result[ 0 ].Source );
			Assert.AreEqual( four, result[ 0 ].Target );
			Assert.AreEqual( one, result[ 1 ].Source );
			Assert.AreEqual( three, result[ 1 ].Target );
		}

		[Test]
		public void EdgesBetweenDirectedTestTwo()
		{
			List<string> set1 = new List<string>( );
			set1.Add( one ); set1.Add( two );

			List<string> set2 = new List<string>( );
			set2.Add( three ); set2.Add( four );

			List<Edge<string>> result = directedGraph.GetEdgesBetween( set2, set1 ).ToList();

			Assert.AreEqual( 1, result.Count );
			Assert.AreEqual( four, result[ 0 ].Source );
			Assert.AreEqual( two, result[ 0 ].Target );
		}
	}
}