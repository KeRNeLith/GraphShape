using System;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape
{
    /// <summary>
    /// Edge implementation with a weight.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [Serializable]
    public class WeightedEdge<TVertex> : Edge<TVertex>
    {
        /// <summary>
        /// Edge weight.
        /// </summary>
        public double Weight { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedEdge{TVertex}"/> class.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public WeightedEdge([NotNull] TVertex source, [NotNull] TVertex target)
            : this(source, target, 1.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedEdge{TVertex}"/> class.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="weight">Edge weight.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public WeightedEdge([NotNull] TVertex source, [NotNull] TVertex target, double weight)
            : base(source, target)
        {
            this.Weight = weight;
        }
    }
}