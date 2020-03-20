using System.Windows;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Handler for an object highlight trigger changed event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void HighlightTriggerEventHandler([NotNull] object sender, [NotNull] HighlightTriggeredEventArgs args);

    /// <summary>
    /// Highlight triggered event arguments.
    /// </summary>
    public class HighlightTriggeredEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Indicates if event has been canceled or not.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Highlight triggered?
        /// </summary>
        public bool IsPositiveTrigger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="RoutedEventArgs" /> class.</param>
        /// <param name="source">
        /// An alternate source that will be reported when the event is handled.
        /// This pre-populates the <see cref="RoutedEventArgs.Source" /> property.
        /// </param>
        /// <param name="isPositiveTrigger">Indicates if trigger has been triggered.</param>
        public HighlightTriggeredEventArgs(
            [NotNull] RoutedEvent routedEvent,
            [NotNull] object source,
            bool isPositiveTrigger)
            : base(routedEvent, source)
        {
            IsPositiveTrigger = isPositiveTrigger;
        }
    }
}
