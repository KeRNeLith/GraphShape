namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    /// <summary>
    /// Enumeration of possible position calculation methods.
    /// </summary>
    public enum PositionCalculationMethodTypes
    {
        /// <summary>
        /// Barycenter of the vertices computed based on the 
        /// indexes of the vertices.
        /// </summary>
        IndexBased,

        /// <summary>
        /// Barycenter of the vertices computed based on
        /// the vertices sizes and positions.
        /// </summary>
        PositionBased
    }
}
