using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class TestCaseExecutor : MethodExecutorBase
	{
		private readonly string _parameters;

		public TestCaseExecutor( MethodDeclarationSyntax method, ClassDeclarationSyntax @class, string parameters )
			: base( method, @class )
		{
			_parameters = parameters;
		}

		public override void Execute( ICodeCompiler compiler )
		{
			string instanceVariableName = GenerateVariableName();

			compiler.Compile( "var {0} = new {1}();", false, instanceVariableName, FullLiveCodingClassName );
			compiler.Compile( "{0}.{1}( {2} )", false, instanceVariableName, MethodName, _parameters );
		}
	}
}