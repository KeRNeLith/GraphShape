using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Random layout algorithm parameters.
    /// </summary>
    public class RandomLayoutParameters : LayoutParametersBase
    {
        private double _xOffset;

        /// <summary>
        /// X offset for the bounding box.
        /// </summary>
        public double XOffset
        {
            get => _xOffset;
            set
            {
                if (NearEqual(_xOffset, value))
                    return;

                _xOffset = value;
                OnPropertyChanged();
            }
        }

        private double _yOffset;

        /// <summary>
        /// Y offset for the bounding box.
        /// </summary>
        public double YOffset
        {
            get => _yOffset;
            set
            {
                if (NearEqual(_yOffset, value))
                    return;

                _yOffset = value;
                OnPropertyChanged();
            }
        }

        private double _width = 100;

        /// <summary>
        /// Width of the bounding box.
        /// </summary>
        public double Width
        {
            get => _width;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Width)} must be positive or 0.");

                if (NearEqual(_width, value))
                    return;

                _width = value;
                OnPropertyChanged();
            }
        }

        private double _height = 100;

        /// <summary>
        /// Height of the bounding box.
        /// </summary>
        public double Height
        {
            get => _height;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Height)} must be positive or 0.");

                if (NearEqual(_height, value))
                    return;

                _height = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityElements()
        {
            foreach (object element in base.GetEqualityElements())
            {
                yield return element;
            }

            yield return _xOffset;
            yield return _yOffset;
            yield return _width;
            yield return _height;
        }
    }
}