using System;
using System.Windows.Data;

namespace GraphSharp.Sample.Converters
{
	public class IntegerToDoubleConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			double r = (int)value;
			return r;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return Math.Round( (double)value );
		}

		#endregion
	}
}
