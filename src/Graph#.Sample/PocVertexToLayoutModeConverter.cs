using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using GraphSharp.Algorithms.Layout.Compound;

namespace GraphSharp.Sample
{
    public class PocVertexToLayoutModeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vertex = value as string;
            if (vertex == "2" || vertex == "3")
                return CompoundVertexInnerLayoutType.Fixed;
            else
                return CompoundVertexInnerLayoutType.Automatic;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
