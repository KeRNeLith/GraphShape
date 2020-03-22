using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using GraphShape.Sample.Properties;
using GraphShape.Sample.Utils;
using JetBrains.Annotations;
using WPFExtensions.ViewModel.Commanding;

namespace GraphShape.Sample.ViewModels
{
    internal partial class LayoutAnalyzerViewModel : CommandSink, INotifyPropertyChanged
    {
        #region Commands

        [NotNull]
        public static readonly RoutedCommand AddLayoutCommand = new RoutedCommand("AddLayout", typeof(LayoutAnalyzerViewModel));

        [NotNull]
        public static readonly RoutedCommand RemoveLayoutCommand = new RoutedCommand("RemoveLayout", typeof(LayoutAnalyzerViewModel));

        [NotNull]
        public static readonly RoutedCommand RelayoutCommand = new RoutedCommand("Relayout", typeof(LayoutAnalyzerViewModel));

        [NotNull]
        public static readonly RoutedCommand ContinueLayoutCommand = new RoutedCommand("ContinueLayout", typeof(LayoutAnalyzerViewModel));

        [NotNull]
        public static readonly RoutedCommand OpenGraphCommand = new RoutedCommand("OpenGraph", typeof(LayoutAnalyzerViewModel));

        [NotNull]
        public static readonly RoutedCommand SaveGraphsCommand = new RoutedCommand("SaveGraphs", typeof(LayoutAnalyzerViewModel));

        #endregion

        public LayoutAnalyzerViewModel()
        {
            AnalyzedLayout = new GraphLayoutViewModel
            {
                LayoutAlgorithmType = "FR"
            };

            RegisterCommand(
                ContinueLayoutCommand,
                _ => true,
                _ => ContinueLayout());

            RegisterCommand(
                RelayoutCommand,
                _ => true,
                _ => Relayout());

            RegisterCommand(
                OpenGraphCommand,
                _ => true,
                _ => OpenGraphs());

            RegisterCommand(
                SaveGraphsCommand,
                _ => GraphModels.Count > 0,
                _ => SaveGraphs());

            CreateSampleGraph();
        }

        [NotNull, ItemNotNull]
        public ObservableCollection<GraphViewModel> GraphModels { get; } = new ObservableCollection<GraphViewModel>();

        [CanBeNull]
        private GraphViewModel _selectedGraphModel;

        [CanBeNull]
        public GraphViewModel SelectedGraphModel
        {
            get => _selectedGraphModel;
            set
            {
                if (_selectedGraphModel == value)
                    return;

                _selectedGraphModel = value;
                SelectedGraphChanged();
                OnPropertyChanged(nameof(SelectedGraphModel));
            }
        }

        private void SelectedGraphChanged()
        {
            AnalyzedLayout.Graph = SelectedGraphModel?.Graph;
        }

        [NotNull]
        public GraphLayoutViewModel AnalyzedLayout { get; }

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
            var dialog = new OpenFileDialog
            {
                CheckPathExists = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Open the file and load the graphs
                PocGraph graph = PocSerializeHelper.LoadGraph(dialog.FileName);

                GraphModels.Add(
                    new GraphViewModel(
                        Path.GetFileNameWithoutExtension(dialog.FileName),
                        graph));
            }
        }

        public void SaveGraphs()
        {
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (GraphViewModel model in GraphModels)
                {
                    PocSerializeHelper.SaveGraph(
                        model.Graph,
                        Path.Combine(dialog.SelectedPath, $"{model.Name}.{Settings.Default.GraphMLExtension}"));
                }
            }
        }

        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises a <see cref="PropertyChanged"/> event for the given <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            Debug.Assert(propertyName != null);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}