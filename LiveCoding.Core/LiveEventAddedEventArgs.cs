using System;

namespace LiveCoding.Core
{
	public sealed class LiveEventAddedEventArgs : EventArgs
	{
		public LiveEventAddedEventArgs( LiveEvent addedEvent )
		{
			AddedEvent = addedEvent;
		}

		public LiveEvent AddedEvent { get; private set; }
	}
}