using System.Windows;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Handler for an object position changed event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void PositionChangedEventHandler([NotNull] object sender, [NotNull] PositionChangedEventArgs args);

    /// <summary>
    /// Object position changed event arguments.
    /// </summary>
    public class PositionChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// X displacement.
        /// </summary>
        public double XChange { get; }
        
        /// <summary>
        /// Y displacement.
        /// </summary>
        public double YChange { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="RoutedEventArgs" /> class.</param>
        /// <param name="source">
        /// An alternate source that will be reported when the event is handled.
        /// This pre-populates the <see cref="RoutedEventArgs.Source" /> property.
        /// </param>
        /// <param name="xChange">X displacement.</param>
        /// <param name="yChange">Y displacement.</param>
        public PositionChangedEventArgs(
            [NotNull] RoutedEvent routedEvent,
            [NotNull] object source,
            double xChange,
            double yChange)
            : base(routedEvent, source)
        {
            XChange = xChange;
            YChange = yChange;
        }
    }
}