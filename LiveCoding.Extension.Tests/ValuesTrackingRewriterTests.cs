using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.Tests
{
    [TestFixture]
    public sealed class ValuesTrackingRewriterTests
    {
        [Test]
        public void Rewrite()
        {
            ValuesTrackingRewriter rewriter = new ValuesTrackingRewriter();

            var tree = SyntaxTree.ParseText(@"
    public class Program
    {
        public static void Method()
        {
            int i = 0;
            int b = 1;

            for (int j = 0; j < 10; j++)
            {
                i += i;
            }

            b = i + 1;

            Console.WriteLine(""QQ"");
        }
    }
");

            var rewritten = tree.GetRoot().Accept(rewriter).NormalizeWhitespace();

        }
    }
}
