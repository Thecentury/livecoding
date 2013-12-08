using System.Linq;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	public static class SyntaxTokenListExtensions
	{
		public static SyntaxTokenList ToPublic( this SyntaxTokenList tokenList )
		{
			var firstAccessModifierToken = tokenList.FirstOrDefault( t => t.IsAccessModifier() );

			SyntaxTriviaList? leadingTrivia = null;
			SyntaxTriviaList? trailingTrivia = null;
			if ( firstAccessModifierToken != default( SyntaxToken ) )
			{
				leadingTrivia = firstAccessModifierToken.LeadingTrivia;
				trailingTrivia = firstAccessModifierToken.TrailingTrivia;
			}

			return Syntax.TokenList(
				Enumerable.Repeat( Syntax.Token( SyntaxKind.PublicKeyword ).WithOptionalLeadingTrivia( leadingTrivia ).WithOptionalTrailingTrivia( trailingTrivia ), 1 )
				.Concat( tokenList.Where( t => !t.IsAccessModifier() ) ) );
		}
	}
}