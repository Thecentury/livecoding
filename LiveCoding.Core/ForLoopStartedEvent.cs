using System;

namespace LiveCoding.Core
{
	[Serializable]
	public sealed class ForLoopStartedEvent : LiveEvent, ILoopEvent
	{
		public ForLoopStartedEvent()
		{
			LoopId = Guid.NewGuid();
		}

		public Guid LoopId { get; private set; }

		public int LoopStartLineNumber { get; set; }
	}
}