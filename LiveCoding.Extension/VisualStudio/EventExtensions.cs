using System;

namespace LiveCoding.Extension.VisualStudio
{
	internal static class EventExtensions
	{
		public static void Raise<T>( this EventHandler<T> handler, object sender, T eventArgs )
		{
			if ( handler != null )
			{
				handler( sender, eventArgs );
			}
		}
	}
}