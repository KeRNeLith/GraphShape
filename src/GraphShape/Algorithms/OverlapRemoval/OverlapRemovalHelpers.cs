using System.Windows;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Overlap removal helpers.
    /// </summary>
    public static class OverlapRemovalHelpers
    {
        /// <summary>
        /// Gets the center of this <paramref name="rectangle"/>.
        /// </summary>
        /// <param name="rectangle">Rectangle from which getting center.</param>
        /// <returns>Rectangle center.</returns>
        [Pure]
        public static Point GetCenter(this Rect rectangle)
        {
            return new Point(
                rectangle.Left + rectangle.Width / 2,
                rectangle.Top + rectangle.Height / 2);
        }
    }
}