using System;
using JetBrains.Annotations;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	internal sealed class WhileLoopAdapter : ILoopAdapter
	{
		private readonly WhileStatementSyntax _loop;

		public WhileLoopAdapter( [NotNull] WhileStatementSyntax loop )
		{
			if ( loop == null )
			{
				throw new ArgumentNullException( "loop" );
			}
			_loop = loop;
		}

		public StatementSyntax GetStatement()
		{
			return _loop.Statement;
		}

		public StatementSyntax WithStatement( StatementSyntax statement )
		{
			return _loop.WithStatement( statement );
		}

		public SyntaxToken? GetIteratorName()
		{
			return null;
		}
	}
}