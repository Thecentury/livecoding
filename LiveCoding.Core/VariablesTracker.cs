using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LiveCoding.Core
{
	public static class VariablesTracker
	{
		// todo brinchuk thread safety
		private static readonly List<ValueChange> _changes = new List<ValueChange>();

		public static IReadOnlyList<ValueChange> Changes
		{
			get { return _changes.AsReadOnly(); }
		}

		public static void ClearRecords()
		{
			_changes.Clear();
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
			_changes.Add( valueChange );

			RaiseValueAdded( valueChange );
		}

		public static event EventHandler<ValueAddedEventArgs> ValueAdded;

		private static void RaiseValueAdded( ValueChange valueChange )
		{
			EventHandler<ValueAddedEventArgs> handler = ValueAdded;
			if ( handler != null )
			{
				handler( null, new ValueAddedEventArgs( valueChange ) );
			}
		}
	}
}
