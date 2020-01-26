using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Highlight
{
    /// <summary>
    /// Simple implementation of an highlight algorithm factory.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class StandardHighlightAlgorithmFactory<TVertex, TEdge, TGraph> : IHighlightAlgorithmFactory<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        [NotNull]
        private const string SimpleMode = "Simple";

        /// <inheritdoc />
        public IEnumerable<string> HighlightModes { get; } = new[] { SimpleMode };

        /// <inheritdoc />
        public IHighlightAlgorithm<TVertex, TEdge> CreateAlgorithm(
            string highlightMode,
            IHighlightContext<TVertex, TEdge, TGraph> context,
            IHighlightController<TVertex, TEdge, TGraph> controller,
            IHighlightParameters parameters)
        {
            if (highlightMode is null)
                throw new ArgumentNullException(nameof(highlightMode));
            if (context is null)
                throw new ArgumentNullException(nameof(context));
            if (controller is null)
                throw new ArgumentNullException(nameof(controller));

            switch (highlightMode)
            {
                case SimpleMode:
                    return new SimpleHighlightAlgorithm<TVertex, TEdge, TGraph>(controller, parameters);
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public IHighlightParameters CreateParameters(string highlightMode, IHighlightParameters oldParameters)
        {
            if (highlightMode is null)
                throw new ArgumentNullException(nameof(highlightMode));

            switch (highlightMode)
            {
                case SimpleMode:
                    return new HighlightParameters();
                default:
                    return new HighlightParameters();
            }
        }

        /// <inheritdoc />
        public bool IsValidMode(string mode)
        {
            return string.IsNullOrEmpty(mode) || HighlightModes.Contains(mode);
        }

        /// <inheritdoc />
        public string GetHighlightMode(IHighlightAlgorithm<TVertex, TEdge> algorithm)
        {
            if (algorithm is null)
                throw new ArgumentNullException(nameof(algorithm));

            if (algorithm is SimpleHighlightAlgorithm<TVertex, TEdge, TGraph>)
                return SimpleMode;
            return null;
        }
    }
}