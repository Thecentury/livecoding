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

		public static bool IsInsideOfLiveCodingSubmission( this Type type )
		{
			return type.FullName.Contains( LiveCodingConstants.LiveCodingWrapperClassName );
		}
	}
}