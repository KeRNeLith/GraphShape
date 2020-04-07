using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    /// <summary>
    /// Sugiyama layout algorithm parameters.
    /// </summary>
    public class SugiyamaLayoutParameters : LayoutParametersBase
    {
        private float _verticalGap = 10;

        /// <summary>
        /// Minimal vertical gap between the vertices.
        /// </summary>
        public float VerticalGap
        {
            get => _verticalGap;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(VerticalGap)} must be positive or 0.");

                if (NearEqual(_verticalGap, value))
                    return;

                _verticalGap = value;
                OnPropertyChanged();
            }
        }

        private float _horizontalGap = 10;

        /// <summary>
        /// Minimal horizontal gap between the vertices.
        /// </summary>
        public float HorizontalGap
        {
            get => _horizontalGap;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(HorizontalGap)} must be positive or 0.");

                if (NearEqual(_horizontalGap, value))
                    return;

                _horizontalGap = value;
                OnPropertyChanged();
            }
        }

        private bool _dirty = true;

        /// <summary>
        /// Starts with a dirty round (allow to increase the number of the edge-crossings, but 
        /// try to put the vertices to it's barycenter).
        /// </summary>
        public bool DirtyRound
        {
            get => _dirty;
            set
            {
                if (_dirty == value)
                    return;

                _dirty = value;
                OnPropertyChanged();
            }
        }
        
        private int _phase1IterationCount = 8;

        /// <summary>
        /// Maximum iteration count in the Phase 1 of the Sugiyama algorithm.
        /// </summary>
        public int Phase1IterationCount
        {
            get => _phase1IterationCount;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Phase1IterationCount)} must be positive or 0.");

                if (_phase1IterationCount == value)
                    return;

                _phase1IterationCount = value;
                OnPropertyChanged();
            }
        }

        private int _phase2IterationCount = 5;

        /// <summary>
        /// Maximum iteration count in the Phase 2 of the Sugiyama algorithm.
        /// </summary>
        public int Phase2IterationCount
        {
            get => _phase2IterationCount;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Phase2IterationCount)} must be positive or 0.");

                if (_phase2IterationCount == value)
                    return;

                _phase2IterationCount = value;
                OnPropertyChanged();
            }
        }

        private bool _minimizeHierarchicalEdgeLong = true;

        /// <summary>
        /// Indicates if algorithm should try to minimize hierarchical edge length.
        /// </summary>
        public bool MinimizeHierarchicalEdgeLong
        {
            get => _minimizeHierarchicalEdgeLong;
            set
            {
                if (_minimizeHierarchicalEdgeLong == value)
                    return;

                _minimizeHierarchicalEdgeLong = value;
                OnPropertyChanged();
            }
        }

        private PositionCalculationMethodTypes _positionCalculationMethod = PositionCalculationMethodTypes.PositionBased;

        /// <summary>
        /// Layout calculation method.
        /// </summary>
        public PositionCalculationMethodTypes PositionCalculationMethod
        {
            get => _positionCalculationMethod;
            set
            {
                if (_positionCalculationMethod == value)
                    return;

                _positionCalculationMethod = value;
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

            yield return _verticalGap;
            yield return _horizontalGap;
            yield return _dirty;
            yield return _phase1IterationCount;
            yield return _phase2IterationCount;
            yield return _minimizeHierarchicalEdgeLong;
            yield return _positionCalculationMethod;
        }
    }
}