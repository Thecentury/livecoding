using System.Collections.Generic;

namespace LiveCoding.Core.Internal
{
	internal static class StackExtensions
	{
		public static IEnumerable<T> PreviewHeads<T>( this Stack<T> stack )
		{
			while ( stack.Count > 0 )
			{
				yield return stack.Peek();
			}
		}
	}
}