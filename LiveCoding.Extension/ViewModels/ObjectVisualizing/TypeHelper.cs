using System;

namespace LiveCoding.Extension.ViewModels.ObjectVisualizing
{
	internal static class TypeHelper
	{
		public static bool IsExpandable( object o )
		{
			if ( o == null )
			{
				return false;
			}

			return !IsPrintableType( o.GetType() );
		}

		/// <summary>
		/// Gets a value indicating if the object graph can display this type without enumerating its children
		/// </summary>
		public static bool IsPrintableType( Type type )
		{
			return type != null && (
				type.IsPrimitive ||
				type.IsAssignableFrom( typeof( string ) ) ||
				type.IsEnum );
		}
	}
}