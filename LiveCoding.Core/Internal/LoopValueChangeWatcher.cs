using System.Collections.Generic;

namespace LiveCoding.Core.Internal
{
	internal sealed class LoopValueChangeWatcher : SpecificLiveEventWatcher<ValueChange>
	{
		private readonly Stack<LiveEventWatcher> _watchers;
		private readonly ForLoopIteration _iteration;

		public LoopValueChangeWatcher( Stack<LiveEventWatcher> watchers, ForLoopIteration iteration )
		{
			_watchers = watchers;
			_iteration = iteration;
		}

		protected override void AcceptCore( ValueChange evt )
		{
			_iteration.EventsInternal.OnNext( evt );
		}
	}
}