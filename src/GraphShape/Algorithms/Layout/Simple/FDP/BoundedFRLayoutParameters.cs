using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Fruchterman-Reingold layout algorithm parameters (FDP), bounded version.
    /// </summary>
    public class BoundedFRLayoutParameters : FRLayoutParametersBase
    {
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
                UpdateParameters();
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
                UpdateParameters();
                OnPropertyChanged();
            }
        }

        private double _k;

        /// <summary>
        /// IdealEdgeLength = sqrt(height * width / vertexCount).
        /// </summary>
        public override double K => _k;

        private void UpdateK()
        {
            _k = Math.Sqrt(_width * _height / VertexCount);
            OnPropertyChanged(nameof(K));
        }

        private double _initialTemperature;

        [Pure]
        private double ComputeInitialTemperature()
        {
            return Math.Min(_width, _height) / 10;
        }

        private void UpdateInitialTemperature()
        {
            double newTemperature = ComputeInitialTemperature();
            if (NearEqual(_initialTemperature, newTemperature))
                return;

            _initialTemperature = newTemperature;
            OnPropertyChanged(nameof(InitialTemperature));
        }

        /// <inheritdoc />
        public override double InitialTemperature => _initialTemperature;

        /// <inheritdoc />
        protected override void UpdateParameters()
        {
            UpdateK();
            UpdateInitialTemperature();
            base.UpdateParameters();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedFRLayoutParameters"/> class.
        /// </summary>
        public BoundedFRLayoutParameters()
        {
            UpdateK();
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
        }
    }
}