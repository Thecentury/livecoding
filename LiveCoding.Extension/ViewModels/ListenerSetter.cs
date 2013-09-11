using System;
using LiveCoding.Core;

namespace LiveCoding.Extension.ViewModels
{
	[Serializable]
	internal sealed class ListenerSetter
	{
		private readonly ILiveEventListener _listener;

		public ListenerSetter( ILiveEventListener listener )
		{
			_listener = listener;
		}

		public void SetListener()
		{
			VariablesTrackerFacade.SetListener( _listener );
		}
	}
}