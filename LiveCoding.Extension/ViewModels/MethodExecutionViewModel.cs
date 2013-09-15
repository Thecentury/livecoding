using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCoding.Extension.VisualStudio;
using LiveCoding.Extension.VisualStudio.If;
using LiveCoding.Extension.VisualStudio.Loops;
using LiveCoding.Extension.VisualStudio.VariableValues;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.ViewModels
{
	public sealed class MethodExecutionViewModel : ViewModelBase, IMethodExecutingStateOwner, IDisposable
	{
		private readonly MethodExecutionData _data;
		private readonly IWpfTextView _view;

		public MethodExecutionData Data
		{
			get { return _data; }
		}

		public IWpfTextView View
		{
			get { return _view; }
		}

		public MethodExecutionViewModel( MethodExecutionData data, IWpfTextView view )
		{
			if ( data == null )
			{
				throw new ArgumentNullException( "data" );
			}
			if ( view == null )
			{
				throw new ArgumentNullException( "view" );
			}

			_data = data;
			_view = view;

			_executeCommand = new RelayCommand( Execute );
			_clearCommand = new RelayCommand( Clear );

			this.GotoState( new ReadyToExecuteState() );
		}

		private void Clear()
		{
			var properties = _view.TextBuffer.Properties;

			var variableValueTagger = properties.GetProperty<VariableValueTagger>( typeof( VariableValueTagger ) );
			variableValueTagger.ClearVariableChanges();

			var booleanConditionTagger = properties.GetProperty<BooleanAdorment>( typeof( BooleanAdorment ) );
			booleanConditionTagger.Clear();

			var forLoopTagger = properties.GetProperty<LoopTagger>( typeof( LoopTagger ) );
			forLoopTagger.Clear();
		}

		public MethodExecutionState State
		{
			get { return _currentState.State; }
		}

		private readonly RelayCommand _executeCommand;
		private MethodExecutionStateBase _currentState;

		public ICommand ExecuteCommand
		{
			get { return _executeCommand; }
		}

		private readonly RelayCommand _clearCommand;

		public ICommand ClearCommand
		{
			get { return _clearCommand; }
		}

		private void Execute()
		{
			CurrentState.ExecuteMainAction();
		}

		public MethodExecutionStateBase CurrentState
		{
			get { return _currentState; }
			set
			{
				if ( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				if ( _currentState != null )
				{
					_currentState.Exit();
				}

				_currentState = value;

				RaisePropertyChanged( () => CurrentState );
				RaisePropertyChanged( () => State );

				_currentState.BeginInit();
				_currentState.Owner = this;
				_currentState.EndInit();

				_currentState.Enter();
			}
		}

		private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
		public Dictionary<string, object> Cache
		{
			get { return _cache; }
		}

		public void Dispose()
		{
			foreach ( var value in _cache.Values.OfType<IDisposable>() )
			{
				value.Dispose();
			}

			_cache.Clear();
		}
	}
}