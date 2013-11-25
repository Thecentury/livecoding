using System;
using System.Linq;
using System.Text;
using LiveCoding.Extension.Extensions;
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

		protected string FullLiveCodingClassName
		{
			get { return ClassFromNamespaceRewriter.LiveCodingWrapperClassName + "." + FullClassName; }
		}

		protected string FullClassName
		{
			get
			{
				return _class.GetSelfAndAllEnclosingClasses()
					.Select( c => c.Identifier.ValueText )
					.FoldR( new StringBuilder(), ( l, r ) => l.Length > 0 ? l.Append( "." ).Append( r ) : l.Append( r ) ).ToString();
			}
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

		public abstract void Execute( ICodeCompiler compiler );

		protected static string GenerateVariableName()
		{
			return "__liveCodingInstance_" + Guid.NewGuid().ToString( "N" );
		}
	}
}