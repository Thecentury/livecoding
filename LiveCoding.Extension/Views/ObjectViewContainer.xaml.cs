using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
