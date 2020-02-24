using GraphShape.Utils;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Base class for all <see cref="ILayoutParameters"/> implementation.
    /// </summary>
    public abstract class LayoutParametersBase : NotifierObject, ILayoutParameters
    {
        /// <inheritdoc />
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}