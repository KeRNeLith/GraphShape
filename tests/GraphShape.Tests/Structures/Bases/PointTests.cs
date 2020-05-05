using NUnit.Framework;

namespace GraphShape.Tests
{
    /// <summary>
    /// Tests for <see cref="Point"/>.
    /// </summary>
    internal class PointTests
    {
        [Test]
        public void Constructor()
        {
            var point = new Point();
            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);

            point = new Point(1, 1.5);
            Assert.AreEqual(1, point.X);
            Assert.AreEqual(1.5, point.Y);

            point = new Point(-1.6, 3.4);
            Assert.AreEqual(-1.6, point.X);
            Assert.AreEqual(3.4, point.Y);
        }

        [Test]
        public void Update()
        {
            var point = new Point(1, 2);

            Assert.AreEqual(new Point(1, 2), point);

            point.X = 12;
            Assert.AreEqual(new Point(12, 2), point);

            point.Y = 25;
            Assert.AreEqual(new Point(12, 25), point);

            point.X = 42;
            Assert.AreEqual(new Point(42, 25), point);
        }

        [Test]
        public void Add()
        {
            var point = new Point(1, -1.5);
            var vector = new Vector(1, 1);

            Assert.AreEqual(new Point(2, -0.5), point + vector);

            vector = new Vector(-5, 2);

            Assert.AreEqual(new Point(-4, 0.5), point + vector);
        }

        [Test]
        public void Subtract()
        {
            var point = new Point(1, -1.5);
            var vector = new Vector(1, 1);

            Assert.AreEqual(new Point(0, -2.5), point - vector);

            vector = new Vector(-5, 2);

            Assert.AreEqual(new Point(6, -3.5), point - vector);
        }

        [Test]
        public void SubtractVector()
        {
            var point1 = new Point(1, -1.5);
            var point2 = new Point(-2, 1.6);

            Assert.AreEqual(new Vector(3, -3.1), point1 - point2);
            Assert.AreEqual(new Vector(-3, 3.1), point2 - point1);

            point1 = new Point(-2, -5);

            Assert.AreEqual(new Vector(0, -6.6), point1 - point2);
            Assert.AreEqual(new Vector(0, 6.6), point2 - point1);
        }

        [Test]
        public void Equals()
        {
            var point1 = new Point(1.0, 2.0);
            var point2 = new Point(1.0, 2.0);
            var point3 = new Point(4.0, 3.0);

            Assert.AreEqual(point1, point1);

            Assert.IsTrue(point1 == point2);
            Assert.IsTrue(point1.Equals(point2));
            Assert.IsTrue(point1.Equals((object)point2));
            Assert.IsFalse(point1 != point2);
            Assert.AreEqual(point1, point2);

            Assert.IsFalse(point1 == point3);
            Assert.IsFalse(point1.Equals(point3));
            Assert.IsFalse(point1.Equals((object)point3));
            Assert.IsTrue(point1 != point3);
            Assert.AreNotEqual(point1, point3);

            Assert.AreNotEqual(point1, null);
            Assert.AreNotEqual(point1, new TestVertex());
        }

        [Test]
        public void HashCode()
        {
            var point1 = new Point();
            var point2 = new Point();
            var point3 = new Point(1.0, 2.0);
            var point4 = new Point(1.0, 2.0);

            Assert.AreEqual(point1.GetHashCode(), point2.GetHashCode());
            Assert.AreNotEqual(point1.GetHashCode(), point3.GetHashCode());
            Assert.AreEqual(point3.GetHashCode(), point4.GetHashCode());
        }

        [Test]
        public void PointToString()
        {
            var point = new Point();
            Assert.AreEqual("0;0", point.ToString());

            point = new Point(1, 2);
            Assert.AreEqual("1;2", point.ToString());

            point = new Point(-2, 1);
            Assert.AreEqual("-2;1", point.ToString());
        }
    }
}