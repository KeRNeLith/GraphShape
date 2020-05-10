using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphSharp.Optimization.GeneticAlgorithm
{
    public abstract class GeneticAlgorithmBase<TChromosome, TParam>
        where TChromosome : class
        where TParam : GeneticAlgorithmParameters
    {
        protected static readonly Random rnd = new Random(DateTime.Now.Millisecond);
        private TParam _parameters;

        protected List<Solution> _population;

        protected GeneticAlgorithmBase(TParam parameters)
        {
            Parameters = parameters;
        }

        public IList<TChromosome> Population
        {
            get { return _population.Select(sol => sol.Chromosome).ToList(); }
        }

        public IList<double> Fitnesses
        {
            get { return _population.Select(sol => sol.Fitness).ToList(); }
        }

        public event Action<int> GenerationExtincts;

        public TParam Parameters
        {
            get { return _parameters; }
            private set { _parameters = value; }
        }

        protected void InitPopulation()
        {
            _population = new List<Solution>(Parameters.PopulationSize);
            for (int i = 0; i < Parameters.PopulationSize; i++)
            {
                _population.Add(
                    new Solution
                        {
                            Chromosome = CreateIndividual()
                        }
                    );
            }
        }

        protected void EvaluateFitnesses()
        {
            int i = 0;
            foreach (var solution in _population)
            {
                //we do not have to recalculate the fitness of the elite
                if (double.IsNaN(solution.Fitness))
                {
                    Console.WriteLine("Evaluating Fitness on Solution {0}", i);
                    solution.Fitness = EvaluateFitness(solution.Chromosome);
                }
                i++;
            }
        }

        protected void SortByFitness()
        {
            _population.Sort(delegate(Solution s1, Solution s2) { return s1.Fitness.CompareTo(s2.Fitness); });
        }

        protected void CreateNewPopulation()
        {
            var newPopulation = new List<Solution>(Parameters.PopulationSize);
            CopyElite(newPopulation);
            GeneratePopulation(newPopulation);
            _population = newPopulation;
        }

        protected void GeneratePopulation(IList<Solution> newPopulation)
        {
            var generatedPopulationSize = Parameters.PopulationSize - newPopulation.Count;
            for (int i = 0; i < generatedPopulationSize; i++)
            {
                TChromosome parent1 = null, parent2 = null;
                SelectParents(out parent1, out parent2);
                TChromosome offspring = Crossover(parent1, parent2);
                offspring = Mutate(offspring);

                newPopulation.Add(new Solution {Chromosome = offspring});
            }
        }

        public void Run()
        {
            InitPopulation();

            for (int i = 0; i < Parameters.Generations; i++)
            {
                Console.WriteLine("Starting generation {0}", i);
                EvaluateFitnesses();
                SortByFitness();

                if (GenerationExtincts != null)
                    GenerationExtincts(i);
                CreateNewPopulation();
            }
        }

        protected abstract TChromosome Mutate(TChromosome chromosome);

        protected abstract TChromosome Crossover(TChromosome parent1, TChromosome parent2);

        protected virtual void SelectParents(out TChromosome parent1, out TChromosome parent2)
        {
            var parent1Index = rnd.Next(_population.Count);
            parent1 = _population[parent1Index].Chromosome;

            int parent2Index;
            do
            {
                parent2Index = rnd.Next(_population.Count);
            } while (parent1Index == parent2Index);
            parent2 = _population[parent2Index].Chromosome;
        }

        protected void CopyElite(IList<Solution> newPopulation)
        {
            var eliteSize = Math.Floor(Parameters.PopulationSize*Parameters.ElitismRate);
            for (int i = 0; i < eliteSize; i++)
                newPopulation.Add(_population[i]);
        }

        protected abstract double EvaluateFitness(TChromosome individual);
        protected abstract TChromosome CreateIndividual();

        #region Nested type: Solution

        protected class Solution<TChromo>
            where TChromo : class
        {
            public TChromo Chromosome;
            public double Fitness = double.NaN;
        }

        protected class Solution : Solution<TChromosome>
        {
        }

        #endregion
    }
}