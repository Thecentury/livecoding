using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.VisualStudio
{
	public sealed class MethodExecutionData
	{
		public MethodExecutionData( SnapshotSpan snapshotSpan, MethodExecutionKind kind )
		{
			Kind = kind;
			SnapshotSpan = snapshotSpan;
		}

		public SnapshotSpan SnapshotSpan { get; private set; }

		public string Call { get; set; }

		public MethodExecutionKind Kind { get; private set; }

		public MethodDeclarationSyntax GetMethodDeclaration( CompilationUnitSyntax compilationUnit, CancellationToken cancellationToken )
		{
			var snapshot = SnapshotSpan.Snapshot;
			int methodStartPosition = SnapshotSpan.Start.Position;
			int lineNumber = snapshot.GetLineNumberFromPosition( methodStartPosition );

			if ( Kind == MethodExecutionKind.CommonMethod )
			{
				var line = snapshot.GetLineFromLineNumber( lineNumber );
				string text = line.GetText();

				var methodTree = SyntaxTree.ParseText( text, cancellationToken: cancellationToken );

				var methodDeclaration = methodTree.GetRoot( cancellationToken ).ChildNodes().OfType<MethodDeclarationSyntax>().First();

				return methodDeclaration;
			}
			else
			{
				var methodDeclaration = ( from method in compilationUnit.DescendantNodes().OfType<MethodDeclarationSyntax>()
										  let span = compilationUnit.SyntaxTree.GetLineSpan( method.Span, true, cancellationToken )
										  where span.StartLinePosition.Line <= lineNumber && lineNumber <= span.EndLinePosition.Line
										  orderby ( span.StartLinePosition.Line - lineNumber )
										  select method ).First();
				return methodDeclaration;
			}
		}
	}
}