using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Accessibility;
using GraphShape.Algorithms.Layout;
using GraphShape.Optimization.Algorithms;
using GraphShape.Optimization.GeneticAlgorithm;
using JetBrains.Annotations;

namespace GraphShape.Optimization
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    internal partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RunOptimizer([NotNull] object sender, [NotNull] RoutedEventArgs args)
        {
            await Task.Run(() =>
            {
                var parameters = new GeneticAlgorithmParameters
                {
                    CrossoverRate = 0.85,
                    ElitismRate = 0.1,
                    Generations = 300,
                    MutationRate = 0.7,
                    PopulationSize = 50
                };
                
                var optimizer = new GeneticCompoundFDPOptimizer(parameters);

                optimizer.GenerationExtincts += OnGenerationExtincts;
                optimizer.Run();
                optimizer.GenerationExtincts -= OnGenerationExtincts;

                PrintChromosomes(optimizer);

                void OnGenerationExtincts(int generation)
                {
                    Dispatcher.Invoke(() =>
                    {
                        FeedbackLabel.Text = $"Generation {generation}/{parameters.Generations}";
                        GenerationResult.Text = PrintChromosomes(optimizer);
                    });
                }
            });
        }

        private static string PrintChromosomes([NotNull] GeneticCompoundFDPOptimizer optimizer)
        {
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < optimizer.Population.Length; i++)
            {
                CompoundFDPLayoutParameters chromosome = optimizer.Population[i];

                strBuilder.AppendLine($"---------- Chromosome[{i}] ---------------");
                strBuilder.AppendLine($"ElasticConstant: {chromosome.ElasticConstant}");
                strBuilder.AppendLine($"RepulsionConstant: {chromosome.RepulsionConstant}");
                strBuilder.AppendLine($"GravitationFactor: {chromosome.GravitationFactor}");
                strBuilder.AppendLine($"NestingFactor: {chromosome.NestingFactor}");
                strBuilder.AppendLine($"Phase1Iterations: {chromosome.Phase1Iterations}");
                strBuilder.AppendLine($"Phase2Iterations: {chromosome.Phase2Iterations}");
                strBuilder.AppendLine($"Phase3Iterations: {chromosome.Phase3Iterations}");
                strBuilder.AppendLine($"DisplacementLimitMultiplier: {chromosome.DisplacementLimitMultiplier}");
                strBuilder.AppendLine($"SeparationMultiplier: {chromosome.SeparationMultiplier}");
                strBuilder.AppendLine($"TemperatureDecreasing: {chromosome.TemperatureDecreasing}");
                strBuilder.AppendLine($"Fitness: {optimizer.Fitnesses[i]}");
                strBuilder.AppendLine("---------------------------------------------");
            }

            return strBuilder.ToString();
        }
    }
}
