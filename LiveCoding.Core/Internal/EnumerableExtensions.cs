using System.Collections.Generic;
using System.Linq;

namespace LiveCoding.Core.Internal
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> NotOfType<T, TDesired>( this IEnumerable<T> enumerable ) where TDesired : T
		{
			return enumerable.TakeWhile( i => !( i is TDesired ) );
		}
	}
}