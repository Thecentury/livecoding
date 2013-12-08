using System.Linq;
using LiveCoding.Core;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	public sealed class ClassFromNamespaceRewriter : SyntaxRewriter
	{
		public override SyntaxNode VisitNamespaceDeclaration( NamespaceDeclarationSyntax node )
		{
			return Syntax.ClassDeclaration( LiveCodingConstants.LiveCodingWrapperClassName )
				.WithMembers(
				Syntax.List(
					node.Members.Cast<MemberDeclarationSyntax>()
				) );
		}
	}
}