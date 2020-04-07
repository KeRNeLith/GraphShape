using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.FDP
{
    /// <summary>
    /// Kamada-Kawai layout algorithm parameters.
    /// </summary>
    public class KKLayoutParameters : LayoutParametersBase
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

        private int _maxIterations = 200;

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

        private double _k = 1;

        /// <summary>
        /// Vertex attraction constant (can be the number of vertices in the treated graph).
        /// </summary>
        public double K
        {
            get => _k;
            set
            {
                if (NearEqual(_k, value))
                    return;

                _k = value;
                OnPropertyChanged();
            }
        }

        private bool _exchangeVertices;

        /// <summary>
        /// Indicates that vertices can be exchanged to improve algorithm result.
        /// </summary>
        public bool ExchangeVertices
        {
            get => _exchangeVertices;
            set
            {
                if (_exchangeVertices == value)
                    return;

                _exchangeVertices = value;
                OnPropertyChanged();
            }
        }

        private double _lengthFactor = 1;

        /// <summary>
        /// Multiplier of the ideal edge length. (With this parameter the user can modify the ideal edge length).
        /// </summary>
        public double LengthFactor
        {
            get => _lengthFactor;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(LengthFactor)} must be positive or 0.");

                if (NearEqual(_lengthFactor, value))
                    return;

                _lengthFactor = value;
                OnPropertyChanged();
            }
        }

        private double _disconnectedMultiplier = 0.5;

        /// <summary>
        /// Ideal distance between the disconnected points (1 is equal the ideal edge length).
        /// </summary>
        public double DisconnectedMultiplier
        {
            get => _disconnectedMultiplier;
            set
            {
                if (NearEqual(_disconnectedMultiplier, value))
                    return;

                _disconnectedMultiplier = value;
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
            yield return _maxIterations;
            yield return _k;
            yield return _exchangeVertices;
            yield return _lengthFactor;
            yield return _disconnectedMultiplier;
        }
    }
}