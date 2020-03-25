namespace GraphShape.Tests
{
    /// <summary>
    /// Represents an entity able to produce some metrics.
    /// </summary>
    internal interface IMetricCalculator
    {
        /// <summary>
        /// Calculate some metrics.
        /// </summary>
        void Calculate();
    }
}
