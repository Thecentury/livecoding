using System;
using System.Collections;
using System.Collections.Generic;

namespace LiveCoding.Core
{
	public interface IObjectInfoProxy
	{
		bool IsPrintable();

		bool IsArray();

		string GetTypeName();

		IEnumerable AsEnumerable();

		IEnumerable<MemberValue> GetMemberValues();
	}
}