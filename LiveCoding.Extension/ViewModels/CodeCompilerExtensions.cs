using System;
using JetBrains.Annotations;

namespace LiveCoding.Extension.ViewModels
{
	internal static class CodeCompilerExtensions
	{
		[StringFormatMethod( "format" )]
		public static void Compile( this ICodeCompiler compiler, string format, params object[] parameters )
		{
			compiler.Compile( String.Format( format, parameters ) );
		}
	}
}