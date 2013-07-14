using System;

namespace LiveCoding.Extension
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
	}
}