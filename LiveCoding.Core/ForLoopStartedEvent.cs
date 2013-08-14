using System;

namespace LiveCoding.Core
{
	public sealed class ForLoopStartedEvent : LiveEvent
	{
		public ForLoopStartedEvent()
		{
			LoopId = Guid.NewGuid();
		}

		public Guid LoopId { get; private set; }

		public int LoopStartLineNumber { get; set; }
	}
}