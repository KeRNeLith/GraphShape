using System;
using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.EdgeRouting;
using GraphShape.Algorithms.Highlight;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.OverlapRemoval;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Controls
{
    public partial class GraphLayout<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        static GraphLayout()
        {
            // Initialize the readonly dependency properties
            StateCountProperty = StateCountPropertyKey.DependencyProperty;
            LayoutStatusPercentProperty = LayoutStatusPercentPropertyKey.DependencyProperty;
            LayoutAlgorithmProperty = LayoutAlgorithmPropertyKey.DependencyProperty;
            OverlapRemovalAlgorithmProperty = OverlapRemovalAlgorithmPropertyKey.DependencyProperty;
            EdgeRoutingAlgorithmProperty = EdgeRoutingAlgorithmPropertyKey.DependencyProperty;
            LayoutStateProperty = LayoutStatusPercentPropertyKey.DependencyProperty;
            HighlightAlgorithmProperty = HighlightAlgorithmPropertyKey.DependencyProperty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphLayout"/> class.
        /// </summary>
        public GraphLayout()
        {
            AddHandler(
                GraphElementBehaviour.HighlightTriggeredEvent,
                new HighlightTriggerEventHandler(OnHighlightTriggered));
        }

        /// <inheritdoc />
        public override bool CanAnimate
        {
            get
            {
                return base.CanAnimate
                       && AnimationLength > new TimeSpan(0, 0, 0, 0, 0)
                       && Graph != null
                       && (AnimationDisablerVertexCount < 0 || Graph.VertexCount < AnimationDisablerVertexCount)
                       && (AnimationDisablerEdgeCount < 0 || Graph.EdgeCount < AnimationDisablerEdgeCount);
            }
        }

        #region Dependency Properties

        #region AnimationDisablerVertexCount

        /// <summary>
        /// If the graph has more vertex than this count, the animation will be disabled automatically.
        /// </summary>
        public int AnimationDisablerVertexCount
        {
            get => (int)GetValue(AnimationDisablerVertexCountProperty);
            set => SetValue(AnimationDisablerVertexCountProperty, value);
        }

        /// <summary>
        /// Animation disabler via vertex count dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty AnimationDisablerVertexCountProperty = DependencyProperty.Register(
            nameof(AnimationDisablerVertexCount), typeof(int), typeof(GraphLayout<TVertex, TEdge, TGraph>), new UIPropertyMetadata(200));

        #endregion

        #region AnimationDisablerEdgeCount

        /// <summary>
        /// If the graph has more edge than this count, the animations will be disabled automatically.
        /// </summary>
        public int AnimationDisablerEdgeCount
        {
            get => (int)GetValue(AnimationDisablerEdgeCountProperty);
            set => SetValue(AnimationDisablerEdgeCountProperty, value);
        }

        /// <summary>
        /// Animation disabler via edge count dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty AnimationDisablerEdgeCountProperty = DependencyProperty.Register(
            nameof(AnimationDisablerEdgeCount), typeof(int), typeof(GraphLayout<TVertex, TEdge, TGraph>), new UIPropertyMetadata(500));

        #endregion

        // Computing and status

        #region AsyncCompute

        /// <summary>
        /// If this property is true the algorithm will be executed on a separate <see cref="T:System.Threading.Thread"/>.
        /// </summary>
        public bool AsyncCompute
        {
            get => (bool)GetValue(AsyncComputeProperty);
            set => SetValue(AsyncComputeProperty, value);
        }

        /// <summary>
        /// Async computing dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty AsyncComputeProperty = DependencyProperty.Register(
            nameof(AsyncCompute), typeof(bool), typeof(GraphLayout<TVertex, TEdge, TGraph>), new UIPropertyMetadata(false));

        #endregion

        #region EdgeRoutingAlgorithmFactory

        /// <summary>
        /// Edge routing algorithm factory.
        /// </summary>
        public IEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph> EdgeRoutingAlgorithmFactory
        {
            get => (IEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph>)GetValue(EdgeRoutingAlgorithmFactoryProperty);
            set => SetValue(EdgeRoutingAlgorithmFactoryProperty, value);
        }

        /// <summary>
        /// Edge routing algorithm factory dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty EdgeRoutingAlgorithmFactoryProperty = DependencyProperty.Register(
            nameof(EdgeRoutingAlgorithmFactory),
            typeof(IEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph>),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(
                new StandardEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph>(),
                null,
                CoerceEdgeRoutingAlgorithmFactory));

        /// <summary>
        /// Coerce callback of the <see cref="EdgeRoutingAlgorithmFactoryProperty"/> dependency property.
        /// </summary>
        protected static object CoerceEdgeRoutingAlgorithmFactory(DependencyObject d, object baseValue)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;
            return baseValue ?? graphLayout.EdgeRoutingAlgorithmFactory;
        }

        #endregion

        #region EdgeRoutingAlgorithm

        /// <summary>
        /// The algorithm used for the edge routing.
        /// </summary>
        /// <remarks>
        /// It can be the same object as the <see cref="LayoutAlgorithm"/>, in this case the
        /// <see cref="EdgeRoutingAlgorithm"/> won't be run separately, the edge routing
        /// should be calculated in the <see cref="LayoutAlgorithm"/>.
        /// </remarks>
        public IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> EdgeRoutingAlgorithm
        {
            get => (IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>)GetValue(EdgeRoutingAlgorithmProperty);
            protected set => SetValue(EdgeRoutingAlgorithmPropertyKey, value);
        }

        /// <summary>
        /// Edge routing algorithm dependency property.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty EdgeRoutingAlgorithmProperty;

        /// <summary>
        /// Edge routing algorithm property key.
        /// </summary>
        [NotNull]
        protected static readonly DependencyPropertyKey EdgeRoutingAlgorithmPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(EdgeRoutingAlgorithm),
            typeof(IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(null));

        #endregion

        #region EdgeRoutingAlgorithmType

        /// <summary>
        /// Edge routing algorithm type.
        /// </summary>
        public string EdgeRoutingAlgorithmType
        {
            get => (string)GetValue(EdgeRoutingAlgorithmTypeProperty);
            set => SetValue(EdgeRoutingAlgorithmTypeProperty, value);
        }

        /// <summary>
        /// Edge routing algorithm type dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty EdgeRoutingAlgorithmTypeProperty = DependencyProperty.Register(
            nameof(EdgeRoutingAlgorithmType),
            typeof(string),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(string.Empty, OnEdgeRoutingAlgorithmTypePropertyChanged));

        private static void OnEdgeRoutingAlgorithmTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            string newAlgorithmType = args.NewValue is null ? string.Empty : args.NewValue.ToString();

            // Regenerate parameters
            graphLayout.EdgeRoutingParameters = graphLayout.EdgeRoutingAlgorithmFactory.CreateParameters(
                newAlgorithmType,
                graphLayout.EdgeRoutingParameters);

            if (graphLayout.Graph != null)
            {
                graphLayout.RecalculateEdgeRouting();
            }
        }

        #endregion

        #region EdgeRoutingConstraint

        /// <summary>
        /// Edge routing constraint.
        /// </summary>
        public AlgorithmConstraints EdgeRoutingConstraint
        {
            get => (AlgorithmConstraints)GetValue(EdgeRoutingConstraintProperty);
            set => SetValue(EdgeRoutingConstraintProperty, value);
        }

        /// <summary>
        /// Edge routing constraint dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty EdgeRoutingConstraintProperty = DependencyProperty.Register(
            nameof(EdgeRoutingConstraint),
            typeof(AlgorithmConstraints),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(AlgorithmConstraints.Automatic, OnEdgeRoutingConstraintPropertyChanged));

        private static void OnEdgeRoutingConstraintPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            // Regenerate parameters
            if (graphLayout.Graph != null)
            {
                graphLayout.RecalculateEdgeRouting();
            }
        }

        #endregion

        #region EdgeRoutingParameters

        /// <summary>
        /// Edge routing parameters.
        /// </summary>
        public IEdgeRoutingParameters EdgeRoutingParameters
        {
            get => (IEdgeRoutingParameters)GetValue(EdgeRoutingParametersProperty);
            set => SetValue(EdgeRoutingParametersProperty, value);
        }

        /// <summary>
        /// Edge routing parameters dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty EdgeRoutingParametersProperty = DependencyProperty.Register(
            nameof(EdgeRoutingParameters),
            typeof(IEdgeRoutingParameters),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(null));

        #endregion

        #region HighlightAlgorithmFactory

        /// <summary>
        /// Highlight algorithm factory.
        /// </summary>
        public IHighlightAlgorithmFactory<TVertex, TEdge, TGraph> HighlightAlgorithmFactory
        {
            get => (IHighlightAlgorithmFactory<TVertex, TEdge, TGraph>)GetValue(HighlightAlgorithmFactoryProperty);
            set => SetValue(HighlightAlgorithmFactoryProperty, value);
        }

        /// <summary>
        /// Highlight algorithm factory dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty HighlightAlgorithmFactoryProperty = DependencyProperty.Register(
            nameof(HighlightAlgorithmFactory),
            typeof(IHighlightAlgorithmFactory<TVertex, TEdge, TGraph>),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(
                new StandardHighlightAlgorithmFactory<TVertex, TEdge, TGraph>(),
                OnHighlightAlgorithmFactoryPropertyChanged,
                CoerceHighlightAlgorithmFactory));

        private static void OnHighlightAlgorithmFactoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            string highlightMethod = graphLayout.HighlightAlgorithmType;
            graphLayout.HighlightAlgorithmType = null;
            graphLayout.HighlightAlgorithmType = highlightMethod;
        }

        /// <summary>
        /// Coerce callback of the <see cref="HighlightAlgorithmFactoryProperty"/> dependency property.
        /// </summary>
        protected static object CoerceHighlightAlgorithmFactory(DependencyObject d, object baseValue)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;
            return baseValue ?? graphLayout.HighlightAlgorithmFactory;
        }

        #endregion

        #region HighlightAlgorithm

        /// <summary>
        /// Highlight algorithm.
        /// </summary>
        public IHighlightAlgorithm<TVertex, TEdge> HighlightAlgorithm
        {
            get => (IHighlightAlgorithm<TVertex, TEdge>)GetValue(HighlightAlgorithmProperty);
            protected set => SetValue(HighlightAlgorithmPropertyKey, value);
        }

        /// <summary>
        /// Highlight algorithm dependency property.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty HighlightAlgorithmProperty;

        /// <summary>
        /// Highlight algorithm property key.
        /// </summary>
        [NotNull]
        protected static readonly DependencyPropertyKey HighlightAlgorithmPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(HighlightAlgorithm),
            typeof(IHighlightAlgorithm<TVertex, TEdge>),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(null, OnHighlightAlgorithmPropertyChanged));

        private static void OnHighlightAlgorithmPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is IHighlightAlgorithm<TVertex, TEdge> algorithm)
            {
                algorithm.ResetHighlight();
            }
        }

        #endregion

        #region HighlightAlgorithmType

        /// <summary>
        /// Highlight algorithm type.
        /// </summary>
        public string HighlightAlgorithmType
        {
            get => (string)GetValue(HighlightAlgorithmTypeProperty);
            set => SetValue(HighlightAlgorithmTypeProperty, value);
        }

        /// <summary>
        /// Highlight algorithm type dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty HighlightAlgorithmTypeProperty = DependencyProperty.Register(
            nameof(HighlightAlgorithmType),
            typeof(string),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(string.Empty, OnHighlightAlgorithmTypePropertyChanged, CoerceHighlightAlgorithmType));

        private static void OnHighlightAlgorithmTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            string newAlgorithmType = args.NewValue is null ? string.Empty : args.NewValue.ToString();

            // Regenerate algorithm, parameters
            IHighlightParameters parameters = graphLayout.HighlightAlgorithmFactory.CreateParameters(
                newAlgorithmType,
                graphLayout.HighlightParameters);

            graphLayout.HighlightAlgorithm = graphLayout.HighlightAlgorithmFactory.CreateAlgorithm(
                newAlgorithmType,
                graphLayout.CreateHighlightContext(),
                graphLayout,
                parameters);
        }

        /// <summary>
        /// Coerce callback of the <see cref="HighlightAlgorithmTypeProperty"/> dependency property.
        /// </summary>
        protected static object CoerceHighlightAlgorithmType(DependencyObject d, object baseValue)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            if (!graphLayout.HighlightAlgorithmFactory.IsValidMode(baseValue as string))
                return null;

            return baseValue;
        }

        #endregion

        #region HighlightParameters

        /// <summary>
        /// Highlight parameters.
        /// </summary>
        public IHighlightParameters HighlightParameters
        {
            get => (IHighlightParameters)GetValue(HighlightParametersProperty);
            set => SetValue(HighlightParametersProperty, value);
        }

        /// <summary>
        /// Highlight parameters dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty HighlightParametersProperty = DependencyProperty.Register(
            nameof(HighlightParameters),
            typeof(IHighlightParameters),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(null, null, CoerceHighlightParameters));

        /// <summary>
        /// Coerce callback of the <see cref="HighlightParametersProperty"/> dependency property.
        /// </summary>
        protected static object CoerceHighlightParameters(DependencyObject d, object baseValue)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            if (graphLayout.HighlightAlgorithm != null)
            {
                graphLayout.HighlightAlgorithm.TrySetParameters(baseValue as IHighlightParameters);
                return graphLayout.HighlightAlgorithm.Parameters;
            }

            return null;
        }

        #endregion

        #region OverlapRemovalAlgorithmFactory

        /// <summary>
        /// Overlap removal algorithm factory.
        /// </summary>
        public IOverlapRemovalAlgorithmFactory<TVertex> OverlapRemovalAlgorithmFactory
        {
            get => (IOverlapRemovalAlgorithmFactory<TVertex>)GetValue(OverlapRemovalAlgorithmFactoryProperty);
            set => SetValue(OverlapRemovalAlgorithmFactoryProperty, value);
        }

        /// <summary>
        /// Overlap removal algorithm factory dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty OverlapRemovalAlgorithmFactoryProperty = DependencyProperty.Register(
            nameof(OverlapRemovalAlgorithmFactory),
            typeof(IOverlapRemovalAlgorithmFactory<TVertex>),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(
                new StandardOverlapRemovalAlgorithmFactory<TVertex>(),
                null,
                CoerceOverlapRemovalAlgorithmFactory));

        /// <summary>
        /// Coerce callback of the <see cref="OverlapRemovalAlgorithmFactoryProperty"/> dependency property.
        /// </summary>
        protected static object CoerceOverlapRemovalAlgorithmFactory(DependencyObject d, object baseValue)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;
            return baseValue ?? graphLayout.OverlapRemovalAlgorithmFactory;
        }

        #endregion

        #region OverlapRemovalAlgorithm

        /// <summary>
        /// Overlap removal algorithm.
        /// </summary>
        public IOverlapRemovalAlgorithm<TVertex> OverlapRemovalAlgorithm
        {
            get => (IOverlapRemovalAlgorithm<TVertex>)GetValue(OverlapRemovalAlgorithmProperty);
            protected set => SetValue(OverlapRemovalAlgorithmPropertyKey, value);
        }

        /// <summary>
        /// Overlap removal algorithm factory dependency property.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty OverlapRemovalAlgorithmProperty;

        /// <summary>
        /// Overlap removal algorithm property key.
        /// </summary>
        [NotNull]
        protected static readonly DependencyPropertyKey OverlapRemovalAlgorithmPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(OverlapRemovalAlgorithm),
            typeof(IOverlapRemovalAlgorithm<TVertex>),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(null));

        #endregion

        #region OverlapRemovalAlgorithmType

        /// <summary>
        /// Overlap removal algorithm type.
        /// </summary>
        public string OverlapRemovalAlgorithmType
        {
            get => (string)GetValue(OverlapRemovalAlgorithmTypeProperty);
            set => SetValue(OverlapRemovalAlgorithmTypeProperty, value);
        }

        /// <summary>
        /// Overlap removal algorithm type dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty OverlapRemovalAlgorithmTypeProperty = DependencyProperty.Register(
            nameof(OverlapRemovalAlgorithmType),
            typeof(string),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(string.Empty, OnOverlapRemovalAlgorithmTypePropertyChanged));

        private static void OnOverlapRemovalAlgorithmTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            string newAlgorithmType = args.NewValue is null ? string.Empty : args.NewValue.ToString();

            // Regenerate parameters
            graphLayout.OverlapRemovalParameters = graphLayout.OverlapRemovalAlgorithmFactory.CreateParameters(
                newAlgorithmType,
                graphLayout.OverlapRemovalParameters);

            if (graphLayout.Graph != null)
            {
                graphLayout.RecalculateOverlapRemoval();
            }
        }

        #endregion

        #region OverlapRemovalConstraint

        /// <summary>
        /// Overlap removal constraint.
        /// </summary>
        public AlgorithmConstraints OverlapRemovalConstraint
        {
            get => (AlgorithmConstraints)GetValue(OverlapRemovalConstraintProperty);
            set => SetValue(OverlapRemovalConstraintProperty, value);
        }

        /// <summary>
        /// Overlap removal constraint dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty OverlapRemovalConstraintProperty = DependencyProperty.Register(
            nameof(OverlapRemovalConstraint),
            typeof(AlgorithmConstraints),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(AlgorithmConstraints.Automatic, OnOverlapRemovalConstraintPropertyChanged));

        private static void OnOverlapRemovalConstraintPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            // Regenerate parameters
            if (graphLayout.Graph != null)
            {
                graphLayout.RecalculateOverlapRemoval();
            }
        }

        #endregion

        #region OverlapRemovalParameters

        /// <summary>
        /// Overlap removal parameters.
        /// </summary>
        public IOverlapRemovalParameters OverlapRemovalParameters
        {
            get => (IOverlapRemovalParameters)GetValue(OverlapRemovalParametersProperty);
            set => SetValue(OverlapRemovalParametersProperty, value);
        }

        /// <summary>
        /// Overlap removal parameters dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty OverlapRemovalParametersProperty = DependencyProperty.Register(
            nameof(OverlapRemovalParameters),
            typeof(IOverlapRemovalParameters),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(null));

        #endregion

        #region LayoutAlgorithmFactory

        /// <summary>
        /// Layout algorithm factory.
        /// </summary>
        public ILayoutAlgorithmFactory<TVertex, TEdge, TGraph> LayoutAlgorithmFactory
        {
            get => (ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>)GetValue(LayoutAlgorithmFactoryProperty);
            set => SetValue(LayoutAlgorithmFactoryProperty, value);
        }

        /// <summary>
        /// Layout algorithm factory dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty LayoutAlgorithmFactoryProperty = DependencyProperty.Register(
            nameof(LayoutAlgorithmFactory),
            typeof(ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(
                new StandardLayoutAlgorithmFactory<TVertex, TEdge, TGraph>(),
                null,
                CoerceLayoutAlgorithmFactory));

        /// <summary>
        /// Coerce callback of the <see cref="LayoutAlgorithmFactoryProperty"/> dependency property.
        /// </summary>
        protected static object CoerceLayoutAlgorithmFactory(DependencyObject d, object baseValue)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;
            return baseValue ?? graphLayout.LayoutAlgorithmFactory;
        }

        #endregion

        #region LayoutAlgorithm

        /// <summary>
        /// The algorithm which have been used/is running/will be executed.
        /// </summary>
        public ILayoutAlgorithm<TVertex, TEdge, TGraph> LayoutAlgorithm
        {
            get => (ILayoutAlgorithm<TVertex, TEdge, TGraph>)GetValue(LayoutAlgorithmProperty);
            protected set => SetValue(LayoutAlgorithmPropertyKey, value);
        }

        /// <summary>
        /// Layout algorithm dependency property.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty LayoutAlgorithmProperty;

        /// <summary>
        /// Layout algorithm property key.
        /// </summary>
        [NotNull]
        protected static readonly DependencyPropertyKey LayoutAlgorithmPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(LayoutAlgorithm),
            typeof(ILayoutAlgorithm<TVertex, TEdge, TGraph>),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(null));

        #endregion

        #region LayoutAlgorithmType

        /// <summary>
        /// Layout algorithm type.
        /// </summary>
        public string LayoutAlgorithmType
        {
            get => (string)GetValue(LayoutAlgorithmTypeProperty);
            set => SetValue(LayoutAlgorithmTypeProperty, value);
        }

        /// <summary>
        /// Layout algorithm type dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty LayoutAlgorithmTypeProperty = DependencyProperty.Register(
            nameof(LayoutAlgorithmType),
            typeof(string),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(string.Empty, OnLayoutAlgorithmTypePropertyChanged));

        private static void OnLayoutAlgorithmTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            string newAlgorithmType = args.NewValue is null ? string.Empty : args.NewValue.ToString();

            // Regenerate parameters
            graphLayout.LayoutParameters = graphLayout.LayoutAlgorithmFactory.CreateParameters(
                newAlgorithmType,
                graphLayout.LayoutParameters);

            if (graphLayout.Graph != null)
            {
                graphLayout.Relayout();
            }
        }

        #endregion

        #region LayoutMode

        /// <summary>
        /// Layout mode.
        /// </summary>
        public LayoutMode LayoutMode
        {
            get => (LayoutMode)GetValue(LayoutModeProperty);
            set => SetValue(LayoutModeProperty, value);
        }

        /// <summary>
        /// Layout mode dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register(
            nameof(LayoutMode),
            typeof(LayoutMode),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(LayoutMode.Automatic, OnLayoutModePropertyChanged, CoerceOnLayoutMode));

        private static void OnLayoutModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            // Check if we need to register or unregister watches on graph changes events
            if (graphLayout.Graph is IMutableBidirectionalGraph<TVertex, TEdge> mutableGraph)
            {
                var oldMode = (LayoutMode)args.OldValue;
                var newMode = (LayoutMode)args.NewValue;

                bool wasCompoundMode = IsCompoundModeInternal(oldMode, graphLayout.Graph);
                bool isCompoundMode = IsCompoundModeInternal(newMode, graphLayout.Graph);

                // Update only if mode changed
                if (wasCompoundMode != isCompoundMode)
                {
                    if (!wasCompoundMode)
                    {
                        graphLayout.UnregisterMutableGraphHandlers(mutableGraph);
                    }
                    else
                    {
                        graphLayout.RegisterMutableGraphHandlers(mutableGraph);
                    }
                }
            }

            graphLayout.OnRelayoutInduction(false);
        }

        /// <summary>
        /// Coerce callback of the <see cref="LayoutMode"/> dependency property.
        /// </summary>
        protected static object CoerceOnLayoutMode(DependencyObject d, object baseValue)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;
            if (graphLayout.Graph is null)
                return LayoutMode.Automatic;
            return baseValue;
        }

        #endregion

        #region LayoutParameters

        /// <summary>
        /// Layout parameters.
        /// </summary>
        public ILayoutParameters LayoutParameters
        {
            get => (ILayoutParameters)GetValue(LayoutParametersProperty);
            set => SetValue(LayoutParametersProperty, value);
        }

        /// <summary>
        /// Layout parameters dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty LayoutParametersProperty = DependencyProperty.Register(
            nameof(LayoutParameters),
            typeof(ILayoutParameters),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new PropertyMetadata(null));

        #endregion

        #region LayoutState

        /// <summary>
        /// Layout state.
        /// </summary>
        public LayoutState<TVertex, TEdge> LayoutState
        {
            get => (LayoutState<TVertex, TEdge>)GetValue(LayoutStateProperty);
            protected set => SetValue(LayoutStatePropertyKey, value);
        }

        /// <summary>
        /// Layout state dependency property.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty LayoutStateProperty;

        /// <summary>
        /// Layout state property key.
        /// </summary>
        [NotNull]
        protected static readonly DependencyPropertyKey LayoutStatePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(LayoutState),
            typeof(LayoutState<TVertex, TEdge>),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(null));

        #endregion

        #region LayoutStatusPercent

        /// <summary>
        /// Status of the layout process in percent.
        /// </summary>
        public double LayoutStatusPercent
        {
            get => (double)GetValue(LayoutStatusPercentProperty);
            protected set => SetValue(LayoutStatusPercentPropertyKey, value);
        }

        /// <summary>
        /// Layout status percent dependency property.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty LayoutStatusPercentProperty;

        /// <summary>
        /// Layout status percent property key.
        /// </summary>
        [NotNull]
        protected static readonly DependencyPropertyKey LayoutStatusPercentPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(LayoutStatusPercent),
            typeof(double),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(0.0));

        #endregion

        #region ShowAllStates

        /// <summary>
        /// If true all states of the layout process will be stored, so you can "replay" the layout, otherwise
        /// only the start and end states will be stored.
        /// </summary>
        public bool ShowAllStates
        {
            get => (bool)GetValue(ShowAllStatesProperty);
            set => SetValue(ShowAllStatesProperty, value);
        }

        /// <summary>
        /// Show all states dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty ShowAllStatesProperty = DependencyProperty.Register(
            nameof(ShowAllStates),
            typeof(bool),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(false));

        #endregion

        #region StateCount

        /// <summary>
        /// Number of layout states.
        /// </summary>
        public int StateCount
        {
            get => (int)GetValue(StateCountProperty);
            protected set => SetValue(StateCountPropertyKey, value);
        }

        /// <summary>
        /// Layout states count dependency property.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty StateCountProperty;

        /// <summary>
        /// Layout states count property key.
        /// </summary>
        [NotNull]
        protected static readonly DependencyPropertyKey StateCountPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(StateCount),
            typeof(int),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(0, OnStateCountPropertyChanged));

        /// <summary>
        /// Property change callback of the <see cref="StateCount"/> dependency property.
        /// It coerces the <see cref="StateIndex"/>.
        /// </summary>
        private static void OnStateCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;
            graphLayout.CoerceValue(StateIndexProperty);
        }

        #endregion

        #region StateIndex

        /// <summary>
        /// Index of the currently shown layout state.
        /// </summary>
        public int StateIndex
        {
            get => (int)GetValue(StateIndexProperty);
            set => SetValue(StateIndexProperty, value);
        }

        /// <summary>
        ///  Index of the currently shown layout state dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty StateIndexProperty = DependencyProperty.Register(
            nameof(StateIndex),
            typeof(int),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new UIPropertyMetadata(0, OnStateIndexPropertyChanged, CoerceStateIndex));

        /// <summary>
        /// Property Change Callback of the <code>StateIndex</code> dependency property. It call the animation, 
        /// so the layout will animate from one state to the another.
        /// </summary>
        private static void OnStateIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;
            graphLayout.ChangeState((int)args.NewValue);
        }

        /// <summary>
        /// Coerce Callback of the <see cref="StateIndex"/> dependency property.
        /// </summary>
        /// <returns>It coerces the value of the <see cref="StateIndex"/> between 0 and (<see cref="StateCount"/>-1).</returns>
        protected static object CoerceStateIndex(DependencyObject d, object baseValue)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;
            int p = (int)baseValue;
            if (p < 0 || graphLayout._layoutStates.Count == 0)
                return 0;
            if (p > graphLayout._layoutStates.Count - 1)
                return graphLayout._layoutStates.Count - 1;
            return p;
        }

        #endregion

        #region Graph

        /// <summary>
        /// The graph we want to show.
        /// </summary>
        public TGraph Graph
        {
            get => (TGraph)GetValue(GraphProperty);
            set => SetValue(GraphProperty, value);
        }

        /// <summary>
        /// Graph dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register(
            nameof(Graph),
            typeof(TGraph),
            typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnGraphPropertyChanged));

        /// <summary>
        /// It's called when the <see cref="Graph"/> dependency property changed.
        /// </summary>
        /// <param name="d">The <see cref="GraphLayout"/> instance which <see cref="Graph"/> dependency property changed.</param>
        /// <param name="args">OldValue &amp; NewValue</param>
        protected static void OnGraphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)d;

            if (args.OldValue is TGraph oldGraph
                && !IsCompoundModeInternal(graphLayout.LayoutMode, oldGraph)
                && oldGraph is IMutableBidirectionalGraph<TVertex, TEdge> oldMutableGraph)
            {
                // Unsubscribe to events of the old graph mutations
                graphLayout.UnregisterMutableGraphHandlers(oldMutableGraph);
            }

            var newGraph = args.NewValue as TGraph;
            if (newGraph is null)
            {
                graphLayout.LayoutMode = LayoutMode.Automatic;
                return;
            }

            if (!graphLayout.IsCompoundMode && newGraph is IMutableBidirectionalGraph<TVertex, TEdge> newMutableGraph)
            {
                // Subscribe to events of the new graph mutations
                graphLayout.RegisterMutableGraphHandlers(newMutableGraph);
            }

            graphLayout.OnRelayoutInduction(true);
        }

        private void RegisterMutableGraphHandlers([NotNull] IMutableBidirectionalGraph<TVertex, TEdge> graph)
        {
            graph.VertexAdded += OnMutableGraphVertexAdded;
            graph.VertexRemoved += OnMutableGraphVertexRemoved;
            graph.EdgeAdded += OnMutableGraphEdgeAdded;
            graph.EdgeRemoved += OnMutableGraphEdgeRemoved;
        }

        private void UnregisterMutableGraphHandlers([NotNull] IMutableBidirectionalGraph<TVertex, TEdge> graph)
        {
            graph.VertexAdded -= OnMutableGraphVertexAdded;
            graph.VertexRemoved -= OnMutableGraphVertexRemoved;
            graph.EdgeAdded -= OnMutableGraphEdgeAdded;
            graph.EdgeRemoved -= OnMutableGraphEdgeRemoved;
        }

        #endregion

        #endregion

        #region Dependency Properties callbacks

        private void OnHighlightTriggered([NotNull] object sender, [NotNull] HighlightTriggeredEventArgs args)
        {
            if (Graph is null || HighlightAlgorithm is null)
                return;

            if (args.OriginalSource is VertexControl vertexControl)
            {
                OnVertexHighlightTriggered(vertexControl, args);
            }
            else if (args.OriginalSource is EdgeControl edgeControl)
            {
                OnEdgeHighlightTriggered(edgeControl, args);
            }
        }

        private void OnVertexHighlightTriggered([NotNull] VertexControl vertexControl, [NotNull] HighlightTriggeredEventArgs args)
        {
            var vertex = vertexControl.Vertex as TVertex;
            if (vertex is null || !Graph.ContainsVertex(vertex))
                return;

            if (args.IsPositiveTrigger)
            {
                HighlightAlgorithm.OnVertexHighlighting(vertex);
            }
            else
            {
                HighlightAlgorithm.OnVertexHighlightRemoving(vertex);
            }
        }

        private void OnEdgeHighlightTriggered([NotNull] EdgeControl edgeControl, [NotNull] HighlightTriggeredEventArgs args)
        {
            var edge = default(TEdge);
            try
            {
                edge = (TEdge)edgeControl.Edge;
            }
            catch
            {
                // ignored
            }

            // ReSharper disable once AssignNullToNotNullAttribute, Justification: Already checked.
            if (EqualityComparer<TEdge>.Default.Equals(edge, default(TEdge)) || !Graph.ContainsEdge(edge))
                return;

            if (args.IsPositiveTrigger)
            {
                HighlightAlgorithm.OnEdgeHighlighting(edge);
            }
            else
            {
                HighlightAlgorithm.OnEdgeHighlightRemoving(edge);
            }
        }

        private void OnRelayoutInduction(bool tryKeepControls)
        {
            HighlightAlgorithm?.ResetHighlight();

            // Recreate the graph elements
            RecreateGraphElements(tryKeepControls);

            // Do the layout process again
            Relayout();
        }

        #endregion
    }
}