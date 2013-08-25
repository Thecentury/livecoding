using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			return builder.ToString();
		}

		private static object Capture( object o )
		{
			return o;
		}
	}
}
