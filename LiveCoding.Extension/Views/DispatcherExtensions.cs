using System;
using System.Windows.Threading;
using GalaSoft.MvvmLight;

namespace LiveCoding.Extension.Views
{
	public static class DispatcherExtensions
	{
		public static void BeginInvoke( this Dispatcher dispatcher, Action callback, DispatcherPriority priority )
		{
			dispatcher.BeginInvoke( callback, priority );
		}
	}

	internal sealed class ConditionViewModel : ViewModelBase
	{
		private bool _value;

		public bool Value
		{
			get { return _value; }
			set
			{
				_value = value;
				RaisePropertyChanged( () => Value );
			}
		}
	}
}