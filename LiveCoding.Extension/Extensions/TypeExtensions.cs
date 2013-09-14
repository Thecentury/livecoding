using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LiveCoding.Extension.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsGenericICollection( this Type type )
		{
			if ( !type.IsGenericType )
			{
				return false;
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition.GetInterfaces().Any( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof( ICollection<> ) );
		}

		public static bool IsCollection( this Type type )
		{
			return type.GetInterfaces().Contains( typeof( ICollection ) );
		}
	}
}
