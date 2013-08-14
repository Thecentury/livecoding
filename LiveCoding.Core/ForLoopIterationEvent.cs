using System;

namespace LiveCoding.Core
{
	public sealed class ForLoopIterationEvent : LiveEvent
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