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
		private readonly IMemberValue _info;

		bool _isExpanded;
		bool _isSelected;

		public ObjectViewModel( object obj )
			: this( obj, null, null )
		{
		}

		private ObjectViewModel( object obj, IMemberValue info, ObjectViewModel parent )
		{
			_object = obj != null ? ( ( obj as IObjectInfoProxy ) ?? new ReflectionObjectInfoProxy( obj ) ) : null;
			_info = info;
			if ( _object != null )
			{
				if ( !_object.IsPrintable() )
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

		public IMemberValue Info
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
					if ( _object.IsArray() )
					{
						string arrayElementTypeName = _object.GetArrayElementTypeName();

						value = String.Format( "{0}[ {1} ]", arrayElementTypeName, _object.GetArrayDimensions() );
					}
					else if ( _object.Execute( o => o.GetType().IsGenericICollection() ) )
					{
						string cleanedTypeName = _object.Execute( o => TypePrettyPrinter.GetCleanedNameOfGenericType( o.GetType() ) );
						value = String.Format( "{0}<{1}>[ {2} ]", cleanedTypeName, String.Join( ", ", _object.Execute( o => o.GetType().GetGenericArguments().Select( t => t.Name ) ) ), _object.GetCollectionCount() );
					}
					else if ( _object.Execute( o => o.GetType().IsCollection() ) )
					{
						value = String.Format( "{0}[ {1} ]", _object.PrettyPrintType(), _object.GetCollectionCount() );
					}
					else if ( _object.Is<string>() || _object.Is<StringBuilder>() )
					{
						value = String.Format( "\"{0}\"", _object );
					}
					else if ( _object.ToString() == _object.Execute( o => o.GetType().ToString() ) )
					{
						value = _object.PrettyPrintType();
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
