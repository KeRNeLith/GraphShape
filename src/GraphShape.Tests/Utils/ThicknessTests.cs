using GraphShape.Utils;
using NUnit.Framework;

namespace GraphShape.Tests
{
    /// <summary>
    /// Tests for <see cref="Thickness"/>.
    /// </summary>
    internal class ThicknessTests
    {
        [Test]
        public void Constructor()
        {
            var thickness = new Thickness();
            Assert.AreEqual(0, thickness.Left);
            Assert.AreEqual(0, thickness.Top);
            Assert.AreEqual(0, thickness.Right);
            Assert.AreEqual(0, thickness.Bottom);

            thickness = new Thickness(1.0, 2.0, 3.0, 4.0);
            Assert.AreEqual(1.0, thickness.Left);
            Assert.AreEqual(2.0, thickness.Top);
            Assert.AreEqual(3.0, thickness.Right);
            Assert.AreEqual(4.0, thickness.Bottom);

            thickness = new Thickness(-1.0, -2.0, -3.0, -4.0);
            Assert.AreEqual(-1.0, thickness.Left);
            Assert.AreEqual(-2.0, thickness.Top);
            Assert.AreEqual(-3.0, thickness.Right);
            Assert.AreEqual(-4.0, thickness.Bottom);
        }

        [Test]
        public void Equals()
        {
            var thickness1 = new Thickness(1.0, 2.0, 3.0, 4.0);
            var thickness2 = new Thickness(1.0, 2.0, 3.0, 4.0);
            var thickness3 = new Thickness(4.0, 3.0, 2.0, 1.0);

            Assert.AreEqual(thickness1, thickness1);

            Assert.IsTrue(thickness1 == thickness2);
            Assert.IsTrue(thickness1.Equals(thickness2));
            Assert.IsTrue(thickness1.Equals((object)thickness2));
            Assert.IsFalse(thickness1 != thickness2);
            Assert.AreEqual(thickness1, thickness2);

            Assert.IsFalse(thickness1 == thickness3);
            Assert.IsFalse(thickness1.Equals(thickness3));
            Assert.IsFalse(thickness1.Equals((object)thickness3));
            Assert.IsTrue(thickness1 != thickness3);
            Assert.AreNotEqual(thickness1, thickness3);

            Assert.AreNotEqual(thickness1, null);
            Assert.AreNotEqual(thickness1, new TestVertex());
        }

        [Test]
        public void HashCode()
        {
            var thickness1 = new Thickness();
            var thickness2 = new Thickness();
            var thickness3 = new Thickness(1.0, 2.0, 3.0, 4.0);
            var thickness4 = new Thickness(1.0, 2.0, 3.0, 4.0);

            Assert.AreEqual(thickness1.GetHashCode(), thickness2.GetHashCode());
            Assert.AreNotEqual(thickness1.GetHashCode(), thickness3.GetHashCode());
            Assert.AreEqual(thickness3.GetHashCode(), thickness4.GetHashCode());
        }
    }
}