using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LiveCoding.Core
{
	internal static class TypesHelper
	{
		public static IEnumerable<PropertyInfo> GetPublicPropertiesOf( Type type )
		{
			return type.GetProperties( BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance )
				.Where( p => !p.GetIndexParameters().Any() );
		}

		public static IEnumerable<FieldInfo> GetFieldsOf( Type type )
		{
			return type.GetFields( BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
		}
	}
}