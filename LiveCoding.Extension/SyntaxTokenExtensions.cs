using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension
{
    public static class SyntaxTokenExtensions
    {
        public static bool IsAccessModifier( this SyntaxToken token )
        {
            return token.Kind.IsAccessibilityModifier();
        }
    }
}