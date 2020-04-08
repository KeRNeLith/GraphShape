using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.FDP
{
    /// <summary>
    /// Inverted Self-Organizing Map (ISOM) layout algorithm parameters.
    /// </summary>
    public class ISOMLayoutParameters : LayoutParametersBase
    {
        private double _width = 300;

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

        private double _height = 300;

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

        private int _maxEpoch = 2000;

        /// <summary>
        /// Maximum number of iteration.
        /// </summary>
        public int MaxEpoch
        {
            get => _maxEpoch;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxEpoch)} must be positive or 0.");

                if (_maxEpoch == value)
                    return;

                _maxEpoch = value;
                OnPropertyChanged();
            }
        }

        private int _radiusConstantTime = 100;

        /// <summary>
        /// Radius constant time.
        /// </summary>
        public int RadiusConstantTime
        {
            get => _radiusConstantTime;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(RadiusConstantTime)} must be positive or 0.");

                if (_radiusConstantTime == value)
                    return;

                _radiusConstantTime = value;
                OnPropertyChanged();
            }
        }

        private int _initialRadius = 5;

        /// <summary>
        /// Initial radius.
        /// </summary>
        public int InitialRadius
        {
            get => _initialRadius;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(InitialRadius)} must be positive or 0.");

                if (_initialRadius == value)
                    return;

                _initialRadius = value;
                OnPropertyChanged();
            }
        }

        private int _minRadius = 1;

        /// <summary>
        /// Minimal radius.
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

        private double _initialAdaptation = 0.9;

        /// <summary>
        /// Initial adaption.
        /// </summary>
        public double InitialAdaptation
        {
            get => _initialAdaptation;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(InitialAdaptation)} must be positive or 0.");

                if (NearEqual(_initialAdaptation, value))
                    return;

                _initialAdaptation = value;
                OnPropertyChanged();
            }
        }

        private double _minAdaptation;

        /// <summary>
        /// Minimal adaption.
        /// </summary>
        public double MinAdaptation
        {
            get => _minAdaptation;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MinAdaptation)} must be positive or 0.");

                if (NearEqual(_minAdaptation, value))
                    return;

                _minAdaptation = value;
                OnPropertyChanged();
            }
        }

        private double _coolingFactor = 2;

        /// <summary>
        /// Cooling factor.
        /// </summary>
        public double CoolingFactor
        {
            get => _coolingFactor;
            set
            {
                if (NearEqual(_coolingFactor, value))
                    return;

                _coolingFactor = value;
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

            yield return _width;
            yield return _height;
            yield return _maxEpoch;
            yield return _radiusConstantTime;
            yield return _initialRadius;
            yield return _minRadius;
            yield return _initialAdaptation;
            yield return _minAdaptation;
            yield return _coolingFactor;
        }
    }
}