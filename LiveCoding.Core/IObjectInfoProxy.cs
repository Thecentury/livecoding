using System;
using System.Collections;
using System.Collections.Generic;

namespace LiveCoding.Core
{
	public interface IObjectInfoProxy
	{
		TResult Execute<TResult>( Func<object, TResult> callback );

		IEnumerable AsEnumerable();

		IEnumerable<IMemberValue> GetMemberValues();
	}
}