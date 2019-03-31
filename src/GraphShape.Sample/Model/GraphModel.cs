namespace GraphSharp.Sample.Model
{
	public class GraphModel
	{
		public string Name { get; private set; }
		public PocGraph Graph { get; private set; }

		public GraphModel(string name, PocGraph graph)
		{
			Name = name;
			Graph = graph;
		}
	}
}