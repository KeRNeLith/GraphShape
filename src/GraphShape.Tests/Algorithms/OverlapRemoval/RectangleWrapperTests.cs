using System;
using System.Windows;
using GraphShape.Algorithms.OverlapRemoval;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Tests for <see cref="RectangleWrapper{TObject}"/>.
    /// </summary>
    [TestFixture]
    internal class RectangleWrapperTests
    {
        [Test]
        public void Constructor()
        {
            var rect = new Rect(new Point(10, 15), new Size(25, 50));
            var wrapper = new RectangleWrapper<double>(1.5, rect);

            Assert.AreEqual(1.5, wrapper.Id);
            Assert.AreEqual(rect, wrapper.Rectangle);
            Assert.AreEqual(new Point(22.5, 40.0), wrapper.Center);
            Assert.AreEqual(22.5, wrapper.CenterX);
            Assert.AreEqual(40, wrapper.CenterY);

            rect = new Rect(new Point(-15.0, 5.0), new Size(40, 12.2));
            wrapper = new RectangleWrapper<double>(-7.0, rect);

            Assert.AreEqual(-7.0, wrapper.Id);
            Assert.AreEqual(rect, wrapper.Rectangle);
            Assert.AreEqual(new Point(5.0, 11.1), wrapper.Center);
            Assert.AreEqual(5.0, wrapper.CenterX);
            Assert.AreEqual(11.1, wrapper.CenterY);

            rect = new Rect(new Point(5.0, -5.0), new Size(4.4, 4.4));
            var wrapper2 = new RectangleWrapper<int>(72, rect);

            Assert.AreEqual(72, wrapper2.Id);
            Assert.AreEqual(rect, wrapper2.Rectangle);
            Assert.AreEqual(new Point(7.2, -2.8), wrapper2.Center);
            Assert.AreEqual(7.2, wrapper2.CenterX);
            Assert.AreEqual(-2.8, wrapper2.CenterY);
        }

        [Test]
        public void Constructor_Throws()
        {
            var rect = new Rect(new Point(10, 15), new Size(25, 50));
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new RectangleWrapper<TestVertex>(null, rect));
        }

        [Test]
        public void RectUpdate()
        {
            var rect = new Rect(new Point(10, 15), new Size(25, 50));
            var wrapper = new RectangleWrapper<double>(1.5, rect);

            Assert.AreEqual(1.5, wrapper.Id);
            Assert.AreEqual(rect, wrapper.Rectangle);
            Assert.AreEqual(new Point(22.5, 40.0), wrapper.Center);
            Assert.AreEqual(22.5, wrapper.CenterX);
            Assert.AreEqual(40, wrapper.CenterY);

            rect = new Rect(new Point(-15.0, 5.0), new Size(40, 12.2));
            wrapper.Rectangle = rect;

            Assert.AreEqual(1.5, wrapper.Id);
            Assert.AreEqual(rect, wrapper.Rectangle);
            Assert.AreEqual(new Point(5.0, 11.1), wrapper.Center);
            Assert.AreEqual(5.0, wrapper.CenterX);
            Assert.AreEqual(11.1, wrapper.CenterY);
        }
    }
}