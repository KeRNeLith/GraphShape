using System;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.Layout;
using GraphShape.Utils;
using JetBrains.Annotations;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests related to <see cref="LayoutUtils"/>.
    ///</summary>
    [TestFixture]
    internal class LayoutUtilsTests
    {
        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> FillWithRandomPositionsTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(
                    15, 15, 0, 0,
                    new[] { 1, 2, 3, 4, 5 },
                    new Dictionary<int, Point>
                    {
                        [1] = new Point(1, 2),
                        [2] = new Point(2, 2),
                        [3] = new Point(3, 4),
                        [4] = new Point(7, 15),
                        [5] = new Point(16, 9)
                    });

                yield return new TestCaseData(
                    15, 15, 0, 0,
                    new[] { 1, 2, 3, 4, 5 },
                    new Dictionary<int, Point>());

                yield return new TestCaseData(
                    15, 15, 0, 0,
                    new[] { 1, 2, 3, 4, 5 },
                    new Dictionary<int, Point>
                    {
                        [1] = new Point(1, 2),
                        [4] = new Point(7, 15),
                        [5] = new Point(16, 9)
                    });

                yield return new TestCaseData(
                    15, 25, 0, 0,
                    new[] { 1, 2, 3, 4, 5 },
                    new Dictionary<int, Point>
                    {
                        [1] = new Point(1, 2),
                        [5] = new Point(16, 9)
                    });

                yield return new TestCaseData(
                    15, 25, 5, 7,
                    new[] { 1, 2, 3, 4, 5 },
                    new Dictionary<int, Point>
                    {
                        [1] = new Point(1, 2),
                        [5] = new Point(16, 9)
                    });
            }
        }

        [TestCaseSource(nameof(FillWithRandomPositionsTestCases))]
        public void FillWithRandomPositions(
            double width,
            double height,
            double translateX,
            double translateY,
            [NotNull] IEnumerable<int> vertices,
            [NotNull] IDictionary<int, Point> verticesPositions)
        {
            int[] verticesArray = vertices as int[] ?? vertices.ToArray();
            int[] initialVertices = verticesPositions.Keys.ToArray();
            LayoutUtils.FillWithRandomPositions(
                width,
                height,
                translateX,
                translateY,
                verticesArray,
                verticesPositions,
                new Random(123));

            Assert.GreaterOrEqual(verticesPositions.Count, verticesArray.Length);
            foreach (int vertex in verticesArray)
            {
                CollectionAssert.Contains(verticesPositions.Keys, vertex);
            }

            // Only on added vertices positions
            foreach (int vertex in verticesArray.Except(initialVertices))
            {
                Assert.GreaterOrEqual(verticesPositions[vertex].X, translateX);
                Assert.LessOrEqual(verticesPositions[vertex].X, width + translateX);

                Assert.GreaterOrEqual(verticesPositions[vertex].Y, translateY);
                Assert.LessOrEqual(verticesPositions[vertex].Y, height + translateY);
            }
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> NormalizePositionsTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(null)
                {
                    ExpectedResult = null
                };

                yield return new TestCaseData(new Dictionary<int, Point>())
                {
                    ExpectedResult = new Dictionary<int, Point>()
                };

                yield return new TestCaseData(
                    new Dictionary<int, Point>
                    {
                        [1] = new Point(5, 2),
                        [2] = new Point(9, 1),
                        [3] = new Point(3, 4),
                        [4] = new Point(7, 15),
                        [5] = new Point(16, 9)
                    })
                {
                    ExpectedResult = new Dictionary<int, Point>
                    {
                        [1] = new Point(2, 1),
                        [2] = new Point(6, 0),
                        [3] = new Point(0, 3),
                        [4] = new Point(4, 14),
                        [5] = new Point(13, 8)
                    }
                };

                yield return new TestCaseData(
                    new Dictionary<int, Point>
                    {
                        [1] = new Point(7, 6),
                        [2] = new Point(8, 6),
                        [4] = new Point(5, 5),
                        [5] = new Point(4, 7),
                        [10] = new Point(12, 4)
                    })
                {
                    ExpectedResult = new Dictionary<int, Point>
                    {
                        [1] = new Point(3, 2),
                        [2] = new Point(4, 2),
                        [4] = new Point(1, 1),
                        [5] = new Point(0, 3),
                        [10] = new Point(8, 0)
                    }
                };
            }
        }

        [NotNull]
        [TestCaseSource(nameof(NormalizePositionsTestCases))]
        public IDictionary<int, Point> NormalizePositions([NotNull] IDictionary<int, Point> verticesPositions)
        {
            LayoutUtils.NormalizePositions(verticesPositions);
            return verticesPositions;
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> IsSameDirectionTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new Vector(), new Vector())
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(new Vector(10, 5), new Vector(10, 5))
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(new Vector(-10, 5), new Vector(-10, 5))
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(new Vector(15, 2), new Vector(10, 5))
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(new Vector(-15, 2), new Vector(-10, 5))
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(new Vector(15, -2), new Vector(10, -5))
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(new Vector(-1, -2), new Vector(-10, -5))
                {
                    ExpectedResult = true
                };

                yield return new TestCaseData(new Vector(15, -2), new Vector(10, 0))
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(new Vector(15, -2), new Vector(10, 1))
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(new Vector(1, 1), new Vector(-1, -1))
                {
                    ExpectedResult = false
                };

                yield return new TestCaseData(new Vector(-1, 1), new Vector(-1, -1))
                {
                    ExpectedResult = false
                };
            }
        }

        [TestCaseSource(nameof(IsSameDirectionTestCases))]
        public bool IsSameDirection(Vector a, Vector b)
        {
            return LayoutUtils.IsSameDirection(a, b);
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> GetClippingPointTestCases
        {
            [UsedImplicitly]
            get
            {
                // Target outside source rect clipping point on corner
                yield return new TestCaseData(new Size(10, 10), new Point(5, 5), new Point(20, 20), new Point(10, 10));

                // Target outside source rect clipping point on top side
                yield return new TestCaseData(new Size(20, 10), new Point(5, 5), new Point(20, 20), new Point(10, 10));

                // Target outside source rect clipping point on right side
                yield return new TestCaseData(new Size(4, 2), new Point(2, 3), new Point(6, 4), new Point(4, 3.5));

                // Target outside source rect clipping point on left side
                yield return new TestCaseData(new Size(2, 4), new Point(-1, 2), new Point(-5, 6), new Point(-2, 3));

                // Target outside source rect clipping point on bottom side
                yield return new TestCaseData(new Size(10, 10), new Point(5, 5), new Point(20, 30), new Point(8, 10));

                // Target inside source rect clipping point on bottom side
                yield return new TestCaseData(new Size(10, 10), new Point(5, 5), new Point(5.3, 5.5), new Point(8, 10));

                // Target inside source rect clipping point on left side
                yield return new TestCaseData(new Size(10, 10), new Point(2.5, 2.5), new Point(0, 0), new Point(-2.5, -2.5));

                // Target inside source rect clipping point on top side
                yield return new TestCaseData(new Size(4, 2), new Point(0, 0), new Point(1, 0.5), new Point(2, 1));

                // Target on source rect clipping point on corner side
                yield return new TestCaseData(new Size(2, 3), new Point(1, 3), new Point(2, 4), new Point(2, 4));
            }
        }

        [TestCaseSource(nameof(GetClippingPointTestCases))]
        public void GetClippingPoint(Size size, Point center, Point p, Point expected)
        {
            Point actual = LayoutUtils.GetClippingPoint(size, center, p);
            Assert.IsTrue(MathUtils.NearEqual(actual.X, expected.X));
            Assert.IsTrue(MathUtils.NearEqual(actual.Y, expected.Y));
        }
    }
}
