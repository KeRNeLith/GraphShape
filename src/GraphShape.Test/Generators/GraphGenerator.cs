using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace Palesz.QuickGraph.Test.Generators
{
	public static class GraphGenerator
	{

		public static IBidirectionalGraph<TVertex, TEdge> CreateTree<TVertex, TEdge>( int vertexCount, int componentCount, Func<int, TVertex> vertexFactory, Func<TVertex, TVertex, TEdge> edgeFactory )
			where TEdge : IEdge<TVertex>
		{
			BidirectionalGraph<TVertex, TEdge> treeGraph = new BidirectionalGraph<TVertex, TEdge>( false, vertexCount );

			for ( int i = 0; i < componentCount; i++ )
			{
				treeGraph.AddVertex( vertexFactory( i ) );
			}

			Random rnd = new Random( DateTime.Now.Millisecond );

			for ( int n = treeGraph.VertexCount; n < vertexCount; n++ )
			{
				int parentNum = rnd.Next( treeGraph.VertexCount );
				TVertex parent = treeGraph.Vertices.ElementAt( parentNum );

				TVertex child = vertexFactory( n );
				treeGraph.AddVertex( child );
				treeGraph.AddEdge( edgeFactory( parent, child ) );
			}

			return treeGraph;
		}

		public static IBidirectionalGraph<TVertex, TEdge> CreateDAG<TVertex, TEdge>( int vertexCount, int edgeCount, int maxParent, int maxChild, bool parallelEdgeAllowed, Func<int, TVertex> vertexFactory, Func<TVertex, TVertex, TEdge> edgeFactory )
			where TEdge : IEdge<TVertex>
		{
			BidirectionalGraph<TVertex, TEdge> dagGraph = new BidirectionalGraph<TVertex, TEdge>( false, vertexCount );

			Dictionary<int, TVertex> vertexMap = new Dictionary<int, TVertex>();

			for ( int i = 0; i < vertexCount; i++ )
			{
				TVertex v = vertexFactory( i );
				vertexMap[i] = v;
				dagGraph.AddVertex( v );
			}

			Random rnd = new Random( DateTime.Now.Millisecond );
			int childIndex;
			int parentIndex;
			TVertex child;
			TVertex parent;
			for ( int i = 0; i < edgeCount; i++ )
			{
				do
				{
					childIndex = rnd.Next( vertexCount - 1 ) + 1;
					parentIndex = rnd.Next( childIndex );
					child = vertexMap[childIndex];
					parent = vertexMap[parentIndex];
				} while ( ( !parallelEdgeAllowed && dagGraph.ContainsEdge( parent, child ) ) ||
					dagGraph.OutDegree( parent ) >= maxChild ||
					dagGraph.InDegree( child ) >= maxParent );

				//create the edge between the 2 vertex
				TEdge e = edgeFactory( parent, child );
				dagGraph.AddEdge( e );
			}

			return dagGraph;
		}


		public static IBidirectionalGraph<TVertex, TEdge> CreateGeneralGraph<TVertex, TEdge>( int vertexCount, int edgeCount, int maxDegree, bool parallelEdgeAllowed, Func<int, TVertex> vertexFactory, Func<TVertex, TVertex, TEdge> edgeFactory )
			where TEdge : IEdge<TVertex>
		{
			BidirectionalGraph<TVertex, TEdge> graph = new BidirectionalGraph<TVertex, TEdge>( false, vertexCount );

			Dictionary<int, TVertex> vertexMap = new Dictionary<int, TVertex>();

			for ( int i = 0; i < vertexCount; i++ )
			{
				TVertex v = vertexFactory( i );
				vertexMap[i] = v;
				graph.AddVertex( v );
			}

			Random rnd = new Random( DateTime.Now.Millisecond );
			int childIndex;
			int parentIndex;
			TVertex child;
			TVertex parent;
			for ( int i = 0; i < edgeCount; i++ )
			{
				do
				{
					childIndex = rnd.Next( vertexCount );
					parentIndex = rnd.Next( vertexCount );
					child = vertexMap[childIndex];
					parent = vertexMap[parentIndex];
				} while ( childIndex == parentIndex ||
					( !parallelEdgeAllowed && graph.ContainsEdge( parent, child ) ) ||
					graph.Degree( parent ) >= maxDegree ||
					graph.Degree( child ) >= maxDegree );

				//create the edge between the 2 vertex
				TEdge e = edgeFactory( parent, child );
				graph.AddEdge( e );
			}

			return graph;
		}
	}
}
