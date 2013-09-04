using System;
using System.Globalization;
using System.Windows.Data;

namespace LiveCoding.Extension.Views
{
	internal sealed class BooleanToZIndexConverter : IValueConverter
	{
		public static readonly BooleanToZIndexConverter Default = new BooleanToZIndexConverter { TrueZIndex = 1, FalseZIndex = 0 };

		public int TrueZIndex { get; set; }

		public int FalseZIndex { get; set; }

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			bool flag = (bool)value;
			if ( flag )
			{
				return TrueZIndex;
			}
			else
			{
				return FalseZIndex;
			}
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}