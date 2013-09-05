using System.Windows;
using System.Windows.Controls;

namespace HorizontalGridSample
{
	public sealed class RawValueTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate( object item, DependencyObject container )
		{
			if ( item == null )
			{
				return null;
			}

			ContentPresenter presenter = (ContentPresenter)container;

			FrameworkElement templatedParent = (FrameworkElement)presenter.TemplatedParent;

			object foundResource = templatedParent.TryFindResource( item.GetType() );

			if ( foundResource == null )
			{
				foundResource = templatedParent.TryFindResource( typeof(object) );
			}

			return foundResource as DataTemplate;
		}
	}
}