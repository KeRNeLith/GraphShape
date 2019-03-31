using GraphSharp.Controls;
using System.ComponentModel;

namespace GraphSharp.Sample.ViewModel
{
	public class PocGraphLayout : GraphLayout<PocVertex, PocEdge, PocGraph> { }

	public class GraphLayoutViewModel : INotifyPropertyChanged
	{
		private string layoutAlgorithmType;
		private PocGraph graph;

		public string LayoutAlgorithmType
		{
			get { return layoutAlgorithmType; }
			set
			{
				if (value != layoutAlgorithmType)
				{
					layoutAlgorithmType = value;
					NotifyChanged("LayoutAlgorithmType");
				}
			}
		}

		public PocGraph Graph
		{
			get { return graph; }
			set
			{
				if (value != graph)
				{
					graph = value;
					NotifyChanged("Graph");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void NotifyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}