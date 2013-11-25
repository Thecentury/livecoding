using System;

namespace LiveCoding.Core
{
	public static class ValueChangeExtensions
	{
		public static string GetValueString( this ValueChange change )
		{
			object capturedValue = change.CapturedValue;
			if ( capturedValue == null )
			{
				return "null";
			}

			if ( Equals( capturedValue, String.Empty ) )
			{
				return "\"\"";
			}

			return capturedValue.ToString();
		}
	}
}