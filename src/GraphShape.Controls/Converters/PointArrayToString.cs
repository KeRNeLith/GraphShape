using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace GraphShape.Controls.Converters
{
    /// <summary>
    /// Converter from <see cref="Point"/> array to <see cref="string"/>.
    /// </summary>
    public class PointArrayToString : IValueConverter
    {
        #region IValueConverter

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var points = value as Point[];
            if (points is null)
                return string.Empty;
            
            var stringBuilder = new StringBuilder();
            foreach (Point point in points)
            {
                stringBuilder.AppendLine(point.ToString());
            }
            
            return stringBuilder;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"String to {nameof(Point)}[] conversion not supported.");
        }

        #endregion
    }
}