using System;

namespace LiveCoding.Extension.ViewModels
{
	public sealed class ExecutedState : MethodExecutionStateBase
	{
		private readonly TimeSpan _executionDuration;

		public ExecutedState( TimeSpan executionDuration )
		{
			_executionDuration = executionDuration;
		}

		public override MethodExecutionState State
		{
			get { return MethodExecutionState.Executed; }
		}

		public TimeSpan ExecutionDuration
		{
			get { return _executionDuration; }
		}
	}
}