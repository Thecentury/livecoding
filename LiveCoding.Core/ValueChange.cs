using System;
using System.Diagnostics;

namespace LiveCoding.Core
{
	[Serializable]
	[DebuggerDisplay( "Change {VariableName} = {OriginalValue} @ {LineNumber} on {TimestampUtc}" )]
	public sealed class ValueChange : LiveEvent
	{
		public string VariableName { get; set; }

		public object OriginalValue { get; set; }

		public object CapturedValue { get; set; }

		public int LineNumber { get; set; }

		public string FilePath { get; set; }

		public int OriginalLineNumber { get; set; }

		public override string ToString()
		{
			return String.Format( "{0}", CapturedValue );
		}
	}
}