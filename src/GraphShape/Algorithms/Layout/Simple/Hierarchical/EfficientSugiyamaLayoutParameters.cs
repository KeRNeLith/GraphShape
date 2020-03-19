using System;
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

        private double _layerDistance = 15.0;

        /// <summary>
        /// Distance between layers.
        /// </summary>
        public double LayerDistance
        {
            get => _layerDistance;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(LayerDistance)} must be positive or 0.");

                if (NearEqual(_layerDistance, value))
                    return;

                _layerDistance = value;
                OnPropertyChanged();
            }
        }

        private double _vertexDistance = 15.0;

        /// <summary>
        /// Distance between vertices.
        /// </summary>
        public double VertexDistance
        {
            get => _vertexDistance;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(VertexDistance)} must be positive or 0.");

                if (NearEqual(_vertexDistance, value))
                    return;

                _vertexDistance = value;
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
    }
}