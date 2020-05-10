using System;

namespace GraphShape.Optimization.GeneticAlgorithm
{
    /// <summary>
    /// Genetic algorithm parameters.
    /// </summary>
    internal class GeneticAlgorithmParameters
    {
        private int _generations;

        public int Generations
        {
            get => _generations;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Generations)} must be positive or 0.");
                _generations = value;
            }
        }

        private int _populationSize;
        
        public int PopulationSize
        {
            get => _populationSize;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(PopulationSize)} must be positive or 0.");
                _populationSize = value;
            }
        }

        private double _mutationRate;

        public double MutationRate
        {
            get => _mutationRate;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MutationRate)} must be in [0, 1].");
                _mutationRate = value;
            }
        }

        private double _crossoverRate;

        public double CrossoverRate
        {
            get => _crossoverRate;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(CrossoverRate)} must be in [0, 1].");
                _crossoverRate = value;
            }
        }

        private double _elitismRate;

        public double ElitismRate
        {
            get => _elitismRate;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ElitismRate)} must be in [0, 1].");
                _elitismRate = value;
            }
        }
    }
}