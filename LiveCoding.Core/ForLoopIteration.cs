using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace LiveCoding.Core
{
	public sealed class ForLoopIteration
	{
		private readonly ForLoopInfo _loop;

		public ForLoopIteration(int iterationNumber, object iteratorValue, ForLoopInfo forLoopInfo)
		{
			_loop = forLoopInfo;
			IterationNumber = iterationNumber;
			IteratorValue = iteratorValue;
		}

		public int IterationNumber { get; private set; }

		public object IteratorValue { get; private set; }

		private readonly Subject<LiveEvent> _eventsDuringIteration = new Subject<LiveEvent>();

		internal IObserver<LiveEvent> EventsInternal
		{
			get { return _eventsDuringIteration; }
		}

		public IObservable<LiveEvent> EventsDuringIteration
		{
			get { return _eventsDuringIteration; }
		}

		public ForLoopInfo Loop
		{
			get { return _loop; }
		}
	}
}