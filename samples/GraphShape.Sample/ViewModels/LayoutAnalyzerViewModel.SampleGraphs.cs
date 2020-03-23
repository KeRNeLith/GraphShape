using System.Linq;

namespace GraphShape.Sample.ViewModels
{
    internal partial class LayoutAnalyzerViewModel
    {
        private void CreateSampleGraph()
        {
            #region SimpleTree

            var graph = new PocGraph();

            for (int i = 0; i < 8; ++i)
            {
                var vertex = new PocVertex(i.ToString());
                graph.AddVertex(vertex);
            }

            PocVertex[] vertices = graph.Vertices.ToArray();

            graph.AddEdge(new PocEdge("0to1", vertices[0], vertices[1]));
            graph.AddEdge(new PocEdge("1to2", vertices[1], vertices[2]));
            graph.AddEdge(new PocEdge("2to3", vertices[2], vertices[3]));
            graph.AddEdge(new PocEdge("2to4", vertices[2], vertices[4]));
            graph.AddEdge(new PocEdge("0to5", vertices[0], vertices[5]));
            graph.AddEdge(new PocEdge("1to7", vertices[1], vertices[7]));
            graph.AddEdge(new PocEdge("4to6", vertices[4], vertices[6]));
            graph.AddEdge(new PocEdge("0to4", vertices[0], vertices[4]));

            GraphModels.Add(new GraphViewModel("Fa", graph));

            #endregion
        }
    }
}