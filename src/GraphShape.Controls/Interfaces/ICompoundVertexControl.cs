using System.Windows;

namespace GraphShape.Controls
{
    /// <summary>
    /// Represents a compound vertex control.
    /// </summary>
    internal interface ICompoundVertexControl
    {
        /// <summary>
        /// Gets the 'border thickness' of the control around the inner canvas.
        /// </summary>
        Thickness VertexBorderThickness { get; }

        /// <summary>
        /// Gets the size of the inner canvas.
        /// </summary>
        Size InnerCanvasSize { get; }

        /// <summary>
        /// Fired when the control is expanded.
        /// </summary>
        event RoutedEventHandler Expanded;

        /// <summary>
        /// Fired when the control is collapsed.
        /// </summary>
        event RoutedEventHandler Collapsed;
    }
}
