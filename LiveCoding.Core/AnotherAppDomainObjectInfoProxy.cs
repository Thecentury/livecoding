using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
			// todo brinchuk 
			return null;
		}

		public IEnumerable<IMemberValue> GetMemberValues()
		{
			return PropertiesHelper.FilterProperties( _obj.GetType() ).Select( p => new PropertyValueProxy( _obj, p ) ).ToList();
		}

		public override string ToString()
		{
			return _obj.ToString();
		}
	}
}