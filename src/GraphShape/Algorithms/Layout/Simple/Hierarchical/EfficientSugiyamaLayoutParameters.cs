using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    /// <summary>
    /// Efficient Sugiyama layout algorithm parameters.
    /// </summary>
    public class EfficientSugiyamaLayoutParameters : LayoutParametersBase
    {
        internal const int MaxPermutations = 50;

        private LayoutDirection _direction = LayoutDirection.TopToBottom;

        /// <summary>
        /// Layout direction (orientation)
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

        private double _layerGap = 15.0;

        /// <summary>
        /// Distance between layers.
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

        private double _sliceGap = 15.0;

        /// <summary>
        /// Distance between slices.
        /// </summary>
        public double SliceGap
        {
            get => _sliceGap;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SliceGap)} must be positive or 0.");

                if (NearEqual(_sliceGap, value))
                    return;

                _sliceGap = value;
                OnPropertyChanged();
            }
        }

        private int _positionMode = -1;

        /// <summary>
        /// Position mode (can be negative or in [0, 3]).
        /// </summary>
        public int PositionMode
        {
            get => _positionMode;
            set
            {
                if (value > 3)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(PositionMode)} must be in [0, 3] or negative.");

                if (_positionMode == value)
                    return;

                _positionMode = value;
                OnPropertyChanged();
            }
        }

        private bool _optimizeWidth;

        /// <summary>
        /// Indicates if a width optimization should be performed.
        /// </summary>
        public bool OptimizeWidth
        {
            get => _optimizeWidth;
            set
            {
                if (_optimizeWidth == value)
                    return;

                _optimizeWidth = value;
                OnPropertyChanged();
            }
        }

        private double _widthPerHeight = 1.0;

        /// <summary>
        /// Width per height ratio.
        /// </summary>
        public double WidthPerHeight
        {
            get => _widthPerHeight;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(WidthPerHeight)} must be positive or 0.");

                if (NearEqual(_widthPerHeight, value))
                    return;

                _widthPerHeight = value;
                OnPropertyChanged();
            }
        }

        private bool _minimizeEdgeLength = true;

        /// <summary>
        /// Indicates if edge length should be minimized or not.
        /// </summary>
        public bool MinimizeEdgeLength
        {
            get => _minimizeEdgeLength;
            set
            {
                if (_minimizeEdgeLength == value)
                    return;

                _minimizeEdgeLength = value;
                OnPropertyChanged();
            }
        }

        private SugiyamaEdgeRouting _edgeRouting = SugiyamaEdgeRouting.Traditional;

        /// <summary>
        /// Edge routing method.
        /// </summary>
        public SugiyamaEdgeRouting EdgeRouting
        {
            get => _edgeRouting;
            set
            {
                if (_edgeRouting == value)
                    return;

                _edgeRouting = value;
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
            yield return _layerGap;
            yield return _sliceGap;
            yield return _positionMode;
            yield return _optimizeWidth;
            yield return _widthPerHeight;
            yield return _minimizeEdgeLength;
            yield return _edgeRouting;
        }
    }
}