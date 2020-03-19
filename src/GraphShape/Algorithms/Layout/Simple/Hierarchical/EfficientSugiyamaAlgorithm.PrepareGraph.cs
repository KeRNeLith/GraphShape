using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Search;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    public partial class EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        private void PrepareGraph()
        {
            RemoveCycles();
            RemoveLoops();
            RemoveIsolatedVertices(); // It must run after the two method above           
        }

        /// <summary>
        /// Removes the cycles from the original graph with simply reverting some edges.
        /// </summary>
        private void RemoveCycles()
        {
            // Find the cycle edges with dfs
            var cycleEdges = new List<SugiEdge>();
            var dfs = new DepthFirstSearchAlgorithm<SugiVertex, SugiEdge>(_graph);
            dfs.BackEdge += cycleEdges.Add;
            dfs.Compute();
            dfs.BackEdge -= cycleEdges.Add;

            // And revert them
            foreach (SugiEdge edge in cycleEdges)
            {
                _graph.RemoveEdge(edge);
                _graph.AddEdge(new SugiEdge(edge.OriginalEdge, edge.Target, edge.Source));
            }
        }

        /// <summary>
        /// Removes the edges which source and target is the same vertex.
        /// </summary>
        private void RemoveLoops()
        {
            _graph.RemoveEdgeIf(edge => edge.Source == edge.Target);
        }

        private void RemoveIsolatedVertices()
        {
            _isolatedVertices = _graph.IsolatedVertices().ToArray();
            foreach (SugiVertex isolatedVertex in _isolatedVertices)
                _graph.RemoveVertex(isolatedVertex);
        }
    }
}
