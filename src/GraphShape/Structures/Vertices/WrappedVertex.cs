using JetBrains.Annotations;

namespace GraphShape
{
    /// <summary>
    /// Vertex wrapper.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public class WrappedVertex<TVertex>
    {
        /// <summary>
        /// Wrapped vertex.
        /// </summary>
        [CanBeNull]
        public TVertex Original { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedVertex{TVertex}"/> class.
        /// </summary>
        /// <param name="original">Vertex to wrap.</param>
        public WrappedVertex([CanBeNull] TVertex original)
        {
            Original = original;
        }
    }
}