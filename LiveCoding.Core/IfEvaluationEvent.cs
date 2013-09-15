using System;

namespace LiveCoding.Core
{
	[Serializable]
	public sealed class IfEvaluationEvent : LiveEvent, IPositionAware
	{
		public bool ConditionValue { get; set; }

		public int ConditionStartPosition { get; set; }

		public int ConditionEndPosition { get; set; }

		public int StartIfLine { get; set; }

		public int EndIfLine { get; set; }

		public int? StartElseLine { get; set; }

		public int?	EndElseLine { get; set; }
		
		public int GetOriginalLineNumber()
		{
			return StartIfLine;
		}
	}
}