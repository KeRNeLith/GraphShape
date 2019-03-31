using GraphSharp.Sample.Model;
using GraphSharp.Sample.Properties;
using WPFExtensions.ViewModel.Commanding;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace GraphSharp.Sample.ViewModel
{
    public partial class LayoutAnalyzerViewModel : CommandSink, INotifyPropertyChanged
    {
        #region Commands
        public static readonly RoutedCommand AddLayoutCommand = new RoutedCommand("AddLayout", typeof(LayoutAnalyzerViewModel));
        public static readonly RoutedCommand RemoveLayoutCommand = new RoutedCommand("RemoveLayout", typeof(LayoutAnalyzerViewModel));
        public static readonly RoutedCommand RelayoutCommand = new RoutedCommand("Relayout", typeof(LayoutAnalyzerViewModel));
        public static readonly RoutedCommand ContinueLayoutCommand = new RoutedCommand("ContinueLayout", typeof(LayoutAnalyzerViewModel));
        public static readonly RoutedCommand OpenGraphsCommand = new RoutedCommand("OpenGraphs", typeof(LayoutAnalyzerViewModel));
        public static readonly RoutedCommand SaveGraphsCommand = new RoutedCommand("SaveGraphs", typeof(LayoutAnalyzerViewModel));
        #endregion

        private GraphModel selectedGraphModel;

        public ObservableCollection<GraphModel> GraphModels { get; private set; }
        public GraphModel SelectedGraphModel
        {
            get { return selectedGraphModel; }
            set
            {
                if (selectedGraphModel != value)
                {
                    selectedGraphModel = value;
                    SelectedGraphChanged();
                    NotifyChanged("SelectedGraphModel");
                }
            }
        }

        private void SelectedGraphChanged()
        {
            if (AnalyzedLayouts != null)
            {
                AnalyzedLayouts.Graph = selectedGraphModel.Graph;
            }
        }

        public GraphLayoutViewModel AnalyzedLayouts { get; private set; }

        public PocVertex SampleVertex = new PocVertex("173");

        public LayoutAnalyzerViewModel()
        {
            AnalyzedLayouts = new GraphLayoutViewModel
            {
                LayoutAlgorithmType = "FR"
            };
            GraphModels = new ObservableCollection<GraphModel>();

            RegisterCommand(ContinueLayoutCommand,
                             param => AnalyzedLayouts != null,
                             param => ContinueLayout());

            RegisterCommand(RelayoutCommand,
                             param => AnalyzedLayouts != null,
                             param => Relayout());

            RegisterCommand(OpenGraphsCommand,
                             param => true,
                             param => OpenGraphs());

            RegisterCommand(SaveGraphsCommand,
                             param => GraphModels.Count > 0,
                             param => SaveGraphs());

            CreateSampleGraphs();
        }

        partial void CreateSampleGraphs();

        public void ContinueLayout()
        {
            LayoutManager.Instance.ContinueLayout();
        }

        public void Relayout()
        {
            LayoutManager.Instance.Relayout();
        }

        public void OpenGraphs()
        {
            var ofd = new OpenFileDialog
                        {
                            CheckPathExists = true
                        };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //open the file and load the graphs
                var graph = PocSerializeHelper.LoadGraph(ofd.FileName);

                GraphModels.Add(new GraphModel(Path.GetFileNameWithoutExtension(ofd.FileName), graph));
            }
        }

        public void SaveGraphs()
        {
            var fd = new FolderBrowserDialog
                        {
                            ShowNewFolderButton = true
                        };
            if (fd.ShowDialog() == DialogResult.OK)
            {
                foreach (var model in GraphModels)
                {
                    PocSerializeHelper.SaveGraph(model.Graph, Path.Combine(fd.SelectedPath, string.Format("{0}.{1}", model.Name, Settings.Default.GraphMLExtension)));
                }
            }
        }

        private void NotifyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}