using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HorizontalGridSample
{
	public static class TypePrettyPrinter
	{
		private static readonly Regex _typeExtractionRegex = new Regex( @"^(?<typeName>[\w]+)`\d+$", RegexOptions.Compiled );

		public static string PrettyPrint( Type type )
		{
			if ( type.IsGenericType )
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();

				var typeExtractionMatch = _typeExtractionRegex.Match( genericTypeDefinition.Name );
				if ( !typeExtractionMatch.Success )
				{
					return type.Name;
				}
				else
				{
					string typeName = typeExtractionMatch.Groups[ "typeName" ].Value;

					string prettyName = String.Format( "{0}<{1}>", typeName,
						String.Join( ", ", type.GetGenericArguments().Select( a => PrettyPrint( a ) ) ) );
					return prettyName;
				}
			}

			return type.Name;
		}

		public static string GetCleanedNameOfGenericType( Type type )
		{
			if ( !type.IsGenericType )
			{
				throw new ArgumentException( String.Format( "Type '{0}' is not generic", type ) );
			}

			var typeExtractionMatch = _typeExtractionRegex.Match( type.GetGenericTypeDefinition().Name );
			if ( !typeExtractionMatch.Success )
			{
				return type.Name;
			}
			else
			{
				return typeExtractionMatch.Groups[ "typeName" ].Value;
			}
		}
	}
}
