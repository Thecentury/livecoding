using System;
using System.Collections.Generic;

namespace LiveCoding.Core.Internal
{
	internal sealed class ForLoopFinishWatcher : SpecificLiveEventWatcher<ForLoopFinishedEvent>
	{
		private readonly Stack<LiveEventWatcher> _watchers;
		private readonly ForLoopIteration _iteration;
		private readonly ForLoopInfo _forLoopInfo;

		public ForLoopFinishWatcher( Stack<LiveEventWatcher> watchers, ForLoopIteration iteration, ForLoopInfo forLoopInfo )
		{
			if ( watchers == null )
			{
				throw new ArgumentNullException( "watchers" );
			}
			_watchers = watchers;
			_iteration = iteration;
			_forLoopInfo = forLoopInfo;
		}

		protected override void AcceptCore( ForLoopFinishedEvent evt )
		{
			foreach ( var watcher in _watchers.PreviewHeads().NotOfType<LiveEventWatcher, ForLoopStartWatcher>() )
			{
				_watchers.Pop();
			}

			if ( _iteration != null )
			{
				_iteration.EventsInternal.OnCompleted();
			}
			_forLoopInfo.IterationsInternal.OnCompleted();
		}
	}
}