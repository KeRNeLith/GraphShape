using System;
using System.Windows;
using GraphSharp.Optimization.Algorithms.Layout.CompoundFDP;

namespace GraphSharp.Optimization
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Run_Optimizer(object sender, RoutedEventArgs e)
        {
            var parameters = new GeneticAlgorithm.GeneticAlgorithmParameters
                                 {
                                     CrossoverRate = 0.85,
                                     ElitismRate = 0.1,
                                     Generations = 300,
                                     MutationRate = 0.7,
                                     PopulationSize = 50
                                 };
            var optimizer = new GeneticCompoundFDPOptimizer(parameters);
            optimizer.GenerationExtincts += (generation) =>
                                                {
                                                    Console.WriteLine("Generation {0}", generation);
                                                    PrintChromosomes(optimizer);
                                                };
            optimizer.Run();
            Console.WriteLine("Last generation");
            PrintChromosomes(optimizer);
        }

        private void PrintChromosomes(GeneticCompoundFDPOptimizer optimizer)
        {
            for (int i = 0; i < optimizer.Population.Count; i++)
            {
                var chromosome = optimizer.Population[i];
                Console.WriteLine("Chromosome[{0}]::::::", i);
                Console.WriteLine(chromosome.ElasticConstant);
                Console.WriteLine(chromosome.RepulsionConstant);
                Console.WriteLine(chromosome.GravitationFactor);
                Console.WriteLine(chromosome.NestingFactor);
                Console.WriteLine(chromosome.Phase1Iterations);
                Console.WriteLine(chromosome.Phase2Iterations);
                Console.WriteLine(chromosome.Phase3Iterations);
                Console.WriteLine(chromosome.DisplacementLimitMultiplier);
                Console.WriteLine(chromosome.SeparationMultiplier);
                Console.WriteLine(chromosome.TemperatureDecreasing);
                Console.WriteLine(chromosome.TemperatureFactor);
                Console.WriteLine("Fitness: {0}", optimizer.Fitnesses[i]);
                Console.WriteLine(
                    "---------------------------------------------");
            }
        }
    }
}
