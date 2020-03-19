using GraphShape.Utils;

namespace GraphShape.Algorithms.EdgeRouting
{
    /// <summary>
    /// Edge routing algorithm parameters.
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