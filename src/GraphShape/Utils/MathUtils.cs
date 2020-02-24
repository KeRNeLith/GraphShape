using System;
using JetBrains.Annotations;

namespace GraphShape.Utils
{
    /// <summary>
    /// Math utilities.
    /// </summary>
    internal static class MathUtils
    {
        /// <summary>
        /// Smallest value such that 1.0+<see cref="DoubleEpsilon"/> != 1.0
        /// </summary>
        public const double DoubleEpsilon = 2.2204460492503131e-016;

        /// <summary>
        /// Returns whether or not the double is "close" to 0, but this is faster.
        /// </summary>
        /// <returns>The result of the comparision.</returns>
        /// <param name="a">The double to compare to 0.</param>
        [Pure]
        public static bool IsZero(double a)
        {
            return Math.Abs(a) < 10.0 * DoubleEpsilon;
        }

        /// <summary>
        /// Returns whether or not two <see cref="float"/>s are "equal". That is, whether or
        /// not they are within epsilon of each other.
        /// </summary>
        /// <remarks>
        /// Take into account the magnitude of floating point numbers.
        /// The code is using the technique described by Bruce Dawson in
        /// <a href="http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/">Comparing Floating point numbers 2012 edition</a>.
        /// </remarks>
        /// <param name="a">The first <see cref="float"/> to compare.</param>
        /// <param name="b">The second <see cref="float"/> to compare.</param>
        /// <returns>The result of the comparision.</returns>
        public static unsafe bool NearEqual(float a, float b)
        {
            // Check if the numbers are really close
            // Needed when comparing numbers near zero
            if (IsZero(a - b))
                return true;

            int aInt = *(int*)&a;
            int bInt = *(int*)&b;
            
            // Different signs means they do not match
            if (aInt < 0 != bInt < 0)
                return false;

            // Find the difference in ULPs
            int ulp = Math.Abs(aInt - bInt);

            // Choose of maxUlp = 4
            // according to http://code.google.com/p/googletest/source/browse/trunk/include/gtest/internal/gtest-internal.h
            const int maxUlp = 4;
            return ulp <= maxUlp;
        }

        /// <summary>
        /// Returns whether or not two <see cref="double"/>s are "equal". That is, whether or
        /// not they are within epsilon of each other.
        /// </summary>
        /// <param name="a">The first <see cref="double"/> to compare.</param>
        /// <param name="b">The second <see cref="double"/> to compare.</param>
        /// <returns>The result of the comparision.</returns>
        [Pure]
        public static bool NearEqual(double a, double b)
        {
            // In case they are Infinities (then epsilon check does not work)
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (a == b)
                return true;

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DoubleEpsilon
            double eps = (Math.Abs(a) + Math.Abs(b) + 10.0) * DoubleEpsilon;
            double delta = a - b;
            return -eps < delta && eps > delta;
        }
    }
}