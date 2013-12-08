using LiveCoding.Extension.Rewriting;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Tests
{
	/// <summary>
	/// This rewriter reproduces a bug when access modifier has a leading trivia that is ignored when this modifier is replaced with 'public'.
	/// </summary>
	public sealed class EmptyRewriter : SyntaxRewriter
	{
		public override SyntaxNode VisitMethodDeclaration( MethodDeclarationSyntax node )
		{
			MethodDeclarationSyntax visitMethodDeclaration = (MethodDeclarationSyntax)base.VisitMethodDeclaration( node );
			return visitMethodDeclaration.WithModifiers( visitMethodDeclaration.Modifiers.ToPublic() );
		}
	}
}