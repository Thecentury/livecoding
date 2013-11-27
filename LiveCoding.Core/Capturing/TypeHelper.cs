using System;

namespace LiveCoding.Core.Capturing
{
	public static class TypeHelper
	{
		public static bool IsExpandable( object o )
		{
			if ( o == null )
			{
				return false;
			}

			if ( o is IObjectInfoProxy )
			{
				return true;
			}

			return !IsPrintable( o.GetType() );
		}

		/// <summary>
		/// Gets a value indicating if the object graph can display this type without enumerating its children
		/// </summary>
		public static bool IsPrintable( this Type type )
		{
			return type != null && (
				type.IsPrimitive ||
				type.IsAssignableFrom( typeof( string ) ) ||
				type.IsEnum );
		}
	}
}