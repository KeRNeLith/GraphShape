using System;
using GraphShape.Utils;

namespace GraphShape
{
    /// <summary>
    /// Represents the thickness of an object.
    /// </summary>
    [Serializable]
    public struct Thickness : IEquatable<Thickness>
    {
        /// <summary>
        /// Left bound value.
        /// </summary>
        public readonly double Left;

        /// <summary>
        /// Top bound value.
        /// </summary>
        public readonly double Top;

        /// <summary>
        /// Right bound value.
        /// </summary>
        public readonly double Right;

        /// <summary>
        /// Bottom bound value.
        /// </summary>
        public readonly double Bottom;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="left">Left bound value.</param>
        /// <param name="top">Top bound value.</param>
        /// <param name="right">Right bound value.</param>
        /// <param name="bottom">Bottom bound value.</param>
        public Thickness(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Indicates whether both <see cref="Thickness"/> are equal.
        /// </summary>
        /// <param name="thickness1">First <see cref="Thickness"/> to compare.</param>
        /// <param name="thickness2">Second <see cref="Thickness"/> to compare.</param>
        /// <returns>True if both <see cref="Thickness"/> are equal, otherwise false.</returns>
        public static bool operator ==(Thickness thickness1, Thickness thickness2)
        {
            return MathUtils.NearEqual(thickness1.Left, thickness2.Left)
                && MathUtils.NearEqual(thickness1.Top, thickness2.Top)
                && MathUtils.NearEqual(thickness1.Right, thickness2.Right)
                && MathUtils.NearEqual(thickness1.Bottom, thickness2.Bottom);
        }

        /// <summary>
        /// Indicates whether both <see cref="Thickness"/> are not equal.
        /// </summary>
        /// <param name="thickness1">First <see cref="Thickness"/> to compare.</param>
        /// <param name="thickness2">Second <see cref="Thickness"/> to compare.</param>
        /// <returns>True if both <see cref="Thickness"/> are equal, otherwise false.</returns>
        public static bool operator !=(Thickness thickness1, Thickness thickness2)
        {
            return !(thickness1 == thickness2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Thickness thickness)
                return this == thickness;
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Thickness other)
        {
            return this == other;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
        }
    }
}