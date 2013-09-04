using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using LiveCoding.Core;
using LiveCoding.Extension.Annotations;

namespace LiveCoding.Extension.Views
{
	internal sealed class ValueChangeViewModel : DynamicObject, INotifyPropertyChanged
	{
		private readonly Dictionary<int, ValueChange> _changes = new Dictionary<int, ValueChange>();

		public void AddChange( int columnNumber, ValueChange change )
		{
			_changes.Add( columnNumber, change );

			OnPropertyChanged( "p" + columnNumber );
		}

		private PropertyChangedEventHandler _propertyChangedEventHandler;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { _propertyChangedEventHandler += value; }
			remove { _propertyChangedEventHandler = (PropertyChangedEventHandler)Delegate.Remove( _propertyChangedEventHandler, value ); }
		}

		public override bool TryGetMember( GetMemberBinder binder, out object result )
		{
			int columnNumber = Int32.Parse( binder.Name.Substring( 1 ) );
			ValueChange change;
			if (_changes.TryGetValue(columnNumber, out change))
			{
				result = change;
			}
			else
			{
				result = null;
			}

			return true;
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged( [CallerMemberName] string propertyName = null )
		{
			var handler = _propertyChangedEventHandler;
			if ( handler != null )
			{
				handler( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}
	}
}