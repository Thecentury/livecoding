using System;
using System.Collections;
using System.Linq;
using LiveCoding.Core.Capturing;

namespace LiveCoding.Core
{
	public static class ObjectInfoProxyExtensions
	{
		public static bool Is<T>( this IObjectInfoProxy proxy )
		{
			return proxy.Execute( o => o is T );
		}

		public static bool IsArray( this IObjectInfoProxy proxy )
		{
			return proxy.Execute( o => o.GetType().IsArray );
		}

		public static string GetArrayElementTypeName( this IObjectInfoProxy proxy )
		{
			return proxy.Execute( o => o.GetType().GetElementType().Name );
		}

		public static string GetArrayDimensions( this IObjectInfoProxy proxy )
		{
			return proxy.Execute( o =>
			{
				var array = (Array)o;
				return String.Join( ", ", Enumerable.Range( 0, array.Rank ).Select( d => array.GetLength( d ) ) );
			} );
		}

		public static int GetCollectionCount( this IObjectInfoProxy proxy )
		{
			return proxy.Execute( o => ( (ICollection)o ).Count );
		}

		public static string PrettyPrintType( this IObjectInfoProxy proxy )
		{
			return proxy.Execute( o => TypePrettyPrinter.PrettyPrint( o.GetType() ) );
		}

		public static string GetTypeName( this IObjectInfoProxy proxy )
		{
			return proxy.Execute( o => o.GetType().Name );
		}

		public static bool IsPrintable( this IObjectInfoProxy proxy )
		{
			return proxy.Execute( o => TypeHelper.IsPrintable( o.GetType() ) );
		}
	}
}