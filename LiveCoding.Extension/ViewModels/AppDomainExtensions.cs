using System;

namespace LiveCoding.Extension.ViewModels
{
	internal static class AppDomainExtensions
	{
		public static T CreateInstanceAndUnwrap<T>( this AppDomain domain ) where T : MarshalByRefObject
		{
			return (T) domain.CreateInstanceAndUnwrap( typeof (T).Assembly.FullName, typeof (T).FullName );
		}
	}
}