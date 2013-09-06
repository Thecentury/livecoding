using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LiveCoding.Core
{
	public static class VariablesTracker
	{
		// todo brinchuk thread safety
		private static readonly List<LiveEvent> _events = new List<LiveEvent>();

		public static IReadOnlyList<LiveEvent> Events
		{
			get { return _events.AsReadOnly(); }
		}

		public static void ClearEvents()
		{
			_events.Clear();
			_forLoops.Dispose();
			_forLoops = new ReplaySubject<ForLoopInfo>();
			_forLoopHandler = new ForLoopHandler( _forLoops );
		}

		public static void AddEvent( LiveEvent evt )
		{
			_events.Add( evt );
			RaiseEventAdded( evt );
		}

		public static event EventHandler<LiveEventAddedEventArgs> EventAdded;

		private static readonly IObservable<LiveEvent> _eventsObservable =
			Observable.FromEventPattern<LiveEventAddedEventArgs>(
				h => EventAdded += h,
				h => EventAdded -= h )
			.Select( e => e.EventArgs.AddedEvent );

		public static IObservable<LiveEvent> EventsObservable
		{
			get { return _eventsObservable; }
		}

		static VariablesTracker()
		{
			_forLoopHandler = new ForLoopHandler( _forLoops );
		}

		private static ReplaySubject<ForLoopInfo> _forLoops = new ReplaySubject<ForLoopInfo>();
		private static ForLoopHandler _forLoopHandler;

		public static IObservable<ForLoopInfo> ForLoops
		{
			get { return _forLoops; }
		}

		private static void RaiseEventAdded( LiveEvent evt )
		{
			if ( _forLoopHandler.Accept( evt ) )
			{
				return;
			}

			EventHandler<LiveEventAddedEventArgs> handler = EventAdded;
			if ( handler != null )
			{
				handler( null, new LiveEventAddedEventArgs( evt ) );
			}
		}
	}
}
