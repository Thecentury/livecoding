using System;
using JetBrains.Annotations;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	internal sealed class ForLoopAdapter : ILoopAdapter
	{
		private readonly ForStatementSyntax _loop;

		public ForLoopAdapter( [NotNull] ForStatementSyntax loop )
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
			return _loop.Declaration.Variables[ 0 ].Identifier;
		}
	}
}