using System;
using LiveCoding.Extension.Rewriting;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.ViewModels
{
	internal abstract class MethodExecutorBase : IMethodExecutor
	{
		private readonly MethodDeclarationSyntax _method;
		private readonly ClassDeclarationSyntax _class;

		protected MethodDeclarationSyntax Method
		{
			get { return _method; }
		}

		protected ClassDeclarationSyntax Class
		{
			get { return _class; }
		}

		protected string FullClassName
		{
			get { return ClassFromNamespaceRewriter.LiveCodingWrapperClassName + "." + ClassName; }
		}

		protected string ClassName
		{
			get { return _class.Identifier.ValueText; }
		}

		protected string MethodName
		{
			get { return _method.Identifier.ValueText; }
		}

		protected MethodExecutorBase( MethodDeclarationSyntax method, ClassDeclarationSyntax @class )
		{
			_method = method;
			_class = @class;
		}

		protected string DefaultMethodParameterValues
		{
			get { return _method.ParameterList.GetDefaultParametersValuesString(); }
		}

		protected string DefaultCtorParameterValues
		{
			get { throw new NotImplementedException(); }
		}

		public abstract void Execute( ICodeCompiler compiler );
	}
}