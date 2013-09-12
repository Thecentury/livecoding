using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCoding.Core;
using LiveCoding.Extension.Annotations;

namespace LiveCoding.Extension.Views
{
	internal sealed class ValueChangeViewModel : INotifyPropertyChanged
	{
		private readonly Dictionary<int, ValueChange> _changes = new Dictionary<int, ValueChange>();

		public void AddChange( int columnNumber, ValueChange change )
		{
			_changes.Add( columnNumber, change );

			OnPropertyChanged( "p" + columnNumber );
		}

		[CanBeNull]
		public ValueChange GetChangeByIndex( int index )
		{
			ValueChange change;
			_changes.TryGetValue( index, out change );
			return change;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged( [CallerMemberName] string propertyName = null )
		{
			var handler = PropertyChanged;
			if ( handler != null )
			{
				handler( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}
	}
}