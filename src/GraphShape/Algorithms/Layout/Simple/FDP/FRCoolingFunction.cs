namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Enumeration of possible cooling functions for <see cref="FRLayoutAlgorithm{TVertex,TEdge,TGraph}"/> algorithm.
    /// </summary>
    public enum FRCoolingFunction
    {
        /// <summary>
        /// Linear.
        /// </summary>
        Linear,

        /// <summary>
        /// Exponential.
        /// </summary>
        Exponential
    }
}