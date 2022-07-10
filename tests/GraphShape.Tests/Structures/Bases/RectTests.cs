using System;
using NUnit.Framework;

namespace GraphShape.Tests
{
    /// <summary>
    /// Tests for <see cref="Rect"/>.
    /// </summary>
    [TestFixture]
    internal class RectTests
    {
        [Test]
        public void Constructor()
        {
            var rect = new Rect();
            Assert.AreEqual(0, rect.X);
            Assert.AreEqual(0, rect.Y);
            Assert.AreEqual(0, rect.Width);
            Assert.AreEqual(0, rect.Height);
            Assert.IsFalse(rect.IsEmpty);

            rect = new Rect(1, -2, 1, 1.5);
            Assert.AreEqual(1, rect.X);
            Assert.AreEqual(-2, rect.Y);
            Assert.AreEqual(1, rect.Width);
            Assert.AreEqual(1.5, rect.Height);
            Assert.IsFalse(rect.IsEmpty);

            rect = new Rect(-5, 2.6, 1.6, 3.4);
            Assert.AreEqual(-5, rect.X);
            Assert.AreEqual(2.6, rect.Y);
            Assert.AreEqual(1.6, rect.Width);
            Assert.AreEqual(3.4, rect.Height);
            Assert.IsFalse(rect.IsEmpty);

            rect = new Rect(new Point(-5, 2.6), new Size(1.6, 3.4));
            Assert.AreEqual(-5, rect.X);
            Assert.AreEqual(2.6, rect.Y);
            Assert.AreEqual(1.6, rect.Width);
            Assert.AreEqual(3.4, rect.Height);
            Assert.IsFalse(rect.IsEmpty);

            rect = new Rect(new Point(-5, 2.6), Size.Empty);
            Assert.IsTrue(rect.IsEmpty);

            Assert.IsTrue(Rect.Empty.IsEmpty);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new Rect(1, 2, 1, -2));
            Assert.Throws<ArgumentException>(() => new Rect(1, 2, -1, 2));
            Assert.Throws<ArgumentException>(() => new Rect(1, 2, -1, -2));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Update()
        {
            var rect = new Rect(1, 2, 2, 3);

            Assert.AreEqual(new Rect(1, 2, 2, 3), rect);

            rect.X = 12;
            Assert.AreEqual(new Rect(12, 2, 2, 3), rect);

            rect.Y = 21;
            Assert.AreEqual(new Rect(12, 21, 2, 3), rect);
            
            rect.Width = 24;
            Assert.AreEqual(new Rect(12, 21, 24, 3), rect);

            rect.Height = 25;
            Assert.AreEqual(new Rect(12, 21, 24, 25), rect);

            rect.X = 42;
            Assert.AreEqual(new Rect(42, 21, 24, 25), rect);
        }

        [Test]
        public void Update_Throws()
        {
            var rect = new Rect(1, 2, 3, 3);
            var empty = Rect.Empty;

            Assert.Throws<ArgumentOutOfRangeException>(() => rect.Width = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => rect.Height = -1);
            Assert.Throws<InvalidOperationException>(() => empty.X = 1);
            Assert.Throws<InvalidOperationException>(() => empty.Y = 1);
            Assert.Throws<InvalidOperationException>(() => empty.Width = 1);
            Assert.Throws<InvalidOperationException>(() => empty.Height = 1);
        }

        [Test]
        public void GetLocationAndSize()
        {
            var rect = new Rect(1, 2, 2, 3);
            Assert.AreEqual(new Point(1, 2), rect.Location);
            Assert.AreEqual(new Size(2, 3), rect.Size);

            rect = new Rect(-4, -1, 10, 2);
            Assert.AreEqual(new Point(-4, -1), rect.Location);
            Assert.AreEqual(new Size(10, 2), rect.Size);

            rect = Rect.Empty;
            Assert.AreEqual(new Point(double.PositiveInfinity, double.PositiveInfinity), rect.Location);
            Assert.AreEqual(Size.Empty, rect.Size);
        }

        [Test]
        public void SetLocationAndSize()
        {
            var rect = new Rect(1, 2, 2, 3);
            rect.Location = new Point(12, 21);
            rect.Size = new Size(5, 4);
            Assert.AreEqual(new Point(12, 21), rect.Location);
            Assert.AreEqual(new Size(5, 4), rect.Size);

            rect = new Rect(-4, -1, 10, 2);
            rect.Location = new Point(-41, 2);
            rect.Size = new Size(2, 3);
            Assert.AreEqual(new Point(-41, 2), rect.Location);
            Assert.AreEqual(new Size(2, 3), rect.Size);

            rect = new Rect(-4, -1, 10, 2);
            rect.Size = Size.Empty;
            Assert.AreEqual(Size.Empty, rect.Size);
            Assert.IsTrue(rect.IsEmpty);
        }

