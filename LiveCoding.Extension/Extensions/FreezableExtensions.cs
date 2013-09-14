using System.Windows;

namespace LiveCoding.Extension.Extensions
{
	public static class FreezableExtensions
	{
		public static T AsFrozen<T>( this T freezable ) where T : Freezable
		{
			if ( freezable.CanFreeze && !freezable.IsFrozen )
			{
				freezable.Freeze();
			}
			return freezable;
		}
	}
}