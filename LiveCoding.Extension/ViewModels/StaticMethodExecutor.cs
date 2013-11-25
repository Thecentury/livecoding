using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class StaticMethodExecutor : MethodExecutorBase
	{
		public StaticMethodExecutor( MethodDeclarationSyntax method, ClassDeclarationSyntax @class )
			: base( method, @class )
		{
		}

		public override void Execute( ICodeCompiler compiler )
		{
			bool isStaticCtor = MethodName == ClassName;
			if ( isStaticCtor )
			{
				throw new CannotExecuteException( "Cannot execute static ctor" );
			}

			compiler.Compile( "{0}.{1}( {2} );", FullLiveCodingClassName, MethodName, DefaultMethodParameterValues );
		}
	}
}