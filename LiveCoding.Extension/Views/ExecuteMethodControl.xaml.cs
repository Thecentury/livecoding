using System.Windows.Controls;
using System.Windows.Input;
using LiveCoding.Extension.ViewModels;

namespace LiveCoding.Extension.Views
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
