using System;
using NUnit.Framework;

namespace GraphShape.Tests
{
    /// <summary>
    /// Tests for <see cref="Size"/>.
    /// </summary>
    [TestFixture]
    internal class SizeTests
    {
        [Test]
        public void Constructor()
        {
            var size = new Size();
            Assert.AreEqual(0, size.Width);
            Assert.AreEqual(0, size.Height);
            Assert.IsFalse(size.IsEmpty);

            size = new Size(1, 1.5);
            Assert.AreEqual(1, size.Width);
            Assert.AreEqual(1.5, size.Height);
            Assert.IsFalse(size.IsEmpty);

            size = new Size(1.6, 3.4);
            Assert.AreEqual(1.6, size.Width);
            Assert.AreEqual(3.4, size.Height);
            Assert.IsFalse(size.IsEmpty);

            Assert.IsTrue(Size.Empty.IsEmpty);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new Size(1, -2));
            Assert.Throws<ArgumentException>(() => new Size(-1, 2));
            Assert.Throws<ArgumentException>(() => new Size(-1, -2));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Update()
        {
            var size = new Size(1, 2);

            Assert.AreEqual(new Size(1, 2), size);

            size.Width = 12;
            Assert.AreEqual(new Size(12, 2), size);

            size.Height = 25;
            Assert.AreEqual(new Size(12, 25), size);

            size.Width = 42;
            Assert.AreEqual(new Size(42, 25), size);
        }

        [Test]
        public void Update_Throws()
        {
            var size = new Size(1, 2);
            var empty = Size.Empty;

            Assert.Throws<ArgumentOutOfRangeException>(() => size.Width = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => size.Height = -1);
            Assert.Throws<InvalidOperationException>(() => empty.Width = 1);
            Assert.Throws<InvalidOperationException>(() => empty.Height = 1);
        }

        [Test]
        public void Equals()
        {
            var size1 = new Size();
            var size2 = Size.Empty;
            var size3 = Size.Empty;
            var size4 = new Size(1.0, 2.0);
            var size5 = new Size(1.0, 2.0);
            var size6 = new Size(4.0, 3.0);

            Assert.AreNotEqual(size1, size2);

            Assert.IsFalse(size1 == size2);
            Assert.IsFalse(size1.Equals(size2));
            Assert.IsFalse(size1.Equals((object)size2));
            Assert.IsTrue(size1 != size2);
            Assert.AreNotEqual(size1, size2);

            Assert.IsTrue(size2 == size3);
            Assert.IsTrue(size2.Equals(size3));
            Assert.IsTrue(size2.Equals((object)size3));
            Assert.IsFalse(size2 != size3);
            Assert.AreEqual(size2, size3);

            Assert.IsFalse(size2 == size4);
            Assert.IsFalse(size2.Equals(size4));
            Assert.IsFalse(size2.Equals((object)size4));
            Assert.IsTrue(size2 != size4);
            Assert.AreNotEqual(size2, size4);

            Assert.AreEqual(size4, size4);

            Assert.IsTrue(size4 == size5);
            Assert.IsTrue(size4.Equals(size5));
            Assert.IsTrue(size4.Equals((object)size5));
            Assert.IsFalse(size4 != size5);
            Assert.AreEqual(size4, size5);

            Assert.IsFalse(size4 == size6);
            Assert.IsFalse(size4.Equals(size6));
            Assert.IsFalse(size4.Equals((object)size6));
            Assert.IsTrue(size4 != size6);
            Assert.AreNotEqual(size4, size6);

            Assert.AreNotEqual(size4, null);
            Assert.AreNotEqual(size4, new TestVertex());
        }

        [Test]
        public void HashCode()
        {
            var size1 = Size.Empty;
            var size2 = Size.Empty;
            var size3 = new Size();
            var size4 = new Size();
            var size5 = new Size(1.0, 2.0);
            var size6 = new Size(1.0, 2.0);

            Assert.AreEqual(size1.GetHashCode(), size2.GetHashCode());
            Assert.AreEqual(size1.GetHashCode(), size3.GetHashCode());
            Assert.AreNotEqual(size1.GetHashCode(), size5.GetHashCode());
            Assert.AreEqual(size3.GetHashCode(), size4.GetHashCode());
            Assert.AreNotEqual(size3.GetHashCode(), size5.GetHashCode());
            Assert.AreEqual(size5.GetHashCode(), size6.GetHashCode());
        }

        [Test]
        public void SizeToString()
        {
            var size = new Size();
            Assert.AreEqual("0;0", size.ToString());

            size = new Size(1, 2);
            Assert.AreEqual("1;2", size.ToString());

            size = new Size(6, 2);
            Assert.AreEqual("6;2", size.ToString());

            size = Size.Empty;
            Assert.AreEqual("Empty", size.ToString());
        }
    }
}