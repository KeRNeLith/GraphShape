using System;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.Layout.Simple.Tree;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Contextual
{
    /// <summary>
    /// Implementation of a contextual layout algorithm factory.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class ContextualLayoutAlgorithmFactory<TVertex, TEdge, TGraph> : IContextualLayoutAlgorithmFactory<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        [NotNull]
        private const string DoubleTreeAlgorithm = "DoubleTree";
        [NotNull]
        private const string BalloonTreeAlgorithm = "BalloonTree";

        /// <inheritdoc />
        public IEnumerable<string> AlgorithmTypes { get; } = new[] { DoubleTreeAlgorithm, BalloonTreeAlgorithm };

        /// <inheritdoc />
        public ILayoutAlgorithm<TVertex, TEdge, TGraph> CreateAlgorithm(
            string algorithmType,
            ILayoutContext<TVertex, TEdge, TGraph> context,
            ILayoutParameters parameters)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var layoutContext = context as ContextualLayoutContext<TVertex, TEdge, TGraph>;
            if (layoutContext is null)
            {
                throw new ArgumentException(
                    $"Layout context must be a not null {nameof(ContextualLayoutContext<TVertex, TEdge, TGraph>)}.",
                    nameof(context));
            }

            switch (algorithmType)
            {
                case DoubleTreeAlgorithm:
                    return new DoubleTreeLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        layoutContext.Graph,
                        layoutContext.Positions,
                        layoutContext.Sizes,
                        layoutContext.SelectedVertex,
                        parameters as DoubleTreeLayoutParameters);
                case BalloonTreeAlgorithm:
                    return new BalloonTreeLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        layoutContext.Graph,
                        layoutContext.Positions,
                        layoutContext.SelectedVertex,
                        parameters as BalloonTreeLayoutParameters);
            }

            return null;
        }

        /// <inheritdoc />
        public ILayoutParameters CreateParameters(string algorithmType, ILayoutParameters parameters)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));

            switch (algorithmType)
            {
                case DoubleTreeAlgorithm:
                    return parameters.CreateNewParameters<DoubleTreeLayoutParameters>();
                case BalloonTreeAlgorithm:
                    return parameters.CreateNewParameters<BalloonTreeLayoutParameters>();
            }

            return null;
        }

        /// <inheritdoc />
        public bool IsValidAlgorithm(string algorithmType)
        {
            return AlgorithmTypes.Contains(algorithmType);
        }

        /// <inheritdoc />
        public string GetAlgorithmType(ILayoutAlgorithm<TVertex, TEdge, TGraph> algorithm)
        {
            if (algorithm is null)
                throw new ArgumentNullException(nameof(algorithm));

            if (algorithm is DoubleTreeLayoutAlgorithm<TVertex, TEdge, TGraph>)
                return DoubleTreeAlgorithm;
            if (algorithm is BalloonTreeLayoutAlgorithm<TVertex, TEdge, TGraph>)
                return BalloonTreeAlgorithm;
            return string.Empty;
        }

        /// <inheritdoc />
        public bool NeedEdgeRouting(string algorithmType)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));
            return IsValidAlgorithm(algorithmType);
        }

        /// <inheritdoc />
        public bool NeedOverlapRemoval(string algorithmType)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));
            return false;
        }
    }
}