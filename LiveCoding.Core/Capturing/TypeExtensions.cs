using System;
using System.Linq;

namespace LiveCoding.Core.Capturing
{
	internal static class TypeExtensions
	{
		public static bool IsSerializable( this Type type )
		{
			return type.GetCustomAttributes( typeof( SerializableAttribute ), true ).Any();
		}
	}
}