using System;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout.Simple.FDP
{
    /// <summary>
    /// Base class for Fruchterman-Reingold algorithm parameters (FDP).
    /// </summary>
    public abstract class FRLayoutParametersBase : LayoutParametersBase
    {
        private int _vertexCount;

        /// <summary>
        /// Count of the vertices (used to calculate the constants)
        /// </summary>
        internal int VertexCount
        {
            get => _vertexCount;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(VertexCount)} must be positive or 0.");

                if (_vertexCount == value)
                    return;

                _vertexCount = value;
                UpdateParameters();
                OnPropertyChanged();
            }
        }

        private void CalculateConstants()
        {
            CalculateConstantOfRepulsion();
            CalculateConstantOfAttraction();
        }

        /// <summary>
        /// Updates computed parameters based on other parameters.
        /// </summary>
        protected virtual void UpdateParameters()
        {
            CalculateConstants();
        }

        private void CalculateConstantOfRepulsion()
        {
            ConstantOfRepulsion = Math.Pow(K * _repulsiveMultiplier, 2);
            OnPropertyChanged(nameof(ConstantOfRepulsion));
        }

        private void CalculateConstantOfAttraction()
        {
            ConstantOfAttraction = K * _attractionMultiplier;
            OnPropertyChanged(nameof(ConstantOfAttraction));
        }

        /// <summary>
        /// Gets the computed ideal edge length.
        /// </summary>
        public abstract double K { get; }

        /// <summary>
        /// Gets the initial temperature of the mass.
        /// </summary>
        public abstract double InitialTemperature { get; }

        /// <summary>
        /// Constant <see cref="K"/> * <see cref="AttractionMultiplier"/>.
        /// </summary>
        public double ConstantOfAttraction { get; private set; }

        private  double _attractionMultiplier = 1.2;

        /// <summary>
        /// Multiplier of the attraction.
        /// </summary>
        public double AttractionMultiplier
        {
            get => _attractionMultiplier;
            set
            {
                if (NearEqual(_attractionMultiplier, value))
                    return;

                _attractionMultiplier = value;
                CalculateConstantOfAttraction();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Constant (<see cref="K"/> * <see cref="RepulsiveMultiplier"/>².
        /// </summary>
        public double ConstantOfRepulsion { get; private set; }

        private double _repulsiveMultiplier = 0.6;

        /// <summary>
        /// Multiplier of the repulsion.
        /// </summary>
        public double RepulsiveMultiplier
        {
            get => _repulsiveMultiplier;
            set
            {
                if (NearEqual(_repulsiveMultiplier, value))
                    return;

                _repulsiveMultiplier = value;
                CalculateConstantOfRepulsion();
                OnPropertyChanged();
            }
        }

        private int _iterationLimit = 200;

        /// <summary>
        /// Limit of the iterations.
        /// </summary>
        public int IterationLimit
        {
            get => _iterationLimit;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(IterationLimit)} must be positive or 0.");

                if (_iterationLimit == value)
                    return;

                _iterationLimit = value;
                OnPropertyChanged();
            }
        }

        private double _lambda = 0.95;

        /// <summary>
        /// Lambda for the cooling.
        /// </summary>
        public double Lambda
        {
            get => _lambda;
            set
            {
                if (NearEqual(_lambda, value))
                    return;

                _lambda = value;
                OnPropertyChanged();
            }
        }

        private FRCoolingFunction _coolingFunction = FRCoolingFunction.Exponential;

        /// <summary>
        /// Cooling function.
        /// </summary>
        public FRCoolingFunction CoolingFunction
        {
            get => _coolingFunction;
            set
            {
                if (_coolingFunction == value)
                    return;

                _coolingFunction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FRLayoutParametersBase"/> class.
        /// </summary>
        protected FRLayoutParametersBase()
        {
            CalculateConstants();
        }
    }
}
