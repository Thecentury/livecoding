using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using LiveCoding.Core;
using LiveCoding.Extension.Extensions;

namespace LiveCoding.Extension.ViewModels.ObjectVisualizing
{
	internal sealed class ObjectViewModel : INotifyPropertyChanged
	{
		private ReadOnlyCollection<ObjectViewModel> _children;
		private readonly ObjectViewModel _parent;
		private readonly IObjectInfoProxy _object;
		private readonly MemberValue _info;

		bool _isExpanded;
		bool _isSelected;

		public ObjectViewModel( object obj )
			: this( obj, null, null )
		{
		}

		private ObjectViewModel( object obj, MemberValue info, ObjectViewModel parent )
		{
			_object = obj != null ? ( ( obj as IObjectInfoProxy ) ?? new ReflectionObjectInfoProxy( obj ) ) : null;
			_info = info;
			if ( _object != null )
			{
				if ( _object.IsPrintable() )
				{
					// load the _children object with an empty collection to allow the + expander to be shown
					_children = new ReadOnlyCollection<ObjectViewModel>( new[] { new ObjectViewModel( null ) } );
				}
			}
			_parent = parent;
		}

		public void LoadChildren()
		{
			if ( _object == null )
			{
				return;
			}

			// exclude value types and strings from listing child members
			if ( _object.IsPrintable() )
			{
				return;
			}

			List<ObjectViewModel> children = new List<ObjectViewModel>();

			bool shouldAddSelfProperties = !_object.IsArray();

			if ( shouldAddSelfProperties )
			{
				// the public properties of this object are its children
				children.AddRange( _object.GetMemberValues().Select( p => new ObjectViewModel( p.GetValue(), p, this ) ) );
			}

			// if this is a collection type, add the contained items to the children
			IEnumerable collection = _object.AsEnumerable();
			if ( collection != null )
			{
				foreach ( var item in collection )
				{
					children.Add( new ObjectViewModel( item, null, this ) ); // todo: add something to view the index value
				}
			}

			_children = new ReadOnlyCollection<ObjectViewModel>( children );
			OnPropertyChanged( "Children" );
		}



		public ObjectViewModel Parent
		{
			get { return _parent; }
		}

		public MemberValue Info
		{
			get { return _info; }
		}

		public bool IsRoot
		{
			get { return _info != null; }
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
					type = string.Format( "({0})", _object.GetTypeName() );
				}
				else
				{
					if ( _info != null )
					{
						type = string.Format( "({0})", _info.MemberType.Name );
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
					name = _info.MemberName;
				}
				return name;
			}
		}

		public object RawValue
		{
			get { return _object; }
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
					else if ( type.IsGenericICollection() )
					{
						var list = (ICollection)_object;
						string cleanedTypeName = TypePrettyPrinter.GetCleanedNameOfGenericType( type );
						value = String.Format( "{0}<{1}>[ {2} ]", cleanedTypeName, String.Join( ", ", type.GetGenericArguments().Select( t => t.Name ) ), list.Count );
					}
					else if ( type.IsCollection() )
					{
						var collection = (ICollection)_object;
						value = String.Format( "{0}[ {1} ]", TypePrettyPrinter.PrettyPrint( type ), collection.Count );
					}
					else if ( _object is string || _object is StringBuilder )
					{
						value = String.Format( "\"{0}\"", _object );
					}
					else if ( _object.ToString() == _object.GetType().ToString() )
					{
						value = TypePrettyPrinter.PrettyPrint( type );
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
					OnPropertyChanged( "IsExpanded" );
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
					OnPropertyChanged( "IsSelected" );
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

		[NotifyPropertyChangedInvocator( "propertyName" )]
		private void OnPropertyChanged( string propertyName )
		{
			if ( PropertyChanged != null )
			{
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		#endregion
	}
}
