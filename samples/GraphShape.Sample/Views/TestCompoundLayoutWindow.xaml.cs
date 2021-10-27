using System.Windows;
using GraphShape.Controls;
using QuikGraph;

namespace GraphShape.Sample
{
    /// <summary>
    /// Interaction logic for TestCompoundLayoutWindow.xaml
    /// </summary>
    internal partial class TestCompoundLayoutWindow
    {
        public TestCompoundLayoutWindow()
        {
            InitializeComponent();

            var graph = new CompoundGraph<object, IEdge<object>>();

            var vertices = new string[30];
            for (int i = 0; i < 30; ++i)
            {
                vertices[i] = i.ToString();
                graph.AddVertex(vertices[i]);
            }

            for (int i = 6; i < 15; ++i)
            {
                graph.AddChildVertex(vertices[i % 5], vertices[i]);
            }

            graph.AddChildVertex(vertices[5], vertices[4]);
            graph.AddChildVertex(vertices[5], vertices[2]);
            graph.AddChildVertex(vertices[16], vertices[0]);
            graph.AddChildVertex(vertices[16], vertices[1]);
            graph.AddChildVertex(vertices[16], vertices[3]);
            graph.AddChildVertex(vertices[16], vertices[20]);
            graph.AddChildVertex(vertices[16], vertices[21]);
            graph.AddChildVertex(vertices[16], vertices[22]);
            graph.AddChildVertex(vertices[16], vertices[23]);
            graph.AddChildVertex(vertices[16], vertices[24]);
            graph.AddChildVertex(vertices[4], vertices[25]);
            graph.AddChildVertex(vertices[4], vertices[26]);
            graph.AddChildVertex(vertices[4], vertices[27]);

            graph.AddEdge(new Edge<object>(vertices[0], vertices[1]));
            graph.AddEdge(new Edge<object>(vertices[0], vertices[2]));
            graph.AddEdge(new Edge<object>(vertices[2], vertices[4]));
            graph.AddEdge(new Edge<object>(vertices[0], vertices[7]));
            graph.AddEdge(new Edge<object>(vertices[8], vertices[7]));
            graph.AddEdge(new Edge<object>(vertices[3], vertices[20]));
            graph.AddEdge(new Edge<object>(vertices[20], vertices[21]));
            graph.AddEdge(new Edge<object>(vertices[20], vertices[22]));
            graph.AddEdge(new Edge<object>(vertices[22], vertices[23]));
            graph.AddEdge(new Edge<object>(vertices[23], vertices[24]));
            graph.AddEdge(new Edge<object>(vertices[0], vertices[28]));
            graph.AddEdge(new Edge<object>(vertices[0], vertices[29]));
            graph.AddEdge(new Edge<object>(vertices[25], vertices[27]));
            graph.AddEdge(new Edge<object>(vertices[26], vertices[25]));
            graph.AddEdge(new Edge<object>(vertices[14], vertices[27]));
            graph.AddEdge(new Edge<object>(vertices[14], vertices[26]));
            graph.AddEdge(new Edge<object>(vertices[14], vertices[25]));
            graph.AddEdge(new Edge<object>(vertices[26], vertices[27]));


            Layout.LayoutMode = LayoutMode.Automatic;
            Layout.LayoutAlgorithmType = "CompoundFDP";
            Layout.OverlapRemovalConstraint = AlgorithmConstraints.Automatic;
            Layout.OverlapRemovalAlgorithmType = "FSA";
            Layout.HighlightAlgorithmType = "Simple";
            Layout.Graph = graph;
        }

        private void OnRelayoutClick(object sender, RoutedEventArgs args)
        {
            Layout.Relayout();
        }
    }
}