        [Test]
        public void SetLocationAndSize_Throws()
        {
            var rect = Rect.Empty;
            Assert.Throws<InvalidOperationException>(() => rect.Location = new Point(1, 2));
            Assert.Throws<InvalidOperationException>(() => rect.Size = new Size(1, 2));
        }

        [Test]
        public void Sides()
        {
            var rect = new Rect(1, 2, 2, 3);
            Assert.AreEqual(1, rect.Left);
            Assert.AreEqual(2, rect.Top);
            Assert.AreEqual(3, rect.Right);
            Assert.AreEqual(5, rect.Bottom);

            rect = new Rect(-4, -1, 10, 2);
            Assert.AreEqual(-4, rect.Left);
            Assert.AreEqual(-1, rect.Top);
            Assert.AreEqual(6, rect.Right);
            Assert.AreEqual(1, rect.Bottom);

            rect = Rect.Empty;
            Assert.AreEqual(double.PositiveInfinity, rect.Left);
            Assert.AreEqual(double.PositiveInfinity, rect.Top);
            Assert.AreEqual(double.NegativeInfinity, rect.Right);
            Assert.AreEqual(double.NegativeInfinity, rect.Bottom);
        }

        [Test]
        public void Corners()
        {
            var rect = new Rect(1, 2, 2, 3);
            Assert.AreEqual(new Point(1, 2), rect.TopLeft);
            Assert.AreEqual(new Point(3, 2), rect.TopRight);
            Assert.AreEqual(new Point(1, 5), rect.BottomLeft);
            Assert.AreEqual(new Point(3, 5), rect.BottomRight);

            rect = new Rect(-4, -1, 10, 2);
            Assert.AreEqual(new Point(-4, -1), rect.TopLeft);
            Assert.AreEqual(new Point(6, -1), rect.TopRight);
            Assert.AreEqual(new Point(-4, 1), rect.BottomLeft);
            Assert.AreEqual(new Point(6, 1), rect.BottomRight);

            rect = Rect.Empty;
            Assert.AreEqual(new Point(double.PositiveInfinity, double.PositiveInfinity), rect.TopLeft);
            Assert.AreEqual(new Point(double.NegativeInfinity, double.PositiveInfinity), rect.TopRight);
            Assert.AreEqual(new Point(double.PositiveInfinity, double.NegativeInfinity), rect.BottomLeft);
            Assert.AreEqual(new Point(double.NegativeInfinity, double.NegativeInfinity), rect.BottomRight);
        }

        [Test]
        public void IntersectWith()
        {
            var rect1 = new Rect(1, 2, 2, 3);
            var rect2 = new Rect(4, 4, 2, 3);
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect1 = new Rect(1, 2, 2, 3);
            rect2 = Rect.Empty;
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect1 = Rect.Empty;
            rect2 = Rect.Empty;
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect1 = new Rect(1, 2, 2, 3);
            rect2 = Rect.Empty;
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect1 = new Rect(1, 2, 2, 3);
            rect2 = new Rect(0, 1, 3, 4);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));

