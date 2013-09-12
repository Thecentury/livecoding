using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCoding.Core;
using LiveCoding.Extension.ViewModels.ObjectVisualizing;
using LiveCoding.Extension.Views;

namespace LiveCoding.Extension.VisualStudio
{
	internal static class ValueViewFactory
	{
		public static FrameworkElement CreateView( ValueChange change )
		{
			object capturedValue = change.CapturedValue;
			if ( TypeHelper.IsExpandable( capturedValue ) )
			{
				return new ObjectView( capturedValue );
			}
			else
			{
				return new TextBox
				{
					IsReadOnly = true,
					Text = change.GetValueString(),
					BorderBrush = null,
					BorderThickness = new Thickness(),
					Background = Brushes.Transparent
				};
			}
		}
	}
}
