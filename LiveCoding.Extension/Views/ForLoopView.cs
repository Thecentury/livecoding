using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LiveCoding.Core;

namespace LiveCoding.Extension.Views
{
	internal sealed class ForLoopView : Canvas
	{
		private readonly DataGrid _dataGrid = new DataGrid
		{
			CanUserResizeColumns = false,
			CanUserResizeRows = false,
			CanUserReorderColumns = false,
			CanUserSortColumns = false
		};

		public ForLoopView()
		{
			SetTop( _dataGrid, -10 );

			Children.Add( _dataGrid );
		}

		public void BeginWatching( ForLoopInfo loop )
		{
			_dataGrid.Columns.Clear();
			loop.Iterations.ObserveOnDispatcher().Subscribe( i => OnNextIteration( i ) );
		}

		private void OnNextIteration( ForLoopIteration iteration )
		{
			ObservableCollection<ValueChangeViewModel> dataSource = new ObservableCollection<ValueChangeViewModel>();
			_dataGrid.Columns.Add( new DataGridTextColumn
			{
				Binding = new Binding
					{
						Source = dataSource,
						Path = new PropertyPath( "Count" )
					},
				Header = iteration.IterationNumber
			} );

			iteration.EventsDuringIteration.ObserveOnDispatcher().Subscribe( e => OnValueAdded( e, iteration, dataSource ) );
		}

		private static void OnValueAdded( LiveEvent liveEvent, ForLoopIteration iteration, ICollection<ValueChangeViewModel> dataSource )
		{
			ValueChange valueChange = liveEvent as ValueChange;
			if ( valueChange == null )
			{
				return;
			}

			int loopStartLineNumber = iteration.Loop.LoopStartLineNumber;
			int lineIndexInLoop = valueChange.LineNumber - loopStartLineNumber;

			while ( dataSource.Count < lineIndexInLoop )
			{
				dataSource.Add( null );
			}

			dataSource.Add( new ValueChangeViewModel { Text = String.Format( "{0} = {1}", valueChange.VariableName, valueChange.Value ) } );
		}
	}

	internal sealed class ValueChangeViewModel
	{
		public string Text { get; set; }
	}
}
