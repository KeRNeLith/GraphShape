using System;
using GraphShape.Utils;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Overlap removal algorithm parameters.
    /// </summary>
    public class OverlapRemovalParameters : NotifierObject, IOverlapRemovalParameters
    {
        private float _verticalGap = 10;

        /// <inheritdoc />
        public float VerticalGap
        {
            get => _verticalGap;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(VerticalGap)} must be positive or 0.");

                if (NearEqual(_verticalGap, value))
                    return;

                _verticalGap = value;
                OnPropertyChanged();
            }
        }

        private float _horizontalGap = 10;

        /// <inheritdoc />
        public float HorizontalGap
        {
            get => _horizontalGap;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(HorizontalGap)} must be positive or 0.");

                if (NearEqual(_horizontalGap, value))
                    return;

                _horizontalGap = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}