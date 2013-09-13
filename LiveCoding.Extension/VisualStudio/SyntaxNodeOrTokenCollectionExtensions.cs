using System.Collections.Generic;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.VisualStudio
{
	internal static class SyntaxNodeOrTokenCollectionExtensions
	{
		public static void AddComma( this ICollection<SyntaxNodeOrToken> collection )
		{
			collection.Add( Syntax.Token( SyntaxKind.CommaToken ) );
		}

		public static void AddLiteral( this ICollection<SyntaxNodeOrToken> collection, int literal )
		{
			collection.Add( Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.NumericLiteralExpression, Syntax.Literal( literal ) ) ) );
		}

		public static void AddIdentifier( this ICollection<SyntaxNodeOrToken> collection, string identifierName )
		{
			collection.Add( Syntax.Argument( Syntax.IdentifierName( identifierName ) ) );
		}
	}
}