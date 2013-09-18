using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EnvDTE;
using LiveCoding.Core;
using LiveCoding.Extension.Extensions;
using LiveCoding.Extension.Rewriting;
using LiveCoding.Extension.Views;
using LiveCoding.Extension.VisualStudio;
using LiveCoding.Extension.VisualStudio.If;
using LiveCoding.Extension.VisualStudio.Invocations;
using LiveCoding.Extension.VisualStudio.Loops;
using LiveCoding.Extension.VisualStudio.VariableValues;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using VSLangProj;

namespace LiveCoding.Extension.ViewModels
{
	public sealed class ExecutingState : MethodExecutionStateBase
	{
		private const string CompilationDataKey = "CompilationData";

		private sealed class MethodExecutionContext
		{
			public Stopwatch Stopwatch;
		}

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly MethodExecutionContext _context = new MethodExecutionContext();
		private Task _executeTask;
		private ICodeCompiler _codeCompiler;

		public override void Enter()
		{
			var cancellationToken = _cancellationTokenSource.Token;
			_executeTask = Task.Factory.StartNew( () => RewriteAndExecute( cancellationToken ), cancellationToken );
			_executeTask.ContinueWith( t => OnCompleted( t ), TaskContinuationOptions.OnlyOnRanToCompletion );
			_executeTask.ContinueWith( t => OnFailed( t ), TaskContinuationOptions.OnlyOnFaulted );
			_executeTask.ContinueWith( t => OnCanceled( t ), TaskContinuationOptions.OnlyOnCanceled );
		}

		private void OnCanceled( Task task )
		{
			Owner.GotoState( new CanceledState() );
		}

		private void OnFailed( Task task )
		{
			Owner.GotoState( new FailedState( task.Exception ) );
		}

		private void OnCompleted( Task task )
		{
			Owner.GotoState( new ExecutedState( _context.Stopwatch.Elapsed ) );
		}

		public override void ExecuteMainAction()
		{
			_cancellationTokenSource.Cancel();
			if ( _codeCompiler != null )
			{
				var compiler = _codeCompiler;
				_codeCompiler = null;
				compiler.Dispose();
				Owner.Cache.Remove( CompilationDataKey );
			}
		}

		private Tuple<ICodeCompiler, CompilationUnitSyntax> GetCompilationData()
		{
			object value;
			Owner.Cache.TryGetValue( CompilationDataKey, out value );
			return value as Tuple<ICodeCompiler, CompilationUnitSyntax>;
		}

		private void SetCompilationData( ICodeCompiler codeCompiler, CompilationUnitSyntax compilationUnitSyntax )
		{
			Owner.Cache[ CompilationDataKey ] = Tuple.Create( codeCompiler, compilationUnitSyntax );
		}

