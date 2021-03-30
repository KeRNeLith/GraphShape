using JetBrains.Annotations;

namespace GraphShape.Sample.Controls
{
    /// <summary>
    /// Handler for a content size changed event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="newSize">New content size.</param>
    internal delegate void ContentSizeChangedHandler([NotNull] object sender, System.Windows.Size newSize);
}