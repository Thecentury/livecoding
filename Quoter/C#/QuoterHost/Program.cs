using System;
using Roslyn.Compilers.CSharp;

namespace QuoterHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceText = @"
using System;

class C1 {
	class Inner { }
}

class C2 {
}
";
            var sourceNode = SyntaxTree.ParseText(sourceText).GetRoot();
            var quoter = new Quoter
            {
                OpenParenthesisOnNewLine = false,
                ClosingParenthesisOnNewLine = false,
                UseDefaultFormatting = true,
                RemoveRedundantModifyingCalls = true
            };

            var generatedCode = quoter.Quote(sourceNode);
            var resultText = quoter.Evaluate(generatedCode, quoter.UseDefaultFormatting);

            if (quoter.UseDefaultFormatting)
            {
                sourceNode = sourceNode.NormalizeWhitespace();
                sourceText = sourceNode.ToFullString();
            }

            if (sourceText != resultText)
            {
                throw new Exception();
            }

            Console.WriteLine(generatedCode);
        }
    }
}
