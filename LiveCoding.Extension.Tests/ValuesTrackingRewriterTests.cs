using System.IO;
using System.Reflection;
using LiveCoding.Core;
using LiveCoding.Extension.Rewriting;
using NUnit.Framework;
using Roslyn.Compilers.CSharp;
using Roslyn.Scripting.CSharp;

namespace LiveCoding.Extension.Tests
{
	[TestFixture]
	public sealed class ValuesTrackingRewriterTests
	{
		[Test]
		public void GetScriptAssemblyName()
		{
			ScriptEngine engine = new ScriptEngine();
			var session = engine.CreateSession();

			//Assembly.GetExecutingAssembly().FullName;

			string executingAssemblyName = session.Execute<string>( "var s = System.Reflection.Assembly.GetExecutingAssembly().FullName; s" );
		}

		[Test]
		public void Rewrite()
		{
			ValuesTrackingRewriter rewriter = new ValuesTrackingRewriter();

			var tree = SyntaxTree.ParseText( @"
    using System;

    public class Program
    {
        private static void Method()
        {
            int i = 0;
            int b = 1;

            for (int j = 0; j < 10; j++)
            {
                i += j;
            }

            b = i + 1;

			b++;
			++b;

            Console.WriteLine(""QQ"");
        }
    }
" );

			var rewritten = tree.GetRoot().Accept( rewriter ).NormalizeWhitespace();

			var compilation = Compilation.Create( "1.dll" )
				.AddSyntaxTrees( SyntaxTree.Create( Syntax.CompilationUnit().WithMembers( Syntax.List( rewritten ) ) ) );

			ScriptEngine engine = new ScriptEngine();
			engine.AddReference( typeof( VariablesTracker ).Assembly );

			var session = engine.CreateSession();
			string code = rewritten.ToString();
			session.Execute( code );

			session.Execute( "Program.Method();" );
		}
	}
}
