using System;
using System.Reflection;
using LiveCoding.Core.Capturing;

namespace LiveCoding.Core
{
	internal sealed class FieldValueProxy : MarshalByRefObject, IMemberValue
	{
		private readonly object _target;
		private readonly FieldInfo _field;

		public FieldValueProxy( object target, FieldInfo field )
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
			object rawValue = _field.GetValue( _target );
			if ( rawValue == null )
			{
				return rawValue;
			}
			else
			{
				var type = rawValue.GetType();
				if ( type.IsSerializable() && !type.IsInsideOfLiveCodingSubmission() )
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