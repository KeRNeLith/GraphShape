using System;
using System.Globalization;
using System.Windows.Data;

// ReSharper disable PossibleNullReferenceException

namespace GraphShape.Sample.Converters
{
    /// <summary>
    /// Converter from <see cref="double"/> value to <see cref="Math.Log10"/> <see cref="double"/> value and vice versa.
    /// </summary>
    internal sealed class DoubleToLog10Converter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double) value;
            return Math.Log10(val);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double) value;
            return Math.Pow(10, val);
        }
    }
}