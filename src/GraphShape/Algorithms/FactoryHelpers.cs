using GraphShape.Algorithms.Layout;
using JetBrains.Annotations;

namespace GraphShape.Algorithms
{
    /// <summary>
    /// Helpers to create layout parameters sets.
    /// </summary>
    public static class FactoryHelpers
    {
        /// <summary>
        /// Creates a new set of layout parameters.
        /// Clones given <paramref name="parameters"/> if possible, creates default otherwise.
        /// </summary>
        /// <typeparam name="TParam">Parameter type.</typeparam>
        /// <param name="parameters">Set of parameters.</param>
        /// <returns>Created set of parameters.</returns>
        [Pure]
        [NotNull]
        public static TParam CreateNewParameters<TParam>([CanBeNull] this ILayoutParameters parameters)
            where TParam : class, ILayoutParameters, new()
        {
            return parameters is TParam oldParams
                ? (TParam)oldParams.Clone()
                : new TParam();
        }
    }
}
