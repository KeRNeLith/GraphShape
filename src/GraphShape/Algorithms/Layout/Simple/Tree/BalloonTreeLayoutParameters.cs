using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Balloon Tree layout algorithm parameters.
    /// </summary>
    public class BalloonTreeLayoutParameters : LayoutParametersBase
    {
        private int _minRadius = 2;

        /// <summary>
        /// Minimum radius.
        /// </summary>
        public int MinRadius
        {
            get => _minRadius;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MinRadius)} must be positive or 0.");

                if (_minRadius == value)
                    return;

                _minRadius = value;
                OnPropertyChanged();
            }
        }

        private float _border = 20.0f;

        /// <summary>
        /// Border.
        /// </summary>
        public float Border
        {
            get => _border;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Border)} must be positive or 0.");

                if (NearEqual(_border, value))
                    return;

                _border = value;
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

            yield return _minRadius;
            yield return _border;
        }
    }
}