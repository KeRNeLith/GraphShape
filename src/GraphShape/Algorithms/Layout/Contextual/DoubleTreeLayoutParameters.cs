using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Contextual
{
    /// <summary>
    /// Double tree layout algorithm parameters.
    /// </summary>
    public class DoubleTreeLayoutParameters : LayoutParametersBase
    {
        private LayoutDirection _direction = LayoutDirection.LeftToRight;
        
        /// <summary>
        /// The layout direction.
        /// </summary>
        public LayoutDirection Direction
        {
            get => _direction;
            set
            {
                if (_direction == value)
                    return;

                _direction = value;
                OnPropertyChanged();
            }
        }

        private double _vertexGap = 10;

        /// <summary>
        /// Minimum gap between the neighbor vertices in a layer.
        /// </summary>
        public double VertexGap
        {
            get => _vertexGap;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(VertexGap)} must be positive or 0.");

                if (NearEqual(_vertexGap, value))
                    return;

                _vertexGap = value;
                OnPropertyChanged();
            }
        }

        private double _layerGap = 10;

        /// <summary>
        /// Minimum gap between layers.
        /// </summary>
        public double LayerGap
        {
            get => _layerGap;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(LayerGap)} must be positive or 0.");

                if (NearEqual(_layerGap, value))
                    return;
                
                _layerGap = value;
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

            yield return _direction;
            yield return _vertexGap;
            yield return _layerGap;
        }
    }
}