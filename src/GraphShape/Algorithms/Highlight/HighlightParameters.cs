using GraphShape.Utils;

namespace GraphShape.Algorithms.Highlight
{
    /// <summary>
    /// Base class for highlight algorithm parameters.
    /// </summary>
    public class HighlightParameters : NotifierObject, IHighlightParameters
    {
        /// <inheritdoc />
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}