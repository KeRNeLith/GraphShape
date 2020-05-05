using System;
using GraphShape.Algorithms.Layout;
using GraphShape.Utils;
using NUnit.Framework;

namespace GraphShape.Tests
{
    /// <summary>
    /// Tests for <see cref="Vector"/>.
    /// </summary>
    internal class VectorTests
    {
        [Test]
        public void Constructor()
        {
            var vector = new Vector();
            Assert.AreEqual(0, vector.X);
            Assert.AreEqual(0, vector.Y);

            vector = new Vector(1, 1.5);
            Assert.AreEqual(1, vector.X);
            Assert.AreEqual(1.5, vector.Y);

            vector = new Vector(-1.6, 3.4);
            Assert.AreEqual(-1.6, vector.X);
            Assert.AreEqual(3.4, vector.Y);
        }

        [Test]
        public void Update()
        {
            var vector = new Vector(1, 2);

            Assert.AreEqual(new Vector(1, 2), vector);

            vector.X = 12;
            Assert.AreEqual(new Vector(12, 2), vector);

            vector.Y = 25;
            Assert.AreEqual(new Vector(12, 25), vector);

            vector.X = 42;
            Assert.AreEqual(new Vector(42, 25), vector);
        }

        [Test]
        public void Length()
        {
            var vector = new Vector(1, 2);
            Assert.AreEqual(Math.Sqrt(5), vector.Length);
            Assert.AreEqual(5, vector.LengthSquared);

            vector = new Vector(5, -2);
            Assert.AreEqual(Math.Sqrt(29), vector.Length);
            Assert.AreEqual(29, vector.LengthSquared);
        }

        [Test]
        public void Normalize()
        {
            var initialVector = new Vector(1, 2);
            var vector = new Vector(1, 2);
            Assert.AreNotEqual(1, vector.Length);
            vector.Normalize();
            Assert.IsTrue(MathUtils.NearEqual(1, vector.Length));
            Assert.IsTrue(LayoutUtils.IsSameDirection(initialVector, vector));

            initialVector = new Vector(-21, 15);
            vector = new Vector(-21, 15);
            Assert.AreNotEqual(1, vector.Length);
            vector.Normalize();
            Assert.IsTrue(MathUtils.NearEqual(1, vector.Length));
            Assert.IsTrue(LayoutUtils.IsSameDirection(initialVector, vector));
        }

        [Test]
        public void Negate()
        {
            var vector = new Vector(1, -1.5);
            Assert.AreEqual(new Vector(-1, 1.5), -vector);

            vector = new Vector(-5, -2);
            Assert.AreEqual(new Vector(5, 2), -vector);
        }

        [Test]
        public void Add()
        {
            var vector1 = new Vector(1, -1.5);
            var vector2 = new Vector(1, 1);

            Assert.AreEqual(new Vector(2, -0.5), vector1 + vector2);

            vector2 = new Vector(-5, 2);

            Assert.AreEqual(new Vector(-4, 0.5), vector1 + vector2);
        }

        [Test]
        public void Subtract()
        {
            var vector1 = new Vector(1, -1.5);
            var vector2 = new Vector(1, 1);

            Assert.AreEqual(new Vector(0, -2.5), vector1 - vector2);

            vector2 = new Vector(-5, 2);

            Assert.AreEqual(new Vector(6, -3.5), vector1 - vector2);
        }

        [Test]
        public void Multiply()
        {
            var vector = new Vector(1, -1.5);
            Assert.AreEqual(new Vector(4, -6), vector * 4);
            Assert.AreEqual(new Vector(4, -6), 4 * vector);

            vector = new Vector(-5, -2);
            Assert.AreEqual(new Vector(10, 4), vector * -2);
            Assert.AreEqual(new Vector(10, 4), -2 * vector);
        }

        [Test]
        public void Divide()
        {
            var vector = new Vector(4, -6);
            Assert.AreEqual(new Vector(1, -1.5), vector / 4);

            vector = new Vector(-5, -2);
            Assert.AreEqual(new Vector(2.5, 1), vector / -2);
        }

        [Test]
        public void DotProduct()
        {
            var vector1 = new Vector(2, 3);
            var vector2 = new Vector(1, -2);
            Assert.AreEqual(-4, vector1 * vector2);

            vector1 = new Vector(-5, -2);
            vector2 = new Vector(-2, -2);
            Assert.AreEqual(14, vector1 * vector2);
        }

        [Test]
        public void Equals()
        {
            var vector1 = new Vector(1.0, 2.0);
            var vector2 = new Vector(1.0, 2.0);
            var vector3 = new Vector(4.0, 3.0);

            Assert.AreEqual(vector1, vector1);

            Assert.IsTrue(vector1 == vector2);
            Assert.IsTrue(vector1.Equals(vector2));
            Assert.IsTrue(vector1.Equals((object)vector2));
            Assert.IsFalse(vector1 != vector2);
            Assert.AreEqual(vector1, vector2);

            Assert.IsFalse(vector1 == vector3);
            Assert.IsFalse(vector1.Equals(vector3));
            Assert.IsFalse(vector1.Equals((object)vector3));
            Assert.IsTrue(vector1 != vector3);
            Assert.AreNotEqual(vector1, vector3);

            Assert.AreNotEqual(vector1, null);
            Assert.AreNotEqual(vector1, new TestVertex());
        }

        [Test]
        public void HashCode()
        {
            var vector1 = new Vector();
            var vector2 = new Vector();
            var vector3 = new Vector(1.0, 2.0);
            var vector4 = new Vector(1.0, 2.0);

            Assert.AreEqual(vector1.GetHashCode(), vector2.GetHashCode());
            Assert.AreNotEqual(vector1.GetHashCode(), vector3.GetHashCode());
            Assert.AreEqual(vector3.GetHashCode(), vector4.GetHashCode());
        }

        [Test]
        public void VectorToString()
        {
            var vector = new Vector();
            Assert.AreEqual("0;0", vector.ToString());

            vector = new Vector(1, 2);
            Assert.AreEqual("1;2", vector.ToString());

            vector = new Vector(-2, 1);
            Assert.AreEqual("-2;1", vector.ToString());
        }
    }
}