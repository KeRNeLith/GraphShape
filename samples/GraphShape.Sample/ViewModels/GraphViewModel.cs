using System;
using JetBrains.Annotations;

namespace GraphShape.Sample.ViewModels
{
    /// <summary>
    /// Graph entry model.
    /// </summary>
    internal class GraphViewModel
    {
        /// <summary>
        /// Graph name.
        /// </summary>
        [NotNull]
        public string Name { get; }
        
        /// <summary>
        /// Graph.
        /// </summary>
        [NotNull]
        public PocGraph Graph { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphViewModel"/> class.
        /// </summary>
        public GraphViewModel(
            [NotNull] string name,
            [NotNull] PocGraph graph)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
    }
}