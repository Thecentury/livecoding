namespace LiveCoding.Core
{
	public static class ValueChangeExtensions
	{
		public static string GetValueString( this ValueChange change )
		{
			if ( change.Value == null )
			{
				return "null";
			}
			return change.Value.ToString();
		}
	}
}