using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphShape.Sample.Controls
{
    /// <summary>
    /// Zoom content presenter control.
    /// </summary>
    internal sealed class ZoomContentPresenter : ContentPresenter
    {
        public event ContentSizeChangedHandler ContentSizeChanged;

        private System.Windows.Size _contentSize;

        public System.Windows.Size ContentSize
        {
            get => _contentSize;
            private set
            {
                if (_contentSize == value)
                    return;

                _contentSize = value;
                ContentSizeChanged?.Invoke(this, _contentSize);
            }
        }

        private const int InfiniteSize = 1000000000;

        /// <inheritdoc />
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            base.MeasureOverride(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));

            return new System.Windows.Size(
                double.IsInfinity(constraint.Width) ? InfiniteSize : constraint.Width,
                double.IsInfinity(constraint.Height) ? InfiniteSize : constraint.Height);
        }

        /// <inheritdoc />
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeSize)
        {
            UIElement uiElement = VisualChildrenCount > 0
                ? VisualTreeHelper.GetChild(this, 0) as UIElement
                : null;
            if (uiElement is null)
                return arrangeSize;

            ContentSize = uiElement.DesiredSize;
            uiElement.Arrange(new System.Windows.Rect(uiElement.DesiredSize));

            return arrangeSize;
        }
    }
}