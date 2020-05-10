using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Utils;

namespace GraphShape.Optimization.GeneticAlgorithm
{
    /// <summary>
    /// Base class for genetic algorithm.
    /// </summary>
    /// <typeparam name="TChromosome">Chromosome type.</typeparam>
    /// <typeparam name="TParameters">Algorithm parameters type.</typeparam>
    internal abstract class GeneticAlgorithmBase<TChromosome, TParameters>
        where TChromosome : class
        where TParameters : GeneticAlgorithmParameters
    {
        [NotNull]
        protected readonly Random Rand = new CryptoRandom();

        [NotNull]
        public TParameters Parameters { get; }

        [ItemNotNull]
        protected List<Solution> PopulationInternal;

        [NotNull, ItemNotNull]
        public TChromosome[] Population => PopulationInternal?.Select(sol => sol.Chromosome).ToArray() ?? Array.Empty<TChromosome>();

        [NotNull]
        public double[] Fitnesses => PopulationInternal?.Select(sol => sol.Fitness).ToArray() ?? Array.Empty<double>();

        protected GeneticAlgorithmBase([NotNull] TParameters parameters)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// Fired when a generation is extincted.
        /// </summary>
        public event Action<int> GenerationExtincts;

        protected void InitPopulation()
        {
            PopulationInternal = new List<Solution>(Parameters.PopulationSize);
            for (int i = 0; i < Parameters.PopulationSize; ++i)
            {
                PopulationInternal.Add(
                    new Solution
                    {
                        Chromosome = CreateIndividual()
                    });
            }
        }

        protected void EvaluateFitnesses()
        {
            foreach (Solution solution in PopulationInternal)
            {
                // We do not have to recalculate the fitness of the elite
                if (double.IsNaN(solution.Fitness))
                {
                    solution.Fitness = EvaluateFitness(solution.Chromosome);
                }
            }
        }

        protected void SortByFitness()
        {
            PopulationInternal.Sort((s1, s2) => s1.Fitness.CompareTo(s2.Fitness));
        }

        protected void CreateNewPopulation()
        {
            var newPopulation = new List<Solution>(Parameters.PopulationSize);
            CopyElite(newPopulation);
            GeneratePopulation(newPopulation);
            PopulationInternal = newPopulation;
        }

        protected void GeneratePopulation([NotNull] IList<Solution> newPopulation)
        {
            var generatedPopulationSize = Parameters.PopulationSize - newPopulation.Count;
            for (int i = 0; i < generatedPopulationSize; ++i)
            {
                SelectParents(out TChromosome parent1, out TChromosome parent2);
                TChromosome offspring = Crossover(parent1, parent2);
                offspring = Mutate(offspring);

                newPopulation.Add(
                    new Solution
                    {
                        Chromosome = offspring
                    });
            }
        }

        public void Run()
        {
            InitPopulation();

            for (int i = 0; i < Parameters.Generations; ++i)
            {
                EvaluateFitnesses();
                SortByFitness();

                GenerationExtincts?.Invoke(i);
                CreateNewPopulation();
            }
        }

        [Pure]
        [NotNull]
        protected abstract TChromosome Mutate([NotNull] TChromosome chromosome);

        [Pure]
        [NotNull]
        protected abstract TChromosome Crossover([NotNull] TChromosome parent1, [NotNull] TChromosome parent2);

        protected virtual void SelectParents([NotNull] out TChromosome parent1, [NotNull] out TChromosome parent2)
        {
            var parent1Index = Rand.Next(PopulationInternal.Count);
            parent1 = PopulationInternal[parent1Index].Chromosome;

            int parent2Index;
            do
            {
                parent2Index = Rand.Next(PopulationInternal.Count);
            } while (parent1Index == parent2Index);
            parent2 = PopulationInternal[parent2Index].Chromosome;
        }

        protected void CopyElite([NotNull, ItemNotNull] IList<Solution> newPopulation)
        {
            double eliteSize = Math.Floor(Parameters.PopulationSize * Parameters.ElitismRate);
            for (int i = 0; i < eliteSize; ++i)
            {
                newPopulation.Add(PopulationInternal[i]);
            }
        }

        [Pure]
        protected abstract double EvaluateFitness([NotNull] TChromosome individual);

        [Pure]
        [NotNull]
        protected abstract TChromosome CreateIndividual();

        #region Nested type: Solution

        protected class Solution
        {
            public TChromosome Chromosome;
            public double Fitness = double.NaN;
        }

        #endregion
    }
}