using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.FDP
{
    /// <summary>
    /// Fruchterman-Reingold layout algorithm parameters (FDP).
    /// </summary>
    public class FreeFRLayoutParameters : FRLayoutParametersBase
    {
        /// <inheritdoc />
        public override double K => _idealEdgeLength;

        /// <inheritdoc />
        public override double InitialTemperature => Math.Sqrt(Math.Pow(_idealEdgeLength, 2) * VertexCount);

        private double _idealEdgeLength = 10;

        /// <summary>
        /// Represents the ideal length of the edges.
        /// </summary>
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

            yield return _idealEdgeLength;
        }
    }
}
