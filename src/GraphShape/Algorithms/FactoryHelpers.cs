using JetBrains.Annotations;

namespace GraphShape.Algorithms
{
    /// <summary>
    /// Helpers to create layout parameters sets.
    /// </summary>
    public static class FactoryHelpers
    {
        /// <summary>
        /// Creates a new set of algorithm parameters.
        /// Clones given <paramref name="parameters"/> if possible, creates default otherwise.
        /// </summary>
        /// <typeparam name="TParam">Parameter type.</typeparam>
        /// <param name="parameters">Set of parameters.</param>
        /// <returns>Created set of parameters.</returns>
        [Pure]
        [NotNull]
        public static TParam CreateNewParameters<TParam>([CanBeNull] this IAlgorithmParameters parameters)
            where TParam : class, IAlgorithmParameters, new()
        {
            return parameters is TParam oldParams
                ? (TParam)oldParams.Clone()
                : new TParam();
        }
    }
}