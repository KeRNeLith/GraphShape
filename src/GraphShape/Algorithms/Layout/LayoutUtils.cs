using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using static GraphShape.Utils.MathUtils;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Utilities for layout operations.
    /// </summary>
    public static class LayoutUtils
    {
        /// <summary>
        /// Initializes the positions of the vertices. Assigns a random position inside the 'bounding box' to the vertices without positions.
        /// It does NOT modify the position of the other vertices.
        /// Bounding box:
        /// x coordinates: <see cref="double.Epsilon"/> - <paramref name="width"/>
        /// y coordinates: <see cref="double.Epsilon"/> - <paramref name="height"/>
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="width">Width of the bounding box.</param>
        /// <param name="height">Height of the bounding box.</param>
        /// <param name="translateX">Translates the generated x coordinate.</param>
        /// <param name="translateY">Translates the generated y coordinate.</param>
        /// <param name="vertices">Vertices to fill positions.</param>
        /// <param name="verticesPositions">Initial vertices positions.</param>
        public static void FillWithRandomPositions<TVertex>(
            double width,
            double height,
            double translateX,
            double translateY,
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices,
            [NotNull] IDictionary<TVertex, Point> verticesPositions)
        {
            var random = new Random(DateTime.Now.Millisecond);

            // Initialize with random position
            foreach (TVertex vertex in vertices)
            {
                // For vertices without assigned position
                if (!verticesPositions.ContainsKey(vertex))
                {
                    verticesPositions.Add(
                        vertex,
                        new Point(
                            Math.Max(double.Epsilon, random.NextDouble() * width + translateX),
                            Math.Max(double.Epsilon, random.NextDouble() * height + translateY)));
                }
            }
        }

        /// <summary>
        /// Normalizes the given <paramref name="vertexPositions"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="vertexPositions">Vertices positions to normalize.</param>
        public static void NormalizePositions<TVertex>([CanBeNull] IDictionary<TVertex, Point> vertexPositions)
        {
            if (vertexPositions is null || vertexPositions.Count == 0)
                return;

            // Get the topLeft position
            var topLeft = new Point(float.PositiveInfinity, float.PositiveInfinity);
            foreach (Point position in vertexPositions.Values)
            {
                topLeft.X = Math.Min(topLeft.X, position.X);
                topLeft.Y = Math.Min(topLeft.Y, position.Y);
            }

            // Translate with the topLeft position
            foreach (TVertex vertex in vertexPositions.Keys.ToArray())
            {
                Point pos = vertexPositions[vertex];
                pos.X -= topLeft.X;
                pos.Y -= topLeft.Y;
                vertexPositions[vertex] = pos;
            }
        }

        /// <summary>
        /// Checks if both <see cref="Vector"/>s have the same direction.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>True if both vectors have the same direction, false otherwise.</returns>
        [Pure]
        public static bool IsSameDirection(Vector a, Vector b)
        {
            return Math.Sign(a.X) == Math.Sign(b.X)
                   && Math.Sign(a.Y) == Math.Sign(b.Y);
        }

        /// <summary>
        /// Computes the clipping point based on clipping rectangle
        /// <paramref name="size"/> and two points (<paramref name="center"/> and <paramref name="p"/>)).
        /// </summary>
        /// <param name="size">Clipping zone size.</param>
        /// <param name="center">Center of the clipping zone.</param>
        /// <param name="p">Point to clip.</param>
        /// <returns>Clipping point.</returns>
        [Pure]
        public static Point GetClippingPoint(Size size, Point center, Point p)
        {
            var sides = new double[4];
            sides[0] = (center.X - size.Width / 2.0 - p.X) / (center.X - p.X);
            sides[1] = (center.Y - size.Height / 2.0 - p.Y) / (center.Y - p.Y);
            sides[2] = (center.X + size.Width / 2.0 - p.X) / (center.X - p.X);
            sides[3] = (center.Y + size.Height / 2.0 - p.Y) / (center.Y - p.Y);

            double fi = 0;
            for (int i = 0; i < 4; ++i)
            {
                if (sides[i] <= 1)
                    fi = Math.Max(fi, sides[i]);
            }

            if (IsZero(fi))
            {
                fi = double.PositiveInfinity;
                for (int i = 0; i < 4; ++i)
                    fi = Math.Min(fi, Math.Abs(sides[i]));
                fi *= -1;
            }

            return p + fi * (center - p);
        }
    }
}