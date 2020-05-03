using System.Collections.ObjectModel;
using System.Windows;
using GraphShape.Algorithms.Layout.Compound;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Compound vertex control.
    /// </summary>
    [TemplatePart(Name = PartInnerCanvas, Type = typeof(FrameworkElement))]
    public class CompoundVertexControl : VertexControl, ICompoundVertexControl
    {
        static CompoundVertexControl()
        {
            // Override the StyleKey property
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CompoundVertexControl),
                new FrameworkPropertyMetadata(typeof(CompoundVertexControl)));

            // Register a class handler for the GraphCanvas.PositionChanged routed event
            EventManager.RegisterClassHandler(
                typeof(CompoundVertexControl),
                GraphCanvas.PositionChangedEvent,
                new PositionChangedEventHandler(OnPositionChanged));
        }

        /// <summary>
        /// Inner canvas part name.
        /// </summary>
        protected const string PartInnerCanvas = "PART_InnerCanvas";

        /// <summary>
        /// Inner canvas control.
        /// </summary>
        protected FrameworkElement InnerCanvas;

        private bool _activePositionChangeReaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundVertexControl"/> class.
        /// </summary>
        public CompoundVertexControl()
        {
            Vertices = new ObservableCollection<VertexControl>();
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the control of the inner canvas
            InnerCanvas = Template.FindName(PartInnerCanvas, this) as FrameworkElement ?? this;
        }

        #region Dependency Properties

        #region Vertices

        /// <summary>
        /// Vertices controls.
        /// </summary>
        public ObservableCollection<VertexControl> Vertices
        {
            get => (ObservableCollection<VertexControl>)GetValue(VerticesProperty);
            protected set => SetValue(VerticesPropertyKey, value);
        }

        /// <summary>
        /// Vertices property key.
        /// </summary>
        [NotNull]
        protected static readonly DependencyPropertyKey VerticesPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Vertices), typeof(ObservableCollection<VertexControl>), typeof(CompoundVertexControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Vertices dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty VerticesProperty = VerticesPropertyKey.DependencyProperty;

        #endregion

        #region LayoutMode

        /// <summary>
        /// Layout mode.
        /// </summary>
        public CompoundVertexInnerLayoutType LayoutMode
        {
            get => (CompoundVertexInnerLayoutType)GetValue(LayoutModeProperty);
            set => SetValue(LayoutModeProperty, value);
        }

        /// <summary>
        /// Layout mode dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register(
            nameof(LayoutMode),
            typeof(CompoundVertexInnerLayoutType),
            typeof(CompoundVertexControl),
            new UIPropertyMetadata(CompoundVertexInnerLayoutType.Automatic));

        #endregion

        #region IsExpanded

        /// <summary>
        /// Indicates if vertex is expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        /// <summary>
        /// Is expanded dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            nameof(IsExpanded),
            typeof(bool),
            typeof(CompoundVertexControl),
            new UIPropertyMetadata(true, OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var compoundVertexControl = (CompoundVertexControl)d;
            if ((bool)args.NewValue)
            {
                compoundVertexControl.RaiseEvent(new RoutedEventArgs(ExpandedEvent, compoundVertexControl));
            }
            else
            {
                compoundVertexControl.RaiseEvent(new RoutedEventArgs(CollapsedEvent, compoundVertexControl));
            }
        }

        #endregion

        private static void OnPositionChanged([NotNull] object sender, [NotNull] PositionChangedEventArgs args)
        {
            var compoundVertexControl = args.Source as CompoundVertexControl;
            if (compoundVertexControl is null || compoundVertexControl._activePositionChangeReaction)
                return;

            compoundVertexControl._activePositionChangeReaction = true;

            // We are moving the compound vertex itself
            if (compoundVertexControl == args.OriginalSource)
            {
                // Move the children with the same amount
                foreach (VertexControl childVertexControl in compoundVertexControl.Vertices)
                {
                    GraphCanvas.SetX(childVertexControl, GraphCanvas.GetX(childVertexControl) + args.XChange);
                    GraphCanvas.SetY(childVertexControl, GraphCanvas.GetY(childVertexControl) + args.YChange);
                }
            }
            else
            {
                // We are moving the parent or one of it's child
                var childVertexControl = args.OriginalSource as VertexControl;
                if (childVertexControl is null)
                    return;

                if (compoundVertexControl.Vertices.Contains(childVertexControl))
                {
                    // Update the position of all child vertices
                    foreach (VertexControl control in compoundVertexControl.Vertices)
                    {
                        if (control == childVertexControl)
                            continue;

                        var childCenterPos = new System.Windows.Point(control.ActualWidth / 2, control.ActualHeight / 2);
                        System.Windows.Point translatedChildCenterPos = control.TranslatePoint(childCenterPos, control.RootCanvas);
                        
                        GraphCanvas.SetX(control, translatedChildCenterPos.X - control.RootCanvas.Translation.X);
                        GraphCanvas.SetY(control, translatedChildCenterPos.Y - control.RootCanvas.Translation.Y);
                    }
                }
            }

            compoundVertexControl._activePositionChangeReaction = false;
        }

        #endregion

        #region ICompoundVertexControl

        /// <inheritdoc />
        public Size InnerCanvasSize => InnerCanvas is null
            ? default(Size)
            : new Size(InnerCanvas.ActualWidth, InnerCanvas.ActualHeight);

        /// <inheritdoc />
        public Thickness VertexBorderThickness
        {
            get
            {
                if (InnerCanvas is null)
                    return default(Thickness);

                System.Windows.Point innerCanvasPosition = InnerCanvas.TranslatePoint(default(System.Windows.Point), this);
                Size innerCanvasSize = InnerCanvasSize;
                var size = new Size(ActualWidth, ActualHeight);

                // Calculate the thickness
                return new Thickness(
                    innerCanvasPosition.X,
                    innerCanvasPosition.Y,
                    size.Width - (innerCanvasPosition.X + innerCanvasSize.Width),
                    size.Height - (innerCanvasPosition.Y + innerCanvasSize.Height));
            }
        }

        #region Routed Events

        /// <summary>
        /// Expanded event.
        /// </summary>
        [NotNull]
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(
            "Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CompoundVertexControl));

        /// <inheritdoc />
        public event RoutedEventHandler Expanded
        {
            add => AddHandler(ExpandedEvent, value);
            remove => RemoveHandler(ExpandedEvent, value);
        }

        /// <summary>
        /// Collapsed event.
        /// </summary>
        [NotNull]
        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent(
            "Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CompoundVertexControl));

        /// <inheritdoc />
        public event RoutedEventHandler Collapsed
        {
            add => AddHandler(CollapsedEvent, value);
            remove => RemoveHandler(CollapsedEvent, value);
        }

        #endregion

        #endregion
    }
}
