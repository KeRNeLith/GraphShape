using System;
using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.OverlapRemoval;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Tests for <see cref="OverlapRemovalContext{TVertex}"/>.
    /// </summary>
    [TestFixture]
    internal class OverlapRemovalContextTests
    {
        [Test]
        public void Constructor()
        {
            var rectangles = new Dictionary<int, Rect>();
            var context = new OverlapRemovalContext<int>(rectangles);

            Assert.AreSame(rectangles, context.Rectangles);

            rectangles = new Dictionary<int, Rect>
            {
                [1] = new Rect(),
                [2] = new Rect(new Point(1.0, 2.0), new Size(12.0, 25.0))
            };
            context = new OverlapRemovalContext<int>(rectangles);

            Assert.AreSame(rectangles, context.Rectangles);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new OverlapRemovalContext<int>(null));
        }
    }
}