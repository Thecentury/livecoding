using System.Linq;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension
{
	public static class SyntaxTokenListExtensions
	{
		public static SyntaxTokenList ToPublic( this SyntaxTokenList tokenList )
		{
			return Syntax.TokenList( tokenList.Where( t => !t.IsAccessModifier() )
				.Concat( Enumerable.Repeat( Syntax.Token( SyntaxKind.PublicKeyword ), 1 ) ) );
		}
	}
}