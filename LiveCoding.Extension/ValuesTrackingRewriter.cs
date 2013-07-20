using System;
using System.Collections.Generic;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension
{
	public sealed class ValuesTrackingRewriter : SyntaxRewriter
	{
		private const string VariablesTracker = "global::LiveCoding.Core.VariablesTracker";
		private const string AddValueMethod = "AddValue";
		private const string QuotedValueStringFormat = "\"{0}\"";

		private static string Quote( string value )
		{
			return String.Format( QuotedValueStringFormat, value );
		}

		private IEnumerable<StatementSyntax> VisitStatement( dynamic statement )
		{
			return VisitStatement( statement );
		}

		private static IEnumerable<StatementSyntax> VisitStatement( StatementSyntax statement )
		{
			yield return statement;
		}

		private static IEnumerable<StatementSyntax> VisitStatement( LocalDeclarationStatementSyntax localDeclaration )
		{
			yield return localDeclaration;

			var identifier = localDeclaration.Declaration.Variables[0].Identifier;

			string identifierName = Quote( identifier.ValueText );

			var lineSpan = localDeclaration.SyntaxTree.GetLineSpan( localDeclaration.Span, usePreprocessorDirectives: true );

			var separatedList = Syntax.SeparatedList<ArgumentSyntax>(
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression, Syntax.Literal( identifierName, identifierName ) ) ),
				 Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.IdentifierName( localDeclaration.Declaration.Variables[0].Identifier ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.NumericLiteralExpression, Syntax.Literal( lineSpan.StartLinePosition.Line ) ) )
				);

			var track = CreateVariableTrackingExpression( separatedList );

			yield return track;
		}

		private static IEnumerable<StatementSyntax> VisitStatement( ExpressionStatementSyntax expression )
		{
			var rewritten = RewriteExpressionStatement( (dynamic)expression.Expression );
			return rewritten;
		}

		private IEnumerable<StatementSyntax> RewriteExpressionStatement( dynamic expression )
		{
			return RewriteExpressionStatement( expression );
		}

		private static IEnumerable<StatementSyntax> RewriteExpressionStatement( ExpressionSyntax expression )
		{
			yield return (StatementSyntax)expression.Parent;
		}

		private static IEnumerable<StatementSyntax> RewriteExpressionStatement( PostfixUnaryExpressionSyntax postfixExpression )
		{
			yield return (StatementSyntax)postfixExpression.Parent;

			var lineSpan = postfixExpression.SyntaxTree.GetLineSpan( postfixExpression.Span, usePreprocessorDirectives: true );
			string quotedOperand = Quote( postfixExpression.Operand.ToString() );
			var arguments = Syntax.SeparatedList<ArgumentSyntax>(
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression,
					Syntax.Literal( quotedOperand, quotedOperand ) ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.IdentifierName( postfixExpression.Operand.ToString() ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.NumericLiteralExpression, Syntax.Literal( lineSpan.StartLinePosition.Line ) ) )
				);

			var track = CreateVariableTrackingExpression( arguments );
			yield return track;
		}

		private static IEnumerable<StatementSyntax> RewriteExpressionStatement( PrefixUnaryExpressionSyntax prefixExpression )
		{
			yield return (StatementSyntax)prefixExpression.Parent;

			var lineSpan = prefixExpression.SyntaxTree.GetLineSpan( prefixExpression.Span, usePreprocessorDirectives: true );
			string quotedOperand = Quote( prefixExpression.Operand.ToString() );
			var arguments = Syntax.SeparatedList<ArgumentSyntax>(
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression,
					Syntax.Literal( quotedOperand, quotedOperand ) ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.IdentifierName( prefixExpression.Operand.ToString() ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.NumericLiteralExpression, Syntax.Literal( lineSpan.StartLinePosition.Line ) ) )
				);

			var track = CreateVariableTrackingExpression( arguments );
			yield return track;
		}

		private static IEnumerable<StatementSyntax> RewriteExpressionStatement( BinaryExpressionSyntax binaryExpression )
		{
			if ( !ChangesValue( binaryExpression.Kind ) )
			{
				yield return (StatementSyntax)binaryExpression.Parent;
			}

			string tempVariableName = GenerateTempVariableName();

			var initializer = binaryExpression.Kind == SyntaxKind.AssignExpression ? binaryExpression.Right : binaryExpression.Left;

			LocalDeclarationStatementSyntax localVariable =
				Syntax.LocalDeclarationStatement(
					Syntax.VariableDeclaration( Syntax.IdentifierName( "var" ) )
						.WithVariables( Syntax.SeparatedList(
							Syntax.VariableDeclarator( Syntax.Identifier( tempVariableName ) )
								.WithInitializer( Syntax.EqualsValueClause( initializer ) ) ) ) );
			yield return localVariable;

			if ( binaryExpression.Kind == SyntaxKind.AssignExpression )
			{
				var operation =
					Syntax.ExpressionStatement( Syntax.BinaryExpression( binaryExpression.Kind,
						binaryExpression.Left,
						Syntax.IdentifierName( tempVariableName ) ) );

				yield return operation;
			}
			else
			{
				var operation =
					Syntax.ExpressionStatement( Syntax.BinaryExpression( binaryExpression.Kind,
					Syntax.IdentifierName( tempVariableName ),
					binaryExpression.Right
						) );

				yield return operation;

				var assignment =
					Syntax.ExpressionStatement( Syntax.BinaryExpression( SyntaxKind.AssignExpression,
						binaryExpression.Left, Syntax.IdentifierName( tempVariableName ) ) );
				yield return assignment;
			}

			string assignmentLeftSideName = Quote( binaryExpression.Left.ToString() );

			var lineSpan = binaryExpression.SyntaxTree.GetLineSpan( binaryExpression.Span, usePreprocessorDirectives: true );
			var arguments = Syntax.SeparatedList<ArgumentSyntax>(
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression,
					Syntax.Literal( assignmentLeftSideName, assignmentLeftSideName ) ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.IdentifierName( tempVariableName ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.NumericLiteralExpression, Syntax.Literal( lineSpan.StartLinePosition.Line ) ) )
				);

			var track = CreateVariableTrackingExpression( arguments );
			yield return track;
		}


		public override SyntaxNode VisitBlock( BlockSyntax node )
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			foreach ( var statement in node.Statements )
			{
				StatementSyntax visited = (StatementSyntax)Visit( statement );

				statements.AddRange( VisitStatement( (dynamic)visited ) );
			}

			var result = Syntax.Block( statements );
			return result;
		}

		public override SyntaxNode VisitClassDeclaration( ClassDeclarationSyntax node )
		{
			ClassDeclarationSyntax visited = (ClassDeclarationSyntax)base.VisitClassDeclaration( node );
			visited = visited.WithModifiers( visited.Modifiers.ToPublic() );
			return visited;
		}

		public override SyntaxNode VisitMethodDeclaration( MethodDeclarationSyntax node )
		{
			MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)base.VisitMethodDeclaration( node );
			return methodDeclaration.WithModifiers( methodDeclaration.Modifiers.ToPublic() );
		}

		private static string GenerateTempVariableName()
		{
			var id = Guid.NewGuid().ToString( "N" );

			return String.Format( "__liveCodingTemp_{0}", id );
		}

		private static bool ChangesValue( SyntaxKind syntaxKind )
		{
			switch ( syntaxKind )
			{
				case SyntaxKind.AddAssignExpression:
				case SyntaxKind.AssignExpression:
				case SyntaxKind.AndAssignExpression:
				case SyntaxKind.DivideAssignExpression:
				case SyntaxKind.ExclusiveOrAssignExpression:
				case SyntaxKind.LeftShiftAssignExpression:
				case SyntaxKind.ModuloAssignExpression:
				case SyntaxKind.MultiplyAssignExpression:
				case SyntaxKind.OrAssignExpression:
				case SyntaxKind.RightShiftAssignExpression:
				case SyntaxKind.SubtractAssignExpression:
					{
						return true;
					}
				default:
					{
						return false;
					}
			}
		}

		private static ExpressionStatementSyntax CreateVariableTrackingExpression( SeparatedSyntaxList<ArgumentSyntax> arguments )
		{
			return Syntax.ExpressionStatement(
				Syntax.InvocationExpression(
					Syntax.MemberAccessExpression( SyntaxKind.MemberAccessExpression,
						Syntax.IdentifierName( VariablesTracker ), Syntax.IdentifierName( AddValueMethod ) ),
					Syntax.ArgumentList(
						arguments ) )
				);
		}
	}
}
