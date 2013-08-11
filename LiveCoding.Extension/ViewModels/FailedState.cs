using System;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class FailedState : MethodExecutionStateBase
	{
		private readonly AggregateException _exception;

		public FailedState( AggregateException exception )
		{
			_exception = exception;
		}

		public override MethodExecutionState State
		{
			get { return MethodExecutionState.Failed; }
		}

		public override void ExecuteMainAction()
		{
			Owner.GotoState( new ExecutingState() );
		}
	}
}