using System;
using System.Runtime.CompilerServices;
using LiveCoding.Core.Capturing;

namespace LiveCoding.Core
{
	public static class VariablesTrackerFacade
	{
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
			_listener.OnEventAdded( evt );
		}

		private static ILiveEventListener _listener;

		public static void SetListener( ILiveEventListener listener )
		{
			if ( listener == null )
			{
				throw new ArgumentNullException( "listener" );
			}
			_listener = listener;
		}
	}
}