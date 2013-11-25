using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LiveCoding.Core
{
	internal static class PropertiesHelper
	{
		public static IEnumerable<PropertyInfo> FilterProperties( Type type )
		{
			return type.GetProperties( BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance )
				.Where( p => !p.GetIndexParameters().Any() );
		}
	}
}