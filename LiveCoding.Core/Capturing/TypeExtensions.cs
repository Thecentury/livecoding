using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveCoding.Core.Capturing
{
	internal static class TypeExtensions
	{
		public static bool IsSerializable( this Type type )
		{
			bool isSerializable = type.GetCustomAttributes( typeof( SerializableAttribute ), true ).Any();
			return isSerializable;
		}

		public static bool IsInsideOfLiveCodingSubmission( this Type type )
		{
			bool isInsideOfLiveCodingSubmission = type.FullName.Contains( LiveCodingConstants.LiveCodingWrapperClassName );
			return isInsideOfLiveCodingSubmission;
		}

		public static IEnumerable<Type> GetBaseTypes( this Type type )
		{
			var baseType = type.BaseType;
			while ( baseType != null )
			{
				yield return baseType;

				baseType = baseType.BaseType;
			}
		}

		public static bool IsDelegate( this Type type )
		{
			bool isDelegate = type.GetBaseTypes().Contains( typeof( Delegate ) );
			return isDelegate;
		}
	}
}