            rect1 = new Rect(-10, -2, 15, 4);
            rect2 = new Rect(0, 0, 3, 4);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));
            
            rect1 = new Rect(-10, -2, 15, 4);
            rect2 = new Rect(5, 2, 3, 4);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));
        }

        [Test]
        public void Intersect()
        {
            var rect1 = new Rect(1, 2, 2, 3);
            var rect2 = new Rect(4, 4, 2, 3);
            rect1.Intersect(rect2);
            Assert.AreEqual(Rect.Empty, rect1);

            rect1 = new Rect(1, 2, 2, 3);
            rect2 = Rect.Empty;
            rect1.Intersect(rect2);
            Assert.AreEqual(Rect.Empty, rect1);

            rect1 = Rect.Empty;
            rect2 = Rect.Empty;
            rect1.Intersect(rect2);
            Assert.AreEqual(Rect.Empty, rect1);

            rect1 = new Rect(1, 2, 2, 3);
            rect2 = Rect.Empty;
            rect1.Intersect(rect2);
            Assert.AreEqual(Rect.Empty, rect1);

            rect1 = new Rect(1, 2, 2, 3);
            rect2 = new Rect(0, 1, 3, 4);
            rect1.Intersect(rect2);
            Assert.AreEqual(new Rect(1, 2, 2, 3), rect1);

            rect1 = new Rect(-10, -2, 15, 4);
            rect2 = new Rect(0, 0, 3, 4);
            rect1.Intersect(rect2);
            Assert.AreEqual(new Rect(0, 0, 3, 2), rect1);

            rect1 = new Rect(-10, -2, 15, 4);
            rect2 = new Rect(5, 2, 3, 4);
            rect1.Intersect(rect2);
            Assert.AreEqual(new Rect(5, 2, 0, 0), rect1);
        }

        [Test]
        public void Offset()
        {
            var rect = new Rect(1, 2, 2, 3);
            rect.Offset(1, 2);
            Assert.AreEqual(new Rect(2, 4, 2, 3), rect);

            rect.Offset(-5, 2);
            Assert.AreEqual(new Rect(-3, 6, 2, 3), rect);
        }

        [Test]
        public void Offset_Throws()
        {
            var empty = Rect.Empty;
            Assert.Throws<InvalidOperationException>(() => empty.Offset(1, 2));
        }

        [Test]
        public void Equals()
        {
            var rect1 = new Rect();
            var rect2 = Rect.Empty;
            var rect3 = Rect.Empty;
            var rect4 = new Rect(1.0, 2.0, 3.0, 2.0);
            var rect5 = new Rect(1.0, 2.0, 3.0, 2.0);
            var rect6 = new Rect(4.0, 3.0, 3.0, 2.0);
            var rect8 = new Rect(1.0, 2.0, 2.0, 3.0);
            var rect7 = new Rect(4.0, 3.0, 2.0, 3.0);

            Assert.AreNotEqual(rect1, rect2);

            Assert.IsFalse(rect1 == rect2);
            Assert.IsFalse(rect1.Equals(rect2));
            Assert.IsFalse(rect1.Equals((object)rect2));
            Assert.IsTrue(rect1 != rect2);
            Assert.AreNotEqual(rect1, rect2);

            Assert.IsTrue(rect2 == rect3);
            Assert.IsTrue(rect2.Equals(rect3));
            Assert.IsTrue(rect2.Equals((object)rect3));
            Assert.IsFalse(rect2 != rect3);
            Assert.AreEqual(rect2, rect3);

            Assert.IsFalse(rect2 == rect4);
            Assert.IsFalse(rect2.Equals(rect4));
            Assert.IsFalse(rect2.Equals((object)rect4));
            Assert.IsTrue(rect2 != rect4);
            Assert.AreNotEqual(rect2, rect4);

            Assert.AreEqual(rect4, rect4);

            Assert.IsTrue(rect4 == rect5);
            Assert.IsTrue(rect4.Equals(rect5));
            Assert.IsTrue(rect4.Equals((object)rect5));
            Assert.IsFalse(rect4 != rect5);
            Assert.AreEqual(rect4, rect5);

            Assert.IsFalse(rect4 == rect6);
            Assert.IsFalse(rect4.Equals(rect6));
            Assert.IsFalse(rect4.Equals((object)rect6));
            Assert.IsTrue(rect4 != rect6);
            Assert.AreNotEqual(rect4, rect6);

            Assert.IsFalse(rect4 == rect7);
            Assert.IsFalse(rect4.Equals(rect7));
            Assert.IsFalse(rect4.Equals((object)rect7));
            Assert.IsTrue(rect4 != rect7);
            Assert.AreNotEqual(rect4, rect7);

            Assert.IsFalse(rect4 == rect8);
            Assert.IsFalse(rect4.Equals(rect8));
            Assert.IsFalse(rect4.Equals((object)rect8));
            Assert.IsTrue(rect4 != rect8);
            Assert.AreNotEqual(rect4, rect8);

            Assert.AreNotEqual(null, rect4);
            Assert.AreNotEqual(new TestVertex(), rect4);
        }

        [Test]
        public void HashCode()
        {
            var rect1 = Rect.Empty;
            var rect2 = Rect.Empty;
            var rect3 = new Rect();
            var rect4 = new Rect();
            var rect5 = new Rect(1.0, 2.0, 2.0, 3.0);
            var rect6 = new Rect(1.0, 2.0, 2.0, 3.0);
            var rect7 = new Rect(1.0, 2.0, 3.0, 3.0);

            Assert.AreEqual(rect1.GetHashCode(), rect2.GetHashCode());
            Assert.AreEqual(rect1.GetHashCode(), rect3.GetHashCode());
            Assert.AreNotEqual(rect1.GetHashCode(), rect5.GetHashCode());
            Assert.AreEqual(rect3.GetHashCode(), rect4.GetHashCode());
            Assert.AreNotEqual(rect3.GetHashCode(), rect5.GetHashCode());
            Assert.AreEqual(rect5.GetHashCode(), rect6.GetHashCode());
            Assert.AreNotEqual(rect5.GetHashCode(), rect7.GetHashCode());
        }

        [Test]
        public void RectToString()
        {
            var rect = new Rect();
            Assert.AreEqual("0;0;0;0", rect.ToString());

            rect = new Rect(1, 2, 2, 3);
            Assert.AreEqual("1;2;2;3", rect.ToString());

            rect = new Rect(-6, 2, 1, 4);
            Assert.AreEqual("-6;2;1;4", rect.ToString());

            rect = Rect.Empty;
            Assert.AreEqual("Empty", rect.ToString());
        }
    }
}