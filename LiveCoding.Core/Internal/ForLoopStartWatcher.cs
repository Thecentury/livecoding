using System;
using System.Collections.Generic;

namespace LiveCoding.Core.Internal
{
	internal sealed class ForLoopStartWatcher : SpecificLiveEventWatcher<ForLoopStartedEvent>
	{
		private readonly Stack<LiveEventWatcher> _watchers;
		private readonly IObserver<ForLoopInfo> _loopsObserver;

		public ForLoopStartWatcher( Stack<LiveEventWatcher> watchers, IObserver<ForLoopInfo> loopsObserver )
		{
			_watchers = watchers;
			_loopsObserver = loopsObserver;
		}

		protected override void AcceptCore( ForLoopStartedEvent evt )
		{
			var forLoopInfo = new ForLoopInfo( evt.LoopStartLineNumber );
			_watchers.Push( new ForLoopFinishWatcher( _watchers, null, forLoopInfo ) );
			_watchers.Push( new LoopIterationWatcher( forLoopInfo, _watchers ) );
			_loopsObserver.OnNext( forLoopInfo );
		}
	}
}