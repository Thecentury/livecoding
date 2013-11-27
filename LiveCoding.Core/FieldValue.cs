using System;
using System.Reflection;

namespace LiveCoding.Core
{
	internal sealed class FieldValue : IMemberValue
	{
		private readonly object _target;
		private readonly FieldInfo _field;

		public FieldValue( object target, FieldInfo field )
		{
			if ( target == null )
			{
				throw new ArgumentNullException( "target" );
			}
			if ( field == null )
			{
				throw new ArgumentNullException( "field" );
			}
			_target = target;
			_field = field;
		}

		public string MemberName
		{
			get { return _field.Name; }
		}

		public object GetValue()
		{
			return _field.GetValue( _target );
		}
	}
}