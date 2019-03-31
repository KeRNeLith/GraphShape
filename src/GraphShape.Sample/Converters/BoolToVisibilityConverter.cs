using System.Windows.Data;
using System.Windows;

namespace GraphSharp.Sample.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool b = (bool) value;
			if (b)
			{
				return Visibility.Visible;
			}
			
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}