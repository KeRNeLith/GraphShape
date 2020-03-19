using System;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        [Flags]
        private enum Barycenter
        {
            Up = 1,
            Down = 2,
            Sub = 4
        }

        [Flags]
        private enum CrossCount
        {
            Up = 1,
            Down = 2
        }
    }
}