		private void RewriteAndExecute( CancellationToken token )
		{
			string filePath = Owner.View.GetFilePath();

			var project = ProjectHelper.GetContainingProject( filePath );
			string projectOutputFullPath = project.GetOutputFullPath();

			bool shouldBuild = project.IsDirty || !File.Exists( projectOutputFullPath );

			CompilationUnitSyntax compilationUnit = null;
			if ( !shouldBuild )
			{
				var compilationData = GetCompilationData();
				if ( compilationData != null )
				{
					_codeCompiler = compilationData.Item1;
					compilationUnit = compilationData.Item2;
				}
			}
			else
			{
				Solution solution = project.CodeModel.DTE.Solution;

				solution.SolutionBuild.BuildProject( solution.SolutionBuild.ActiveConfiguration.Name, project.UniqueName, true );
				// todo brinchuk failed compilation handling
			}

			if ( _codeCompiler == null || compilationUnit == null )
			{
				_codeCompiler = new AppDomainCodeCompiler();

				var syntaxTree = SyntaxTree.ParseFile( filePath, cancellationToken: token );

				ValuesTrackingRewriter rewriter = new ValuesTrackingRewriter();

				compilationUnit = syntaxTree.GetRoot( token );

				SetCompilationData( _codeCompiler, compilationUnit );

				var rewritten = compilationUnit
					.Accept( rewriter )
					.Accept( new ClassFromNamespaceRewriter() )
					.NormalizeWhitespace();

				List<string> namespaces = new List<string>();
				foreach ( var usingDirectiveSyntax in compilationUnit.Usings )
				{
					namespaces.Add( usingDirectiveSyntax.Name.ToString() );
				}
				foreach ( var namespaceDeclaration in compilationUnit.ChildNodes().OfType<NamespaceDeclarationSyntax>() )
				{
					namespaces.Add( namespaceDeclaration.Name.ToString() );
				}

				List<string> references = new List<string>();

				foreach ( Reference reference in project.GetReferences().References )
				{
					string path = reference.Path;
					references.Add( path );
				}

				if ( File.Exists( projectOutputFullPath ) )
				{
					references.Add( projectOutputFullPath );
				}
				references.Add( typeof( VariablesTracker ).Assembly.Location );

				_codeCompiler.SetupScriptEngine( namespaces, references );

				_codeCompiler.SetLiveEventListener( new EventProxyListener() );

				_codeCompiler.Compile( rewritten.ToString() );
			}

			//var compilation = Compilation.Create( "1.dll",
			//	new CompilationOptions( OutputKind.DynamicallyLinkedLibrary, debugInformationKind: DebugInformationKind.Full ) )
			//	.AddSyntaxTrees( rewritten.SyntaxTree )
			//	.AddReferences(
			//		project.GetReferences()
			//			.References.OfType<Reference>()
			//			.Select( r => MetadataReference.CreateAssemblyReference( r.Name ) ) )
			//	.AddReferences( MetadataReference.CreateAssemblyReference( typeof( VariablesTracker ).Assembly.FullName ) );

			VariablesTracker.ClearEvents();

			Dispatcher dispatcher = Owner.View.VisualElement.Dispatcher;
			var view = Owner.View;
			VariableValueTagger tagger =
				view.TextBuffer.Properties.GetProperty<VariableValueTagger>( typeof( VariableValueTagger ) );

			var subscription = VariablesTracker.EventsObservable.OfType<ValueChange>().Subscribe( Observer.Create<ValueChange>(
				change =>
				{
					token.ThrowIfCancellationRequested();

					dispatcher.BeginInvoke( () =>
					{
						var valueLine = view.TextSnapshot.GetLineFromLineNumber( change.OriginalLineNumber );
						var span = view.GetTextElementSpan( valueLine.Start );

						tagger.AddVariableChange( change, span );
					}, DispatcherPriority.Normal );
				} ) );

			var ifTagger = view.TextBuffer.Properties.GetProperty<BooleanAdorment>( typeof( BooleanAdorment ) );

			var ifsSubscription = VariablesTracker.EventsObservable.OfType<IfEvaluationEvent>().Subscribe( e =>
			{
				token.ThrowIfCancellationRequested();

				dispatcher.BeginInvoke( () =>
				{
					var valueLine = view.TextSnapshot.GetLineFromLineNumber( e.StartIfLine );
					var span = view.GetTextElementSpan( valueLine.Start );
					ifTagger.AddVariableChange( e, span );
				}, DispatcherPriority.Normal );
			} );

			LoopTagger loopTagger = view.TextBuffer.Properties.GetProperty<LoopTagger>( typeof( LoopTagger ) );
			var forLoopSubscription = VariablesTracker.ForLoops.Subscribe( loop =>
			{
				token.ThrowIfCancellationRequested();

				dispatcher.BeginInvoke( () =>
				{
					var loopLine = view.TextSnapshot.GetLineFromLineNumber( loop.LoopStartLineNumber - 1 );
					var span = view.GetTextElementSpan( loopLine.End );

					loopTagger.BeginLoopWatch( loop, span );
				}, DispatcherPriority.Normal );
			} );

			var invocationTagger = view.TextBuffer.Properties.GetProperty<InvocationTagger>( typeof( InvocationTagger ) );
			var invocationsSubscription = VariablesTracker.EventsObservable.OfType<InvocationEvent>().Subscribe( e =>
			{
				token.ThrowIfCancellationRequested();

				dispatcher.BeginInvoke( () =>
				{
					var valueLine = view.TextSnapshot.GetLineFromLineNumber( e.LineNumber );
					var span = view.GetTextElementSpan( valueLine.Start );
					invocationTagger.AddInvocation( e, span );
				}, DispatcherPriority.Normal );
			} );

			try
			{
				if ( Owner.Data.Call == null )
				{
					var snapshot = Owner.Data.SnapshotSpan.Snapshot;

					int methodStartPosition = Owner.Data.SnapshotSpan.Start.Position;
					var line = snapshot.GetLineFromLineNumber( snapshot.GetLineNumberFromPosition( methodStartPosition ) );
					string text = line.GetText();

					var methodTree = SyntaxTree.ParseText( text, cancellationToken: token );

					var methodDeclaration = methodTree.GetRoot( token ).ChildNodes().OfType<MethodDeclarationSyntax>().First();

					string methodName = methodDeclaration.Identifier.ValueText;

					var classDeclaration =
						compilationUnit.DescendantNodes()
							.OfType<ClassDeclarationSyntax>()
							.Where( cd => cd.Span.Contains( methodStartPosition ) )
							.First();

					string className = classDeclaration.Identifier.ValueText;
					string fullClassName = ClassFromNamespaceRewriter.LiveCodingWrapperClassName + "." + className;

					string parameterValues = methodDeclaration.ParameterList.GetDefaultParametersValuesString();

					_context.Stopwatch = Stopwatch.StartNew();

					if ( methodDeclaration.IsStatic() )
					{
						bool isStaticCtor = methodName == className;
						if ( isStaticCtor )
						{
							throw new CannotExecuteException( "Cannot execute static ctor" );
						}

						_codeCompiler.Compile( String.Format( "{0}.{1}( {2} );", fullClassName, methodName, parameterValues ) );
					}
					else
					{
						string instanceVariableName = "__liveCodingInstance_" + Guid.NewGuid().ToString( "N" );

						bool hasParameterlessConstructor = classDeclaration.ChildNodes()
							.OfType<ConstructorDeclarationSyntax>()
							.Where( c => c.ParameterList.Parameters.Count == 0 )
							.Any();

						bool doesnotHaveConstructorDeclarationAtAll =
							!classDeclaration.ChildNodes().OfType<ConstructorDeclarationSyntax>().Any();

						if ( hasParameterlessConstructor || doesnotHaveConstructorDeclarationAtAll )
						{
							_codeCompiler.Compile( String.Format( "var {0} = new {1}();", instanceVariableName, fullClassName ) );
						}
						else
						{
							var firstConstructor = classDeclaration.ChildNodes().OfType<ConstructorDeclarationSyntax>().First();

							string constructorParameters = firstConstructor.ParameterList.GetDefaultParametersValuesString();
							_codeCompiler.Compile(
								String.Format( "var {0} = new {1}( {2} );", instanceVariableName, fullClassName, constructorParameters ) );
						}

						bool ctorInvocation = className == methodName;
						if ( !ctorInvocation )
						{
							_codeCompiler.Compile( String.Format( "{0}.{1}( {2} )", instanceVariableName, methodName, parameterValues ) );
						}
					}
				}
				else
				{
					_context.Stopwatch = Stopwatch.StartNew();
					_codeCompiler.Compile( Owner.Data.Call );
				}
			}
			finally
			{
				if ( _context.Stopwatch != null )
				{
					_context.Stopwatch.Stop();
				}
				subscription.Dispose();
				ifsSubscription.Dispose();
				forLoopSubscription.Dispose();
				invocationsSubscription.Dispose();
			}
		}

		public override MethodExecutionState State
		{
			get { return MethodExecutionState.Executing; }
		}
	}
}