namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Represents overlap removal algorithm parameters.
    /// </summary>
    public interface IOverlapRemovalParameters : IAlgorithmParameters
    {
        /// <summary>
        /// Vertical gap.
        /// </summary>
        float VerticalGap { get; }

        /// <summary>
        /// Horizontal gap.
        /// </summary>
        float HorizontalGap { get; }
    }
}