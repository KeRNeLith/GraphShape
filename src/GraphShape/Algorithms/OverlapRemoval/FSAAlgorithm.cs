using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using JetBrains.Annotations;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Fast Statistical Alignment algorithm (FSA).
    /// </summary>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <typeparam name="TParameters">Algorithm parameters type.</typeparam>
    public class FSAAlgorithm<TObject, TParameters> : OverlapRemovalAlgorithmBase<TObject, TParameters>
        where TParameters : IOverlapRemovalParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FSAAlgorithm{TObject,TParameters}"/> class.
        /// </summary>
        /// <param name="rectangles">Overlap rectangles.</param>
        /// <param name="parameters">Algorithm parameters.</param>
        public FSAAlgorithm(
            [NotNull] IDictionary<TObject, Rect> rectangles,
            [NotNull] TParameters parameters)
            : base(rectangles, parameters)
        {
        }

        #region OverlapRemovalAlgorithmBase

        /// <inheritdoc />
        protected override void RemoveOverlap()
        {
#if DEBUG
            DateTime t0 = DateTime.Now;

            double cost =
#endif
            HorizontalImproved();
#if DEBUG
            DateTime t1 = DateTime.Now;

            Debug.WriteLine($"PFS horizontal: cost={cost} time={t1 - t0}");

            t1 = DateTime.Now;
            cost =
#endif
            VerticalImproved();

#if DEBUG
            DateTime t2 = DateTime.Now;
            Debug.WriteLine($"PFS vertical: cost={cost} time={t2 - t1}");
            Debug.WriteLine($"PFS total: time={t2 - t0}");
#endif
        }

        #endregion

        /// <summary>
        /// Specifies the bounding force of the two rectangles.
        /// </summary>
        /// <param name="vi">First rectangle.</param>
        /// <param name="vj">Second rectangle.</param>
        /// <returns>Force vector.</returns>
        [Pure]
        protected Vector Force(Rect vi, Rect vj)
        {
            var force = new Vector(0, 0);
            Vector distance = vj.GetCenter() - vi.GetCenter();
            double absDistanceX = Math.Abs(distance.X);
            double absDistanceY = Math.Abs(distance.Y);
            double gij = distance.Y / distance.X;
            // ReSharper disable once InconsistentNaming
            double Gij = (vi.Height + vj.Height) / (vi.Width + vj.Width);

            if (Gij >= gij && gij > 0 || -Gij <= gij && gij < 0 || IsZero(gij))
            {
                // vi and vj touch with y-direction boundaries
                force.X = distance.X / absDistanceX * ((vi.Width + vj.Width) / 2.0 - absDistanceX);
                force.Y = force.X * gij;
            }

            if (Gij < gij && gij > 0 || -Gij > gij && gij < 0)
            {
                // vi and vj touch with x-direction boundaries
                force.Y = distance.Y / absDistanceY * ((vi.Height + vj.Height) / 2.0 - absDistanceY);
                force.X = force.Y / gij;
            }

            return force;
        }

        /// <summary>
        /// Specifies the bounding force of the two rectangles (version 2).
        /// </summary>
        /// <param name="vi">First rectangle.</param>
        /// <param name="vj">Second rectangle.</param>
        /// <returns>Force vector.</returns>
        [Pure]
        protected Vector Force2(Rect vi, Rect vj)
        {
            var force = new Vector(0, 0);
            Vector distance = vj.GetCenter() - vi.GetCenter();
            double gij = distance.Y / distance.X;
            if (vi.IntersectsWith(vj))
            {
                force.X = (vi.Width + vj.Width) / 2.0 - distance.X;
                force.Y = (vi.Height + vj.Height) / 2.0 - distance.Y;
                // In the X dimension
                if (force.X > force.Y && !IsZero(gij))
                {
                    force.X = force.Y / gij;
                }

                force.X = Math.Max(force.X, 0);
                force.Y = Math.Max(force.Y, 0);
            }

            return force;
        }

        /// <summary>
        /// Compares both rectangle center on X axis.
        /// </summary>
        /// <param name="r1">First rectangle.</param>
        /// <param name="r2">Second rectangle.</param>
        /// <returns>
        /// 0 if both rectangle have same X center,
        /// -1 if <paramref name="r1"/> X center is lower than <paramref name="r2"/>,
        /// 1 otherwise.
        /// </returns>
        [Pure]
        protected int XComparison([NotNull] RectangleWrapper<TObject> r1, [NotNull] RectangleWrapper<TObject> r2)
        {
            double r1CenterX = r1.CenterX;
            double r2CenterX = r2.CenterX;

            if (r1CenterX < r2CenterX)
                return -1;
            if (r1CenterX > r2CenterX)
                return 1;
            return 0;
        }

        /// <summary>
        /// Horizontal offset rectangles.
        /// </summary>
        protected void Horizontal()
        {
            WrappedRectangles.Sort(XComparison);
            int i = 0;
            int n = WrappedRectangles.Count;
            while (i < n)
            {
                // x_i = x_{i+1} = ... = x_k
                int k = i;
                RectangleWrapper<TObject> u = WrappedRectangles[i];
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

                for (int j = k + 1; j < n; ++j)
                {
                    RectangleWrapper<TObject> r = WrappedRectangles[j];
                    r.Rectangle.Offset(delta, 0);
                }

                i = k + 1;
            }
        }

        /// <summary>
        /// Horizontal improvement.
        /// </summary>
        protected double HorizontalImproved()
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
                        double gg = v.Rectangle.Left + ggg < leftMin.Rectangle.Left 
                            ? sigma
                            : ggg;
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

        /// <summary>
        /// Compares both rectangle center on Y axis.
        /// </summary>
        /// <param name="r1">First rectangle.</param>
        /// <param name="r2">Second rectangle.</param>
        /// <returns>
        /// 0 if both rectangle have same Y center,
        /// -1 if <paramref name="r1"/> Y center is lower than <paramref name="r2"/>,
        /// 1 otherwise.
        /// </returns>
        [Pure]
        protected int YComparison(
            [NotNull] RectangleWrapper<TObject> r1,
            [NotNull] RectangleWrapper<TObject> r2)
        {
            double r1CenterY = r1.CenterY;
            double r2CenterY = r2.CenterY;

            if (r1CenterY < r2CenterY)
                return -1;
            if (r1CenterY > r2CenterY)
                return 1;
            return 0;
        }

        /// <summary>
        /// Vertical offset rectangles.
        /// </summary>
        protected void Vertical()
        {
            WrappedRectangles.Sort(YComparison);
            int i = 0;
            int n = WrappedRectangles.Count;
            while (i < n)
            {
                // y_i = y_{i+1} = ... = y_k
                int k = i;
                RectangleWrapper<TObject> u = WrappedRectangles[i];
                for (int j = i; j < n; ++j)
                {
                    RectangleWrapper<TObject> v = WrappedRectangles[j];
                    if (NearEqual(u.CenterY, v.CenterY))
                    {
                        u = v;
                        k = j;
                    }
                    else
                    {
                        break;
                    }
                }

                // delta = max(0, max{f.y(m,j)|i<=m<=k<j<n})
                double delta = 0;
                for (int m = i; m <= k; ++m)
                {
                    for (int j = k + 1; j < n; ++j)
                    {
                        Vector force = Force2(WrappedRectangles[m].Rectangle, WrappedRectangles[j].Rectangle);
                        if (force.Y > delta)
                        {
                            delta = force.Y;
                        }
                    }
                }

                for (int j = k + 1; j < n; ++j)
                {
                    RectangleWrapper<TObject> r = WrappedRectangles[j];
                    r.Rectangle.Offset(0, delta);
                }

                i = k + 1;
            }
        }

        /// <summary>
        /// Vertical improvement.
        /// </summary>
        protected double VerticalImproved()
        {
            WrappedRectangles.Sort(YComparison);
            int i = 0;
            int n = WrappedRectangles.Count;

            RectangleWrapper<TObject> topMin = WrappedRectangles[0];
            double sigma = 0;
            double y0 = topMin.CenterY;
            var gamma = new double[WrappedRectangles.Count];
            var y = new double[WrappedRectangles.Count];
            while (i < n)
            {
                RectangleWrapper<TObject> u = WrappedRectangles[i];

                int k = i;
                for (int j = i + 1; j < n; ++j)
                {
                    RectangleWrapper<TObject> v = WrappedRectangles[j];
                    if (NearEqual(u.CenterY, v.CenterY))
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
                if (u.CenterY > y0)
                {
                    for (int m = i; m <= k; ++m)
                    {
                        double ggg = 0;
                        for (int j = 0; j < i; ++j)
                        {
                            Vector f = Force2(WrappedRectangles[j].Rectangle, WrappedRectangles[m].Rectangle);
                            ggg = Math.Max(f.Y + gamma[j], ggg);
                        }

                        RectangleWrapper<TObject> v = WrappedRectangles[m];
                        double gg = v.Rectangle.Top + ggg < topMin.Rectangle.Top 
                            ? sigma
                            : ggg;
                        g = Math.Max(g, gg);
                    }
                }

                for (int m = i; m <= k; ++m)
                {
                    gamma[m] = g;
                    RectangleWrapper<TObject> r = WrappedRectangles[m];
                    y[m] = r.Rectangle.Top + g;
                    if (r.Rectangle.Top < topMin.Rectangle.Top)
                    {
                        topMin = r;
                    }
                }

                // delta = max(0, max{f.x(m,j)|i<=m<=k<j<n})
                double delta = 0;
                for (int m = i; m <= k; ++m)
                {
                    for (int j = k + 1; j < n; ++j)
                    {
                        Vector force = Force(WrappedRectangles[m].Rectangle, WrappedRectangles[j].Rectangle);
                        if (force.Y > delta)
                        {
                            delta = force.Y;
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
                double oldPos = r.Rectangle.Top;
                double newPos = y[i];

                r.Rectangle.Y = newPos;

                double diff = oldPos - newPos;
                cost += diff * diff;
            }

            return cost;
        }
    }

    /// <inheritdoc />
    public class FSAAlgorithm<TObject> : FSAAlgorithm<TObject, IOverlapRemovalParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FSAAlgorithm{TObject}"/> class.
        /// </summary>
        /// <param name="rectangles">Overlap rectangles.</param>
        /// <param name="parameters">Algorithm parameters.</param>
        public FSAAlgorithm(
            [NotNull] IDictionary<TObject, Rect> rectangles,
            [NotNull] IOverlapRemovalParameters parameters)
            : base(rectangles, parameters)
        {
        }
    }
}