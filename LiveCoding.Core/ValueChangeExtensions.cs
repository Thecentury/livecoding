using System;

namespace LiveCoding.Core
{
	public static class ValueChangeExtensions
	{
		public static string GetValueString( this ValueChange change )
		{
			if ( change.CapturedValue == null )
			{
				return "null";
			}

			if ( Equals( change.CapturedValue, String.Empty ) )
			{
				return "\"\"";
			}

			return change.CapturedValue.ToString();
		}
	}
}