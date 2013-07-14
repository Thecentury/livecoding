using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension
{
    public sealed class ClassFromNamespaceRewriter : SyntaxRewriter
    {
        public override SyntaxNode VisitNamespaceDeclaration( NamespaceDeclarationSyntax node )
        {
            return Visit( node.Members.First() );
        }
    }
}