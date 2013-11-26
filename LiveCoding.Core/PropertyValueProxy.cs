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

		private Type MemberType
		{
			get { return _property.PropertyType; }
		}

		public object GetValue()
		{
			object rawValue = _property.GetValue( _target );
			if ( MemberType.IsSerializable() && !MemberType.IsInsideOfLiveCodingSubmission() || rawValue == null )
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