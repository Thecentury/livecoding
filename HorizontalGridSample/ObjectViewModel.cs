using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HorizontalGridSample
{
	public class ObjectViewModel : INotifyPropertyChanged
	{
		ReadOnlyCollection<ObjectViewModel> _children;
		readonly ObjectViewModel _parent;
		readonly object _object;
		readonly PropertyInfo _info;
		readonly Type _type;

		bool _isExpanded;
		bool _isSelected;

		public ObjectViewModel( object obj )
			: this( obj, null, null )
		{
		}

		ObjectViewModel( object obj, PropertyInfo info, ObjectViewModel parent )
		{
			_object = obj;
			_info = info;
			if ( _object != null )
			{
				_type = obj.GetType();
				if ( !IsPrintableType( _type ) )
				{
					// load the _children object with an empty collection to allow the + expander to be shown
					_children = new ReadOnlyCollection<ObjectViewModel>( new[] { new ObjectViewModel( null ) } );
				}
			}
			_parent = parent;
		}

		public void LoadChildren()
		{
			if ( _object != null )
			{
				// exclude value types and strings from listing child members
				if ( !IsPrintableType( _type ) )
				{
					List<ObjectViewModel> children = new List<ObjectViewModel>();

					bool shouldAddSelfProperties = !_type.IsArray;

					if ( shouldAddSelfProperties )
					{
						// the public properties of this object are its children
						children.AddRange( _type.GetProperties()
							.Where( p => !p.GetIndexParameters().Any() ) // exclude indexed parameters for now
							.Select( p => new ObjectViewModel( p.GetValue( _object, null ), p, this ) ) );
					}

					// if this is a collection type, add the contained items to the children
					var collection = _object as IEnumerable;
					if ( collection != null )
					{
						foreach ( var item in collection )
						{
							children.Add( new ObjectViewModel( item, null, this ) ); // todo: add something to view the index value
						}
					}

					_children = new ReadOnlyCollection<ObjectViewModel>( children );
					this.OnPropertyChanged( "Children" );
				}
			}
		}

		/// <summary>
		/// Gets a value indicating if the object graph can display this type without enumerating its children
		/// </summary>
		static bool IsPrintableType( Type type )
		{
			return type != null && (
				type.IsPrimitive ||
				type.IsAssignableFrom( typeof( string ) ) ||
				type.IsEnum );
		}

		public ObjectViewModel Parent
		{
			get { return _parent; }
		}

		public PropertyInfo Info
		{
			get { return _info; }
		}

		public ReadOnlyCollection<ObjectViewModel> Children
		{
			get { return _children; }
		}

		public string Type
		{
			get
			{
				var type = string.Empty;
				if ( _object != null )
				{
					type = string.Format( "({0})", _type.Name );
				}
				else
				{
					if ( _info != null )
					{
						type = string.Format( "({0})", _info.PropertyType.Name );
					}
				}
				return type;
			}
		}

		public string Name
		{
			get
			{
				var name = string.Empty;
				if ( _info != null )
				{
					name = _info.Name;
				}
				return name;
			}
		}

		public string Value
		{
			get
			{
				string value;
				if ( _object != null )
				{
					Type type = _object.GetType();

					if ( type.IsArray )
					{
						Array array = (Array)_object;
						Type arrayItemType = type.GetElementType();

						value = String.Format( "{0}[ {1} ]", arrayItemType.Name, String.Join( ", ", Enumerable.Range( 0, array.Rank ).Select( d => array.GetLength( d ) ) ) );
					}
					else if ( type.IsGenericList() )
					{
						value = String.Format( "List<{0}>", String.Join( ", ", type.GetGenericArguments().Select( t => t.Name ) ) );
					}
					else if ( _object is string )
					{
						value = String.Format( "\"{0}\"", _object );
					}
					else
					{
						value = _object.ToString();
					}
				}
				else
				{
					value = "<null>";
				}
				return value;
			}
		}

		#region Presentation Members

		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if ( _isExpanded != value )
				{
					_isExpanded = value;
					if ( _isExpanded )
					{
						LoadChildren();
					}
					this.OnPropertyChanged( "IsExpanded" );
				}

				// Expand all the way up to the root.
				if ( _isExpanded && _parent != null )
				{
					_parent.IsExpanded = true;
				}
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if ( _isSelected != value )
				{
					_isSelected = value;
					this.OnPropertyChanged( "IsSelected" );
				}
			}
		}

		public bool NameContains( string text )
		{
			if ( String.IsNullOrEmpty( text ) || String.IsNullOrEmpty( Name ) )
			{
				return false;
			}

			return Name.IndexOf( text, StringComparison.InvariantCultureIgnoreCase ) > -1;
		}

		public bool ValueContains( string text )
		{
			if ( String.IsNullOrEmpty( text ) || String.IsNullOrEmpty( Value ) )
			{
				return false;
			}

			return Value.IndexOf( text, StringComparison.InvariantCultureIgnoreCase ) > -1;
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged( string propertyName )
		{
			if ( this.PropertyChanged != null )
			{
				this.PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		#endregion
	}
}
