using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using LiveCoding.Core.Capturing;

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

		public static void AddValue( string variableName, object value, int originalLineNumber = 0, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0 )
		{
			object capturedValue = ValueCapturer.CreateCapturedValue( value );
			
			var valueChange = new ValueChange
			{
				VariableName = variableName,
				FilePath = filePath,
				LineNumber = lineNumber,
				OriginalLineNumber = originalLineNumber,
				MethodName = methodName,
				OriginalValue = value,
				CapturedValue = capturedValue
			};

			AddEvent( valueChange );
		}

		public static Guid StartForLoop( int lineNumber )
		{
			var evt = new ForLoopStartedEvent
			{
				LoopStartLineNumber = lineNumber
			};

			AddEvent( evt );

			return evt.LoopId;
		}

		public static void RegisterLoopIteration( Guid loopId, object iteratorValue )
		{
			var evt = new ForLoopIterationEvent( loopId, iteratorValue );

			AddEvent( evt );
		}

		public static void EndForLoop( Guid loopId )
		{
			var evt = new ForLoopFinishedEvent( loopId );

			AddEvent( evt );
		}

		private static void AddEvent( LiveEvent evt )
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
