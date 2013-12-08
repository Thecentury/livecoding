using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	public static class SyntaxTokenExtensions
	{
		public static bool IsAccessModifier( this SyntaxToken token )
		{
			return token.Kind.IsAccessibilityModifier();
		}

		public static SyntaxToken WithOptionalLeadingTrivia( this SyntaxToken token, SyntaxTriviaList? leadingTrivia )
		{
			if ( leadingTrivia.HasValue )
			{
				return token.WithLeadingTrivia( leadingTrivia.Value );
			}
			else
			{
				return token;
			}
		}

		public static SyntaxToken WithOptionalTrailingTrivia( this SyntaxToken token, SyntaxTriviaList? trailingTrivia )
		{
			if ( trailingTrivia.HasValue )
			{
				return token.WithTrailingTrivia( trailingTrivia.Value );
			}
			else
			{
				return token;
			}
		}
	}
}