using System;
using System.Collections.Generic;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension
{
    public sealed class ValuesTrackingRewriter : SyntaxRewriter
    {
        public override SyntaxNode VisitBlock(BlockSyntax node)
        {
            List<StatementSyntax> statements = new List<StatementSyntax>();
            int tempVariablesCount = 0;


            foreach (var statement in node.Statements)
            {
                LocalDeclarationStatementSyntax localDeclaration = statement as LocalDeclarationStatementSyntax;
                if (localDeclaration != null)
                {
                    statements.Add(localDeclaration);

                    var identifier = localDeclaration.Declaration.Variables[0].Identifier;


                    string identifierName = String.Format("\"{0}\"", identifier.ValueText);

                    var separatedList = Syntax.SeparatedList<ArgumentSyntax>(
                        Syntax.Argument(Syntax.LiteralExpression(SyntaxKind.StringLiteralExpression, Syntax.Literal(identifierName, identifierName))),
                         Syntax.Token(SyntaxKind.CommaToken),
                        Syntax.Argument(Syntax.IdentifierName(localDeclaration.Declaration.Variables[0].Identifier))
                        );

                    var track = Syntax.ExpressionStatement(
                        Syntax.InvocationExpression(
                            Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression,
                                Syntax.IdentifierName("global::LiveCoding.Core.VariablesTracker"), Syntax.IdentifierName("AddValue")),
                                Syntax.ArgumentList(
                                    separatedList))
                        );

                    statements.Add(track);
                }
                else
                {
                    ExpressionStatementSyntax expressionSyntax = statement as ExpressionStatementSyntax;
                    if (expressionSyntax != null)
                    {
                        BinaryExpressionSyntax binaryExpression = expressionSyntax.Expression as BinaryExpressionSyntax;

                        string tempVariableName = String.Format("__liveCodingTemp_{0}", tempVariablesCount);
                        tempVariablesCount++;

                        if (binaryExpression != null && binaryExpression.Kind == SyntaxKind.AssignExpression)
                        {
                            LocalDeclarationStatementSyntax localVariable =
                                Syntax.LocalDeclarationStatement(
                                Syntax.VariableDeclaration(Syntax.IdentifierName("var")).WithVariables(Syntax.SeparatedList(
                                Syntax.VariableDeclarator(Syntax.Identifier(tempVariableName)).WithInitializer(Syntax.EqualsValueClause(binaryExpression.Right)))))
                                .NormalizeWhitespace();

                            var assign = Syntax.ExpressionStatement(Syntax.BinaryExpression(SyntaxKind.AssignExpression, binaryExpression.Left,
                                Syntax.IdentifierName(tempVariableName)));

                            statements.Add(localVariable);
                            statements.Add(assign);
                        }
                    }
                    else
                    {
                        statements.Add(statement);
                    }
                }
            }

            var result = Syntax.Block(statements);
            return result;
        }
    }
}
