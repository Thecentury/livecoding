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
			string instanceVariableName = "__liveCodingInstance_" + Guid.NewGuid().ToString( "N" );

			bool hasParameterlessConstructor = Class.ChildNodes()
				.OfType<ConstructorDeclarationSyntax>()
				.Where( c => c.ParameterList.Parameters.Count == 0 )
				.Any();

			bool doesnotHaveConstructorDeclarationAtAll =
				!Class.ChildNodes().OfType<ConstructorDeclarationSyntax>().Any();

			if ( hasParameterlessConstructor || doesnotHaveConstructorDeclarationAtAll )
			{
				compiler.Compile( "var {0} = new {1}();", instanceVariableName, FullClassName );
			}
			else
			{
				var firstConstructor = Class.ChildNodes().OfType<ConstructorDeclarationSyntax>().First();

				string constructorParameters = firstConstructor.ParameterList.GetDefaultParametersValuesString();
				compiler.Compile( "var {0} = new {1}( {2} );", instanceVariableName, FullClassName, constructorParameters );
			}

			bool ctorInvocation = ClassName == MethodName;
			if ( !ctorInvocation )
			{
				compiler.Compile( "{0}.{1}( {2} )", instanceVariableName, MethodName, DefaultMethodParameterValues );
			}
		}
	}
}