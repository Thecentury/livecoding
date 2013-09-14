using System.Windows.Controls;
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

		public new MethodExecutionViewModel DataContext
		{
			get { return (MethodExecutionViewModel)base.DataContext; }
			set
			{
				var previousDataContext = DataContext;
				if ( previousDataContext != null )
				{
					previousDataContext.Dispose();
				}
				base.DataContext = value;
			}
		}
	}
}
