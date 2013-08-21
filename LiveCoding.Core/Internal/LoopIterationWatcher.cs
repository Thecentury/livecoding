using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveCoding.Core.Internal
{
	internal sealed class LoopIterationWatcher : SpecificLiveEventWatcher<ForLoopIterationEvent>
	{
		private readonly ForLoopInfo _forLoopInfo;
		private readonly Stack<LiveEventWatcher> _watchers;

		private int _iterationNumber;

		public LoopIterationWatcher( ForLoopInfo forLoopInfo, Stack<LiveEventWatcher> watchers )
		{
			if ( forLoopInfo == null )
			{
				throw new ArgumentNullException( "forLoopInfo" );
			}
			_forLoopInfo = forLoopInfo;
			_watchers = watchers;
		}

		protected override void AcceptCore( ForLoopIterationEvent evt )
		{
			if ( _forLoopInfo.IterationsCached.Count > 0 )
			{
				_forLoopInfo.IterationsCached.Last().EventsInternal.OnCompleted();
			}

			var iteration = new ForLoopIteration( _iterationNumber, evt.IteratorValue, _forLoopInfo );
			_forLoopInfo.IterationsInternal.OnNext( iteration );

			_watchers.Push( new ForLoopFinishWatcher( _watchers, iteration, _forLoopInfo ) );
			_watchers.Push( new LoopValueChangeWatcher( _watchers, iteration ) );
			_iterationNumber++;
		}
	}
}