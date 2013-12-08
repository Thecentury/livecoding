using System;
using LiveCoding.Core.Capturing;

namespace LiveCoding.Core
{
	public static class VariablesTrackerFacade
	{
		public static void AddValue( string variableName, object value, int originalLineNumber = 0 )
		{
			object capturedValue = ValueCapturer.CreateCapturedValue( value );

			var valueChange = new ValueChange
			{
				VariableName = variableName,
				OriginalLineNumber = originalLineNumber,
				CapturedValue = capturedValue
			};

			AddEvent( valueChange );
		}

		public static Guid StartLoop( int lineNumber )
		{
			var evt = new ForLoopStartedEvent
			{
				LoopStartLineNumber = lineNumber
			};

			AddEvent( evt );

			return evt.LoopId;
		}

		private static object[] CaptureParameters( object[] parameters )
		{
			object[] capturedParameters = new object[parameters.Length];

			for ( var i = 0; i < parameters.Length; i++ )
			{
				var parameter = parameters[i];
				var capturedValue = ValueCapturer.CreateCapturedValue( parameter );
				capturedParameters[i] = capturedValue;
			}

			return capturedParameters;
		}

		public static void RegisterInvocation( int lineNumber, int startPosition, int endPosition, params object[] parameters )
		{
			var evt = new InvocationEvent
			{
				LineNumber = lineNumber,
				InvocationStartPosition = startPosition,
				InvocationEndPosition = endPosition,
				Parameters = CaptureParameters( parameters )
			};

			AddEvent( evt );
		}

		public static bool RegisterIf( bool result, int startIfLine, int endIfLine, int conditionStartPosition, int conditionEndPosition, int? startElseLine = null, int? endElseLine = null )
		{
			var evt = new IfEvaluationEvent
			{
				ConditionValue = result,
				StartIfLine = startIfLine,
				EndIfLine = endIfLine,
				StartElseLine = startElseLine,
				EndElseLine = endElseLine,
				ConditionStartPosition = conditionStartPosition,
				ConditionEndPosition = conditionEndPosition
			};

			AddEvent( evt );

			return result;
		}

		public static void RegisterLoopIteration( Guid loopId, object iteratorValue = null )
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