using System;
using LiveCoding.Core;
using LiveCoding.Extension.Rewriting;
using NUnit.Framework;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Scripting.CSharp;

namespace LiveCoding.Extension.Tests
{
	[TestFixture]
	public sealed class ValuesTrackingRewriterTests
	{
		private const string SomeLoop = @"
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
";

		private const string Invocation1 = @"
class C{
void M(){
	System.Console.WriteLine(""{0} {1}"", 1+1, true == false);
}
}
";
		private const string InvocationWithReturnValue = @"
class C{
void M(){
	string path = System.IO.Path.Combine(""1""+""2"", ""3""+""4"");
}
}";

		private const string OneLineLoop = @"
class C {
	void M() {
		int j = 0;
		for (int i = 0; i < 10; i++)
			j = i;
	}
}";

		private const string OneIf = @"
class C {
	void M() {
		if( System.Console.ReadLine() == null ) {
			System.Console.WriteLine(""if"");
		} 
		else if ( 1 == 2 ) {
		}
		else if ( 2 == 3 ) {
		}
		else {
		}
	}
}";

		private const string EventInsideOfRegion = @"
class C {
#region Q Q

public event System.EventHandler QQ;

#endregion 

public void M(){
//	int i = 1;
}

}";

		private const string EventDeclarationAndSubscription = @"
using System;

class B {
public event EventHandler E;
}

class C : B {
	private void Q() {
		E += (e,s)=>{};
		E -= (e,s)=>{};
	}
}
";

		[Test]
		public void GetScriptAssemblyName()
		{
			ScriptEngine engine = new ScriptEngine();
			var session = engine.CreateSession();

			//Assembly.GetExecutingAssembly().FullName;

			string executingAssemblyName = session.Execute<string>( "var s = System.Reflection.Assembly.GetExecutingAssembly().FullName; s" );
		}

		[TestCase( SomeLoop )]
		[TestCase( OneLineLoop )]
		[TestCase( OneIf )]
		[TestCase( Invocation1 )]
		[TestCase( InvocationWithReturnValue )]
		[TestCase( EventInsideOfRegion )]
		[TestCase( EventDeclarationAndSubscription )]
		public void Rewrite( string code )
		{
			var tree = SyntaxTree.ParseText( code );

			var compilation = Compilation.Create( "1", syntaxTrees: new[] { tree }, references: new[] { new MetadataFileReference( @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll" ) } );

			ValuesTrackingRewriter rewriter = new ValuesTrackingRewriter( tree, compilation );

			var rewritten = tree.GetRoot().Accept( rewriter ).NormalizeWhitespace();

			ScriptEngine engine = new ScriptEngine();
			engine.AddReference( typeof( VariablesTracker ).Assembly );

			var session = engine.CreateSession();
			string rewrittenCode = rewritten.ToString();

			Console.WriteLine( rewrittenCode );

			session.Execute( rewrittenCode );
		}

		[TestCase( EventInsideOfRegion )]
		public void RewriteWithEmptyRewriter( string code )
		{
			var tree = SyntaxTree.ParseText( code );

			var rewriter = new EmptyRewriter();

			var rewritten = tree.GetRoot().Accept( rewriter ).NormalizeWhitespace();

			ScriptEngine engine = new ScriptEngine();

			var session = engine.CreateSession();
			string rewrittenCode = rewritten.ToString();

			Console.WriteLine( rewrittenCode );

			session.Execute( rewrittenCode );
		}
	}
}
