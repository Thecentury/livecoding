using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using LiveCoding.Extension.VisualStudio;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	public sealed class ValuesTrackingRewriter : SyntaxRewriter
	{
		private readonly SyntaxTree _syntaxTree;
		private readonly Compilation _compilation;
		private readonly CancellationToken _cancellationToken;

		private readonly Lazy<SemanticModel> _semanticModelLazy;

		public ValuesTrackingRewriter( [NotNull] SyntaxTree syntaxTree, [NotNull] Compilation compilation, CancellationToken cancellationToken = default (CancellationToken) )
		{
			if ( syntaxTree == null )
			{
				throw new ArgumentNullException( "syntaxTree" );
			}
			if ( compilation == null )
			{
				throw new ArgumentNullException( "compilation" );
			}
			_syntaxTree = syntaxTree;
			_compilation = compilation;
			_cancellationToken = cancellationToken;

			_semanticModelLazy = new Lazy<SemanticModel>( () => _compilation.GetSemanticModel( _syntaxTree ) );
		}

		private const string VariablesTracker = "global::LiveCoding.Core.VariablesTrackerFacade";
		private const string AddValueMethod = "AddValue";
		private const string StartLoopMethod = "StartLoop";
		private const string EndLoopMethod = "EndForLoop";
		private const string RegisterLoopIterationMethod = "RegisterLoopIteration";
		private const string RegisterIf = "RegisterIf";
		private const string RegisterInvocation = "RegisterInvocation";
		private const string QuotedValueStringFormat = "\"{0}\"";

		private static string Quote( string value )
		{
			return String.Format( QuotedValueStringFormat, value );
		}

		public override SyntaxNode VisitIfStatement( IfStatementSyntax node )
		{
			IfStatementSyntax rewrittenIf = (IfStatementSyntax)base.VisitIfStatement( node );

			string conditionIdentifierName = "__live_coding_condition_" + Guid.NewGuid().ToString( "N" );

			var ifSpan = _syntaxTree.GetLineSpan( node.Span, true, _cancellationToken );

			List<SyntaxNodeOrToken> ifRegistrationArguments = new List<SyntaxNodeOrToken>();
			ifRegistrationArguments.AddIdentifier( conditionIdentifierName );
			ifRegistrationArguments.AddComma();
			ifRegistrationArguments.AddLiteral( ifSpan.StartLinePosition.Line );
			ifRegistrationArguments.AddComma();
			ifRegistrationArguments.AddLiteral( ifSpan.EndLinePosition.Line );
			ifRegistrationArguments.AddComma();

			var conditionSpan = node.Condition.Span;
			ifRegistrationArguments.AddLiteral( conditionSpan.Start );
			ifRegistrationArguments.AddComma();
			ifRegistrationArguments.AddLiteral( conditionSpan.End );

			if ( node.Else != null )
			{
				var elseSpan = _syntaxTree.GetLineSpan( node.Else.Span, true, _cancellationToken );

				ifRegistrationArguments.AddComma();
				ifRegistrationArguments.AddLiteral( elseSpan.StartLinePosition.Line );
				ifRegistrationArguments.AddComma();
				ifRegistrationArguments.AddLiteral( elseSpan.EndLinePosition.Line );
			}

			var result = Syntax.Block( Syntax.List<StatementSyntax>(
				Syntax.LocalDeclarationStatement(
				Syntax.VariableDeclaration(
						Syntax.PredefinedType( Syntax.Token( SyntaxKind.BoolKeyword ) ) )
					.WithVariables(
						Syntax.SeparatedList(
						Syntax.VariableDeclarator(
							Syntax.Identifier( conditionIdentifierName ) )
						.WithInitializer( Syntax.EqualsValueClause( rewrittenIf.Condition ) )
				) ) ),
				Syntax.ExpressionStatement(
				Syntax.InvocationExpression(
					Syntax.MemberAccessExpression(
						SyntaxKind.MemberAccessExpression,
						Syntax.IdentifierName( VariablesTracker ),
						Syntax.IdentifierName( RegisterIf ) ) )
					.WithArgumentList(
						Syntax.ArgumentList(
							Syntax.SeparatedList<ArgumentSyntax>( ifRegistrationArguments ) ) ) ),
				rewrittenIf.WithCondition( Syntax.IdentifierName( conditionIdentifierName ) )
			) );

			return result;
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

		public override SyntaxNode VisitForStatement( ForStatementSyntax node )
		{
			ForStatementSyntax rewrittenFor = (ForStatementSyntax)base.VisitForStatement( node );

			return RewriteLoop( node, new ForLoopAdapter( rewrittenFor ) );
		}

		private SyntaxNode RewriteLoop( SyntaxNode node, ILoopAdapter loop )
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

			var loopSpan = _syntaxTree.GetLineSpan( node.Span, true, _cancellationToken );

			var block = Syntax.Block(
				Syntax.LocalDeclarationStatement(
					Syntax.VariableDeclaration(
						Syntax.IdentifierName(
							Syntax.Identifier(
								@"var",
								Syntax.TriviaList() ) ) )
						.WithVariables(
							Syntax.SeparatedList(
								Syntax.VariableDeclarator( loopIdentifier )
								.WithInitializer(
									Syntax.EqualsValueClause(
										Syntax.InvocationExpression(
											Syntax.MemberAccessExpression(
												SyntaxKind.MemberAccessExpression,
												Syntax.IdentifierName(
													VariablesTracker ),
												Syntax.IdentifierName(
													StartLoopMethod ) ) )
											.WithArgumentList(
												Syntax.ArgumentList(
													Syntax.SeparatedList(
														Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.NumericLiteralExpression,
															Syntax.Literal( loopSpan.StartLinePosition.Line + 1 ) ) ) ) ) ) ) ) ) ) ),
				loopWithNewStatement,
				Syntax.ExpressionStatement(
					Syntax.InvocationExpression(
						Syntax.MemberAccessExpression(
							SyntaxKind.MemberAccessExpression,
							Syntax.IdentifierName( VariablesTracker ),
							Syntax.IdentifierName( EndLoopMethod ) ) )
						.WithArgumentList(
							Syntax.ArgumentList( Syntax.SeparatedList( Syntax.Argument( Syntax.IdentifierName( loopIdentifier ) ) ) ) ) ) );

			return block;
		}

		public override SyntaxNode VisitExpressionStatement( ExpressionStatementSyntax node )
		{
			var rewrittenExpressionStatement = base.VisitExpressionStatement( node );
			var rewrittenLines = VisitStatement( rewrittenExpressionStatement, node ).ToList();
			if ( rewrittenLines.Count == 1 )
			{
				return rewrittenLines[0];
			}

			return Syntax.Block( rewrittenLines );
		}

		private IEnumerable<StatementSyntax> VisitStatement( dynamic statement, StatementSyntax sourceNode )
		{
			return VisitStatement( statement, sourceNode );
		}

		private static IEnumerable<StatementSyntax> VisitStatement( StatementSyntax statement, StatementSyntax sourceNode )
		{
			yield return statement;
		}

		private IEnumerable<StatementSyntax> RewriteExpressionStatement( InvocationExpressionSyntax invocation, StatementSyntax source )
		{
			List<string> argNames = new List<string>( invocation.ArgumentList.Arguments.Count );
			foreach ( ArgumentSyntax argument in invocation.ArgumentList.Arguments )
			{
				string argName = String.Format( "__live_coding_arg_{0}", Guid.NewGuid().ToString( "N" ) );
				argNames.Add( argName );
				var arg = Syntax.LocalDeclarationStatement(
					Syntax.VariableDeclaration(
						Syntax.IdentifierName(
							Syntax.Identifier(
								@"var",
								Syntax.TriviaList() ) ) )
						.WithVariables(
							Syntax.SeparatedList(
								Syntax.VariableDeclarator( argName )
									.WithInitializer(
										Syntax.EqualsValueClause( argument.Expression ) ) ) ) );
				yield return arg;
			}

			var lineSpan = _syntaxTree.GetLineSpan( invocation.Span, true, _cancellationToken );
			List<SyntaxNodeOrToken> registerInvocationArgs = new List<SyntaxNodeOrToken>( invocation.ArgumentList.Arguments.Count * 2 + 6 );

			registerInvocationArgs.AddLiteral( lineSpan.StartLinePosition.Line );
			registerInvocationArgs.AddComma();
			registerInvocationArgs.AddLiteral( invocation.Span.Start );
			registerInvocationArgs.AddComma();
			registerInvocationArgs.AddLiteral( invocation.Span.End );
			registerInvocationArgs.AddComma();

			foreach ( string argName in argNames )
			{
				registerInvocationArgs.AddIdentifier( argName );
				registerInvocationArgs.AddComma();
			}

			registerInvocationArgs.RemoveAt( registerInvocationArgs.Count - 1 );

			int invocationArgsCount = invocation.ArgumentList.Arguments.Count * 2 - 1;
			if ( invocationArgsCount < 0 )
			{
				invocationArgsCount = 0;
			}
			SyntaxNodeOrToken[] targetInvocationArgs = new SyntaxNodeOrToken[invocationArgsCount];
			if ( targetInvocationArgs.Length > 0 )
			{
				registerInvocationArgs.CopyTo( 6, targetInvocationArgs, 0, targetInvocationArgs.Length );
			}

			yield return Syntax.ExpressionStatement(
				Syntax.InvocationExpression(
					Syntax.MemberAccessExpression(
						SyntaxKind.MemberAccessExpression,
						Syntax.IdentifierName( VariablesTracker ),
						Syntax.IdentifierName( RegisterInvocation ) ) )
					.WithArgumentList(
						Syntax.ArgumentList( Syntax.SeparatedList<ArgumentSyntax>( registerInvocationArgs ) ) ) );

			yield return Syntax.ExpressionStatement( invocation.WithArgumentList( Syntax.ArgumentList( Syntax.SeparatedList<ArgumentSyntax>( targetInvocationArgs ) ) ) );
		}

		private IEnumerable<StatementSyntax> VisitStatement( LocalDeclarationStatementSyntax localDeclaration, StatementSyntax sourceNode )
		{
			yield return localDeclaration;

			var identifier = localDeclaration.Declaration.Variables[0].Identifier;

			string identifierName = Quote( identifier.ValueText );

			var lineSpan = _syntaxTree.GetLineSpan( localDeclaration.Span, true, _cancellationToken );

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

		private IEnumerable<StatementSyntax> VisitStatement( ExpressionStatementSyntax expression, StatementSyntax source )
		{
			var rewritten = RewriteExpressionStatement( (dynamic)expression.Expression, source );
			return rewritten;
		}

		private IEnumerable<StatementSyntax> RewriteExpressionStatement( dynamic expression, StatementSyntax source )
		{
			return RewriteExpressionStatement( expression, source );
		}

		private IEnumerable<StatementSyntax> RewriteExpressionStatement( ExpressionSyntax expression, StatementSyntax source )
		{
			yield return (StatementSyntax)expression.Parent;
		}

		private IEnumerable<StatementSyntax> RewriteExpressionStatement( PostfixUnaryExpressionSyntax postfixExpression, StatementSyntax source )
		{
			yield return (StatementSyntax)postfixExpression.Parent;

			var lineSpan = _syntaxTree.GetLineSpan( postfixExpression.Span, true, _cancellationToken );
			string quotedOperand = Quote( postfixExpression.Operand.ToString() );
			var arguments = Syntax.SeparatedList<ArgumentSyntax>(
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression, Syntax.Literal( quotedOperand, quotedOperand ) ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.IdentifierName( postfixExpression.Operand.ToString() ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.NumericLiteralExpression, Syntax.Literal( lineSpan.StartLinePosition.Line ) ) )
				);

			var track = CreateVariableTrackingExpression( arguments );
			yield return track;
		}

		private IEnumerable<StatementSyntax> RewriteExpressionStatement( PrefixUnaryExpressionSyntax prefixExpression, StatementSyntax source )
		{
			yield return (StatementSyntax)prefixExpression.Parent;

			var lineSpan = _syntaxTree.GetLineSpan( prefixExpression.Span, true, _cancellationToken );
			string quotedOperand = Quote( prefixExpression.Operand.ToString() );
			var arguments = Syntax.SeparatedList<ArgumentSyntax>(
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression, Syntax.Literal( quotedOperand, quotedOperand ) ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.IdentifierName( prefixExpression.Operand.ToString() ) ),
				Syntax.Token( SyntaxKind.CommaToken ),
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.NumericLiteralExpression, Syntax.Literal( lineSpan.StartLinePosition.Line ) ) )
				);

			var track = CreateVariableTrackingExpression( arguments );
			yield return track;
		}

		private IEnumerable<StatementSyntax> RewriteExpressionStatement( BinaryExpressionSyntax binaryExpression, StatementSyntax source )
		{
			if ( !ChangesValue( binaryExpression.Kind ) )
			{
				yield return (StatementSyntax)binaryExpression.Parent;
				yield break;
			}

			ExpressionStatementSyntax expressionSource = source as ExpressionStatementSyntax;

			if ( expressionSource != null )
			{
				BinaryExpressionSyntax binaryExpressionSyntax = (BinaryExpressionSyntax)expressionSource.Expression;

				if ( binaryExpressionSyntax != null )
				{
					SymbolInfo symbolInfo = _semanticModelLazy.Value.GetSymbolInfo( binaryExpressionSyntax.Left, _cancellationToken );

					if ( symbolInfo.Symbol.Kind == SymbolKind.Event )
					{
						yield return (StatementSyntax)binaryExpression.Parent;
						yield break;
					}
				}
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

			var lineSpan = _syntaxTree.GetLineSpan( binaryExpression.Span, true, _cancellationToken );
			var arguments = Syntax.SeparatedList<ArgumentSyntax>(
				Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression, Syntax.Literal( assignmentLeftSideName, assignmentLeftSideName ) ) ),
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

				statements.AddRange( VisitStatement( (dynamic)visited, statement ) );
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
