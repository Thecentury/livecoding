using System.Collections.Generic;

namespace LiveCoding.Extension
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Append<T>( this IEnumerable<T> sequence, T obj )
		{
			foreach ( var o in sequence )
			{
				yield return o;
			}

			yield return obj;
		}
	}
}