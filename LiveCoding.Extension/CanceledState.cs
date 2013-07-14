namespace LiveCoding.Extension
{
	internal sealed class CanceledState : MethodExecutionStateBase
	{
		public override MethodExecutionState State
		{
			get { return MethodExecutionState.Canceled; }
		}
	}
}