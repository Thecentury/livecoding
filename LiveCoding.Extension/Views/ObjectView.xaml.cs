using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LiveCoding.Extension.ViewModels.ObjectVisualizing;

namespace LiveCoding.Extension.Views
{
	/// <summary>
	/// Interaction logic for ObjectView.xaml
	/// </summary>
	public partial class ObjectView : UserControl
	{
		public ObjectView()
		{
			InitializeComponent();
		}

		public ObjectView( object o )
		{
			InitializeComponent();

			DataContext = new ObjectViewViewModel { Root = new ObjectViewModelHierarchy( o ) };
		}

		public void SetRootObject( object root )
		{
			ObjectViewViewModel vm = (ObjectViewViewModel)DataContext;
			vm.Root = new ObjectViewModelHierarchy( root );
		}

		protected override void OnVisualParentChanged( DependencyObject oldParent )
		{
			base.OnVisualParentChanged( oldParent );
			if ( Parent != null )
			{
				BindingOperations.SetBinding( Parent, Panel.ZIndexProperty, new Binding( "(Panel.ZIndex)" ) { Source = this } );
			}
		}
	}
}
