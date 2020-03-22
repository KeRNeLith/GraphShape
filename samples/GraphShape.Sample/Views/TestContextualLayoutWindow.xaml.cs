using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace GraphShape.Sample
{
    /// <summary>
	/// Interaction logic for TestContextualLayoutWindow.xaml
	/// </summary>
    internal partial class TestContextualLayoutWindow : INotifyPropertyChanged
    {
        [NotNull]
        public PocGraph Graph { get; }

        [CanBeNull]
        private PocVertex _selectedVertex;

        [CanBeNull]
        public PocVertex SelectedVertex
        {
            get => _selectedVertex;
            set
            {
                if (_selectedVertex == value)
                    return;

                _selectedVertex = value;
                OnPropertyChanged(nameof(SelectedVertex));
            }
        }

        public TestContextualLayoutWindow([NotNull] PocGraph graph)
        {
            InitializeComponent();

            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
            SelectedVertex = graph.Vertices.FirstOrDefault();
            DataContext = this;
        }

        private void OnSelectedVertexChangeClick(object sender, RoutedEventArgs args)
        {
            if (args.Source is Button button)
                SelectedVertex = button.Tag as PocVertex;
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