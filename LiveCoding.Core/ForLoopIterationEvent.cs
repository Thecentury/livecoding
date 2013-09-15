using System;

namespace LiveCoding.Core
{
	[Serializable]
	public sealed class ForLoopIterationEvent : LiveEvent, ILoopEvent
	{
		private readonly Guid _loopId;
		private readonly object _iteratorValue;

		public ForLoopIterationEvent( Guid loopId, object iteratorValue )
		{
			_loopId = loopId;
			_iteratorValue = iteratorValue;
		}

		public Guid LoopId
		{
			get { return _loopId; }
		}

		public object IteratorValue
		{
			get { return _iteratorValue; }
		}
	}
}