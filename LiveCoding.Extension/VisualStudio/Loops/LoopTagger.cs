using System;
using System.Collections.Generic;
using System.Linq;
using LiveCoding.Core;
using LiveCoding.Extension.Views;
using LiveCoding.Extension.VisualStudio.VariableValues;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.VisualStudio.Loops
{
	internal sealed class LoopTagger : LiveCodingTagger<LoopTag, ForLoopView>
	{
		private sealed class LoopInfo
		{
			public ITextSnapshotLine StartLine { get; set; }

			public ITextSnapshotLine EndLine { get; set; }

			public SnapshotSpan LoopSpan { get; set; }

			public int LinesHeight { get; set; }

			public int MaxLineLength { get; set; }

			public ITextSnapshotLine LongestLoopLine { get; set; }
		}

		public LoopTagger( IWpfTextView view )
			: base( view )
		{
		}

		protected override ForLoopView CreateAdornment( LoopTag data, SnapshotSpan span )
		{
			ForLoopView canvas = new ForLoopView( data );
			return canvas;
		}

		protected override bool UpdateAdornment( ForLoopView adornment, LoopTag data, SnapshotSpan snapshotSpan )
		{
			//adornment.SetDataContext( data );

			return true;
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, LoopTag>> GetAdornmentData( NormalizedSnapshotSpanCollection spans )
		{
			if ( spans.Count == 0 )
			{
				yield break;
			}

			if ( View.FormattedLineSource == null )
			{
				yield break;
			}

			var currentSnapshot = View.TextBuffer.CurrentSnapshot;
			if ( currentSnapshot.ContentType.TypeName != VisualStudioLanguages.CSharp )
			{
				yield break;
			}

			var firstSpan = spans[ 0 ];

			var textSnapshot = firstSpan.Snapshot;
			string fullText = textSnapshot.GetText();
			var syntaxTree = SyntaxTree.ParseText( fullText );

			// todo brinchuk compilation errors handling

			var root = syntaxTree.GetRoot();

			var loops = new List<StatementSyntax>();

			loops.AddRange( root.DescendantNodes().OfType<ForStatementSyntax>() );
			loops.AddRange( root.DescendantNodes().OfType<DoStatementSyntax>() );
			loops.AddRange( root.DescendantNodes().OfType<WhileStatementSyntax>() );
			loops.AddRange( root.DescendantNodes().OfType<ForEachStatementSyntax>() );

			var loopInfos = new List<LoopInfo>( loops.Count );

			foreach ( var loop in loops )
			{
				var firstTokenSpan = loop.GetFirstToken().GetLocation().SourceSpan;
				var loopStartLine = textSnapshot.GetLineFromPosition( firstTokenSpan.Start );

				var lastTokenSpan = loop.GetLastToken().GetLocation().SourceSpan;
				var loopEndLine = textSnapshot.GetLineFromPosition( lastTokenSpan.End );

				if ( firstTokenSpan.Start < lastTokenSpan.End )
				{
					int loopStartLineNumber = loopStartLine.LineNumber;
					int loopEndLineNumber = loopEndLine.LineNumber;

					ITextSnapshotLine longestLine = loopStartLine;
					int maxLength = loopStartLine.Length;
					for ( int i = loopStartLineNumber; i <= loopEndLineNumber; i++ )
					{
						var line = textSnapshot.GetLineFromLineNumber( i );
						if ( line.Length > maxLength )
						{
							maxLength = line.Length;
							longestLine = line;
						}
					}

					loopInfos.Add( new LoopInfo
					{
						StartLine = loopStartLine,
						EndLine = loopEndLine,
						LoopSpan = new SnapshotSpan(
							new SnapshotPoint( textSnapshot, firstTokenSpan.Start ),
							new SnapshotPoint( textSnapshot, lastTokenSpan.End ) ),
						LinesHeight = loopEndLine.LineNumber - loopStartLine.LineNumber + 1,
						MaxLineLength = maxLength,
						LongestLoopLine = longestLine
					} );
				}
			}

			foreach ( var span in spans )
			{
				foreach ( var loopInfo in loopInfos )
				{
					if ( span.IntersectsWith( loopInfo.LoopSpan ) )
					{
						double leftMargin = View.FormattedLineSource.ColumnWidth * ( loopInfo.LongestLoopLine.Length - loopInfo.StartLine.Length + 1 );

						yield return Tuple.Create( new SnapshotSpan( loopInfo.StartLine.End, 0 ), new PositionAffinity?( PositionAffinity.Predecessor ), new LoopTag
						{
							LoopStartLineNumber = loopInfo.StartLine.LineNumber,
							LineHeight = View.LineHeight,
							RowsCount = loopInfo.LinesHeight,
							LeftMargin = leftMargin
						} );
						break;
					}
				}
			}
		}

		public void BeginLoopWatch( ForLoopInfo loop, SnapshotSpan span )
		{
			ForLoopView loopView;

			var key = new SnapshotSpan( span.Start, 0 );

			if ( !AdornmentCache.TryGetValue( key, out loopView ) )
			{
				// todo brinchuk 
				return;
			}

			loopView.BeginWatching( loop );
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}
	}
}