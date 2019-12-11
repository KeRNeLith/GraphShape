using System.Collections.Generic;
using System.Windows;
using QuikGraph;
using QuikGraph.Algorithms;

namespace GraphSharp.Algorithms.EdgeRouting
{
    public interface IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> : IAlgorithm<TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        IDictionary<TEdge, Point[]> EdgeRoutes { get; }
    }
}