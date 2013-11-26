using System;
using System.Reflection;
using LiveCoding.Core.Capturing;

namespace LiveCoding.Core
{
	internal sealed class PropertyValueProxy : MarshalByRefObject, IMemberValue
	{
		private readonly object _target;
		private readonly PropertyInfo _property;

		public PropertyValueProxy( object target, PropertyInfo property )
		{
			_target = target;
			_property = property;
		}

		public string MemberName
		{
			get { return _property.Name; }
		}

		public Type MemberType
		{
			get
			{
				// todo brinchuk do not expose member type
				return _property.PropertyType;
			}
		}

		public object GetValue()
		{
			object rawValue = _property.GetValue( _target );
			if ( MemberType.IsSerializable() )
			{
				return rawValue;
			}
			else
			{
				return new AnotherAppDomainObjectInfoProxy( rawValue );
			}
		}
	}
}