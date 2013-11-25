using System.Windows.Controls;
using LiveCoding.Extension.ViewModels.ObjectVisualizing;

namespace LiveCoding.Extension.Views
{
	/// <summary>
	/// Interaction logic for ObjectViewContainer.xaml
	/// </summary>
	public partial class ObjectViewContainer : UserControl
	{
		public ObjectViewContainer( object o )
		{
			InitializeComponent();
			DataContext = new ObjectViewViewModel { Root = new ObjectViewModelHierarchy( o ) };
		}

		public void SetRootObject( object value )
		{
			objectView.SetRootObject( value );
		}
	}
}
