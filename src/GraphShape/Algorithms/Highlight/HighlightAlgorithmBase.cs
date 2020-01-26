using System;
using System.ComponentModel;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Highlight
{
    /// <summary>
    /// Base class for all highlight algorithms.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    /// <typeparam name="TParameters">Algorithm parameters type</typeparam>
    public abstract class HighlightAlgorithmBase<TVertex, TEdge, TGraph, TParameters> : IHighlightAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
        where TParameters : class, IHighlightParameters
    {
        /// <summary>
        /// Highlight controller.
        /// </summary>
        [NotNull]
        public IHighlightController<TVertex, TEdge, TGraph> Controller { get; }

        /// <inheritdoc />
        IHighlightParameters IHighlightAlgorithm<TVertex, TEdge>.Parameters => Parameters;

        /// <inheritdoc cref="IHighlightAlgorithm{TVertex,TEdge}.Parameters"/>
        public TParameters Parameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightAlgorithmBase{TVertex,TEdge,TGraph,TParameters}"/> class.
        /// </summary>
        /// <param name="controller">Highlight controller.</param>
        /// <param name="parameters">Highlight algorithm parameters.</param>
        protected HighlightAlgorithmBase(
            [NotNull] IHighlightController<TVertex, TEdge, TGraph> controller,
            [CanBeNull] IHighlightParameters parameters)
        {
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));
            TrySetParameters(parameters);
        }

        /// <inheritdoc />
        public abstract void ResetHighlight();

        /// <inheritdoc />
        public abstract bool OnVertexHighlighting(TVertex vertex);

        /// <inheritdoc />
        public abstract bool OnVertexHighlightRemoving(TVertex vertex);

        /// <inheritdoc />
        public abstract bool OnEdgeHighlighting(TEdge edge);

        /// <inheritdoc />
        public abstract bool OnEdgeHighlightRemoving(TEdge edge);

        /// <inheritdoc />
        public bool IsParametersSettable(IHighlightParameters parameters)
        {
            return parameters is TParameters;
        }

        /// <inheritdoc />
        public bool TrySetParameters(IHighlightParameters parameters)
        {
            if (IsParametersSettable(parameters))
            {
                if (Parameters != null)
                    Parameters.PropertyChanged -= OnParameterPropertyChanged;
                Parameters = (TParameters)parameters;
                if (Parameters != null)
                    Parameters.PropertyChanged += OnParameterPropertyChanged;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called each time a parameter property changed.
        /// Resets highlight.
        /// </summary>
        protected virtual void OnParametersChanged()
        {
            ResetHighlight();
        }

        private void OnParameterPropertyChanged([NotNull] object sender, [NotNull] PropertyChangedEventArgs args)
        {
            OnParametersChanged();
        }
    }
}