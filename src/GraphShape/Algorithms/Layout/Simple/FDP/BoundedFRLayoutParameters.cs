using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.FDP
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

        private void CalculateK()
        {
            _k = Math.Sqrt(_width * Height / VertexCount);
            OnPropertyChanged(nameof(K));
        }

        /// <inheritdoc />
        public override double InitialTemperature => Math.Min(Width, Height) / 10;

        /// <inheritdoc />
        protected override void UpdateParameters()
        {
            CalculateK();
            base.UpdateParameters();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedFRLayoutParameters"/> class.
        /// </summary>
        public BoundedFRLayoutParameters()
        {
            CalculateK();
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