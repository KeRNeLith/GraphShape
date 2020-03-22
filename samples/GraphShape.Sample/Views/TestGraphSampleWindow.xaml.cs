using System;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Sample
{
    /// <summary>
    /// Interaction logic for TestGraphSampleWindow.xaml
    /// </summary>
    internal partial class TestGraphSampleWindow
    {
        [CanBeNull]
        private BidirectionalGraph<string, IEdge<string>> _graph;

        public TestGraphSampleWindow()
        {
            InitializeComponent();
        }

        private void OnGenerateGraphClick(object sender, RoutedEventArgs args)
        {
            _graph = new BidirectionalGraph<string, IEdge<string>>();

            var random = new Random(DateTime.Now.Millisecond);
            int rnd = random.Next(13) + 2;
            for (int i = 0; i < rnd; ++i)
            {
                _graph.AddVertex(i.ToString());
            }

            rnd = random.Next(_graph.VertexCount * 2) + 2;
            for (int i = 0; i < rnd; ++i)
            {
                int index1 = random.Next(_graph.VertexCount);
                int index2 = random.Next(_graph.VertexCount);

                string vertex1 = _graph.Vertices.ElementAt(index1);
                string vertex2 = _graph.Vertices.ElementAt(index2);

                _graph.AddEdge(new Edge<string>(vertex1, vertex2));
            }

            DataContext = _graph;
        }

        private void OnAddVertexClick(object sender, RoutedEventArgs args)
        {
            if (_graph is null || _graph.VertexCount >= 100)
                return;

            var random = new Random(DateTime.Now.Millisecond);
            int verticesToAdd = Math.Max(_graph.VertexCount / 4, 1);
            var parents = new string[verticesToAdd];
            for (int j = 0; j < verticesToAdd; ++j)
            {
                int index = random.Next(_graph.VertexCount);
                parents[j] = _graph.Vertices.ElementAt(index);
            }

            for (int i = 0; i < verticesToAdd; ++i)
            {
                string newVertex;
                do
                {
                    newVertex = $"{random.Next(0, _graph.VertexCount + 20)}_new";
                } while (_graph.ContainsVertex(newVertex));
                _graph.AddVertex(newVertex);

                if (_graph.VertexCount < 2)
                    return;

                _graph.AddEdge(new Edge<string>(parents[i], newVertex));
            }
        }

        private void OnRemoveVertexClick(object sender, RoutedEventArgs args)
        {
            if (_graph is null || _graph.VertexCount < 1)
                return;

            var random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 10 && _graph.VertexCount > 1; ++i)
            {
                int index = random.Next(_graph.VertexCount);
                _graph.RemoveVertex(_graph.Vertices.ElementAt(index));
            }
        }

        private void OnAddEdgeClick(object sender, RoutedEventArgs args)
        {
            if (_graph is null || _graph.VertexCount < 2)
                return;

            var random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 10; ++i)
            {
                int index1 = random.Next(_graph.VertexCount);
                int index2 = random.Next(_graph.VertexCount);

                string vertex1 = _graph.Vertices.ElementAt(index1);
                string vertex2 = _graph.Vertices.ElementAt(index2);

                _graph.AddEdge(new Edge<string>(vertex1, vertex2));
            }
        }

        private void OnRemoveEdgeClick(object sender, RoutedEventArgs args)
        {
            if (_graph is null || _graph.EdgeCount < 1)
                return;

            var random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 10 && _graph.EdgeCount > 0; ++i)
            {
                int index = random.Next(_graph.EdgeCount);
                _graph.RemoveEdge(_graph.Edges.ElementAt(index));
            }
        }

        private void OnRelayoutClick(object sender, RoutedEventArgs args)
        {
            Layout.Relayout();
        }
    }
}