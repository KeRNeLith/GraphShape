using System.Linq;
using GraphShape.Controls.Extensions;
using NUnit.Framework;

namespace GraphShape.Controls.Tests
{
    /// <summary>
    /// Tests for <see cref="BasicStructuresExtensions"/>.
    /// </summary>
    [TestFixture]
    internal class BasicStructuresExtensionsTests
    {
        #region Test helpers

        private static void AssertEqual(System.Windows.Point point, Point gsPoint)
        {
            Assert.AreEqual(gsPoint.X, point.X);
            Assert.AreEqual(gsPoint.Y, point.Y);
        }

        #endregion

        [Test]
        public void ToPoint()
        {
            var gsPoint = new Point();
            System.Windows.Point point = gsPoint.ToPoint();
            AssertEqual(point, gsPoint);

            gsPoint = new Point(1, -2);
            point = gsPoint.ToPoint();
            AssertEqual(point, gsPoint);
        }

        [Test]
        public void ToPoints()
        {
            var gsPoints = new[] { new Point(), new Point(-1, 2) };
            var points = gsPoints.ToPoints().ToArray();

            for (int i = 0; i < gsPoints.Length; ++i)
            {
                AssertEqual(points[i], gsPoints[i]);
            }

            gsPoints = new Point[] { };
            CollectionAssert.IsEmpty(gsPoints.ToPoints());
        }

        [Test]
        public void ToGraphShapePoint()
        {
            var point = new System.Windows.Point();
            Point gsPoint = point.ToGraphShapePoint();
            AssertEqual(point, gsPoint);

            point = new System.Windows.Point(1, -2);
            gsPoint = point.ToGraphShapePoint();
            AssertEqual(point, gsPoint);
        }

        [Test]
        public void ToGraphShapePoints()
        {
            var points = new[] { new System.Windows.Point(), new System.Windows.Point(-1, 2) };
            var gsPoints = points.ToGraphShapePoints().ToArray();

            for (int i = 0; i < points.Length; ++i)
            {
                AssertEqual(points[i], gsPoints[i]);
            }

            points = new System.Windows.Point[] { };
            CollectionAssert.IsEmpty(points.ToGraphShapePoints());
        }
    }
}
