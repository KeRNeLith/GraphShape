using System;
using System.Globalization;
using System.Windows.Data;
using GraphShape.Algorithms.Layout.Compound;

namespace GraphShape.Sample.Converters
{
    /// <summary>
    /// Converter from vertex to <see cref="CompoundVertexInnerLayoutType"/>.
    /// </summary>
    internal class PocVertexToLayoutModeConverter : IValueConverter
    {
        #region IValueConverter

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vertex = value as string;
            if (vertex == "2" || vertex == "3")
                return CompoundVertexInnerLayoutType.Fixed;
            return CompoundVertexInnerLayoutType.Automatic;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(CompoundVertexInnerLayoutType)} to vertex conversion not supported.");
        }

        #endregion
    }
}
