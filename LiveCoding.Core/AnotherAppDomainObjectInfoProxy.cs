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

		public bool IsPrintable()
		{
			return TypeHelper.IsPrintableType( _obj.GetType() );
		}

		public bool IsArray()
		{
			return _obj.GetType().IsArray;
		}

		public string GetTypeName()
		{
			return _obj.GetType().Name;
		}

		public IEnumerable AsEnumerable()
		{
			// todo brinchuk 
			return null;
		}

		public IEnumerable<MemberValue> GetMemberValues()
		{
			return PropertiesHelper.FilterProperties( _obj.GetType() ).Select( p => new PropertyValueProxy( _obj, p ) );
		}
	}
}