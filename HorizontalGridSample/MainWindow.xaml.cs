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

namespace HorizontalGridSample
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			//grid.DataContext = new ClassWithProperties
			//{
			//	Count = 12,
			//	Name = "123"
			//};

			//grid.DataContext = new List<ClassWithProperties>
			//{
			//	new ClassWithProperties {Name = "1", Count = 1},
			//	new ClassWithProperties {Name = "2", Count = 2}
			//};

			//grid.DataContext = Tuple.Create(
			//	new List<ClassWithProperties>
			//	{
			//		new ClassWithProperties {Name = "1", Count = 1},
			//		new ClassWithProperties {Name = "2", Count = 2}
			//	}
			//	);

			tvObjectGraph.DataContext = new ObjectViewModelHierarchy( new { Q = new { A = new[] { 1, 2, 3 } } } );
		}
	}

	public class ClassWithProperties
	{
		public string Name { get; set; }

		public int Count { get; set; }
	}
}
