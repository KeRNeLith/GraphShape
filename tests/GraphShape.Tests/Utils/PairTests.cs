using GraphShape.Utils;
using NUnit.Framework;

namespace GraphShape.Tests
{
    /// <summary>
    /// Tests for <see cref="Pair"/>.
    /// </summary>
    internal class PairTests
    {
        [Test]
        public void Constructor()
        {
            var pair = new Pair();
            CheckPair(0, 0, 1);

            pair.First = 12;
            CheckPair(12, 0, 1);

            pair.Second = 25;
            CheckPair(12, 25, 1);

            pair.Weight = 5;
            CheckPair(12, 25, 5);

            #region Local function

            void CheckPair(int first, int second, int weight)
            {
                Assert.AreEqual(first, pair.First);
                Assert.AreEqual(second, pair.Second);
                Assert.AreEqual(weight, pair.Weight);
            }

            #endregion
        }
    }
}