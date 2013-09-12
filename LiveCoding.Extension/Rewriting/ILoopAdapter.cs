using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	internal interface ILoopAdapter
	{
		StatementSyntax GetStatement();
		StatementSyntax WithStatement( StatementSyntax statement );
		SyntaxToken? GetIteratorName();
	}
}