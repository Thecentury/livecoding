using System;

namespace LiveCoding.Core
{
	[Serializable]
	public sealed class ForLoopFinishedEvent : LiveEvent
	{
		private readonly Guid _loopId;

		public ForLoopFinishedEvent( Guid loopId )
		{
			_loopId = loopId;
		}

		public Guid LoopId
		{
			get { return _loopId; }
		}
	}
}