using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.ViewModels
{
	public sealed class MethodExecutionViewModel : ViewModelBase, IMethodExecutingStateOwner
	{
		private readonly MethodGlyphTag _data;
		private readonly IWpfTextView _view;

		public MethodGlyphTag Data
		{
			get { return _data; }
		}

		public IWpfTextView View
		{
			get { return _view; }
		}

		public MethodExecutionViewModel( MethodGlyphTag data, IWpfTextView view )
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

			this.GotoState( new ReadyToExecuteState() );
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
	}
}