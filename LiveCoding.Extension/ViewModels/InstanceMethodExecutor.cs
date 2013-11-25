using System;
using System.Linq;
using LiveCoding.Extension.Rewriting;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class InstanceMethodExecutor : MethodExecutorBase
	{
		public InstanceMethodExecutor( MethodDeclarationSyntax method, ClassDeclarationSyntax @class )
			: base( method, @class )
		{
		}

		public override void Execute( ICodeCompiler compiler )
		{
			string instanceVariableName = GenerateVariableName();

			bool hasParameterlessConstructor = Class.ChildNodes()
				.OfType<ConstructorDeclarationSyntax>()
				.Where( c => c.ParameterList.Parameters.Count == 0 )
				.Any();

			bool doesnotHaveConstructorDeclarationAtAll =
				!Class.ChildNodes().OfType<ConstructorDeclarationSyntax>().Any();

			if ( hasParameterlessConstructor || doesnotHaveConstructorDeclarationAtAll )
			{
				compiler.Compile( "var {0} = new {1}();", instanceVariableName, FullLiveCodingClassName );
			}
			else
			{
				var firstConstructor = Class.ChildNodes().OfType<ConstructorDeclarationSyntax>().First();

				string constructorParameters = firstConstructor.ParameterList.GetDefaultParametersValuesString();
				compiler.Compile( "var {0} = new {1}( {2} );", instanceVariableName, FullLiveCodingClassName, constructorParameters );
			}

			bool ctorInvocation = ClassName == MethodName;
			if ( !ctorInvocation )
			{
				compiler.Compile( "{0}.{1}( {2} )", instanceVariableName, MethodName, DefaultMethodParameterValues );
			}
		}
	}
}