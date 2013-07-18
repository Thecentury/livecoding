﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using LiveCoding.Core;
using LiveCoding.Extension.Views;
using LiveCoding.Extension.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
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
			executeTask.ContinueWith( t => OnCompleted( t ), TaskContinuationOptions.None );
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

			var session = engine.CreateSession();

			_context.Session = session;

			session.Execute( rewritten.ToString() );

			var textViewLine = Owner.Data.Line;
			var snapshot = textViewLine.Snapshot;
			var line = snapshot.GetLineFromLineNumber( snapshot.GetLineNumberFromPosition( textViewLine.Start.Position ) );
			string text = line.GetText();

			var methodTree = SyntaxTree.ParseText( text, cancellationToken: token );
			MethodDeclarationSyntax methodSyntax = methodTree.GetRoot( token ).ChildNodes().OfType<MethodDeclarationSyntax>().First();
			string methodName = methodSyntax.Identifier.ValueText;
			var classSyntax = rewritten.ChildNodes().OfType<ClassDeclarationSyntax>().First();
			string className = classSyntax.Identifier.ValueText;

			VariablesTracker.ClearRecords();

			_context.Stopwatch = Stopwatch.StartNew();
			session.Execute( String.Format( "{0}.{1}();", className, methodName ) );
			_context.Stopwatch.Stop();

			Owner.View.VisualElement.Dispatcher.BeginInvoke( () =>
			{
				var layer = Owner.View.GetAdornmentLayer( LiveCodingAdornmentLayers.LiveCodingLayer );

				var view = Owner.View;

				VariableValueTagger tagger;
				var found = view.TextBuffer.Properties.TryGetProperty( typeof( VariableValueTagger ), out tagger );

				if ( found )
				{
					List<SnapshotSpan> spans = new List<SnapshotSpan>();

					foreach ( var value in VariablesTracker.Changes )
					{
						var valueLine = view.TextSnapshot.GetLineFromLineNumber( value.OriginalLineNumber );
						var span = view.GetTextElementSpan( valueLine.Start );
						spans.Add( new SnapshotSpan( span.Start, span.End ) );
					}

					tagger.SetVariableValues( VariablesTracker.Changes, spans );
				}
				//foreach ( var valueChange in VariablesTracker.Changes )
				//{
				//	var line2 = view.TextSnapshot.GetLineFromLineNumber( valueChange.OriginalLineNumber );
				//	var textElementSpan = view.GetTextElementSpan( line2.End );
				//	bool added = layer.AddAdornment( AdornmentPositioningBehavior.TextRelative, new SnapshotSpan( line2.Start, line2.End ), null,
				//		new TextBlock
				//		{
				//			Text = valueChange.Value != null ? valueChange.Value.ToString() : "null",
				//			Foreground = Brushes.DarkBlue
				//		}, ( tag, element ) =>
				//		{

				//		} );
				//}

				int i = 0;

			}, DispatcherPriority.Background );
		}

		public override MethodExecutionState State
		{
			get { return MethodExecutionState.Executing; }
		}
	}
}