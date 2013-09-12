using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using LiveCoding.Core;
using LiveCoding.Extension.ViewModels.ObjectVisualizing;
using LiveCoding.Extension.Views;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class ValueTemplateSelector : DataTemplateSelector
	{
		private readonly int _columnIndex;

		public ValueTemplateSelector( int columnIndex )
		{
			_columnIndex = columnIndex;
		}

		private static DataTemplate CreateEmptyTemplate()
		{
			return new DataTemplate { VisualTree = null };
		}

		public override DataTemplate SelectTemplate( object item, DependencyObject container )
		{
			ValueChangeViewModel viewModel = (ValueChangeViewModel)item;
			if ( viewModel == null )
			{
				return CreateEmptyTemplate();
			}

			var change = viewModel.GetChangeByIndex( _columnIndex );
			if ( change == null )
			{
				return CreateEmptyTemplate();
			}

			if ( TypeHelper.IsExpandable( change.CapturedValue ) )
			{
				var objectView = new FrameworkElementFactory( typeof( ObjectView ) );
				objectView.SetBinding( FrameworkElement.DataContextProperty, new Binding( "." )
				{
					Source = new ObjectViewViewModel
					{
						Root = new ObjectViewModelHierarchy( change.CapturedValue )
					}
				} );

				return new DataTemplate { VisualTree = objectView };
			}
			else
			{
				var textBox = new FrameworkElementFactory( typeof( TextBox ) );
				textBox.SetValue( TextBoxBase.IsReadOnlyProperty, true );
				textBox.SetValue( TextBox.TextProperty, change.GetValueString() );
				textBox.SetValue( Control.BorderBrushProperty, null );
				textBox.SetValue( Control.BorderThicknessProperty, new Thickness() );
				textBox.SetValue( Control.BackgroundProperty, Brushes.Transparent );

				return new DataTemplate { VisualTree = textBox };
			}
		}
	}
}