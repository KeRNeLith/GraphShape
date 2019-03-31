using System.Diagnostics.Contracts;

namespace GraphSharp.Optimization.GeneticAlgorithm
{
    public class GeneticAlgorithmParameters
    {
        [ContractInvariantMethod]
        public void Invariants()
        {
            Contract.Invariant(Generations > 0);
            Contract.Invariant(PopulationSize > 0);
            Contract.Invariant(0 <= MutationRate && MutationRate <= 1.0);
            Contract.Invariant(0 <= CrossoverRate && CrossoverRate <= 1.0);
            Contract.Invariant(0 <= ElitismRate && ElitismRate <= 1.0);
        }

        public int Generations { get; set; }
        public int PopulationSize { get; set; }
        public double MutationRate { get; set; }
        public double CrossoverRate { get; set; }
        public double ElitismRate { get; set; }
    }
}
