using System;
using GraphShape.Utils;
using JetBrains.Annotations;

namespace GraphShape
{
    /// <summary>
    /// Represents a displacement in 2D space.
    /// </summary>
    [Serializable]
    public struct Vector : IEquatable<Vector>
    {
        /// <summary>
        /// X component value.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y component value.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X component value.</param>
        /// <param name="y">Y component value.</param>
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Indicates whether both <see cref="Vector"/> are equal.
        /// </summary>
        /// <param name="vector1">First <see cref="Vector"/> to compare.</param>
        /// <param name="vector2">Second <see cref="Vector"/> to compare.</param>
        /// <returns>True if both <see cref="Vector"/> are equal, otherwise false.</returns>
        public static bool operator ==(Vector vector1, Vector vector2)
        {
            return Equals(vector1, vector2);
        }

        /// <summary>
        /// Indicates whether both <see cref="Vector"/> are not equal.
        /// </summary>
        /// <param name="vector1">First <see cref="Vector"/> to compare.</param>
        /// <param name="vector2">Second <see cref="Vector"/> to compare.</param>
        /// <returns>True if both <see cref="Vector"/> are equal, otherwise false.</returns>
        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return !(vector1 == vector2);
        }

        /// <summary>
        /// Compares the two specified <see cref="Vector"/>s for equality.
        /// </summary>
        /// <param name="vector1">The first <see cref="Vector"/> to compare.</param>
        /// <param name="vector2">The second <see cref="Vector"/> to compare.</param>
        /// <returns>True if both <see cref="Vector"/> are equal, otherwise false.</returns>
        public static bool Equals(Vector vector1, Vector vector2)
        {
            return MathUtils.NearEqual(vector1.X, vector2.X)
                   && MathUtils.NearEqual(vector1.Y, vector2.Y);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Vector vector)
                return Equals(this, vector);
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Vector other)
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
        /// Gets the length of this vector.
        /// </summary>
        /// <returns>The length of this vector.</returns>
        public double Length => Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// Gets the squared length of this vector.
        /// </summary>
        /// <returns>The squared length of this vector.</returns>
        public double LengthSquared => X * X + Y * Y;

        /// <summary>
        /// Normalizes this vector.
        /// </summary>
        public void Normalize()
        {
            this /= Math.Max(Math.Abs(X), Math.Abs(Y));
            this /= Length;
        }

        /// <summary>
        /// Negates the specified vector.
        /// </summary>
        /// <param name="vector">The vector to negate.</param>
        /// <returns>
        /// A vector with <see cref="X" /> and <see cref="Y" /> values opposite of the <see cref="X" /> and <see cref="Y" /> values of <paramref name="vector" />.
        /// </returns>
        [Pure]
        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.X, -vector.Y);
        }

        /// <summary>
        /// Adds two vectors and returns the result as a vector.
        /// </summary>
        /// <param name="vector1">The first vector to add.</param>
        /// <param name="vector2">The second vector to add.</param>
        /// <returns>The sum of <paramref name="vector1" /> and <paramref name="vector2" />.</returns>
        [Pure]
        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X + vector2.X, vector1.Y + vector2.Y);
        }

        /// <summary>
        /// Subtracts one specified vector from another.
        /// </summary>
        /// <param name="vector1">The vector from which <paramref name="vector2" /> is subtracted.</param>
        /// <param name="vector2">The vector to subtract from <paramref name="vector1" />.</param>
        /// <returns>The difference between <paramref name="vector1" /> and <paramref name="vector2" />.</returns>
        [Pure]
        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y);
        }

        /// <summary>
        /// Multiplies the specified vector by the specified scalar and returns the resulting vector.
        /// </summary>
        /// <param name="vector">The vector to multiply.</param>
        /// <param name="scalar">The scalar to multiply.</param>
        /// <returns>The result of multiplying <paramref name="vector" /> and <paramref name="scalar" />.</returns>
        [Pure]
        public static Vector operator *(Vector vector, double scalar)
        {
            return new Vector(vector.X * scalar, vector.Y * scalar);
        }

        /// <summary>
        /// Multiplies the specified scalar by the specified vector and returns the resulting vector.
        /// </summary>
        /// <param name="scalar">The scalar to multiply.</param>
        /// <param name="vector">The vector to multiply.</param>
        /// <returns>The result of multiplying <paramref name="scalar" /> and <paramref name="vector" />.</returns>
        [Pure]
        public static Vector operator *(double scalar, Vector vector)
        {
            return new Vector(vector.X * scalar, vector.Y * scalar);
        }

        /// <summary>
        /// Divides the specified vector by the specified scalar and returns the resulting vector.
        /// </summary>
        /// <param name="vector">The vector to divide.</param>
        /// <param name="scalar">The scalar by which <paramref name="vector" /> will be divided.</param>
        /// <returns>The result of dividing <paramref name="vector" /> by <paramref name="scalar" />.</returns>
        [Pure]
        public static Vector operator /(Vector vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        /// <summary>
        /// Calculates the dot product of the two specified vector structures and returns the result as a <see cref="T:System.Double" />.
        /// </summary>
        /// <param name="vector1">The first vector to multiply.</param>
        /// <param name="vector2">The second vector to multiply.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Double" /> containing the scalar dot product of <paramref name="vector1" />
        /// and <paramref name="vector2" />, which is calculated using the following formula:
        ///   vector1.X * vector2.X + vector1.Y * vector2.Y
        /// </returns>
        [Pure]
        public static double operator *(Vector vector1, Vector vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }
    }
}