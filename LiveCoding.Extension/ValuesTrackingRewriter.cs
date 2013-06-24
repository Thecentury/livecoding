using System;
using System.Collections.Generic;
using Roslyn.Compilers;
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

        public override SyntaxNode VisitBlock( BlockSyntax node )
        {
            List<StatementSyntax> statements = new List<StatementSyntax>();

            foreach ( var statement in node.Statements )
            {
                StatementSyntax visited = (StatementSyntax)Visit( statement );

                LocalDeclarationStatementSyntax localDeclaration = visited as LocalDeclarationStatementSyntax;
                if ( localDeclaration != null )
                {
                    statements.Add( localDeclaration );

                    var identifier = localDeclaration.Declaration.Variables[0].Identifier;


                    string identifierName = Quote( identifier.ValueText );

                    var separatedList = Syntax.SeparatedList<ArgumentSyntax>(
                        Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression, Syntax.Literal( identifierName, identifierName ) ) ),
                         Syntax.Token( SyntaxKind.CommaToken ),
                        Syntax.Argument( Syntax.IdentifierName( localDeclaration.Declaration.Variables[0].Identifier ) )
                        );

                    var track = CreateVariableTrackingExpression( separatedList );

                    statements.Add( track );
                }
                else
                {
                    ExpressionStatementSyntax expressionSyntax = visited as ExpressionStatementSyntax;
                    if ( expressionSyntax != null )
                    {
                        BinaryExpressionSyntax binaryExpression = expressionSyntax.Expression as BinaryExpressionSyntax;

                        if ( binaryExpression != null && ChangesValue( binaryExpression.Kind ) )
                        {
                            string tempVariableName = GenerateTempVariableName();

                            var initializer = binaryExpression.Kind == SyntaxKind.AssignExpression ? binaryExpression.Right : binaryExpression.Left;

                            LocalDeclarationStatementSyntax localVariable =
                                Syntax.LocalDeclarationStatement(
                                    Syntax.VariableDeclaration( Syntax.IdentifierName( "var" ) )
                                        .WithVariables( Syntax.SeparatedList(
                                            Syntax.VariableDeclarator( Syntax.Identifier( tempVariableName ) )
                                                .WithInitializer( Syntax.EqualsValueClause( initializer ) ) ) ) );
                            statements.Add( localVariable );

                            if ( binaryExpression.Kind == SyntaxKind.AssignExpression )
                            {
                                var operation =
                                    Syntax.ExpressionStatement( Syntax.BinaryExpression( binaryExpression.Kind,
                                        binaryExpression.Left,
                                        Syntax.IdentifierName( tempVariableName ) ) );

                                statements.Add( operation );
                            }
                            else
                            {
                                var operation =
                                    Syntax.ExpressionStatement( Syntax.BinaryExpression( binaryExpression.Kind,
                                    Syntax.IdentifierName( tempVariableName ),
                                    binaryExpression.Right
                                        ) );

                                statements.Add( operation );

                                var assignment =
                                    Syntax.ExpressionStatement( Syntax.BinaryExpression( SyntaxKind.AssignExpression,
                                        binaryExpression.Left, Syntax.IdentifierName( tempVariableName ) ) );
                                statements.Add( assignment );
                            }

                            string assignmentLeftSideName = Quote( binaryExpression.Left.ToString() );

                            var arguments = Syntax.SeparatedList<ArgumentSyntax>(
                                Syntax.Argument( Syntax.LiteralExpression( SyntaxKind.StringLiteralExpression,
                                    Syntax.Literal( assignmentLeftSideName, assignmentLeftSideName ) ) ),
                                Syntax.Token( SyntaxKind.CommaToken ),
                                Syntax.Argument( Syntax.IdentifierName( tempVariableName ) )
                                );

                            var track = CreateVariableTrackingExpression( arguments );
                            statements.Add( track );
                        }
                        else
                        {
                            statements.Add( visited );
                        }
                    }
                    else
                    {
                        statements.Add( visited );
                    }
                }
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
