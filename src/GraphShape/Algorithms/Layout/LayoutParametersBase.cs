using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GraphShape.Utils;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Base class for all <see cref="ILayoutParameters"/> implementation.
    /// </summary>
    public abstract class LayoutParametersBase : NotifierObject, ILayoutParameters
    {
        /// <summary>
        /// Gets elements to take into account when comparing this <see cref="LayoutParametersBase"/>.
        /// </summary>
        /// <returns>Elements to compare.</returns>
        [Pure]
        [NotNull, ItemCanBeNull]
        protected virtual IEnumerable<object> GetEqualityElements()
        {
            yield break;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null || GetType() != obj.GetType())
                return false;

            var equatableObject = (LayoutParametersBase)obj;
            return GetEqualityElements().SequenceEqual(equatableObject.GetEqualityElements());
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        /// <inheritdoc />
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}