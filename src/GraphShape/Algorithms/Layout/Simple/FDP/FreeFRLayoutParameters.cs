using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Fruchterman-Reingold layout algorithm parameters (FDP).
    /// </summary>
    public class FreeFRLayoutParameters : FRLayoutParametersBase
    {
        /// <inheritdoc />
        public override double K => _idealEdgeLength;

        private double _initialTemperature;

        [Pure]
        private double ComputeInitialTemperature()
        {
            return Math.Sqrt(Math.Pow(_idealEdgeLength, 2) * VertexCount);
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

        private double _idealEdgeLength = 10;

        /// <summary>
        /// Represents the ideal length of the edges.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Value is negative.</exception>
        public double IdealEdgeLength
        {
            get => _idealEdgeLength;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(IdealEdgeLength)} must be positive or 0.");

                if (NearEqual(_idealEdgeLength, value))
                    return;

                _idealEdgeLength = value;
                UpdateParameters();
                OnPropertyChanged(nameof(K));
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        protected override void UpdateParameters()
        {
            UpdateInitialTemperature();
            base.UpdateParameters();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeFRLayoutParameters"/> class.
        /// </summary>
        public FreeFRLayoutParameters()
        {
            _initialTemperature = ComputeInitialTemperature();
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityElements()
        {
            foreach (object element in base.GetEqualityElements())
            {
                yield return element;
            }

            yield return _idealEdgeLength;
        }
    }
}