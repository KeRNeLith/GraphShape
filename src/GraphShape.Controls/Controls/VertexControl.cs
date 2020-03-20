using System;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Vertex control.
    /// </summary>
    public class VertexControl : Control, IPoolObject, IDisposable
    {
        static VertexControl()
        {
            // Override the StyleKey property
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(VertexControl),
                new FrameworkPropertyMetadata(typeof(VertexControl)));
        }

        #region Vertex

        /// <summary>
        /// Vertex object.
        /// </summary>
        public object Vertex
        {
            get => GetValue(VertexProperty);
            set => SetValue(VertexProperty, value);
        }

        /// <summary>
        /// Vertex dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty VertexProperty = DependencyProperty.Register(
            nameof(Vertex), typeof(object), typeof(VertexControl), new UIPropertyMetadata(null));

        #endregion

        #region RootCanvas

        /// <summary>
        /// Root canvas.
        /// </summary>
        public GraphCanvas RootCanvas
        {
            get => (GraphCanvas)GetValue(RootCanvasProperty);
            set => SetValue(RootCanvasProperty, value);
        }

        /// <summary>
        /// Root canvas dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty RootCanvasProperty = DependencyProperty.Register(
            nameof(RootCanvas), typeof(GraphCanvas), typeof(VertexControl), new UIPropertyMetadata(null));

        #endregion

        #region IPoolObject

        /// <inheritdoc />
        public void Reset()
        {
            Vertex = null;
        }

        /// <inheritdoc />
        public void Terminate()
        {
            // Nothing to do, there are no unmanaged resources
        }

        /// <inheritdoc />
        public event DisposingHandler Disposing;

        /// <inheritdoc />
        public void Dispose()
        {
            Disposing?.Invoke(this);
        }

        #endregion
    }
}