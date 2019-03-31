using GraphSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph;
using System.Diagnostics;

namespace GraphSharp.Test
{
	/// <summary>
	///This is a test class for LayeredTopologicalSortAlgorithmTest and is intended
	///to contain all LayeredTopologicalSortAlgorithmTest Unit Tests
	///</summary>
	[TestClass]
	public class LayeredTopologicalSortAlgorithmTest
	{

		[TestMethod]
		public void Simple()
		{
			var g = new BidirectionalGraph<string, Edge<string>>( );
			var vs = new string[ 3 ];
			for ( int i = 1; i < 4; i++ )
			{
				vs[ i - 1 ] = i.ToString( );
				g.AddVertex( i.ToString( ) );
			}
			g.AddEdge( new Edge<string>( vs[ 0 ], vs[ 1 ] ) );
			g.AddEdge( new Edge<string>( vs[ 1 ], vs[ 2 ] ) );

			var lts = new LayeredTopologicalSortAlgorithm<string, Edge<string>>( g );
			lts.Compute( );

			Assert.AreEqual( lts.LayerIndices[ vs[ 0 ] ], 0 );
			Assert.AreEqual( lts.LayerIndices[ vs[ 1 ] ], 1 );
			Assert.AreEqual( lts.LayerIndices[ vs[ 2 ] ], 2 );
		}


		[TestMethod]
		public void MultipleSource()
		{
			var g = new BidirectionalGraph<string, Edge<string>>( );
			var vs = new string[ 5 ];
			for ( int i = 1; i < 6; i++ )
			{
				vs[ i - 1 ] = i.ToString( );
				g.AddVertex( i.ToString( ) );
			}
			g.AddEdge( new Edge<string>( vs[ 0 ], vs[ 1 ] ) );
			g.AddEdge( new Edge<string>( vs[ 1 ], vs[ 2 ] ) );
			g.AddEdge( new Edge<string>( vs[ 3 ], vs[ 1 ] ) );
			g.AddEdge( new Edge<string>( vs[ 4 ], vs[ 2 ] ) );

			var lts = new LayeredTopologicalSortAlgorithm<string, Edge<string>>( g );
			lts.Compute( );

			Assert.AreEqual( 0, lts.LayerIndices[ vs[ 0 ] ] );
			Assert.AreEqual( 1, lts.LayerIndices[ vs[ 1 ] ] );
			Assert.AreEqual( 2, lts.LayerIndices[ vs[ 2 ] ] );
			Assert.AreEqual( 0, lts.LayerIndices[ vs[ 3 ] ] );
			Assert.AreEqual( 0, lts.LayerIndices[ vs[ 4 ] ] );
		}

		[TestMethod]
		public void NonAcyclic()
		{
			var g = new BidirectionalGraph<string, Edge<string>>( );
			var vs = new string[ 4 ];
			for ( int i = 1; i < 5; i++ )
			{
				vs[ i - 1 ] = i.ToString( );
				g.AddVertex( i.ToString( ) );
			}
			g.AddEdge( new Edge<string>( vs[ 0 ], vs[ 1 ] ) );
			g.AddEdge( new Edge<string>( vs[ 1 ], vs[ 2 ] ) );
			g.AddEdge( new Edge<string>( vs[ 2 ], vs[ 0 ] ) );
			g.AddEdge( new Edge<string>( vs[ 3 ], vs[ 0 ] ) );

			try
			{
				var lts = new LayeredTopologicalSortAlgorithm<string, Edge<string>>( g );
				lts.Compute( );

				Assert.Fail( "It does not throw exception for non acyclic graphs." );
			}
			catch ( NonAcyclicGraphException ex )
			{
				Debug.WriteLine( ex.Message );
			}
		}
	}
}