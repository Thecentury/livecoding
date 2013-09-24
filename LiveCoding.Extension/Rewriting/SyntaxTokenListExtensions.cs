using System.Linq;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	public static class SyntaxTokenListExtensions
	{
		public static SyntaxTokenList ToPublic( this SyntaxTokenList tokenList )
		{
			return Syntax.TokenList(
				Enumerable.Repeat( Syntax.Token( SyntaxKind.PublicKeyword ), 1 )
				.Concat( tokenList.Where( t => !t.IsAccessModifier() ) ) );
		}
	}
}