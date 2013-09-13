using System;
using System.Collections.Generic;
using System.Linq;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	public sealed class ValuesTrackingRewriter : SyntaxRewriter
	{
		private const string VariablesTracker = "global::LiveCoding.Core.VariablesTrackerFacade";
		private const string AddValueMethod = "AddValue";
		private const string StartForLoopMethod = "StartForLoop";
		private const string EndForLoopMethod = "EndForLoop";
		private const string RegisterLoopIterationMethod = "RegisterLoopIteration";
		private const string QuotedValueStringFormat = "\"{0}\"";

		private static string Quote( string value )
		{
			return String.Format( QuotedValueStringFormat, value );
		}

		public override SyntaxNode VisitWhileStatement( WhileStatementSyntax node )
		{
			WhileStatementSyntax whileStatement = (WhileStatementSyntax)base.VisitWhileStatement( node );

			return RewriteLoop( node, new WhileLoopAdapter( whileStatement ) );
		}

		public override SyntaxNode VisitDoStatement( DoStatementSyntax node )
		{
			DoStatementSyntax doStatement = (DoStatementSyntax)base.VisitDoStatement( node );

			return RewriteLoop( node, new DoWhileLoopAdapter( doStatement ) );
		}

		public override SyntaxNode VisitForEachStatement( ForEachStatementSyntax node )
		{
			ForEachStatementSyntax forEachStatement = (ForEachStatementSyntax)base.VisitForEachStatement( node );

			return RewriteLoop( node, new ForeachLoopAdapter( forEachStatement ) );
		}

		public override SyntaxNode VisitExpressionStatement( ExpressionStatementSyntax node )
		{
			var rewrittenLines = VisitStatement( base.VisitExpressionStatement( node ) ).ToList();
			if ( rewrittenLines.Count == 1 )
			{
				return rewrittenLines[ 0 ];
			}
			else
			{
				return Syntax.Block( rewrittenLines );
			}
		}

		public override SyntaxNode VisitForStatement( ForStatementSyntax node )
		{
			ForStatementSyntax rewrittenFor = (ForStatementSyntax)base.VisitForStatement( node );

			return RewriteLoop( node, new ForLoopAdapter( rewrittenFor ) );
		}

		private static SyntaxNode RewriteLoop( SyntaxNode node, ILoopAdapter loop )
		{
			Guid loopIdHolderId = Guid.NewGuid();
			SyntaxToken loopIdentifier = Syntax.Identifier( String.Format( "__loop_id_{0}", loopIdHolderId.ToString( "N" ) ) );

			List<StatementSyntax> loopBodyStatements = new List<StatementSyntax>();

			SyntaxToken? iteratorName = loop.GetIteratorName();
			SyntaxNodeOrToken[] registerIterationArguments;

			if ( iteratorName != null )
			{
				registerIterationArguments = new SyntaxNodeOrToken[]
				{
					Syntax.Argument( Syntax.IdentifierName( loopIdentifier ) ),
					Syntax.Token( SyntaxKind.CommaToken ),
					Syntax.Argument( Syntax.IdentifierName( iteratorName.Value ) )
				};
			}
			else
			{
				registerIterationArguments = new SyntaxNodeOrToken[] { Syntax.Argument( Syntax.IdentifierName( loopIdentifier ) ) };
			}

			loopBodyStatements.Add( Syntax.ExpressionStatement(
				Syntax.InvocationExpression(
					Syntax.MemberAccessExpression(
						SyntaxKind.MemberAccessExpression,
						Syntax.IdentifierName( VariablesTracker ),
						Syntax.IdentifierName( RegisterLoopIterationMethod ) ) )
					.WithArgumentList(
						Syntax.ArgumentList(
							Syntax.SeparatedList<ArgumentSyntax>( registerIterationArguments ) ) ) ) );

			BlockSyntax previousLoopBodyBlock = loop.GetStatement() as BlockSyntax;
			if ( previousLoopBodyBlock != null )
			{
				loopBodyStatements.AddRange( previousLoopBodyBlock.Statements );
			}
			else
			{
				loopBodyStatements.Add( loop.GetStatement() );
			}

			var loopWithNewStatement = loop.WithStatement( Syntax.Block( loopBodyStatements ) );

			var loopSpan = node.SyntaxTree.GetLineSpan( node.Span, usePreprocessorDirectives: true );

			var block = Syntax.Block(
				Syntax.LocalDeclarationStatement(
					Syntax.VariableDeclaration(
						Syntax.IdentifierName(
							Syntax.Identifier(
								@"var",
								Syntax.TriviaList() ) ) )
						.WithVariables(
							Syntax.SeparatedList(
								Syntax.VariableDeclarator(
									loopIdentifier )
									.WithInitializer(
										Syntax.EqualsValueClause(
											Syntax.InvocationExpression(
												Syntax.MemberAccessExpression(
													SyntaxKind.MemberAccessExpression,
													Syntax.IdentifierName(
														VariablesTracker ),
													Syntax.IdentifierName(
														StartForLoopMethod ) ) )
												.WithArgumentList(
													Syntax.ArgumentList(
														Syntax.SeparatedList(
															Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.NumericLiteralExpression,
																Syntax.Literal( loopSpan.StartLinePosition.Line ) ) ) ) ) ) ) ) ) ) ),
				loopWithNewStatement,
				Syntax.ExpressionStatement(
					Syntax.InvocationExpression(
						Syntax.MemberAccessExpression(
							SyntaxKind.MemberAccessExpression,
							Syntax.IdentifierName( VariablesTracker ),
							Syntax.IdentifierName( EndForLoopMethod ) ) )
						.WithArgumentList(
							Syntax.ArgumentList( Syntax.SeparatedList( Syntax.Argument( Syntax.IdentifierName( loopIdentifier ) ) ) ) ) ) );

			return block;
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

			var identifier = localDeclaration.Declaration.Variables[ 0 ].Identifier;

			string identifierName = Quote( identifier.ValueText );

			var lineSpan = localDeclaration.SyntaxTree.GetLineSpan( localDeclaration.Span, usePreprocessorDirectives: true );

			var separatedList = Syntax.SeparatedList<ArgumentSyntax>(
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression, Syntax.Literal( identifierName, identifierName ) ) ),
				 Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.IdentifierName( localDeclaration.Declaration.Variables[ 0 ].Identifier ) ),
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
			var visited = (ClassDeclarationSyntax)base.VisitClassDeclaration( node );
			visited = visited.WithModifiers( visited.Modifiers.ToPublic() );
			return visited;
		}

		public override SyntaxNode VisitMethodDeclaration( MethodDeclarationSyntax node )
		{
			MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)base.VisitMethodDeclaration( node );
			return methodDeclaration.WithModifiers( methodDeclaration.Modifiers.ToPublic() );
		}

		public override SyntaxNode VisitConstructorDeclaration( ConstructorDeclarationSyntax node )
		{
			var classDeclaration = (ConstructorDeclarationSyntax)base.VisitConstructorDeclaration( node );
			return classDeclaration.WithModifiers( classDeclaration.Modifiers.ToPublic() );
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
