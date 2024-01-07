﻿using System;
using GraphShape.Utils;
using JetBrains.Annotations;

namespace GraphShape
{
    /// <summary>
    /// Represents an x and y coordinate pair in 2D space.
    /// </summary>
    [Serializable]
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        /// X.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Indicates whether both <see cref="Point"/> are equal.
        /// </summary>
        /// <param name="point1">First <see cref="Point"/> to compare.</param>
        /// <param name="point2">Second <see cref="Point"/> to compare.</param>
        /// <returns>True if both <see cref="Point"/> are equal, otherwise false.</returns>
        public static bool operator ==(Point point1, Point point2)
        {
            return Equals(point1, point2);
        }

        /// <summary>
        /// Indicates whether both <see cref="Point"/> are not equal.
        /// </summary>
        /// <param name="point1">First <see cref="Point"/> to compare.</param>
        /// <param name="point2">Second <see cref="Point"/> to compare.</param>
        /// <returns>True if both <see cref="Point"/> are equal, otherwise false.</returns>
        public static bool operator !=(Point point1, Point point2)
        {
            return !(point1 == point2);
        }

        /// <summary>
        /// Compares the two specified <see cref="Point"/>s for equality.
        /// </summary>
        /// <param name="point1">The first <see cref="Point"/> to compare.</param>
        /// <param name="point2">The second <see cref="Point"/> to compare.</param>
        /// <returns>True if both <see cref="Point"/> are equal, otherwise false.</returns>
        public static bool Equals(Point point1, Point point2)
        {
            return MathUtils.NearEqual(point1.X, point2.X)
                   && MathUtils.NearEqual(point1.Y, point2.Y);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Point point)
                return Equals(this, point);
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Point other)
        {
            return Equals(this, other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{X};{Y}";
        }

        /// <summary>
        /// Translates the specified <see cref="Point" /> by the specified <see cref="Vector" /> and returns the result.
        /// </summary>
        /// <param name="point">The point to translate.</param>
        /// <param name="vector">The amount by which to translate <paramref name="point" />.</param>
        /// <returns>The result of translating the specified point by the specified vector.</returns>
        [Pure]
        public static Point operator +(Point point, Vector vector)
        {
            return new Point(point.X + vector.X, point.Y + vector.Y);
        }

        /// <summary>
        /// Subtracts the specified <see cref="Vector" /> from the specified <see cref="Point" /> and returns the resulting <see cref="Point" />.
        /// </summary>
        /// <param name="point">The point from which <paramref name="vector" /> is subtracted.</param>
        /// <param name="vector">The vector to subtract from <paramref name="point" />.</param>
        /// <returns>The difference between <paramref name="point" /> and <paramref name="vector" />.</returns>
        [Pure]
        public static Point operator -(Point point, Vector vector)
        {
            return new Point(point.X - vector.X, point.Y - vector.Y);
        }

        /// <summary>
        /// Subtracts the specified <see cref="Point" /> from another specified <see cref="Point" /> and returns the difference as a <see cref="Vector" />.
        /// </summary>
        /// <param name="point1">The point from which <paramref name="point2" /> is subtracted.</param>
        /// <param name="point2">The point to subtract from <paramref name="point1" />.</param>
        /// <returns>The difference between <paramref name="point1" /> and <paramref name="point2" />.</returns>
        [Pure]
        public static Vector operator -(Point point1, Point point2)
        {
            return new Vector(point1.X - point2.X, point1.Y - point2.Y);
        }
    }
}