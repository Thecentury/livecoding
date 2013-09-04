using System.Windows;
using System.Windows.Controls;

namespace LiveCoding.Extension.Views
{
	internal sealed class RawValueTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate( object item, DependencyObject container )
		{
			if ( item == null )
			{
				return null;
			}

			ContentPresenter presenter = (ContentPresenter)container;

			FrameworkElement templatedParent = (FrameworkElement)presenter.TemplatedParent;

			object foundResource = templatedParent.TryFindResource( new DataTemplateKey( item.GetType().Name ) );

			if ( foundResource == null )
			{
				foundResource = templatedParent.TryFindResource( new DataTemplateKey( "system:Object" ) );
			}

			return foundResource as DataTemplate;
		}
	}
}
