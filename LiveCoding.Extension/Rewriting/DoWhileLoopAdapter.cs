using System;
using JetBrains.Annotations;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	internal sealed class DoWhileLoopAdapter : ILoopAdapter
	{
		private readonly DoStatementSyntax _loop;

		public DoWhileLoopAdapter( [NotNull] DoStatementSyntax loop )
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