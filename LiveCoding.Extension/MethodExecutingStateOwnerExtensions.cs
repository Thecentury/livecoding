using System;

namespace LiveCoding.Extension
{
	public static class MethodExecutingStateOwnerExtensions
	{
		public static void GotoState( this IMethodExecutingStateOwner owner, MethodExecutionStateBase newState )
		{
			if ( newState == null )
			{
				throw new ArgumentNullException( "newState" );
			}

			owner.CurrentState = newState;
		}
	}
}