using System;
using System.Collections.Generic;

namespace LiveCoding.Extension.Extensions
{
	public static class StringEnumerableExtensions
	{
		public static string Join( this IEnumerable<string> strings, string separator )
		{
			return String.Join( separator, strings );
		}
	}
}