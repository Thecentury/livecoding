using System;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class FailedState : MethodExecutionStateBase
	{
		private readonly Exception _exception;

		public FailedState( Exception exception )
		{
			_exception = exception;
		}

		public override MethodExecutionState State
		{
			get { return MethodExecutionState.Failed; }
		}

		public Exception Exception
		{
			get { return _exception; }
		}

		public override void ExecuteMainAction()
		{
			Owner.GotoState( new ExecutingState() );
		}
	}
}