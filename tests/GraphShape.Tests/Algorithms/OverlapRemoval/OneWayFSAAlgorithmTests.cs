using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphShape.Algorithms.OverlapRemoval;
using JetBrains.Annotations;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Tests for <see cref="OneWayFSAAlgorithm{TObject}"/>.
    /// </summary>
    [TestFixture]
    internal class OneWayFSAAlgorithmTests : FSAAlgorithmTestsBase
    {
        #region Test classes

        private class TestOneWayFSAAlgorithm : OneWayFSAAlgorithm<int>
        {
            public TestOneWayFSAAlgorithm(
                [NotNull] IDictionary<int, Rect> rectangles,
                [NotNull] OneWayFSAParameters parameters)
                : base(rectangles, parameters)
            {
            }

            public void Horizontal_Test()
            {
                Horizontal();

                foreach (RectangleWrapper<int> rectangle in WrappedRectangles)
                    Rectangles[rectangle.Id] = rectangle.Rectangle;
            }

            public void Vertical_Test()
            {
                Vertical();

                foreach (RectangleWrapper<int> rectangle in WrappedRectangles)
                    Rectangles[rectangle.Id] = rectangle.Rectangle;
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            Constructor_Test<double, OneWayFSAParameters, OneWayFSAAlgorithm<double>>(
                (rectangles, parameters) => new OneWayFSAAlgorithm<double>(rectangles, parameters));
        }

        [Test]
        public void Constructor_Throws()
        {
            var rectangles = new Dictionary<double, Rect>();
            var parameters = new OneWayFSAParameters();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new OneWayFSAAlgorithm<double>(null, parameters));
            Assert.Throws<ArgumentNullException>(() => new OneWayFSAAlgorithm<double>(rectangles, null));
            Assert.Throws<ArgumentNullException>(() => new OneWayFSAAlgorithm<double>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Horizontal()
        {
            Dictionary<int, Rect> rectangles = GetRectangles();
            var algorithm = new TestOneWayFSAAlgorithm(rectangles, new OneWayFSAParameters());

            Dictionary<int, Rect> initialRectangles = rectangles.ToDictionary(
                pair => pair.Key,
                pair => new Rect(pair.Value.Location, pair.Value.Size));

            algorithm.Horizontal_Test();

            foreach (KeyValuePair<int, Rect> pair in algorithm.Rectangles)
            {
                Rect initialRectangle = initialRectangles[pair.Key];
                // Only X may have changed
                Assert.AreEqual(initialRectangle.Location.Y, pair.Value.Location.Y);
                Assert.AreEqual(initialRectangle.Size, pair.Value.Size);
            }
        }

        [Test]
        public void Vertical()
        {
            Dictionary<int, Rect> rectangles = GetRectangles();
            var algorithm = new TestOneWayFSAAlgorithm(rectangles, new OneWayFSAParameters());

            Dictionary<int, Rect> initialRectangles = rectangles.ToDictionary(
                pair => pair.Key,
                pair => new Rect(pair.Value.Location, pair.Value.Size));

            algorithm.Vertical_Test();

            foreach (KeyValuePair<int, Rect> pair in algorithm.Rectangles)
            {
                Rect initialRectangle = initialRectangles[pair.Key];
                // Only Y may have changed
                Assert.AreEqual(initialRectangle.Location.X, pair.Value.Location.X);
                Assert.AreEqual(initialRectangle.Size, pair.Value.Size);
            }
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> ComputeTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new OneWayFSAParameters
                {
                    Way = OneWayFSAWay.Horizontal
                });
                yield return new TestCaseData(new OneWayFSAParameters
                {
                    HorizontalGap = 5,
                    VerticalGap = 4,
                    Way = OneWayFSAWay.Horizontal
                });
                yield return new TestCaseData(new OneWayFSAParameters
                {
                    HorizontalGap = 25,
                    VerticalGap = 12,
                    Way = OneWayFSAWay.Horizontal
                });

                yield return new TestCaseData(new OneWayFSAParameters
                {
                    Way = OneWayFSAWay.Vertical
                });
                yield return new TestCaseData(new OneWayFSAParameters
                {
                    HorizontalGap = 5,
                    VerticalGap = 4,
                    Way = OneWayFSAWay.Vertical
                });
                yield return new TestCaseData(new OneWayFSAParameters
                {
                    HorizontalGap = 25,
                    VerticalGap = 12,
                    Way = OneWayFSAWay.Vertical
                });
            }
        }

        [TestCaseSource(nameof(ComputeTestCases))]
        public void Compute([NotNull] OneWayFSAParameters parameters)
        {
            Dictionary<int, Rect> rectangles = GetRectangles();

            Dictionary<int, Rect> initialRectangles = rectangles.ToDictionary(
                pair => pair.Key,
                pair => new Rect(pair.Value.Location, pair.Value.Size));

            var algorithm = new OneWayFSAAlgorithm<int>(rectangles, parameters);
            algorithm.Compute();

            // Fulfill minimum spacing
            AssertNoOverlap(Distinct(algorithm.Rectangles.Values));
            foreach (KeyValuePair<int, Rect> pair in algorithm.Rectangles)
            {
                Rect initialRectangle = initialRectangles[pair.Key];
                // Size must not change
                Assert.AreEqual(initialRectangle.Size, pair.Value.Size);

                // X or Y must not change depending on the used Way
                if (parameters.Way == OneWayFSAWay.Horizontal)
                    Assert.AreEqual(initialRectangle.Y, pair.Value.Y);
                else
                    Assert.AreEqual(initialRectangle.X, pair.Value.X);
            }
        }
    }
}