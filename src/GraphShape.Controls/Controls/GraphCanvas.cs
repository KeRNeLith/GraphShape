using System;
using System.Windows;
using System.Windows.Controls;
using GraphShape.Controls.Animations;
using GraphShape.Controls.Extensions;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Graph canvas.
    /// </summary>
    public class GraphCanvas : Panel
    {
        #region Attached Dependency Property registrations

        /// <summary>
        /// X attached dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty XProperty = DependencyProperty.RegisterAttached(
            "X",
            typeof(double),
            typeof(GraphCanvas),
            new FrameworkPropertyMetadata(
                double.NaN,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                X_PropertyChanged));

        private static void X_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            double xChange = (double)args.NewValue - (double)args.OldValue;
            PositionChanged(d, xChange, 0);
        }

        /// <summary>
        /// Y attached dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty YProperty = DependencyProperty.RegisterAttached(
            "Y",
            typeof(double),
            typeof(GraphCanvas),
            new FrameworkPropertyMetadata(
                double.NaN,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                Y_PropertyChanged));

        private static void Y_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            double yChange = (double)args.NewValue - (double)args.OldValue;
            PositionChanged(d, 0, yChange);
        }

        private static void PositionChanged(DependencyObject d, double xChange, double yChange)
        {
            if (d is UIElement uiElement)
            {
                uiElement.RaiseEvent(
                    new PositionChangedEventArgs(PositionChangedEvent, uiElement, xChange, yChange));
            }
        }

        #endregion

        #region Attached Properties

        /// <summary>
        /// Gets the X attached property value.
        /// </summary>
        [AttachedPropertyBrowsableForChildren]
        public static double GetX(DependencyObject obj)
        {
            return (double)obj.GetValue(XProperty);
        }

        /// <summary>
        /// Sets the X attached property value.
        /// </summary>
        public static void SetX(DependencyObject obj, double value)
        {
            obj.SetValue(XProperty, value);
        }

        /// <summary>
        /// Gets the Y attached property value.
        /// </summary>
        [AttachedPropertyBrowsableForChildren]
        public static double GetY(DependencyObject obj)
        {
            return (double)obj.GetValue(YProperty);
        }

        /// <summary>
        /// Sets the Y attached property value.
        /// </summary>
        public static void SetY(DependencyObject obj, double value)
        {
            obj.SetValue(YProperty, value);
        }

        #endregion

        #region Attached Routed Events

        /// <summary>
        /// Position changed event.
        /// </summary>
        [NotNull]
        public static readonly RoutedEvent PositionChangedEvent = EventManager.RegisterRoutedEvent(
            "PositionChanged", RoutingStrategy.Bubble, typeof(PositionChangedEventHandler), typeof(GraphCanvas));

        /// <summary>
        /// Adds a new <see cref="PositionChangedEvent"/> handler.
        /// </summary>
        public static void AddPositionChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.AddHandler(PositionChangedEvent, handler);
            }
        }

        /// <summary>
        /// Removes the given <paramref name="handler"/> from <see cref="PositionChangedEvent"/>.
        /// </summary>
        public static void RemovePositionChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.RemoveHandler(PositionChangedEvent, handler);
            }
        }

        #endregion

        #region Animation length

        /// <summary>
        /// Gets or sets the length of the animation.
        /// If the length of the animations is 0:0:0.000, there won't be any animations.
        /// </summary>
        public TimeSpan AnimationLength
        {
            get => (TimeSpan)GetValue(AnimationLengthProperty);
            set => SetValue(AnimationLengthProperty, value);
        }

        /// <summary>
        /// Animation length dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty AnimationLengthProperty = DependencyProperty.Register(
            nameof(AnimationLength),
            typeof(TimeSpan),
            typeof(GraphCanvas),
            new UIPropertyMetadata(new TimeSpan(0, 0, 0, 0, 500)));

        #endregion

        #region CreationAnimation

        /// <summary>
        /// Gets or sets the animation controller for the 'Control Creation' animation.
        /// </summary>
        public ITransition CreationTransition
        {
            get => (ITransition)GetValue(CreationTransitionProperty);
            set => SetValue(CreationTransitionProperty, value);
        }

        /// <summary>
        /// Transition creation dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty CreationTransitionProperty = DependencyProperty.Register(
            nameof(CreationTransition), typeof(ITransition), typeof(GraphCanvas), new UIPropertyMetadata(new FadeInTransition()));

        #endregion

        #region IsAnimationEnabled

        /// <summary>
        /// If this property is true, and the other animation disabler
        /// properties are also true, the animation is enabled.
        /// If this is false, the animations will be disabled.
        /// </summary>
        public bool IsAnimationEnabled
        {
            get => (bool)GetValue(IsAnimationEnabledProperty);
            set => SetValue(IsAnimationEnabledProperty, value);
        }

        /// <summary>
        /// Animation enabled dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.Register(
            nameof(IsAnimationEnabled), typeof(bool), typeof(GraphCanvas), new UIPropertyMetadata(true));

        #endregion

        #region MoveAnimation

        /// <summary>
        /// Gets or sets the animation controller for the 'Control Moving' animation.
        /// </summary>
        public IAnimation MoveAnimation
        {
            get => (IAnimation)GetValue(MoveAnimationProperty);
            set => SetValue(MoveAnimationProperty, value);
        }

        /// <summary>
        /// Animation movement dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty MoveAnimationProperty = DependencyProperty.Register(
            nameof(MoveAnimation), typeof(IAnimation), typeof(GraphCanvas), new UIPropertyMetadata(new SimpleMoveAnimation()));

        #endregion

        #region DestructionAnimation

        /// <summary>
        /// Gets or sets the transition controller for the 'Control Destruction' animation.
        /// </summary>
        public ITransition DestructionTransition
        {
            get => (ITransition)GetValue(DestructionTransitionProperty);
            set => SetValue(DestructionTransitionProperty, value);
        }

        /// <summary>
        /// Transition destruction dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty DestructionTransitionProperty = DependencyProperty.Register(
            nameof(DestructionTransition), typeof(ITransition), typeof(GraphCanvas), new UIPropertyMetadata(new FadeOutTransition()));


        #endregion

        #region Origo

        /// <summary>
        /// Gets or sets the virtual origo of the canvas.
        /// </summary>
        public Point Origo
        {
            get => (Point)GetValue(OrigoProperty);
            set => SetValue(OrigoProperty, value);
        }

        /// <summary>
        /// Origo dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty OrigoProperty = DependencyProperty.Register(
            nameof(Origo),
            typeof(Point),
            typeof(GraphCanvas),
            new FrameworkPropertyMetadata(
                default(Point),
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange));

        #endregion

        #region Translation

        /// <summary>
        /// Translation.
        /// </summary>
        public System.Windows.Vector Translation
        {
            get => (System.Windows.Vector)GetValue(TranslationProperty);
            protected set => SetValue(TranslationPropertyKey, value);
        }

        /// <summary>
        /// Translation property key.
        /// </summary>
        [NotNull]
        protected static readonly DependencyPropertyKey TranslationPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Translation), typeof(System.Windows.Vector), typeof(GraphCanvas), new UIPropertyMetadata(default(System.Windows.Vector)));

        /// <summary>
        /// Translation dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty TranslationProperty = TranslationPropertyKey.DependencyProperty;

        #endregion

        #region Measure & Arrange

        /// <summary>
        /// The position of the topLeft corner of the most top-left vertex.
        /// </summary>
        private System.Windows.Point _topLeft;

        /// <summary>
        /// The position of the bottom right corner of the most  bottom-right vertex.
        /// </summary>
        private System.Windows.Point _bottomRight;

        /// <summary>
        /// Arranges the size of the control.
        /// </summary>
        /// <param name="finalSize">The arranged size of the control.</param>
        /// <returns>The size of the control.</returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            var translate = new System.Windows.Vector(-_topLeft.X, -_topLeft.Y);
            System.Windows.Vector graphSize = _bottomRight - _topLeft;

            if (double.IsNaN(graphSize.X)
                || double.IsNaN(graphSize.Y)
                || double.IsInfinity(graphSize.X)
                || double.IsInfinity(graphSize.Y))
            {
                translate = new System.Windows.Vector(0, 0);
            }

            Translation = translate;

            graphSize = InternalChildren.Count > 0
                ? new System.Windows.Vector(double.NegativeInfinity, double.NegativeInfinity)
                : default(System.Windows.Vector);

            // Translate with the topLeft
            foreach (UIElement child in InternalChildren)
            {
                double x = GetX(child);
                double y = GetY(child);
                if (double.IsNaN(x) || double.IsNaN(y))
                {
                    // Not a vertex, set the coordinates of the top-left corner
                    x = double.IsNaN(x) ? translate.X : x;
                    y = double.IsNaN(y) ? translate.Y : y;
                }
                else
                {
                    // This is a vertex
                    x += translate.X;
                    y += translate.Y;

                    // Get the top-left corner
                    x -= child.DesiredSize.Width * 0.5;
                    y -= child.DesiredSize.Height * 0.5;
                }
                child.Arrange(
                    new System.Windows.Rect(
                        new System.Windows.Point(x, y),
                        child.DesiredSize));

                graphSize.X = Math.Max(0, Math.Max(graphSize.X, x + child.DesiredSize.Width));
                graphSize.Y = Math.Max(0, Math.Max(graphSize.Y, y + child.DesiredSize.Height));
            }

            return new System.Windows.Size(graphSize.X, graphSize.Y);
        }

        /// <summary>
        /// Overridden measure. It calculates a size where all of 
        /// of the vertices are visible.
        /// </summary>
        /// <param name="availableSize">The size constraint.</param>
        /// <returns>The calculated size.</returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            _topLeft = new System.Windows.Point(double.PositiveInfinity, double.PositiveInfinity);
            _bottomRight = new System.Windows.Point(double.NegativeInfinity, double.NegativeInfinity);

            foreach (UIElement child in InternalChildren)
            {
                // Measure the child
                child.Measure(availableSize);

                // Get the position of the vertex
                double left = GetX(child);
                double top = GetY(child);

                double halfWidth = child.DesiredSize.Width * 0.5;
                double halfHeight = child.DesiredSize.Height * 0.5;

                if (double.IsNaN(left) || double.IsNaN(top))
                {
                    left = halfWidth;
                    top = halfHeight;
                }

                // Get the top left corner point
                _topLeft.X = Math.Min(_topLeft.X, left - halfWidth - Origo.X);
                _topLeft.Y = Math.Min(_topLeft.Y, top - halfHeight - Origo.Y);

                // Calculate the bottom right corner point
                _bottomRight.X = Math.Max(_bottomRight.X, left + halfWidth - Origo.X);
                _bottomRight.Y = Math.Max(_bottomRight.Y, top + halfHeight - Origo.Y);
            }

            var graphSize = (System.Windows.Size)(_bottomRight - _topLeft);
            graphSize.Width = Math.Max(0, graphSize.Width);
            graphSize.Height = Math.Max(0, graphSize.Height);

            if (double.IsNaN(graphSize.Width)
                || double.IsNaN(graphSize.Height)
                || double.IsInfinity(graphSize.Width)
                || double.IsInfinity(graphSize.Height))
            {
                return default(System.Windows.Size);
            }

            return graphSize;
        }

        #endregion

        /// <summary>
        /// The layout process will be initialized with the current vertices positions.
        /// </summary>
        public virtual void ContinueLayout()
        {
        }

        /// <summary>
        /// The layout process will be started without initial vertices positions.
        /// </summary>
        public virtual void Relayout()
        {
        }

        [CanBeNull]
        private IAnimationContext _animationContext;

        /// <summary>
        /// Gets the context of the animation.
        /// </summary>
        public virtual IAnimationContext AnimationContext => _animationContext ?? (_animationContext = new AnimationContext(this));

        /// <summary>
        /// Gets whether the animation could be run, or not.
        /// </summary>
        public virtual bool CanAnimate => IsAnimationEnabled;

        /// <summary>
        /// Does a transition for the <paramref name="control"/> which has been
        /// already added to this container.
        /// </summary>
        /// <param name="control">The control which has been added.</param>
        protected virtual void RunCreationTransition([NotNull] Control control)
        {
            if (CanAnimate)
            {
                CreationTransition?.Run(AnimationContext, control, AnimationLength);
            }
        }

        /// <summary>
        /// Animates the position of the given <paramref name="control"/> to the given positions.
        /// </summary>
        /// <param name="control">The control which should be moved.</param>
        /// <param name="x">The new horizontal position of the control.</param>
        /// <param name="y">The new vertical position of the control.</param>
        protected virtual void RunMoveAnimation([NotNull] Control control, double x, double y)
        {
            if (MoveAnimation != null && CanAnimate)
            {
                // Animate to the given position
                MoveAnimation.Animate(AnimationContext, control, x, y, AnimationLength);
            }
            else
            {
                // Cannot animate, only the given positions
                SetX(control, x);
                SetY(control, y);
            }
        }

        /// <summary>
        /// Transitions a control which gonna' be removed from this container.
        /// </summary>
        /// <param name="control">The control which will be removed.</param>
        /// <param name="dontRemoveAfter">
        /// If it's true, the control won't be removed automatically
        /// from this container's <see cref="P:System.Windows.Controls.Panel.Children"/>.
        /// </param>
        protected virtual void RunDestructionTransition([NotNull] Control control, bool dontRemoveAfter)
        {
            if (DestructionTransition is null || !CanAnimate)
            {
                if (!dontRemoveAfter)
                {
                    Children.Remove(control);
                }
            }
            else
            {
                if (dontRemoveAfter)
                {
                    DestructionTransition.Run(AnimationContext, control, AnimationLength);
                }
                else
                {
                    DestructionTransition.Run(AnimationContext, control, AnimationLength, Children.Remove);
                }
            }
        }
    }
}