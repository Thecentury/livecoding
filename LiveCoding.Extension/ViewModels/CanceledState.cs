namespace LiveCoding.Extension.ViewModels
{
	internal sealed class CanceledState : MethodExecutionStateBase
	{
		public override MethodExecutionState State
		{
			get { return MethodExecutionState.Canceled; }
		}
	}
}