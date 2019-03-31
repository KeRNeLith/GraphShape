using System;
using System.Linq;
using System.Windows;
using GraphSharp.Controls;
using QuickGraph;

namespace GraphSharp.Sample
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow
    {
        private BidirectionalGraph<string, IEdge<string>> graph;

        public TestWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            graph = new BidirectionalGraph<string, IEdge<string>>();

            var random = new Random(DateTime.Now.Millisecond);
            int rnd = random.Next(13) + 2;
            for (int i = 0; i < rnd; i++)
            {
                graph.AddVertex(i.ToString());
            }

            rnd = random.Next(graph.VertexCount * 2) + 2;
            for (int i = 0; i < rnd; i++)
            {
                int v1 = random.Next(graph.VertexCount);
                int v2 = random.Next(graph.VertexCount);

                string vo1 = graph.Vertices.ElementAt(v1);
                string vo2 = graph.Vertices.ElementAt(v2);

                graph.AddEdge(new Edge<string>(vo1, vo2));
            }

            DataContext = graph;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            layout.Relayout();
        }

        private void AddVertex_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null || graph.VertexCount >= 100)
                return;

            var rnd = new Random(DateTime.Now.Millisecond);
            int verticesToAdd = Math.Max(graph.VertexCount / 4, 1);
            var parents = new string[verticesToAdd];
            for (int j = 0; j < verticesToAdd; j++)
            {
                parents[j] = graph.Vertices.ElementAt(rnd.Next(graph.VertexCount));
            }
            for (int i = 0; i < verticesToAdd; i++)
            {
                string newVertex = string.Empty;
                do
                {
                    newVertex = rnd.Next(0, graph.VertexCount + 20) + "_new";
                } while (graph.ContainsVertex(newVertex));
                graph.AddVertex(newVertex);

                if (graph.VertexCount < 2)
                    return;

                //string vo1 = graph.Vertices.ElementAt(rnd.Next(Math.Max(9 * graph.VertexCount / 10, graph.VertexCount - 1)));
                graph.AddEdge(new Edge<string>(parents[i], newVertex));
            }
        }

        private void RemoveVertex_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null)
                return;

            if (graph.VertexCount < 1)
                return;
            var rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 10 && graph.VertexCount > 1; i++)
                graph.RemoveVertex(graph.Vertices.ElementAt(rnd.Next(graph.VertexCount)));
        }

        private void RemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null)
                return;

            if (graph.EdgeCount < 1)
                return;
            var rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 10 && graph.EdgeCount > 0; i++)
                graph.RemoveEdge(graph.Edges.ElementAt(rnd.Next(graph.EdgeCount)));
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null)
                return;

            if (graph.VertexCount < 2)
                return;

            var rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 10; i++)
            {
                int v1 = rnd.Next(graph.VertexCount);
                int v2 = rnd.Next(graph.VertexCount);

                string vo1 = graph.Vertices.ElementAt(v1);
                string vo2 = graph.Vertices.ElementAt(v2);

                graph.AddEdge(new Edge<string>(vo1, vo2));
            }
        }
    }

    public class MyGraphLayout : GraphLayout<string, IEdge<string>, IBidirectionalGraph<string, IEdge<string>>> { }
}