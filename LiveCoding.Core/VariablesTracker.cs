using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;

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
		}

		public static void AddValue( string variableName, object value, int originalLineNumber = 0, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0 )
		{
			var valueChange = new ValueChange
			{
				VariableName = variableName,
				FilePath = filePath,
				LineNumber = lineNumber,
				OriginalLineNumber = originalLineNumber,
				MethodName = methodName,
				Value = value,
				TimestampUtc = DateTime.UtcNow,
				ThreadId = Thread.CurrentThread.ManagedThreadId
			};
			_events.Add( valueChange );

			RaiseEventAdded( valueChange );
		}

		public static event EventHandler<LiveEventAddedEventArgs> EventAdded;

		private static readonly IObservable<LiveEvent> eventsObservable = 
			Observable.FromEventPattern<LiveEventAddedEventArgs>(
				h => EventAdded += h, 
				h => EventAdded -= h )
			.Select( e => e.EventArgs.AddedEvent );

		public static IObservable<LiveEvent> EventsObservable
		{
			get { return eventsObservable; }
		}

		private static void RaiseEventAdded( LiveEvent evt )
		{
			EventHandler<LiveEventAddedEventArgs> handler = EventAdded;
			if ( handler != null )
			{
				handler( null, new LiveEventAddedEventArgs( evt ) );
			}
		}
	}
}
