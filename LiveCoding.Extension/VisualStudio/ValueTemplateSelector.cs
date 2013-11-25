using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCoding.Core;
using LiveCoding.Core.Capturing;
using LiveCoding.Extension.ViewModels.ObjectVisualizing;
using LiveCoding.Extension.Views;
using LiveCoding.Extension.VisualStudio.If;

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

			var liveEvent = viewModel.GetChangeByIndex( _columnIndex );
			if ( liveEvent == null )
			{
				return CreateEmptyTemplate();
			}

			var valueChange = liveEvent as ValueChange;
			var conditionValue = liveEvent as IfEvaluationEvent;
			if ( valueChange != null )
			{
				if ( TypeHelper.IsExpandable( valueChange.CapturedValue ) )
				{
					var objectView = new FrameworkElementFactory( typeof( ObjectView ) );
					objectView.SetBinding( FrameworkElement.DataContextProperty, new Binding( "." )
					{
						Source = new ObjectViewViewModel
						{
							Root = new ObjectViewModelHierarchy( valueChange.CapturedValue )
						}
					} );

					return new DataTemplate { VisualTree = objectView };
				}
				else
				{
					var textBox = new FrameworkElementFactory( typeof( TextBox ) );
					textBox.SetValue( TextBoxBase.IsReadOnlyProperty, true );
					textBox.SetValue( TextBox.TextProperty, valueChange.GetValueString() );
					textBox.SetValue( Control.BorderBrushProperty, null );
					textBox.SetValue( Control.BorderThicknessProperty, new Thickness() );
					textBox.SetValue( Control.BackgroundProperty, Brushes.Transparent );

					return new DataTemplate { VisualTree = textBox };
				}
			}
			else if ( conditionValue != null )
			{
				var template = new FrameworkElementFactory( typeof( Rectangle ) );
				
				template.SetValue( Shape.FillProperty, LiveCodingBrushes.GetBrush( conditionValue.ConditionValue ) );

				return new DataTemplate { VisualTree = template };
			}
			else
			{
				return CreateEmptyTemplate();
			}
		}
	}
}