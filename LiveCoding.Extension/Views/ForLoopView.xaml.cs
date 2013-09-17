using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Controls;
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

		private void OnNextIteration( ForLoopIteration iteration, IList<ValueChangeViewModel> dataSource )
		{
			_dataGrid.Columns.Add( new DataGridTemplateColumn
			{
				CellTemplateSelector = new ValueTemplateSelector( iteration.IterationNumber ),
				Header = iteration.IteratorValue ?? iteration.IterationNumber,
			} );

			iteration.EventsDuringIteration.ObserveOnDispatcher().Subscribe( e => OnValueAdded( e, iteration, dataSource ) );
		}

		private static void OnValueAdded( LiveEvent liveEvent, ForLoopIteration iteration, IList<ValueChangeViewModel> dataSource )
		{
			IPositionAware positionAwareEvent = liveEvent as IPositionAware;
			if ( positionAwareEvent == null )
			{
				return;
			}

			int loopStartLineNumber = iteration.Loop.LoopStartLineNumber;
			int lineIndexInLoop = positionAwareEvent.GetOriginalLineNumber() - loopStartLineNumber;

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

			viewModel.AddChange( iteration.IterationNumber, liveEvent );
		}
	}
}
