using System;
using System.Windows.Threading;

namespace LiveCoding.Extension.Views
{
	public static class DispatcherExtensions
	{
		public static void BeginInvoke( this Dispatcher dispatcher, Action callback, DispatcherPriority priority )
		{
			dispatcher.BeginInvoke( callback, priority );
		}
	}
}