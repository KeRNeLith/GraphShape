using System.Windows;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Handler for an object highlight information changed event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void HighlightInfoChangedEventHandler([NotNull] object sender, [NotNull] HighlightInfoChangedEventArgs args);

    /// <summary>
    /// Highlight information changed event arguments.
    /// </summary>
    public class HighlightInfoChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Old highlight information.
        /// </summary>
        public object OldHighlightInfo { get; }

        /// <summary>
        /// New highlight information.
        /// </summary>
        public object NewHighlightInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightInfoChangedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="RoutedEventArgs" /> class.</param>
        /// <param name="source">
        /// An alternate source that will be reported when the event is handled.
        /// This pre-populates the <see cref="RoutedEventArgs.Source" /> property.
        /// </param>
        /// <param name="oldHighlightInfo">Old highlight information.</param>
        /// <param name="newHighlightInfo">New highlight information.</param>
        public HighlightInfoChangedEventArgs(
            [NotNull] RoutedEvent routedEvent,
            [NotNull] object source,
            object oldHighlightInfo,
            object newHighlightInfo)
            : base(routedEvent, source)
        {
            OldHighlightInfo = oldHighlightInfo;
            NewHighlightInfo = newHighlightInfo;
        }
    }
}