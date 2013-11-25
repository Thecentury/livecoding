using System;
using System.Collections.Generic;

namespace LiveCoding.Extension.Extensions
{
	public static class EnumerableExtensions
	{
		public static TAccumulator FoldR<T, TAccumulator>( this IEnumerable<T> collection, TAccumulator seed, Func<TAccumulator, T, TAccumulator> folder )
		{
			using ( var enumerator = collection.GetEnumerator() )
			{
				return enumerator.FoldR( seed, folder );
			}
		}

		public static TAccumulator FoldR<T, TAccumulator>( this IEnumerator<T> enumerator, TAccumulator seed, Func<TAccumulator, T, TAccumulator> folder )
		{
			if ( enumerator.MoveNext() )
			{
				var current = enumerator.Current;
				return folder( FoldR( enumerator, seed, folder ), current );
			}
			else
			{
				return seed;
			}
		}
	}
}