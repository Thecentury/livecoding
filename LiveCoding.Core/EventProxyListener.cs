using System;

namespace LiveCoding.Core
{
	public sealed class EventProxyListener : MarshalByRefObject, ILiveEventListener
	{
		public void OnEventAdded( LiveEvent evt )
		{
			VariablesTracker.AddEvent( evt );
		}
	}
}