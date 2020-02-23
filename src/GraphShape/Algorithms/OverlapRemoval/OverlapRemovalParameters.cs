using System;
using GraphShape.Utils;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Base class for overlap removal algorithm parameters.
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

                if (Math.Abs(_verticalGap - value) > float.Epsilon)
                {
                    _verticalGap = value;
                    OnPropertyChanged();
                }
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

                if (Math.Abs(_horizontalGap - value) > float.Epsilon)
                {
                    _horizontalGap = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc />
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}