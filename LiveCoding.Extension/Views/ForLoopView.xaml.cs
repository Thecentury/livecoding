using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
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
using LiveCoding.Core;
using LiveCoding.Extension.VisualStudio;
using LiveCoding.Extension.VisualStudio.Loops;

namespace LiveCoding.Extension.Views
{
	/// <summary>
	/// Interaction logic for ForLoopView.xaml
	/// </summary>
	internal partial class ForLoopView : UserControl
	{
		public ForLoopView( LoopTag data )
		{
			InitializeComponent();

			DataContext = data;
		}

		public void SetDataContext( LoopTag tag )
		{
			DataContext = tag;
			_dataGrid.ItemsSource = null;
			_dataGrid.Columns.Clear();
		}

		public void BeginWatching( ForLoopInfo loop )
		{
			ObservableCollection<ValueChangeViewModel> dataSource = new ObservableCollection<ValueChangeViewModel>();
			_dataGrid.Columns.Clear();
			_dataGrid.ItemsSource = dataSource;
			loop.Iterations.ObserveOnDispatcher().Subscribe( i => OnNextIteration( i, dataSource ) );
		}

		private void OnNextIteration( ForLoopIteration iteration, ObservableCollection<ValueChangeViewModel> dataSource )
		{
			_dataGrid.Columns.Add( new DataGridTemplateColumn
			{
				CellTemplateSelector = new ValueTemplateSelector( iteration.IterationNumber ),
				Header = iteration.IterationNumber,
			} );
			//_dataGrid.Columns.Add( new DataGridTextColumn
			//{
			//	Binding = new Binding( "p" + iteration.IterationNumber ),
			//	Header = iteration.IterationNumber
			//} );

			iteration.EventsDuringIteration.ObserveOnDispatcher().Subscribe( e => OnValueAdded( e, iteration, dataSource ) );
		}

		private static void OnValueAdded( LiveEvent liveEvent, ForLoopIteration iteration, IList<ValueChangeViewModel> dataSource )
		{
			ValueChange valueChange = liveEvent as ValueChange;
			if ( valueChange == null )
			{
				return;
			}

			int loopStartLineNumber = iteration.Loop.LoopStartLineNumber;
			int lineIndexInLoop = valueChange.OriginalLineNumber - loopStartLineNumber - 1;

			ValueChangeViewModel viewModel;
			if ( dataSource.Count > lineIndexInLoop )
			{
				viewModel = dataSource[ lineIndexInLoop ];
			}
			else
			{
				while ( dataSource.Count < lineIndexInLoop )
				{
					dataSource.Add( new ValueChangeViewModel() );
				}
				viewModel = new ValueChangeViewModel();
				dataSource.Add( viewModel );
			}

			viewModel.AddChange( iteration.IterationNumber, valueChange );
		}
	}
}
