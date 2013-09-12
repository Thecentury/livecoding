using System;
using System.Linq;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	public static class ParameterListSyntaxExtensions
	{
		public static string GetDefaultParametersValuesString( this ParameterListSyntax parameterList )
		{
			return String.Join( ", ",
				parameterList.Parameters.Select( p => String.Format( (string) "default({0})", (object) p.Type.ToString() ) ) );
		}
	}
}