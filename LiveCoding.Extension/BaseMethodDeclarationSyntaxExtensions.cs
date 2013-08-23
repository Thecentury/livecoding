using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension
{
	public static class BaseMethodDeclarationSyntaxExtensions
	{
		public static bool IsStatic( this BaseMethodDeclarationSyntax methodDeclaration )
		{
			return methodDeclaration.Modifiers.Any( SyntaxKind.StaticKeyword );
		}
	}
}