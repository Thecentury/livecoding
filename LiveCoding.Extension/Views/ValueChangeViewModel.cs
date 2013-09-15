using System.Collections.Generic;
using GalaSoft.MvvmLight;
using LiveCoding.Core;
using LiveCoding.Extension.Annotations;

namespace LiveCoding.Extension.Views
{
	internal sealed class ValueChangeViewModel : ViewModelBase
	{
		private readonly Dictionary<int, LiveEvent> _changes = new Dictionary<int, LiveEvent>();

		public void AddChange( int columnNumber, LiveEvent change )
		{
			_changes.Add( columnNumber, change );

			RaisePropertyChanged( "p" + columnNumber );
		}

		[CanBeNull]
		public LiveEvent GetChangeByIndex( int index )
		{
			LiveEvent change;
			_changes.TryGetValue( index, out change );
			return change;
		}
	}
}