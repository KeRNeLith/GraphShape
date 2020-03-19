using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace GraphShape
{
    /// <summary>
    /// Helpers to deal with some collection operations.
    /// </summary>
    internal static class CollectionExtensions
    {
        /// <summary>
        /// Removes all <paramref name="elements"/> from <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="collection">Collection to remove item from.</param>
        /// <param name="elements">Elements to remove.</param>
        public static void RemoveAll<T>(
            [NotNull, ItemCanBeNull] this ICollection<T> collection,
            [NotNull, ItemCanBeNull] IEnumerable<T> elements)
        {
            Debug.Assert(collection != null);
            Debug.Assert(elements != null);

            foreach (T item in elements)
            {
                collection.Remove(item);
            }
        }
    }
}
