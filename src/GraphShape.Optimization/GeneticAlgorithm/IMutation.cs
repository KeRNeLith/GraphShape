using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphSharp.Optimization.GeneticAlgorithm
{
    public interface IMutation<TGene> 
        where TGene : class
    {
        ICollection<TGene> Mutate(ICollection<TGene> genes);
    }
}
