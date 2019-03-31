using System.Windows;
using GraphSharp.Controls;
using System.Linq;
using System.ComponentModel;
using System.Windows.Controls;

namespace GraphSharp.Sample
{
	public class PocContextualGraphLayout : ContextualGraphLayout<PocVertex, PocEdge, PocGraph> { }

	/// <summary>
	/// Interaction logic for TestContextualLayout.xaml
	/// </summary>
	public partial class TestContextualLayout : INotifyPropertyChanged
	{
		public PocGraph Graph
		{
			get; private set;
		}

		private PocVertex selectedVertex;
		public PocVertex SelectedVertex
		{
			get
			{
				return selectedVertex;
			}
			set
			{
				selectedVertex = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("SelectedVertex"));
			}
		}

		public TestContextualLayout(PocGraph graph)
		{
			InitializeComponent();

			Graph = graph;
			SelectedVertex = graph.Vertices.FirstOrDefault();
			DataContext = this;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private void SelectedVertexChange_Click(object sender, RoutedEventArgs e)
		{
			var btn = e.Source as Button;
			SelectedVertex = btn.Tag as PocVertex;
		}
	}
}