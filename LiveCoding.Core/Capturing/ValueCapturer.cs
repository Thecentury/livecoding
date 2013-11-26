using System;
using System.Text;

namespace LiveCoding.Core.Capturing
{
	internal static class ValueCapturer
	{
		public static object WrapOriginalValue( object o )
		{
			if ( o == null )
			{
				return null;
			}

			return Capture( o );
		}

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
			Type type = o.GetType();
			if ( type.IsSerializable() && !type.IsInsideOfLiveCodingSubmission() )
			{
				return o;
			}

			return new AnotherAppDomainObjectInfoProxy( o );
		}
	}
}
