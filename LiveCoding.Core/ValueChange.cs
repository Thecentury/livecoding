﻿using System;
using System.Diagnostics;

namespace LiveCoding.Core
{
	[Serializable]
	[DebuggerDisplay( "Change {VariableName} = {OriginalValue} @ {OriginalLineNumber} on {TimestampUtc}" )]
	public sealed class ValueChange : LiveEvent, IPositionAware
	{
		public string VariableName { get; set; }

		public object CapturedValue { get; set; }

		public int OriginalLineNumber { get; set; }

		public override string ToString()
		{
			return String.Format( "{0}", CapturedValue );
		}

		public int GetOriginalLineNumber()
		{
			return OriginalLineNumber;
		}
	}
}