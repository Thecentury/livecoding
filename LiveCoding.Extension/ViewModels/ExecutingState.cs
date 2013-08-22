using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using LiveCoding.Core;
using LiveCoding.Extension.Views;
using LiveCoding.Extension.VisualStudio;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using VSLangProj;

namespace LiveCoding.Extension.ViewModels
{
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

			var rewritten = syntaxTree.GetRoot( token )
				.Accept( rewriter )
				.Accept( new ClassFromNamespaceRewriter() )
				.NormalizeWhitespace();

			ScriptEngine engine = new ScriptEngine();

			var project = ProjectHelper.GetContainingProject( filePath );

			foreach ( Reference reference in project.GetReferences().References )
			{
				engine.AddReference( reference.Name );
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
				session.Execute(rewritten.ToString());
			}
			catch (CompilationErrorException exc)
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

			ForLoopTagger forLoopTagger = view.TextBuffer.Properties.GetProperty<ForLoopTagger>( typeof (ForLoopTagger) );
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

					var line = snapshot.GetLineFromLineNumber( snapshot.GetLineNumberFromPosition( Owner.Data.SnapshotSpan.Start.Position ) );
					string text = line.GetText();

					var methodTree = SyntaxTree.ParseText( text, cancellationToken: token );
					MethodDeclarationSyntax methodSyntax = methodTree.GetRoot( token ).ChildNodes().OfType<MethodDeclarationSyntax>().First();
					string methodName = methodSyntax.Identifier.ValueText;
					var classSyntax = rewritten.ChildNodes().OfType<ClassDeclarationSyntax>().First();
					string className = classSyntax.Identifier.ValueText;

					string parameterValues = String.Join( ", ",
						methodSyntax.ParameterList.Parameters.Select( p => String.Format( "default({0})", p.Type.ToString() ) ) );

					_context.Stopwatch = Stopwatch.StartNew();
					session.Execute( String.Format( "{0}.{1}( {2} );", className, methodName, parameterValues ) );
				}
				else
				{
					_context.Stopwatch = Stopwatch.StartNew();
					session.Execute( Owner.Data.Call );
				}
			}
			finally
			{
				if (_context.Stopwatch != null)
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