using System;
using System.Windows.Data;
using System.Globalization;

namespace GraphShape.Controls.Converters
{
    /// <summary>
    /// Converter or coordinates to <see cref="Point"/> and vice versa.
    /// </summary>
    public class CoordinatesToPointConverter : IMultiValueConverter
    {
        #region IMultiValueConverter

        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null)
                return default(Point);

            if (values.Length != 2)
            {
                throw new ArgumentException(
                    $"{nameof(CoordinatesToPointConverter)} must have 2 parameters: X and Y coordinates.",
                    nameof(values));
            }

            double x = (double)values[0];
            double y = (double)values[1];

            return new Point(x, y);
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is Point point)
                return new object[] {point.X, point.Y};

            throw new ArgumentException(
                $"{nameof(CoordinatesToPointConverter)} back conversion must have 1 parameter: a {nameof(Point)}.",
                nameof(value));
        }

        #endregion
    }
}