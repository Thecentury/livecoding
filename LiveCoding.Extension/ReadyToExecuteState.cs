namespace LiveCoding.Extension
{
	public sealed class ReadyToExecuteState : MethodExecutionStateBase
	{
		public override void ExecuteMainAction()
		{
			Owner.GotoState( new MethodExecutingState() );
		}

		public override MethodExecutionState State
		{
			get { return MethodExecutionState.ReadyToExecute; }
		}
	}
}