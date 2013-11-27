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
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			_target = target;
			_property = property;
		}

		public string MemberName
		{
			get { return _property.Name; }
		}

		public object GetValue()
		{
			object rawValue = _property.GetValue( _target );
			if ( rawValue == null )
			{
				return null;
			}
			else
			{
				var type = rawValue.GetType();
				if ( type.IsPrintable() )
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
}