using System;

namespace LiveCoding.Core
{
	[Serializable]
	public sealed class InvocationEvent : LiveEvent, IPositionAware
	{
		public int LineNumber { get; set; }

		public int InvocationStartPosition { get; set; }

		public int InvocationEndPosition { get; set; }

		public object[] Parameters { get; set; }
		
		public int GetOriginalLineNumber()
		{
			return LineNumber;
		}
	}
}