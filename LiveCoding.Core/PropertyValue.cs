using System;
using System.Reflection;

namespace LiveCoding.Core
{
	public sealed class PropertyValue : IMemberValue
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

		public string MemberName
		{
			get { return _property.Name; }
		}

		public object GetValue()
		{
			return _property.GetValue( _target );
		}
	}
}