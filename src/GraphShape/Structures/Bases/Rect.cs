using System;
using System.Runtime.Serialization;
using GraphShape.Utils;
using JetBrains.Annotations;

namespace GraphShape
{
    /// <summary>
    /// Represents a rectangle.
    /// </summary>
    [Serializable]
    public struct Rect : IEquatable<Rect>, IDeserializationCallback
    {
        /// <summary>
        /// Empty rectangle.
        /// </summary>
        public static readonly Rect Empty = new Rect
        {
            _x = double.PositiveInfinity,
            _y = double.PositiveInfinity,
            _width = double.NegativeInfinity,
            _height = double.NegativeInfinity
        };
        
        // ReSharper disable InconsistentNaming
        internal double _x;
        internal double _y;
        internal double _width;
        internal double _height;
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// X-axis value of the left side of the rectangle.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If trying to update an empty <see cref="Rect"/>.</exception>
        public double X
        {
            get => _x;
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify an empty rectangle.");
                _x = value;
            }
        }

        /// <summary>
        /// Y-axis value of the top side of the rectangle.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If trying to update an empty <see cref="Rect"/>.</exception>
        public double Y
        {
            get => _y;
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify an empty rectangle.");
                _y = value;
            }
        }

        /// <summary>
        /// Width of the rectangle.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If trying to update an empty <see cref="Rect"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Value is negative.</exception>
        public double Width
        {
            get => _width;
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify an empty rectangle.");
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Width)} must be positive or 0.");
                _width = value;
            }
        }

        /// <summary>
        /// Height of the rectangle.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If trying to update an empty <see cref="Rect"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Value is negative.</exception>
        public double Height
        {
            get => _height;
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify an empty rectangle.");
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Height)} must be positive or 0.");
                _height = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">A point that specifies the location of the top-left corner of the rectangle.</param>
        /// <param name="size">A <see cref="Size" /> structure that specifies the width and height of the rectangle.</param>
        public Rect(Point location, Size size)
        {
            if (size.IsEmpty)
            {
                this = Empty;
            }
            else
            {
                _x = location.X;
                _y = location.Y;
                _width = size._width;
                _height = size._height;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">The x-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="width"/> or <paramref name="height"/> is negative.</exception>
        public Rect(double x, double y, double width, double height)
        {
            if (width < 0.0 || height < 0.0)
                throw new ArgumentException("Width and height must be positive or 0.");
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Indicates if this rectangle is empty.
        /// </summary>
        public bool IsEmpty => _width < 0.0;

        /// <summary>
        /// Indicates whether both <see cref="Rect"/> are equal.
        /// </summary>
        /// <param name="rect1">First <see cref="Rect"/> to compare.</param>
        /// <param name="rect2">Second <see cref="Rect"/> to compare.</param>
        /// <returns>True if both <see cref="Rect"/> are equal, otherwise false.</returns>
        public static bool operator ==(Rect rect1, Rect rect2)
        {
            return Equals(rect1, rect2);
        }

        /// <summary>
        /// Indicates whether both <see cref="Rect"/> are not equal.
        /// </summary>
        /// <param name="rect1">First <see cref="Rect"/> to compare.</param>
        /// <param name="rect2">Second <see cref="Rect"/> to compare.</param>
        /// <returns>True if both <see cref="Rect"/> are equal, otherwise false.</returns>
        public static bool operator !=(Rect rect1, Rect rect2)
        {
            return !(rect1 == rect2);
        }

        /// <summary>
        /// Compares the two specified <see cref="Rect"/>s for equality.
        /// </summary>
        /// <param name="rect1">The first <see cref="Rect"/> to compare.</param>
        /// <param name="rect2">The second <see cref="Rect"/> to compare.</param>
        /// <returns>True if both <see cref="Rect"/> are equal, otherwise false.</returns>
        public static bool Equals(Rect rect1, Rect rect2)
        {
            if (rect1.IsEmpty)
                return rect2.IsEmpty;
            return MathUtils.NearEqual(rect1._x, rect2._x)
                   && MathUtils.NearEqual(rect1._y, rect2._y)
                   && MathUtils.NearEqual(rect1._width, rect2._width)
                   && MathUtils.NearEqual(rect1._height, rect2._height);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Rect rect)
                return Equals(this, rect);
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Rect other)
        {
            return Equals(this, other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return IsEmpty
                ? 0 :
                X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsEmpty ? "Empty" : $"{_x};{_y};{_width};{_height}";
        }

        /// <summary>
        /// The position of the top-left corner of the rectangle.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If trying to update an empty <see cref="Rect"/>.</exception>
        public Point Location
        {
            get => new Point(_x, _y);
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify an empty rectangle.");
                _x = value.X;
                _y = value.Y;
            }
        }

        /// <summary>
        /// The width and height of the rectangle.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If trying to update an empty <see cref="Rect"/>.</exception>
        public Size Size
        {
            get => IsEmpty ? Size.Empty : new Size(_width, _height);
            set
            {
                if (value.IsEmpty)
                {
                    this = Empty;
                }
                else
                {
                    if (IsEmpty)
                        throw new InvalidOperationException("Cannot modify an empty rectangle.");
                    _width = value._width;
                    _height = value._height;
                }
            }
        }

        /// <summary>
        /// Gets the x-axis value of the left side of the rectangle.
        /// </summary>
        public double Left => _x;

        /// <summary>
        /// Gets the y-axis position of the top of the rectangle.
        /// </summary>
        public double Top => _y;

        /// <summary>
        /// Gets the x-axis value of the right side of the rectangle.
        /// </summary>
        public double Right => IsEmpty ? double.NegativeInfinity : _x + _width;

        /// <summary>
        /// Gets the y-axis value of the bottom of the rectangle.
        /// </summary>
        public double Bottom => IsEmpty ? double.NegativeInfinity : _y + _height;

        /// <summary>
        /// Gets the position of the top-left corner of the rectangle.
        /// </summary>
        public Point TopLeft => new Point(Left, Top);

        /// <summary>
        /// Gets the position of the top-right corner of the rectangle.
        /// </summary>
        public Point TopRight => new Point(Right, Top);

        /// <summary>
        /// Gets the position of the bottom-left corner of the rectangle.
        /// </summary>
        public Point BottomLeft => new Point(Left, Bottom);

        /// <summary>
        /// Gets the position of the bottom-right corner of the rectangle.
        /// </summary>
        public Point BottomRight => new Point(Right, Bottom);

        /// <summary>
        /// Indicates whether the specified rectangle intersects with the current rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to check.</param>
        /// <returns>True if the specified rectangle intersects with the current rectangle, false otherwise.</returns>
        [Pure]
        public bool IntersectsWith(Rect rect)
        {
            return !IsEmpty
                   && !rect.IsEmpty
                   && rect.Left <= Right && rect.Right >= Left
                   && rect.Top <= Bottom && rect.Bottom >= Top;
        }

        /// <summary>
        /// Intersects this rectangle with <paramref name="rect"/> and updates to be the intersection
        /// of this and <paramref name="rect"/>. If either this or <paramref name="rect"/> are <see cref="Empty"/>,
        /// the result is <see cref="Empty"/> as well.
        /// </summary>
        /// <param name="rect">The rect to intersect with this.</param>
        public void Intersect(Rect rect)
        {
            if (!IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                double left = Math.Max(Left, rect.Left);
                double top = Math.Max(Top, rect.Top);

                //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                _width = Math.Max(Math.Min(Right, rect.Right) - left, 0);
                _height = Math.Max(Math.Min(Bottom, rect.Bottom) - top, 0);

                _x = left;
                _y = top;
            }
        }

        /// <summary>
        /// Moves the rectangle by the specified horizontal and vertical amounts.
        /// </summary>
        /// <param name="offsetX">The amount to move the rectangle horizontally.</param>
        /// <param name="offsetY">The amount to move the rectangle vertically.</param>
        /// <exception cref="T:System.InvalidOperationException">If trying to update an empty <see cref="Rect"/>.</exception>
        public void Offset(double offsetX, double offsetY)
        {
            if (IsEmpty)
                throw new InvalidOperationException("Cannot modify an empty rectangle.");
            _x += offsetX;
            _y += offsetY;
        }

        #region IDeserializationCallback

        /// <inheritdoc />
        void IDeserializationCallback.OnDeserialization(object sender)
        {
            if (_width < 0.0 || _height < 0.0)
                throw new ArgumentException("Width and height must be positive or 0.");
        }

        #endregion
    }
}