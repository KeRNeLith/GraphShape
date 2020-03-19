using System;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.FDP
{
    /// <summary>
    /// LinLog layout algorithm parameters.
    /// </summary>
    public class LinLogLayoutParameters : LayoutParametersBase
    {
        private double _attractionExponent = 1.0;

        /// <summary>
        /// Attraction exponent.
        /// </summary>
        public double AttractionExponent
        {
            get => _attractionExponent;
            set
            {
                if (NearEqual(_attractionExponent, value))
                    return;

                _attractionExponent = value;
                OnPropertyChanged();
            }
        }

        private double _repulsiveExponent;

        /// <summary>
        /// Repulsive exponent.
        /// </summary>
        public double RepulsiveExponent
        {
            get => _repulsiveExponent;
            set
            {
                if (NearEqual(_repulsiveExponent, value))
                    return;

                _repulsiveExponent = value;
                OnPropertyChanged();
            }
        }

        private double _gravitationMultiplier = 0.1;

        /// <summary>
        /// Gravitation multiplier.
        /// </summary>
        public double GravitationMultiplier
        {
            get => _gravitationMultiplier;
            set
            {
                if (NearEqual(_gravitationMultiplier, value))
                    return;

                _gravitationMultiplier = value;
                OnPropertyChanged();
            }
        }

        private int _iterationCount = 100;

        /// <summary>
        /// Number of iteration to perform.
        /// </summary>
        public int IterationCount
        {
            get => _iterationCount;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(IterationCount)} must be positive or 0.");

                if (_iterationCount == value)
                    return;

                _iterationCount = value;
                OnPropertyChanged();
            }
        }
    }
}