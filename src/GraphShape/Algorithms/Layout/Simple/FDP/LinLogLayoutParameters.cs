using System;
using System.Collections.Generic;
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

        private int _maxIterations = 100;

        /// <summary>
        /// Maximum number of the iterations.
        /// </summary>
        public int MaxIterations
        {
            get => _maxIterations;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxIterations)} must be positive or 0.");

                if (_maxIterations == value)
                    return;

                _maxIterations = value;
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

            yield return _attractionExponent;
            yield return _repulsiveExponent;
            yield return _gravitationMultiplier;
            yield return _maxIterations;
        }
    }
}