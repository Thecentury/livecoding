namespace LiveCoding.Extension.ViewModels
{
	public sealed class ReadyToExecuteState : MethodExecutionStateBase
	{
		public override void ExecuteMainAction()
		{
			Owner.GotoState( new ExecutingState() );
		}

		public override MethodExecutionState State
		{
			get { return MethodExecutionState.ReadyToExecute; }
		}
	}
}