using GraphShape.Algorithms.Layout;
using JetBrains.Annotations;

namespace GraphShape.Tests
{
    /// <summary>
    /// Test helpers related to layout.
    /// </summary>
    internal static class LayoutTestHelpers
    {
        [Pure]
        public static bool IsHorizontal(this LayoutDirection direction)
        {
            return direction == LayoutDirection.LeftToRight
                   || direction == LayoutDirection.RightToLeft;
        }

        [Pure]
        public static bool IsVertical(this LayoutDirection direction)
        {
            return direction == LayoutDirection.TopToBottom
                   || direction == LayoutDirection.BottomToTop;
        }
    }
}
