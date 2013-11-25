using System.Collections.Generic;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Extensions
{
	public static class ClassDeclarationSyntaxExtensions
	{
		public static IEnumerable<ClassDeclarationSyntax> GetSelfAndAllEnclosingClasses( this ClassDeclarationSyntax syntax )
		{
			yield return syntax;

			var @class = syntax;

			while ( true )
			{
				var enclosingClass = @class.Parent as ClassDeclarationSyntax;
				if ( enclosingClass != null )
				{
					@class = enclosingClass;
					yield return enclosingClass;
				}
				else
				{
					break;
				}
			}
		}
	}
}