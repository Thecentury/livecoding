namespace LiveCoding.Extension.ViewModels
{
	public interface IMethodExecutingStateOwner
	{
		MethodExecutionStateBase CurrentState { get; set; }
	}
}