using System;
using System.Reflection;

namespace LiveCoding.Core
{
	public sealed class PropertyValue : MemberValue
	{
		private readonly object _target;
		private readonly PropertyInfo _property;

		public PropertyValue( object target, PropertyInfo property )
		{
			if ( target == null )
			{
				throw new ArgumentNullException( "target" );
			}
			if ( property == null )
			{
				throw new ArgumentNullException( "property" );
			}
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
			return _property.GetValue( _target );
		}
	}
}