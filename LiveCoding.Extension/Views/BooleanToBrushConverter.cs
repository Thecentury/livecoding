using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LiveCoding.Extension.Views
{
	internal sealed class BooleanToBrushConverter : IValueConverter
	{
		public Brush TrueBrush { get; set; }

		public Brush FalseBrush { get; set; }

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			bool flag = (bool) value;
			if ( flag )
			{
				return TrueBrush;
			}
			else
			{
				return FalseBrush;
			}
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}