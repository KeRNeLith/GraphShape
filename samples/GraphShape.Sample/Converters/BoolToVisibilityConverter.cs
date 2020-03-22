using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GraphShape.Sample.Converters
{
    /// <summary>
    /// Converter from <see cref="bool"/> to <see cref="Visibility"/>.
    /// </summary>
    internal class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? Visibility.Visible : Visibility.Collapsed;

            throw new ArgumentException(
                $"{nameof(BoolToVisibilityConverter)} must take a boolean in parameter.",
                nameof(value));
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(Visibility)} to boolean conversion not supported.");
        }

        #endregion
    }
}