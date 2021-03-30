using System;
using System.Globalization;
using System.Windows.Data;

namespace GraphShape.Sample.Converters
{
    /// <summary>
    /// Converter from <see cref="int"/> to <see cref="double"/> and vice versa.
    /// </summary>
    internal sealed class IntegerToDoubleConverter : IValueConverter
    {
        #region IValueConverter

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int integer)
                return (double)integer;

            throw new ArgumentException(
                $"{nameof(IntegerToDoubleConverter)} must take an int in parameter.",
                nameof(value));
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is double d)
                return Math.Round(d);

            throw new ArgumentException(
                $"{nameof(IntegerToDoubleConverter)} back conversion must take a double in parameter.",
                nameof(value));
        }

        #endregion
    }
}
