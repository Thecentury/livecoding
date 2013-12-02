using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiveCoding.Core.Capturing;

namespace LiveCoding.Core
{
	internal sealed class AnotherAppDomainObjectInfoProxy : MarshalByRefObject, IObjectInfoProxy
	{
		private readonly object _obj;

		public AnotherAppDomainObjectInfoProxy( object obj )
		{
			if ( obj == null )
			{
				throw new ArgumentNullException( "obj" );
			}
			_obj = obj;
		}

		public TResult Execute<TResult>( Func<object, TResult> callback )
		{
			return callback( _obj );
		}

		public IEnumerable AsEnumerable()
		{
			IEnumerable enumerable = _obj as IEnumerable;
			if ( enumerable == null )
			{
				return null;
			}

			var type = _obj.GetType();

			if ( type.IsArray && type.GetElementType().IsPrintable() )
			{
				return (IEnumerable)_obj;
			}

			// todo brinchuk 
			return ( enumerable ).Cast<object>().Select( o => o != null ? new AnotherAppDomainObjectInfoProxy( o ) : null ).ToList();
		}

		public IEnumerable<IMemberValue> GetMemberValues()
		{
			return TypesHelper.GetPropertiesOf( _obj.GetType() )
				.Select( p => new PropertyValueProxy( _obj, p ) )
				.Cast<IMemberValue>()
				.Concat(
					TypesHelper.GetFieldsOf( _obj.GetType() )
					.Select( f => new FieldValueProxy( _obj, f ) ) )
				.ToList();
		}

		public override string ToString()
		{
			return _obj.ToString();
		}
	}
}