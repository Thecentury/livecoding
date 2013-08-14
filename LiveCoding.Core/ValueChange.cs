using System.Diagnostics;

namespace LiveCoding.Core
{
	[DebuggerDisplay( "Change {VariableName} = {Value} @ {LineNumber} on {TimestampUtc}" )]
	public sealed class ValueChange : LiveEvent
	{
		public string VariableName { get; set; }

		public object Value { get; set; }

		public int LineNumber { get; set; }

		public string FilePath { get; set; }

		public int OriginalLineNumber { get; set; }
	}
}