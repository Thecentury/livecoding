﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EnvDTE;
using LiveCoding.Core;
using LiveCoding.Extension.Views;
using LiveCoding.Extension.VisualStudio;
using LiveCoding.Extension.VisualStudio.ForLoops;
using LiveCoding.Extension.VisualStudio.VariableValues;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using VSLangProj;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class CodeCompiler : MarshalByRefObject
	{
		private readonly ScriptEngine _scriptEngine = new ScriptEngine();

		public void SetupScriptEngine( List<string> namespaces, List<string> references )
		{
			Guid sessionId = Guid.NewGuid();

			foreach ( string ns in namespaces )
			{
				_scriptEngine.ImportNamespace( ns );
			}

			string copyDirectory = Path.Combine( Path.GetTempPath(), sessionId.ToString() );
			Directory.CreateDirectory( copyDirectory );

			foreach ( string reference in references )
			{
				if ( File.Exists( reference ) )
				{
					
				}
			}
		}
	}

	public sealed class ExecutingState : MethodExecutionStateBase
	{
		private sealed class MethodExecutionContext
		{
			public Stopwatch Stopwatch;

			public Session Session;
		}

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly MethodExecutionContext _context = new MethodExecutionContext();

		public override void Enter()
		{
			var cancellationToken = _cancellationTokenSource.Token;
			var executeTask = Task.Factory.StartNew( () => RewriteAndExecute( cancellationToken ), cancellationToken );
			executeTask.ContinueWith( t => OnCompleted( t ), TaskContinuationOptions.OnlyOnRanToCompletion );
			executeTask.ContinueWith( t => OnFailed( t ), TaskContinuationOptions.OnlyOnFaulted );
			executeTask.ContinueWith( t => OnCanceled( t ), TaskContinuationOptions.OnlyOnCanceled );
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
		}

		private void RewriteAndExecute( CancellationToken token )
		{
			string filePath = Owner.View.GetFilePath();

			var syntaxTree = SyntaxTree.ParseFile( filePath, cancellationToken: token );

			ValuesTrackingRewriter rewriter = new ValuesTrackingRewriter();

			var compilationUnit = syntaxTree.GetRoot( token );

			var rewritten = compilationUnit
				.Accept( rewriter )
				.Accept( new ClassFromNamespaceRewriter() )
				.NormalizeWhitespace();

			ScriptEngine engine = new ScriptEngine();

			foreach ( var usingDirective in compilationUnit.Usings )
			{
				engine.ImportNamespace( usingDirective.Name.ToString() );
			}

			foreach ( var namespaceDeclaration in compilationUnit.ChildNodes().OfType<NamespaceDeclarationSyntax>() )
			{
				engine.ImportNamespace( namespaceDeclaration.Name.ToString() );
			}

			var project = ProjectHelper.GetContainingProject( filePath );

			Configuration activeConfiguration = project.ConfigurationManager.ActiveConfiguration;
			string outputName = project.Properties.Item( "OutputFileName" ).Value.ToString();
			string outputPath = activeConfiguration.Properties.Item( "OutputPath" ).Value.ToString();
			string projectPath = project.Properties.Item( "FullPath" ).Value.ToString();
			string projectOutputFullPath = Path.Combine( projectPath, outputPath, outputName );

			bool shouldBuild = project.IsDirty || !File.Exists( projectOutputFullPath );
			if ( shouldBuild )
			{
				Solution solution = project.CodeModel.DTE.Solution;

				solution.SolutionBuild.BuildProject( solution.SolutionBuild.ActiveConfiguration.Name, project.UniqueName, true );
				// todo brinchuk failed compilation handling
			}

			foreach ( Reference reference in project.GetReferences().References )
			{
				string path = reference.Path;
				if ( File.Exists( path ) )
				{
					var assembly = Assembly.LoadFrom( path );
					engine.AddReference( assembly );
				}
			}

			if ( File.Exists( projectOutputFullPath ) )
			{
				engine.AddReference( Assembly.LoadFrom( projectOutputFullPath ) );
			}
			engine.AddReference( typeof( VariablesTracker ).Assembly );

			//var compilation = Compilation.Create( "1.dll",
			//	new CompilationOptions( OutputKind.DynamicallyLinkedLibrary, debugInformationKind: DebugInformationKind.Full ) )
			//	.AddSyntaxTrees( rewritten.SyntaxTree )
			//	.AddReferences(
			//		project.GetReferences()
			//			.References.OfType<Reference>()
			//			.Select( r => MetadataReference.CreateAssemblyReference( r.Name ) ) )
			//	.AddReferences( MetadataReference.CreateAssemblyReference( typeof( VariablesTracker ).Assembly.FullName ) );

			var session = engine.CreateSession();

			_context.Session = session;

			try
			{
				session.Execute( rewritten.ToString() );
			}
			catch ( CompilationErrorException exc )
			{
				throw;
			}

			VariablesTracker.ClearEvents();

			Dispatcher dispatcher = Owner.View.VisualElement.Dispatcher;
			var view = Owner.View;
			VariableValueTagger tagger = view.TextBuffer.Properties.GetProperty<VariableValueTagger>( typeof( VariableValueTagger ) );

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

			ForLoopTagger forLoopTagger = view.TextBuffer.Properties.GetProperty<ForLoopTagger>( typeof( ForLoopTagger ) );
			var forLoopSubscription = VariablesTracker.ForLoops.Subscribe( loop =>
			{
				token.ThrowIfCancellationRequested();

				dispatcher.BeginInvoke( () =>
				{
					var loopLine = view.TextSnapshot.GetLineFromLineNumber( loop.LoopStartLineNumber );
					var span = view.GetTextElementSpan( loopLine.End );

					forLoopTagger.BeginLoopWatch( loop, span );
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

					var liveCodingClass = rewritten.ChildNodes().OfType<ClassDeclarationSyntax>().First();

					var classDeclaration = liveCodingClass.ChildNodes().OfType<ClassDeclarationSyntax>().Where( cd => cd.Span.Contains( methodStartPosition ) ).First();

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

						session.Execute( String.Format( "{0}.{1}( {2} );", fullClassName, methodName, parameterValues ) );
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
							session.Execute( String.Format( "var {0} = new {1}();", instanceVariableName, fullClassName ) );
						}
						else
						{
							var firstConstructor = classDeclaration.ChildNodes().OfType<ConstructorDeclarationSyntax>().First();

							string constructorParameters = firstConstructor.ParameterList.GetDefaultParametersValuesString();
							session.Execute(
								String.Format( "var {0} = new {1}( {2} );", instanceVariableName, fullClassName, constructorParameters ) );
						}

						bool ctorInvocation = className == methodName;
						if ( !ctorInvocation )
						{
							session.Execute( String.Format( "{0}.{1}( {2} )", instanceVariableName, methodName, parameterValues ) );
						}
					}
				}
				else
				{
					_context.Stopwatch = Stopwatch.StartNew();
					session.Execute( Owner.Data.Call );
				}
			}
			finally
			{
				if ( _context.Stopwatch != null )
				{
					_context.Stopwatch.Stop();
				}
				subscription.Dispose();
				forLoopSubscription.Dispose();
			}
		}

		public override MethodExecutionState State
		{
			get { return MethodExecutionState.Executing; }
		}
	}
}