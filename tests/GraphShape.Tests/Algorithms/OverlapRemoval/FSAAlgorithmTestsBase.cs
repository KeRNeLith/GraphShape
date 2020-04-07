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
    /// Base class for FSA algorithm tests.
    /// </summary>
    internal class FSAAlgorithmTestsBase
    {
        #region Test helpers

        [Pure]
        [NotNull]
        protected static Dictionary<int, Rect> GetRectangles()
        {
            return new Dictionary<int, Rect>
            {
                [1] = new Rect(new Point(-5, 7), new Size(3, 2)),
                [2] = new Rect(new Point(-5, 7), new Size(3, 2)),
                [3] = new Rect(new Point(0.5, 4), new Size(3, 1.5)),
                [4] = new Rect(new Point(0.5, 1.5), new Size(2.5, 2)),
                [5] = new Rect(new Point(5.5, 9), new Size(2, 2)),
                [6] = new Rect(new Point(2.5, -1.5), new Size(3, 2)),
                [7] = new Rect(new Point(-5.5, -1.5), new Size(3.5, 2)),
                [8] = new Rect(new Point(-6.5, -2.5), new Size(3.5, 2)),
                [9] = new Rect(new Point(-2.5, 3), new Size(3, 2))
            };
        }

        private const double Epsilon = 0.0001;

        protected static bool NearEqual(Rect rect1, Rect rect2)
        {
            return rect1.Size == rect2.Size
                   && Math.Abs(rect1.Location.X - rect2.Location.X) < Epsilon
                   && Math.Abs(rect1.Location.Y - rect2.Location.Y) < Epsilon;
        }

        private class RectangleEqualityComparer : IEqualityComparer<Rect>
        {
            public bool Equals(Rect x, Rect y)
            {
                return NearEqual(x, y);
            }

            public int GetHashCode(Rect obj)
            {
                return 1;   // Force usage of Equals
            }
        }

        [Pure]
        [NotNull]
        protected static IEnumerable<Rect> Distinct([NotNull] IEnumerable<Rect> rectangles)
        {
            return rectangles.Distinct(new RectangleEqualityComparer());
        }

        protected static void AssertNoOverlap([NotNull] IEnumerable<Rect> rectangles)
        {
            Rect[] rects = rectangles.ToArray();
            List<Rect> rectsToCheck = rects.Skip(1).ToList();
            foreach (Rect rect1 in rects)
            {
                foreach (Rect rect2 in rectsToCheck)
                {
                    Assert.IsFalse(Overlap(rect1, rect2), "Rectangle overlap.");
                }

                if (rectsToCheck.Count > 0)
                    rectsToCheck.RemoveAt(0);
            }
        }

        private static bool Overlap(Rect rect1, Rect rect2)
        {
            Point l1 = rect1.TopLeft;
            Point r1 = new Point(rect1.X + rect1.Width, rect1.Y - rect1.Height);
            Point l2 = rect2.TopLeft;
            Point r2 = new Point(rect2.X + rect2.Width, rect2.Y - rect2.Height);

            // If one rectangle is on left side of other 
            if (l1.X > r2.X || l2.X > r1.X)
                return false;

            // If one rectangle is above other 
            if (l1.Y < r2.Y || l2.Y < r1.Y)
                return false;

            return true;
        }

        #endregion

        protected static void Constructor_Test<TObject, TParams, TAlgorithm>(
            [NotNull, InstantHandle] Func<IDictionary<TObject, Rect>, TParams, TAlgorithm> createAlgorithm)
            where TParams : IOverlapRemovalParameters, new()
            where TAlgorithm : FSAAlgorithm<TObject, TParams>
        {
            var rectangles = new Dictionary<TObject, Rect>();
            var parameters = new TParams();
            TAlgorithm algorithm = createAlgorithm(rectangles, parameters);

            AssertAlgorithmProperties(algorithm, rectangles, parameters);

            #region Local function

            void AssertAlgorithmProperties(
                TAlgorithm algo,
                IDictionary<TObject, Rect> rects,
                TParams p)
            {
                AssertAlgorithmState(algo);
                Assert.AreSame(rects, algo.Rectangles);
                Assert.AreSame(p, algo.Parameters);
                Assert.AreSame(p, algo.GetParameters());
            }

            #endregion
        }
    }
}