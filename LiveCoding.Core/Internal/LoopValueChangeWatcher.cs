using System.Collections.Generic;

namespace LiveCoding.Core.Internal
{
	internal sealed class LoopValueChangeWatcher : LiveEventWatcher
	{
		private readonly Stack<LiveEventWatcher> _watchers;
		private readonly ForLoopIteration _iteration;

		public LoopValueChangeWatcher( Stack<LiveEventWatcher> watchers, ForLoopIteration iteration )
		{
			_watchers = watchers;
			_iteration = iteration;
		}

		public override bool Accept( LiveEvent evt )
		{
			if ( evt is ILoopEvent )
			{
				return false;
			}

			_iteration.EventsInternal.OnNext( evt );

			return true;
		}
	}
}