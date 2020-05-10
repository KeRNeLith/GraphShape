using System;
using System.Linq;
using GraphShape.Factory;
using QuikGraph;

namespace GraphShape.Sample.ViewModels
{
    internal partial class LayoutAnalyzerViewModel
    {
        private void CreateSampleGraph()
        {
            #region Simple tree graph

            var graph = new PocGraph();

            PocVertex[] vertices = Enumerable.Range(0, 8).Select(VertexFactory).ToArray();
            graph.AddVertexRange(vertices);
            graph.AddEdgeRange(new []
            {
                EdgeFactory(vertices[0], vertices[1]),
                EdgeFactory(vertices[1], vertices[2]),
                EdgeFactory(vertices[2], vertices[3]),
                EdgeFactory(vertices[2], vertices[4]),
                EdgeFactory(vertices[0], vertices[5]),
                EdgeFactory(vertices[1], vertices[7]),
                EdgeFactory(vertices[4], vertices[6]),
                EdgeFactory(vertices[0], vertices[4])
            });

            GraphModels.Add(new GraphViewModel("Fa", graph));

            #endregion

            #region Complete graph

            IBidirectionalGraph<PocVertex, PocEdge> completeGraph = GraphFactory.CreateCompleteGraph(
                7,
                VertexFactory,
                EdgeFactory);

            GraphModels.Add(new GraphViewModel("Complete", ConvertToPocGraph(completeGraph)));

            #endregion

            #region Isolated vertices graph

            IBidirectionalGraph<PocVertex, PocEdge> isolatedVerticesGraph = GraphFactory.CreateIsolatedVerticesGraph<PocVertex, PocEdge>(
                25,
                VertexFactory);

            GraphModels.Add(new GraphViewModel("Isolated vertices", ConvertToPocGraph(isolatedVerticesGraph)));

            #endregion

            #region General graph

            IBidirectionalGraph<PocVertex, PocEdge> generalGraph = GraphFactory.CreateGeneralGraph(
                30,
                25,
                10,
                true,
                VertexFactory,
                EdgeFactory,
                new Random(123456));

            GraphModels.Add(new GraphViewModel("General graph", ConvertToPocGraph(generalGraph)));

            #endregion

            #region DAG graph

            IBidirectionalGraph<PocVertex, PocEdge> dagGraph = GraphFactory.CreateDAG(
                30,
                25,
                5,
                10,
                true,
                VertexFactory,
                EdgeFactory,
                new Random(123456));

            GraphModels.Add(new GraphViewModel("DAG graph", ConvertToPocGraph(dagGraph)));

            #endregion

            #region Tree graph

            IBidirectionalGraph<PocVertex, PocEdge> treeGraph = GraphFactory.CreateTree(
                25,
                3,
                VertexFactory,
                EdgeFactory,
                new Random(123456));

            GraphModels.Add(new GraphViewModel("Tree graph", ConvertToPocGraph(treeGraph)));

            #endregion

            #region Local functions

            static PocVertex VertexFactory(int vertex)
            {
                return new PocVertex(vertex.ToString());
            }

            static PocEdge EdgeFactory(PocVertex source, PocVertex target)
            {
                return new PocEdge($"{source.ID}to{target.ID}", source, target);
            }

            static PocGraph ConvertToPocGraph(IEdgeListGraph<PocVertex, PocEdge> g)
            {
                var pocGraph = new PocGraph();
                pocGraph.AddVertexRange(g.Vertices);
                pocGraph.AddEdgeRange(g.Edges);

                return pocGraph;
            }

            #endregion
        }
    }
}