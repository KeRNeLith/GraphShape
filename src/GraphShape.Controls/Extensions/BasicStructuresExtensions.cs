using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphShape.Controls.Extensions
{
    /// <summary>
    /// Extensions related to basic structures types.
    /// </summary>
    public static class BasicStructuresExtensions
    {
        /// <summary>
        /// Converts a <see cref="Point"/> into a <see cref="System.Windows.Point"/>.
        /// </summary>
        [Pure]
        public static System.Windows.Point ToPoint(this Point point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }

        /// <summary>
        /// Converts a set of <see cref="Point"/>s into a set of <see cref="System.Windows.Point"/>s.
        /// </summary>
        [Pure]
        [NotNull]
        public static IEnumerable<System.Windows.Point> ToPoints([NotNull] this IEnumerable<Point> points)
        {
            foreach (Point point in points)
            {
                yield return new System.Windows.Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Converts a <see cref="System.Windows.Point"/> into a <see cref="Point"/>.
        /// </summary>
        [Pure]
        public static Point ToGraphShapePoint(this System.Windows.Point point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
