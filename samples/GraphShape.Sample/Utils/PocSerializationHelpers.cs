using QuikGraph.Serialization;
using System.Xml;
using JetBrains.Annotations;

namespace GraphShape.Sample.Utils
{
    /// <summary>
    /// Serialization helpers related to <see cref="PocGraph"/>.
    /// </summary>
    internal static class PocSerializeHelper
    {
        /// <summary>
        /// Loads a graph from file.
        /// </summary>
        [Pure]
        [NotNull]
        public static PocGraph LoadGraph([NotNull] string filePath)
        {
            // Open the file of the graph
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                // Create the serializer
                var serializer = new GraphMLDeserializer<PocVertex, PocEdge, PocGraph>();

                // Graph where the vertices and edges should be put in
                var pocGraph = new PocGraph();

                // Deserialize the graph
                serializer.Deserialize(
                    reader,
                    pocGraph,
                    id => new PocVertex(id),
                    (source, target, id) => new PocEdge(id, source, target));

                return pocGraph;
            }
        }

        /// <summary>
        /// Saves a graph to file.
        /// </summary>
        public static void SaveGraph([NotNull] PocGraph graph, [NotNull] string filePath)
        {
            // Create the xml writer
            using (var writer = XmlWriter.Create(filePath))
            {
                var serializer = new GraphMLSerializer<PocVertex, PocEdge, PocGraph>();

                // Serialize the graph
                serializer.Serialize(writer, graph, v => v.ID, e => e.ID);
            }
        }
    }
}