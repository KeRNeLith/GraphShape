using System;
using System.Collections.Generic;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Compound.FDP
{
    /// <summary>
    /// Compound FDP layout algorithm parameters.
    /// </summary>
    public class CompoundFDPLayoutParameters : LayoutParametersBase
    {
        private double _idealEdgeLength = 25;

        /// <summary>
        /// Ideal edge length.
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
                OnPropertyChanged();
            }
        }

        private double _elasticConstant = 0.005;

        /// <summary>
        /// Elastic constant for the edges.
        /// </summary>
        public double ElasticConstant
        {
            get => _elasticConstant;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ElasticConstant)} must be positive or 0.");

                if (NearEqual(_elasticConstant, value))
                    return;

                _elasticConstant = value;
                OnPropertyChanged();
            }
        }

        private double _repulsionConstant = 150;

        /// <summary>
        /// Repulsion constant for the node-node repulsion.
        /// </summary>
        public double RepulsionConstant
        {
            get => _repulsionConstant;
            set
            {
                if (NearEqual(_repulsionConstant, value))
                    return;

                _repulsionConstant = value;
                OnPropertyChanged();
            }
        }

        private double _nestingFactor = 0.2;

        /// <summary>
        /// Factor of the ideal edge length for the inter-graph edges.
        /// </summary>
        public double NestingFactor
        {
            get => _nestingFactor;
            set
            {
                if (NearEqual(_nestingFactor, value))
                    return;

                _nestingFactor = value;
                OnPropertyChanged();
            }
        }

        private double _gravitationFactor = 8;

        /// <summary>
        /// Factor of the gravitation.
        /// </summary>
        public double GravitationFactor
        {
            get => _gravitationFactor;
            set
            {
                if (NearEqual(_gravitationFactor, value))
                    return;

                _gravitationFactor = value;
                OnPropertyChanged();
            }
        }


        private int _phase1Iterations = 50;

        /// <summary>
        /// Maximum iterations for phase 1.
        /// </summary>
        public int Phase1Iterations
        {
            get => _phase1Iterations;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Phase1Iterations)} must be positive or 0.");

                if (_phase1Iterations == value)
                    return;

                _phase1Iterations = value;
                OnPropertyChanged();
            }
        }

        private int _phase2Iterations = 70;

        /// <summary>
        /// Maximum iterations for phase 2.
        /// </summary>
        public int Phase2Iterations
        {
            get => _phase2Iterations;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Phase2Iterations)} must be positive or 0.");

                if (_phase2Iterations == value)
                    return;

                _phase2Iterations = value;
                OnPropertyChanged();
            }
        }

        private int _phase3Iterations = 30;

        /// <summary>
        /// Maximum iterations for phase 3.
        /// </summary>
        public int Phase3Iterations
        {
            get => _phase3Iterations;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Phase3Iterations)} must be positive or 0.");

                if (_phase3Iterations == value)
                    return;

                _phase3Iterations = value;
                OnPropertyChanged();
            }
        }


        private double _phase2TemperatureInitialMultiplier = 0.5;

        /// <summary>
        /// Initial multiplier for phase 2 temperature.
        /// </summary>
        public double Phase2TemperatureInitialMultiplier
        {
            get => _phase2TemperatureInitialMultiplier;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Phase2TemperatureInitialMultiplier)} must be positive or 0.");

                if (NearEqual(_phase2TemperatureInitialMultiplier, value))
                    return;

                _phase2TemperatureInitialMultiplier = value;
                OnPropertyChanged();
            }
        }

        private double _phase3TemperatureInitialMultiplier = 0.2;

        /// <summary>
        /// Initial multiplier for phase 3 temperature.
        /// </summary>
        public double Phase3TemperatureInitialMultiplier
        {
            get => _phase3TemperatureInitialMultiplier;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Phase3TemperatureInitialMultiplier)} must be positive or 0.");

                if (NearEqual(_phase3TemperatureInitialMultiplier, value))
                    return;

                _phase3TemperatureInitialMultiplier = value;
                OnPropertyChanged();
            }
        }


        private double _temperatureDecreasing = 0.5;

        /// <summary>
        /// Temperature decreasing factor.
        /// </summary>
        public double TemperatureDecreasing
        {
            get => _temperatureDecreasing;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(TemperatureDecreasing)} must be positive or 0.");

                if (NearEqual(_temperatureDecreasing, value))
                    return;

                _temperatureDecreasing = value;
                OnPropertyChanged();
            }
        }

        private double _displacementLimitMultiplier = 0.5;

        /// <summary>
        /// Displacement limit multiplier.
        /// </summary>
        public double DisplacementLimitMultiplier
        {
            get => _displacementLimitMultiplier;
            set
            {
                if (NearEqual(_displacementLimitMultiplier, value))
                    return;

                _displacementLimitMultiplier = value;
                OnPropertyChanged();
            }
        }

        private double _separationMultiplier = 15;

        /// <summary>
        /// Separation multiplier.
        /// </summary>
        public double SeparationMultiplier
        {
            get => _separationMultiplier;
            set
            {
                if (NearEqual(_separationMultiplier, value))
                    return;

                _separationMultiplier = value;
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
            yield return _elasticConstant;
            yield return _repulsionConstant;
            yield return _nestingFactor;
            yield return _gravitationFactor;
            yield return _phase1Iterations;
            yield return _phase2Iterations;
            yield return _phase3Iterations;
            yield return _phase2TemperatureInitialMultiplier;
            yield return _phase3TemperatureInitialMultiplier;
            yield return _temperatureDecreasing;
            yield return _displacementLimitMultiplier;
            yield return _separationMultiplier;
        }
    }
}
