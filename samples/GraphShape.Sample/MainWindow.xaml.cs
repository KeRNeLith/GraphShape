using System.Windows;
using System.Windows.Input;
using GraphShape.Sample.ViewModels;
using JetBrains.Annotations;

namespace GraphShape.Sample
{
    /// <summary>
    /// Main window of the Proof of Concept application for the <see cref="GraphShape.Controls.GraphLayout"/> control.
    /// </summary>
    internal partial class MainWindow
    {
        [NotNull]
        private readonly LayoutAnalyzerViewModel _analyzerViewModel = new LayoutAnalyzerViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _analyzerViewModel;
        }

        private void OnTestGraphSampleExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var window = new TestGraphSampleWindow();
            window.Show();
        }

        private void OnTestContextualLayoutExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            if (_analyzerViewModel.SelectedGraphModel is null)
                return;

            var window = new TestContextualLayoutWindow(_analyzerViewModel.SelectedGraphModel.Graph);
            window.Show();
        }

        private void OnTestCompoundLayoutExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var window = new TestCompoundLayoutWindow();
            window.Show();
        }

        private void OnTestPlainCompoundLayoutExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var window = new TestPlainCompoundLayoutWindow();
            window.Show();
        }

        private void OnExitExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            Application.Current.Shutdown();
        }
    }
}