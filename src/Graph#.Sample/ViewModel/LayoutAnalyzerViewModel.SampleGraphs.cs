using System.Linq;
using GraphSharp.Sample.Model;

namespace GraphSharp.Sample.ViewModel
{
	public partial class LayoutAnalyzerViewModel
	{
		partial void CreateSampleGraphs()
		{
			#region SimpleTree

			var graph = new PocGraph();

			for (int i = 0; i < 8; i++)
			{
				var v = new PocVertex(i.ToString());
				graph.AddVertex(v);
			}

			graph.AddEdge(new PocEdge("0to1", graph.Vertices.ElementAt(0), graph.Vertices.ElementAt(1)));
			graph.AddEdge(new PocEdge("1to2", graph.Vertices.ElementAt(1), graph.Vertices.ElementAt(2)));
			graph.AddEdge(new PocEdge("2to3", graph.Vertices.ElementAt(2), graph.Vertices.ElementAt(3)));
			graph.AddEdge(new PocEdge("2to4", graph.Vertices.ElementAt(2), graph.Vertices.ElementAt(4)));
			graph.AddEdge(new PocEdge("0to5", graph.Vertices.ElementAt(0), graph.Vertices.ElementAt(5)));
			graph.AddEdge(new PocEdge("1to7", graph.Vertices.ElementAt(1), graph.Vertices.ElementAt(7)));
			graph.AddEdge(new PocEdge("4to6", graph.Vertices.ElementAt(4), graph.Vertices.ElementAt(6)));

			GraphModels.Add(new GraphModel("Fa", graph));

			#endregion
		}
	}
}