using System;
using System.Collections.Generic;

namespace HorizontalGridSample
{
	internal static class TypeExtensions
	{
		public static bool IsGenericList( this Type type )
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof( List<> );
		}
	}
}