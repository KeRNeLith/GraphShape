using System;
using GraphShape.Utils;

namespace GraphShape
{
    /// <summary>
    /// Represents the size of an object.
    /// </summary>
    [Serializable]
    public struct Size : IEquatable<Size>
    {
        /// <summary>
        /// Empty size.
        /// </summary>
        public static readonly Size Empty = new Size
        {
            _width = double.NegativeInfinity,
            _height = double.NegativeInfinity
        };

        // ReSharper disable InconsistentNaming
        internal double _width;
        internal double _height;
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Width.
        /// </summary>
        public double Width
        {
            get => _width;
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify an empty size.");
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Width)} must be positive or 0.");
                _width = value;
            }
        }

        /// <summary>
        /// Height
        /// </summary>
        public double Height
        {
            get => _height;
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify an empty size.");
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Height)} must be positive or 0.");
                _height = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public Size(double width, double height)
        {
            if (width < 0.0 || height < 0.0)
                throw new ArgumentException("Width and height must be positive or 0.");
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Indicates if this size is empty.
        /// </summary>
        public bool IsEmpty => _width < 0.0;

        /// <summary>
        /// Indicates whether both <see cref="Size"/> are equal.
        /// </summary>
        /// <param name="size1">First <see cref="Size"/> to compare.</param>
        /// <param name="size2">Second <see cref="Size"/> to compare.</param>
        /// <returns>True if both <see cref="Size"/> are equal, otherwise false.</returns>
        public static bool operator ==(Size size1, Size size2)
        {
            return Equals(size1, size2);
        }

        /// <summary>
        /// Indicates whether both <see cref="Size"/> are not equal.
        /// </summary>
        /// <param name="size1">First <see cref="Size"/> to compare.</param>
        /// <param name="size2">Second <see cref="Size"/> to compare.</param>
        /// <returns>True if both <see cref="Size"/> are equal, otherwise false.</returns>
        public static bool operator !=(Size size1, Size size2)
        {
            return !(size1 == size2);
        }

        /// <summary>
        /// Compares the two specified <see cref="Vector"/>s for equality.
        /// </summary>
        /// <param name="size1">The first <see cref="Vector"/> to compare.</param>
        /// <param name="size2">The second <see cref="Vector"/> to compare.</param>
        /// <returns>True if both <see cref="Vector"/> are equal, otherwise false.</returns>
        public static bool Equals(Size size1, Size size2)
        {
            if (size1.IsEmpty)
                return size2.IsEmpty;
            return MathUtils.NearEqual(size1._width, size2._width)
                   && MathUtils.NearEqual(size1._height, size2._height);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Size size)
                return Equals(this, size);
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Size other)
        {
            return Equals(this, other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return IsEmpty ? 0 : Width.GetHashCode() ^ Height.GetHashCode();
        }
    }
}
