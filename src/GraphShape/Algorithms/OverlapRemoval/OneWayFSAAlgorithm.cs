using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// One way Fast Statistical Alignment algorithm (FSA).
    /// </summary>
    /// <typeparam name="TObject">Object type.</typeparam>
    public class OneWayFSAAlgorithm<TObject> : FSAAlgorithm<TObject, OneWayFSAParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayFSAAlgorithm{TObject}"/> class.
        /// </summary>
        /// <param name="rectangles">Overlap rectangles.</param>
        /// <param name="parameters">Algorithm parameters.</param>
        public OneWayFSAAlgorithm(
            [NotNull] IDictionary<TObject, Rect> rectangles,
            [NotNull] OneWayFSAParameters parameters)
            : base(rectangles, parameters)
        {
        }

        #region OverlapRemovalAlgorithmBase

        /// <inheritdoc />
        protected override void RemoveOverlap()
        {
            switch (Parameters.Way)
            {
                case OneWayFSAWay.Horizontal:
                    HorizontalImproved();
                    break;
                case OneWayFSAWay.Vertical:
                    VerticalImproved();
                    break;
            }
        }

        #endregion

        /// <inheritdoc cref="FSAAlgorithm{TObject,TParameters}.HorizontalImproved"/>
        protected new double HorizontalImproved()
        {
            WrappedRectangles.Sort(XComparison);
            int i = 0;
            int n = WrappedRectangles.Count;

            // Left side
            RectangleWrapper<TObject> leftMin = WrappedRectangles[0];
            double sigma = 0;
            double x0 = leftMin.CenterX;
            var gamma = new double[WrappedRectangles.Count];
            var x = new double[WrappedRectangles.Count];
            while (i < n)
            {
                RectangleWrapper<TObject> u = WrappedRectangles[i];

                // Rectangle with the same center than Rectangle[i]
                int k = i;
                for (int j = i + 1; j < n; ++j)
                {
                    RectangleWrapper<TObject> v = WrappedRectangles[j];
                    if (NearEqual(u.CenterX, v.CenterX))
                    {
                        u = v;
                        k = j;
                    }
                    else
                    {
                        break;
                    }
                }

                double g = 0;
                for (int z = i + 1; z <= k; ++z)
                {
                    RectangleWrapper<TObject> v = WrappedRectangles[z];
                    v.Rectangle.X += (z - i) * 0.0001;
                }

                // For rectangles in [i, k], compute the left force
                if (u.CenterX > x0)
                {
                    for (int m = i; m <= k; ++m)
                    {
                        double ggg = 0;
                        for (int j = 0; j < i; ++j)
                        {
                            Vector force = Force(WrappedRectangles[j].Rectangle, WrappedRectangles[m].Rectangle);
                            ggg = Math.Max(force.X + gamma[j], ggg);
                        }

                        RectangleWrapper<TObject> v = WrappedRectangles[m];
                        double gg = v.Rectangle.Left + ggg < leftMin.Rectangle.Left ? sigma : ggg;
                        g = Math.Max(g, gg);
                    }
                }

                // Compute offset to elements in x
                // and redefine left side
                for (int m = i; m <= k; ++m)
                {
                    gamma[m] = g;
                    RectangleWrapper<TObject> r = WrappedRectangles[m];
                    x[m] = r.Rectangle.Left + g;
                    if (r.Rectangle.Left < leftMin.Rectangle.Left)
                    {
                        leftMin = r;
                    }
                }

                // Compute the right force of rectangles in [i, k] and store the maximal one
                // delta = max(0, max{f.x(m,j)|i<=m<=k<j<n})
                double delta = 0;
                for (int m = i; m <= k; ++m)
                {
                    for (int j = k + 1; j < n; ++j)
                    {
                        Vector force = Force(WrappedRectangles[m].Rectangle, WrappedRectangles[j].Rectangle);
                        if (force.X > delta)
                        {
                            delta = force.X;
                        }
                    }
                }
                sigma += delta;
                i = k + 1;
            }

            double cost = 0;
            for (i = 0; i < n; ++i)
            {
                RectangleWrapper<TObject> r = WrappedRectangles[i];
                double oldPos = r.Rectangle.Left;
                double newPos = x[i];

                r.Rectangle.X = newPos;

                double diff = oldPos - newPos;
                cost += diff * diff;
            }
            return cost;
        }
    }
}