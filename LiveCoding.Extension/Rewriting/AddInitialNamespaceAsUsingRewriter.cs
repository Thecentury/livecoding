using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Rewriting
{
	public sealed class AddInitialNamespaceAsUsingRewriter : SyntaxRewriter
	{
		private readonly List<string> _usings;

		public AddInitialNamespaceAsUsingRewriter( IEnumerable<string> usings )
		{
			_usings = usings.ToList();
		}

		public override SyntaxNode VisitCompilationUnit( CompilationUnitSyntax node )
		{
			List<UsingDirectiveSyntax> directives = new List<UsingDirectiveSyntax>();
			foreach ( string u in _usings )
			{
				var parts = u.Split( '.' );
				StringBuilder builder = new StringBuilder();

				builder.Append( parts[0] );
				directives.Add( Syntax.UsingDirective( Syntax.ParseName( builder.ToString() ) ) );

				for ( int i = 1; i < parts.Length; i++ )
				{
					builder.Append( "." ).Append( parts[i] );
					directives.Add( Syntax.UsingDirective( Syntax.ParseName( builder.ToString() ) ) );
				}
			}

			UsingDirectiveSyntax[] usingDirectives = directives.ToArray();
			return node.AddUsings( usingDirectives );
		}
	}
}