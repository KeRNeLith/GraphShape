using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Represents an overlap removal algorithm.
    /// </summary>
    /// <typeparam name="TObject">Object type.</typeparam>
    public interface IOverlapRemovalAlgorithm<TObject> : IAlgorithm
    {
        /// <summary>
        /// Overlap rectangles.
        /// </summary>
        [NotNull]
        IDictionary<TObject, Rect> Rectangles { get; }

        /// <summary>
        /// Gets overlap removal parameters.
        /// </summary>
        /// <returns>Overlap removal parameters.</returns>
        [Pure]
        [NotNull]
        IOverlapRemovalParameters GetParameters();
    }

    /// <summary>
    /// Represents an overlap removal algorithm (with parameters).
    /// </summary>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <typeparam name="TParameters">Algorithm parameters type.</typeparam>
    public interface IOverlapRemovalAlgorithm<TObject, out TParameters> : IOverlapRemovalAlgorithm<TObject>
        where TParameters : IOverlapRemovalParameters
    {
        /// <summary>
        /// Overlap removal parameters.
        /// </summary>
        [NotNull]
        TParameters Parameters { get; }
    }
}