using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LiveCoding.Core
{
	public sealed class ReflectionObjectInfoProxy : IObjectInfoProxy
	{
		private readonly object _obj;

		public ReflectionObjectInfoProxy( object obj )
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
			return _obj as IEnumerable;
		}

		public IEnumerable<IMemberValue> GetMemberValues()
		{
			return
				TypesHelper.GetPublicPropertiesOf( _obj.GetType() )
					.Select( p => new PropertyValue( _obj, p ) )
					.Cast<IMemberValue>()
					.Concat( 
						TypesHelper.GetFieldsOf( _obj.GetType() )
						.Select( f => new FieldValue( _obj, f ) ) );
		}

		public override string ToString()
		{
			return _obj.ToString();
		}
	}
}