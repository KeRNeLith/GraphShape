﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphShape.Algorithms.Layout.Compound;
using GraphShape.Algorithms.Layout.Compound.FDP;
using GraphShape.Algorithms.Layout.Simple.Circular;
using GraphShape.Algorithms.Layout.Simple.FDP;
using GraphShape.Algorithms.Layout.Simple.Hierarchical;
using GraphShape.Algorithms.Layout.Simple.Tree;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Simple implementation of a layout algorithm factory.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class StandardLayoutAlgorithmFactory<TVertex, TEdge, TGraph> : ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        // ReSharper disable InconsistentNaming
        [NotNull]
        private const string CircularAlgorithm = "Circular";
        [NotNull]
        private const string TreeAlgorithm = "Tree";
        [NotNull]
        private const string FRAlgorithm = "FR";
        [NotNull]
        private const string BoundedFRAlgorithm = "BoundedFR";
        [NotNull]
        private const string KKAlgorithm = "KK";
        [NotNull]
        private const string ISOMAlgorithm = "ISOM";
        [NotNull]
        private const string LinLogAlgorithm = "LinLog";
        //[NotNull]
        //private const string SugiyamaAlgorithm = "Sugiyama";
        [NotNull]
        private const string EfficientSugiyamaAlgorithm = "EfficientSugiyama";
        [NotNull]
        private const string CompoundFDPAlgorithm = "CompoundFDP";
        // ReSharper restore InconsistentNaming

        /// <inheritdoc />
        public IEnumerable<string> AlgorithmTypes { get; } = new[]
        {
            CircularAlgorithm, TreeAlgorithm, FRAlgorithm, BoundedFRAlgorithm,
            KKAlgorithm, ISOMAlgorithm, LinLogAlgorithm, /*SugiyamaAlgorithm,*/
            EfficientSugiyamaAlgorithm, CompoundFDPAlgorithm
        };

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
            if (context.Graph is null)
                return null;

            if (context.Mode == LayoutMode.Simple)
            {
                if (string.Equals(algorithmType, CircularAlgorithm))
                {
                    return new CircularLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        context.Graph,
                        context.Positions,
                        context.Sizes,
                        parameters as CircularLayoutParameters);
                }
                if (string.Equals(algorithmType, TreeAlgorithm))
                {
                    return new SimpleTreeLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        context.Graph,
                        context.Positions,
                        context.Sizes,
                        parameters as SimpleTreeLayoutParameters);
                }
                if (string.Equals(algorithmType, FRAlgorithm))
                {
                    return new FRLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        context.Graph,
                        context.Positions,
                        parameters as FRLayoutParametersBase);
                }
                if (string.Equals(algorithmType, BoundedFRAlgorithm))
                {
                    return new FRLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        context.Graph,
                        context.Positions,
                        parameters as BoundedFRLayoutParameters);
                }
                if (string.Equals(algorithmType, KKAlgorithm))
                {
                    return new KKLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        context.Graph,
                        context.Positions,
                        parameters as KKLayoutParameters);
                }
                if (string.Equals(algorithmType, ISOMAlgorithm))
                {
                    return new ISOMLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        context.Graph,
                        context.Positions,
                        parameters as ISOMLayoutParameters);
                }
                if (string.Equals(algorithmType, LinLogAlgorithm))
                {
                    return new LinLogLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        context.Graph,
                        context.Positions,
                        parameters as LinLogLayoutParameters);
                }
                //if (string.Equals(algorithmType, SugiyamaAlgorithm))
                //{
                //    return new SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>(
                //        context.Graph,
                //        context.Sizes,
                //        context.Positions,
                //        parameters as SugiyamaLayoutParameters,
                //        edge =>
                //        {
                //            var typedEdge = edge as TypedEdge<TVertex>;
                //            return typedEdge?.Type ?? EdgeTypes.Hierarchical;
                //        });
                //}
                if (string.Equals(algorithmType, EfficientSugiyamaAlgorithm))
                {
                    return new EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        context.Graph,
                        context.Positions,
                        context.Sizes,
                        parameters as EfficientSugiyamaLayoutParameters);
                }
                if (string.Equals(algorithmType, CompoundFDPAlgorithm))
                {
                    return new CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        context.Graph,
                        context.Sizes,
                        new Dictionary<TVertex, Thickness>(),
                        new Dictionary<TVertex, CompoundVertexInnerLayoutType>(),
                        context.Positions,
                        parameters as CompoundFDPLayoutParameters);
                }

                return null;
            }

            if (context.Mode == LayoutMode.Compound
                && context is ICompoundLayoutContext<TVertex, TEdge, TGraph> compoundContext)
            {
                if (string.Equals(algorithmType, CompoundFDPAlgorithm))
                {
                    return new CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>(
                        compoundContext.Graph,
                        compoundContext.Sizes,
                        compoundContext.VertexBorders,
                        compoundContext.LayoutTypes,
                        compoundContext.Positions,
                        parameters as CompoundFDPLayoutParameters);
                }
            }

            return null;
        }

        /// <inheritdoc />
        public ILayoutParameters CreateParameters(string algorithmType, ILayoutParameters oldParameters)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));

            if (string.Equals(algorithmType, CircularAlgorithm))
                return oldParameters.CreateNewParameters<CircularLayoutParameters>();
            if (string.Equals(algorithmType, TreeAlgorithm))
                return oldParameters.CreateNewParameters<SimpleTreeLayoutParameters>();
            if (string.Equals(algorithmType, FRAlgorithm))
                return oldParameters.CreateNewParameters<FreeFRLayoutParameters>();
            if (string.Equals(algorithmType, BoundedFRAlgorithm))
                return oldParameters.CreateNewParameters<BoundedFRLayoutParameters>();
            if (string.Equals(algorithmType, KKAlgorithm))
                return oldParameters.CreateNewParameters<KKLayoutParameters>();
            if (string.Equals(algorithmType, ISOMAlgorithm))
                return oldParameters.CreateNewParameters<ISOMLayoutParameters>();
            if (string.Equals(algorithmType, LinLogAlgorithm))
                return oldParameters.CreateNewParameters<LinLogLayoutParameters>();
            //if (string.Equals(algorithmType, SugiyamaAlgorithm))
            //    return oldParameters.CreateNewParameters<SugiyamaLayoutParameters>();
            if (string.Equals(algorithmType, EfficientSugiyamaAlgorithm))
                return oldParameters.CreateNewParameters<EfficientSugiyamaLayoutParameters>();
            if (string.Equals(algorithmType, CompoundFDPAlgorithm))
                return oldParameters.CreateNewParameters<CompoundFDPLayoutParameters>();
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

            if (algorithm is CircularLayoutAlgorithm<TVertex, TEdge, TGraph>)
                return CircularAlgorithm;
            if (algorithm is SimpleTreeLayoutAlgorithm<TVertex, TEdge, TGraph>)
                return TreeAlgorithm;
            if (algorithm is FRLayoutAlgorithm<TVertex, TEdge, TGraph> frAlgorithm)
            {
                if (frAlgorithm.Parameters is BoundedFRLayoutParameters)
                    return BoundedFRAlgorithm;
                return FRAlgorithm;
            }
            if (algorithm is KKLayoutAlgorithm<TVertex, TEdge, TGraph>)
                return KKAlgorithm;
            if (algorithm is ISOMLayoutAlgorithm<TVertex, TEdge, TGraph>)
                return ISOMAlgorithm;
            if (algorithm is LinLogLayoutAlgorithm<TVertex, TEdge, TGraph>)
                return LinLogAlgorithm;
            //if (algorithm is SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>)
            //    return SugiyamaAlgorithm;
            if (algorithm is EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>)
                return EfficientSugiyamaAlgorithm;
            if (algorithm is CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>)
                return CompoundFDPAlgorithm;
            return string.Empty;
        }

        /// <inheritdoc />
        public bool NeedEdgeRouting(string algorithmType)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));
            return IsValidAlgorithm(algorithmType)
                   //&& algorithmType != SugiyamaAlgorithm
                   && algorithmType != EfficientSugiyamaAlgorithm;
        }

        /// <inheritdoc />
        public bool NeedOverlapRemoval(string algorithmType)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));
            return IsValidAlgorithm(algorithmType)
                   && algorithmType != CircularAlgorithm
                   && algorithmType != TreeAlgorithm
                   //&& algorithmType != SugiyamaAlgorithm
                   && algorithmType != EfficientSugiyamaAlgorithm
                   && algorithmType != CompoundFDPAlgorithm;
        }
    }
}

