﻿using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GraphShape.Algorithms.Layout;

namespace GraphShape.Controls.Converters
{
    /// <summary>
    /// Converter of position and sizes of the source and target points,
    /// and the route information of an edge to a path.
    /// </summary>
    /// <remarks>The edge can bend, or it can be straight line.</remarks>
    public class EdgeRouteToPathConverter : IMultiValueConverter
    {
        #region IMultiValueConverter

        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null)
                return new PathFigureCollection(0);

            if (values.Length != 9)
            {
                throw new ArgumentException(
                    $"{nameof(EdgeRouteToPathConverter)} must have 9 parameters: pos (1,2), size (3,4) of source; pos (5,6), size (7,8) of target; routeInformation (9).",
                    nameof(values));
            }

            #region Get the inputs

            // Get the position of the source
            var sourcePos = new Point(
                values[0] != DependencyProperty.UnsetValue ? (double)values[0] : 0.0,
                values[1] != DependencyProperty.UnsetValue ? (double)values[1] : 0.0);

            // Get the size of the source
            var sourceSize = new Size(
                values[2] != DependencyProperty.UnsetValue ? (double)values[2] : 0.0,
                values[3] != DependencyProperty.UnsetValue ? (double)values[3] : 0.0);

            // Get the position of the target
            var targetPos = new Point(
                values[4] != DependencyProperty.UnsetValue ? (double)values[4] : 0.0,
                values[5] != DependencyProperty.UnsetValue ? (double)values[5] : 0.0);

            // Get the size of the target
            var targetSize = new Size(
                values[6] != DependencyProperty.UnsetValue ? (double)values[6] : 0.0,
                values[7] != DependencyProperty.UnsetValue ? (double)values[7] : 0.0);

            // Get the route information
            Point[] routeInformation = values[8] != DependencyProperty.UnsetValue ? (Point[])values[8] : null;

            #endregion

            bool hasRouteInfo = routeInformation != null && routeInformation.Length > 0;

            // Create the path
            Point p1 = LayoutUtils.GetClippingPoint(
                sourceSize,
                sourcePos,
                hasRouteInfo ? routeInformation[0] : targetPos);

            Point p2 = LayoutUtils.GetClippingPoint(
                targetSize,
                targetPos,
                hasRouteInfo ? routeInformation[routeInformation.Length - 1] : sourcePos);


            var segments = new PathSegment[1 + (hasRouteInfo ? routeInformation.Length : 0)];
            if (hasRouteInfo)
            {
                // Append route points
                for (int i = 0; i < routeInformation.Length; ++i)
                    segments[i] = new LineSegment(routeInformation[i], true);
            }

            Point pLast = hasRouteInfo ? routeInformation[routeInformation.Length - 1] : p1;
            Vector v = pLast - p2;
            v = v / v.Length * 5;
            Vector n = new Vector(-v.Y, v.X) * 0.3;

            segments[segments.Length - 1] = new LineSegment(p2 + v, true);

            var pathCollection = new PathFigureCollection(2)
            {
                new PathFigure(p1, segments, false),
                new PathFigure(
                    p2,
                    new PathSegment[]
                    {
                        new LineSegment(p2 + v - n, true),
                        new LineSegment(p2 + v + n, true)
                    },
                    true)
            };

            return pathCollection;
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Path to edge route conversion not supported.");
        }

        #endregion
    }
}