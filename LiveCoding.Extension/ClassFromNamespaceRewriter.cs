using System.Linq;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension
{
	public sealed class ClassFromNamespaceRewriter : SyntaxRewriter
	{
		public static readonly string LiveCodingWrapperClassName = "__LiveCodingWrapper";

		public override SyntaxNode VisitNamespaceDeclaration( NamespaceDeclarationSyntax node )
		{
			return Syntax.ClassDeclaration( LiveCodingWrapperClassName )
				.WithMembers(
				Syntax.List(
				node.Members.Select( m => Visit( m ) ).Cast<MemberDeclarationSyntax>()
				) );
		}
	}
}