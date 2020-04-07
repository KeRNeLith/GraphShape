using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphShape.Algorithms.OverlapRemoval;
using JetBrains.Annotations;
using NUnit.Framework;
using static GraphShape.Tests.Algorithms.AlgorithmTestHelpers;

namespace GraphShape.Tests.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Tests for <see cref="FSAAlgorithm{TObject, TParameters}"/>.
    /// </summary>
    [TestFixture]
    internal class FSAAlgorithmTests : FSAAlgorithmTestsBase
    {
        #region Test classes

        private class TestFSAAlgorithm : FSAAlgorithm<int>
        {
            public TestFSAAlgorithm(
                [NotNull] IDictionary<int, Rect> rectangles,
                [NotNull] IOverlapRemovalParameters parameters)
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
            Constructor_Test<double, OverlapRemovalParameters, FSAAlgorithm<double, OverlapRemovalParameters>>(
                (rectangles, parameters) => new FSAAlgorithm<double, OverlapRemovalParameters>(rectangles, parameters));

            var rects = new Dictionary<double, Rect>();
            var param = new OverlapRemovalParameters();
            var algorithm = new FSAAlgorithm<double>(rects, param);
            AssertAlgorithmProperties(algorithm, rects, param);

            #region Local function

            void AssertAlgorithmProperties<TObject>(
                FSAAlgorithm<TObject> algo,
                IDictionary<TObject, Rect> r,
                IOverlapRemovalParameters p)
            {
                AssertAlgorithmState(algo);
                Assert.AreSame(r, algo.Rectangles);
                Assert.AreSame(p, algo.Parameters);
                Assert.AreSame(p, algo.GetParameters());
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var rectangles = new Dictionary<double, Rect>();
            var parameters = new OverlapRemovalParameters();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new FSAAlgorithm<double>(null, parameters));
            Assert.Throws<ArgumentNullException>(() => new FSAAlgorithm<double>(rectangles, null));
            Assert.Throws<ArgumentNullException>(() => new FSAAlgorithm<double>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Horizontal()
        {
            Dictionary<int, Rect> rectangles = GetRectangles();
            var algorithm = new TestFSAAlgorithm(rectangles, new OverlapRemovalParameters());

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
            var algorithm = new TestFSAAlgorithm(rectangles, new OverlapRemovalParameters());

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
                yield return new TestCaseData(new OverlapRemovalParameters());
                yield return new TestCaseData(new OverlapRemovalParameters
                {
                    HorizontalGap = 5,
                    VerticalGap = 4
                });
                yield return new TestCaseData(new OverlapRemovalParameters
                {
                    HorizontalGap = 25,
                    VerticalGap = 12
                });
            }
        }

        [TestCaseSource(nameof(ComputeTestCases))]
        public void Compute([NotNull] OverlapRemovalParameters parameters)
        {
            Dictionary<int, Rect> rectangles = GetRectangles();

            Dictionary<int, Rect> initialRectangles = rectangles.ToDictionary(
                pair => pair.Key,
                pair => new Rect(pair.Value.Location, pair.Value.Size));

            var algorithm = new FSAAlgorithm<int>(rectangles, parameters);
            algorithm.Compute();

            foreach (KeyValuePair<int, Rect> pair in algorithm.Rectangles)
            {
                Rect initialRectangle = initialRectangles[pair.Key];
                // Size must not change
                Assert.AreEqual(initialRectangle.Size, pair.Value.Size);

                // Fulfill minimum spacing
                AssertNoOverlap(Distinct(algorithm.Rectangles.Values));
                foreach (KeyValuePair<int, Rect> innerPair in algorithm.Rectangles.Where(p => pair.Key != p.Key))
                {
                    if (NearEqual(pair.Value, innerPair.Value))
                        continue;

                    if (Math.Abs(pair.Value.Location.X - innerPair.Value.Location.X) >= parameters.HorizontalGap)
                        continue;

                    if (Math.Abs(pair.Value.Location.Y - innerPair.Value.Location.Y) >= parameters.VerticalGap)
                        continue;

                    Assert.Fail("Minimum spacing not fulfilled.");
                }
            }
        }
    }
}