namespace GraphShape.Controls
{
    /// <summary>
    /// Enumeration of possible algorithm constraints regarding overlap and edge routing.
    /// </summary>
    public enum AlgorithmConstraints
    {
        /// <summary>
        /// Must perform process.
        /// </summary>
        Must,

        /// <summary>
        /// Automatic process if needed.
        /// </summary>
        Automatic,

        /// <summary>
        /// Skip process.
        /// </summary>
        Skip
    }
}