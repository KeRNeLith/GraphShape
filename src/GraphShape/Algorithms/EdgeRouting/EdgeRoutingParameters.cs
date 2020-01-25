using GraphShape.Utils;

namespace GraphShape.Algorithms.EdgeRouting
{
    /// <summary>
    /// Base class for edge routing algorithm parameters.
    /// </summary>
    public class EdgeRoutingParameters : NotifierObject, IEdgeRoutingParameters
    {
        /// <inheritdoc />
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}