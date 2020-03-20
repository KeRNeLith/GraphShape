using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.Layout.Contextual;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Controls
{
    /// <summary>
    /// Contextual Graph layout control.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class ContextualGraphLayout<TVertex, TEdge, TGraph> : GraphLayout<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        static ContextualGraphLayout()
        {
            LayoutAlgorithmFactoryProperty.OverrideMetadata(
                typeof(ContextualGraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(new ContextualLayoutAlgorithmFactory<TVertex, TEdge, TGraph>(), null, CoerceLayoutAlgorithmFactory));
        }

        #region SelectedVertex

        /// <summary>
        /// Gets or sets the <see cref="SelectedVertex"/> which influences the context.
        /// </summary>
        public TVertex SelectedVertex
        {
            get => (TVertex)GetValue(SelectedVertexProperty);
            set => SetValue(SelectedVertexProperty, value);
        }

        /// <summary>
        /// Selected vertex dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty SelectedVertexProperty = DependencyProperty.Register(
            nameof(SelectedVertex),
            typeof(TVertex),
            typeof(ContextualGraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(default(TVertex), OnSelectedVertexPropertyChanged));

        private static void OnSelectedVertexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = d as ContextualGraphLayout<TVertex, TEdge, TGraph>;

            // Refresh the layout on context change
            graphLayout?.Relayout();
        }

        #endregion

        /// <inheritdoc />
        protected override ILayoutContext<TVertex, TEdge, TGraph> CreateLayoutContext(
            IDictionary<TVertex, Point> positions,
            IDictionary<TVertex, Size> sizes)
        {
            return new ContextualLayoutContext<TVertex, TEdge, TGraph>(Graph, SelectedVertex, positions, sizes);
        }

        /// <inheritdoc />
        protected override bool CanLayout => SelectedVertex != null && base.CanLayout;
    }
}