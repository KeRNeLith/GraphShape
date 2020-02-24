﻿namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
	public class EfficientSugiyamaLayoutParameters : LayoutParametersBase
	{
        private LayoutDirection _direction = LayoutDirection.TopToBottom;
        private double _layerDistance = 15.0;
        private double _vertexDistance = 15.0;
        private int _positionMode = -1;
        private bool _optimizeWidth = false;
        private double _widthPerHeight = 1.0;
        private bool _minimizeEdgeLength = true;
        internal const int MaxPermutations = 50;
        private SugiyamaEdgeRoutings _edgeRouting = SugiyamaEdgeRoutings.Traditional;

        //it will be available on next releases
        public LayoutDirection Direction
        {
            get { return _direction; }
            set
            {
                if (value == _direction)
                    return;

                _direction = value;
                OnPropertyChanged();
            }
        }

        public double LayerDistance
        {
            get { return _layerDistance; }
            set
            {
                if (value == _layerDistance)
                    return;

                _layerDistance = value;
                OnPropertyChanged();
            }
        }

        public double VertexDistance
        {
            get { return _vertexDistance; }
            set
            {
                if (value == _vertexDistance)
                    return;

                _vertexDistance = value;
                OnPropertyChanged();
            }
        }

        public int PositionMode
        {
            get { return _positionMode; }
            set
            {
                if (value == _positionMode)
                    return;

                _positionMode = value;
                OnPropertyChanged();
            }
        }

        public double WidthPerHeight
        {
            get { return _widthPerHeight; }
            set
            {
                if (value == _widthPerHeight)
                    return;

                _widthPerHeight = value;
                OnPropertyChanged();
            }
        }

        public bool OptimizeWidth
        {
            get { return _optimizeWidth; }
            set
            {
                if (value == _optimizeWidth)
                    return;

                _optimizeWidth = value;
                OnPropertyChanged();
            }
        }

        public bool MinimizeEdgeLength
        {
            get { return _minimizeEdgeLength; }
            set
            {
                if (value == _minimizeEdgeLength)
                    return;

                _minimizeEdgeLength = value;
                OnPropertyChanged();
            }
        }

        public SugiyamaEdgeRoutings EdgeRouting
        {
            get { return _edgeRouting; }
            set
            {
                if (value == _edgeRouting)
                    return;

                _edgeRouting = value;
                OnPropertyChanged();
            }
        }
	}
}