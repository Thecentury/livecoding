﻿using System.IO;
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

		private const string OneLineLoop = @"
class C {
void M(){
int j = 0;
for (int i = 0; i < 10; i++)
	j = i;
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
		public void Rewrite( string code )
		{
			ValuesTrackingRewriter rewriter = new ValuesTrackingRewriter();

			var tree = SyntaxTree.ParseText( code );

			var rewritten = tree.GetRoot().Accept( rewriter ).NormalizeWhitespace();

			ScriptEngine engine = new ScriptEngine();
			engine.AddReference( typeof( VariablesTracker ).Assembly );

			var session = engine.CreateSession();
			string rewrittenCode = rewritten.ToString();
			session.Execute( rewrittenCode );
		}
	}
}
