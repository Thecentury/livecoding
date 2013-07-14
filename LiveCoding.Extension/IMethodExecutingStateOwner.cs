namespace LiveCoding.Extension
{
	public interface IMethodExecutingStateOwner
	{
		MethodExecutionStateBase CurrentState { get; set; }
	}
}