using System;

namespace LiveCoding.Core
{
	public abstract class MemberValue
	{
		public abstract string MemberName { get; }

		public abstract Type MemberType { get; }

		public abstract object GetValue();
	}
}