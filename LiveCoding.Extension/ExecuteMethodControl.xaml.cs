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
using LiveCoding.Extension.ViewModels;

namespace LiveCoding.Extension
{
	/// <summary>
	/// Interaction logic for ExecuteMethodControl.xaml
	/// </summary>
	public partial class ExecuteMethodControl : UserControl
	{
		public ExecuteMethodControl()
		{
			InitializeComponent();
		}

		private void OnMouseLeftButtonDown( object sender, MouseButtonEventArgs e )
		{
			MethodExecutionViewModel viewModel = (MethodExecutionViewModel)DataContext;

			viewModel.ExecuteCommand.Execute( null );

			e.Handled = true;
		}
	}
}
