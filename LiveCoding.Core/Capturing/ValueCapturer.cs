using System.Text;

namespace LiveCoding.Core.Capturing
{
	internal static class ValueCapturer
	{
		public static object CreateCapturedValue( dynamic value )
		{
			if ( value == null )
			{
				return null;
			}

			return Capture( value );
		}

		private static object Capture( StringBuilder builder )
		{
			return new StringBuilder( builder.ToString() );
		}

		private static object Capture( object o )
		{
			return o;
		}
	}
}
