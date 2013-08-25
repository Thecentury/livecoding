using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LiveCoding.Core;
using LiveCoding.Extension.Annotations;
using LiveCoding.Extension.VisualStudio;
using LiveCoding.Extension.VisualStudio.ForLoops;

namespace LiveCoding.Extension.Views
{
	internal sealed class ForLoopView : Canvas
	{
		private readonly ForLoopTag _data;

		private readonly DataGrid _dataGrid = new DataGrid
		{
			CanUserResizeColumns = false,
			CanUserResizeRows = false,
			CanUserReorderColumns = false,
			CanUserSortColumns = false,
			AutoGenerateColumns = false,
			IsReadOnly = true
		};

		public ForLoopView( ForLoopTag data )
		{
			_data = data;

			SetTop( _dataGrid, -data.LineHeight );

			Children.Add( _dataGrid );
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
			_dataGrid.Columns.Add( new DataGridTextColumn
			{
				Binding = new Binding( "p" + iteration.IterationNumber ),
				Header = iteration.IterationNumber
			} );

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
				viewModel = dataSource[lineIndexInLoop];
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

	internal sealed class ValueChangeViewModel : DynamicObject, INotifyPropertyChanged
	{
		private readonly Dictionary<int, ValueChange> _changes = new Dictionary<int, ValueChange>();

		public void AddChange( int columnNumber, ValueChange change )
		{
			_changes.Add( columnNumber, change );

			OnPropertyChanged( "p" + columnNumber );
		}

		private PropertyChangedEventHandler _propertyChangedEventHandler;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { _propertyChangedEventHandler += value; }
			remove { _propertyChangedEventHandler = (PropertyChangedEventHandler)Delegate.Remove( _propertyChangedEventHandler, value ); }
		}

		public override bool TryGetMember( GetMemberBinder binder, out object result )
		{
			int columnNumber = Int32.Parse( binder.Name.Substring( 1 ) );
			ValueChange change;
			bool found = _changes.TryGetValue( columnNumber, out change );

			result = change;

			return found;
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged( [CallerMemberName] string propertyName = null )
		{
			var handler = _propertyChangedEventHandler;
			if ( handler != null )
			{
				handler( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}
	}
}
