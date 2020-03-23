using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Edge control.
    /// </summary>
    public class EdgeControl : Control, IPoolObject, IDisposable
    {
        static EdgeControl()
        {
            // Override the StyleKey property
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(EdgeControl),
                new FrameworkPropertyMetadata(typeof(EdgeControl)));
        }

        #region Dependency Properties

        #region Source

        /// <summary>
        /// Source vertex.
        /// </summary>
        public VertexControl Source
        {
            get => (VertexControl)GetValue(SourceProperty);
            internal set => SetValue(SourceProperty, value);
        }

        /// <summary>
        /// Source vertex dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(VertexControl), typeof(EdgeControl), new UIPropertyMetadata(null));

        #endregion

        #region Target

        /// <summary>
        /// Target vertex.
        /// </summary>
        public VertexControl Target
        {
            get => (VertexControl)GetValue(TargetProperty);
            internal set => SetValue(TargetProperty, value);
        }

        /// <summary>
        /// Target vertex dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            nameof(Target), typeof(VertexControl), typeof(EdgeControl), new UIPropertyMetadata(null));

        #endregion

        #region RoutePoints

        /// <summary>
        /// Route points.
        /// </summary>
        public Point[] RoutePoints
        {
            get => (Point[])GetValue(RoutePointsProperty);
            set => SetValue(RoutePointsProperty, value);
        }

        /// <summary>
        /// Route points dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty RoutePointsProperty = DependencyProperty.Register(
            nameof(RoutePoints), typeof(Point[]), typeof(EdgeControl), new UIPropertyMetadata(null));

        #endregion

        #region Edge

        /// <summary>
        /// Edge.
        /// </summary>
        public object Edge
        {
            get => GetValue(EdgeProperty);
            set => SetValue(EdgeProperty, value);
        }

        /// <summary>
        /// Edge dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty EdgeProperty = DependencyProperty.Register(
            nameof(Edge), typeof(object), typeof(EdgeControl), new PropertyMetadata(null));

        #endregion

        #region StrokeThickness

        /// <summary>
        /// Stroke thickness.
        /// </summary>
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        /// <summary>
        /// Stroke thickness dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(
            typeof(EdgeControl), new UIPropertyMetadata(2.0));

        #endregion

        #endregion

        #region IPoolObject

        /// <inheritdoc />
        public void Reset()
        {
            Edge = null;
            RoutePoints = null;
            Source = null;
            Target = null;
        }

        /// <inheritdoc />
        public void Terminate()
        {
            // Nothing to do, there are no unmanaged resources
        }

        /// <inheritdoc />
        public event DisposingHandler Disposing;

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose this object.
        /// </summary>
        /// <param name="disposing">Indicates if called by dispose or finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            Disposing?.Invoke(this);
        }

        #endregion

        #endregion
    }
}