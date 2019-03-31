using QuickGraph.Serialization;
using System.Xml;

namespace GraphSharp.Sample
{
	public static class PocSerializeHelper
	{
		public static PocGraph LoadGraph( string filename )
		{
			//open the file of the graph
			var reader = XmlReader.Create( filename );

			//create the serializer
			var serializer = new GraphMLDeserializer<PocVertex, PocEdge, PocGraph>();

			//graph where the vertices and edges should be put in
			var pocGraph = new PocGraph();

			//deserialize the graph
			serializer.Deserialize( reader, pocGraph,
			                        id => new PocVertex( id ),
			                        ( source, target, id ) => new PocEdge( id, source, target ) );

			return pocGraph;
		}

		public static void SaveGraph( PocGraph graph, string filename )
		{
			//create the xml writer
			using ( var writer = XmlWriter.Create( filename ) )
			{
				var serializer = new GraphMLSerializer<PocVertex, PocEdge, PocGraph>();

				//serialize the graph
				serializer.Serialize( writer, graph, v => v.ID, e => e.ID );
			}
		}
	}
}