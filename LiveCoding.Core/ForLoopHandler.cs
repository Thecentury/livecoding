using System;
using System.Collections.Generic;
using LiveCoding.Core.Internal;

namespace LiveCoding.Core
{
	internal sealed class ForLoopHandler
	{
		private readonly Stack<LiveEventWatcher> _watchers = new Stack<LiveEventWatcher>();

		public ForLoopHandler( IObserver<ForLoopInfo> loopsObserver )
		{
			_watchers.Push( new ForLoopStartWatcher( _watchers, loopsObserver ) );
		}

		public bool Accept( LiveEvent evt )
		{
			foreach ( var watcher in _watchers )
			{
				if ( watcher.Accept( evt ) )
				{
					return true;
				}
			}

			return false;
		}
	}
}