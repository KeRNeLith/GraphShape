using System;
using System.Globalization;
using System.Windows.Data;

namespace GraphShape.Sample.Converters
{
    /// <summary>
    /// Converter that checks equality between value and parameter.
    /// </summary>
    internal sealed class EqualityToBooleanConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, parameter);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is true)
                return parameter;
            throw new NotSupportedException($"{nameof(EqualityToBooleanConverter)}: Can't bind back.");
        }
    }
}