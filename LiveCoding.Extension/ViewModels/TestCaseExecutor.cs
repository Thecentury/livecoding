using System;
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
			//Type type = (Type)compiler.Compile( "typeof({0})", FullClassName );

			string instanceVariableName = GenerateVariableName();

			compiler.Compile( "var {0} = new {1}();", instanceVariableName, FullClassName );
			compiler.Compile( "{0}.{1}( {2} )", instanceVariableName, MethodName, _parameters );
		}
	}
}