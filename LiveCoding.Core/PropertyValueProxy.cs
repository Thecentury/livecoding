using System;
using System.Reflection;
using LiveCoding.Core.Capturing;

namespace LiveCoding.Core
{
	internal sealed class PropertyValueProxy : MemberValue
	{
		private readonly object _target;
		private readonly PropertyInfo _property;

		public PropertyValueProxy( object target, PropertyInfo property )
		{
			_target = target;
			_property = property;
		}

		public override string MemberName
		{
			get { return _property.Name; }
		}

		public override Type MemberType
		{
			get { return _property.PropertyType; }
		}

		public override object GetValue()
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