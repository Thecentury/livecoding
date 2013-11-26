using System;

namespace LiveCoding.Core
{
	public interface IMemberValue
	{
		string MemberName { get; }

		Type MemberType { get; }
		
		object GetValue();
	}
}