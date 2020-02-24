using System.Diagnostics;

namespace GraphShape.Utils
{
    /// <summary>
    /// Weighted pair.
    /// </summary>
    [DebuggerDisplay(nameof(First) + " = {" + nameof(First) + "}, " + nameof(Second) + " = {" + nameof(Second) + "}")]
    public class Pair
    {
        /// <summary>
        /// First value.
        /// </summary>
        public int First { get; set; }

        /// <summary>
        /// Second value.
        /// </summary>
        public int Second { get; set; }

        /// <summary>
        /// Pair weight.
        /// </summary>
        public int Weight { get; set; } = 1;
    }
}
