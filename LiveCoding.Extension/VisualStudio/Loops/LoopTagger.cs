using System;
using System.Collections.Generic;
using System.Linq;
using LiveCoding.Core;
using LiveCoding.Extension.Views;
using LiveCoding.Extension.VisualStudio.VariableValues;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.VisualStudio.Loops
{
	internal sealed class LoopTagger : LiveCodingTagger<LoopTag, ForLoopView>
	{
		private sealed class LoopInfo
		{
			public ITextSnapshotLine StartLine { get; set; }

			public SnapshotSpan LoopSpan { get; set; }

			public int LinesHeight { get; set; }

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
			adornment.SetDataContext( data );

			return true;
		}

		private readonly Dictionary<int, List<LoopInfo>> _loopsCache = new Dictionary<int, List<LoopInfo>>();

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

			var firstSpan = spans[0];

			var loopInfos = GetLoops( firstSpan.Snapshot );

			var tempLoops = new List<LoopInfo>( loopInfos.Count );
			tempLoops.AddRange( loopInfos );

			foreach ( var span in spans )
			{
				foreach ( var loopInfo in tempLoops )
				{
					if ( span.IntersectsWith( loopInfo.LoopSpan ) )
					{
						int tabSize = View.FormattedLineSource.TabSize;
						double leftMargin = View.FormattedLineSource.ColumnWidth * (
							GetExpandedCharactersLength( loopInfo.LongestLoopLine, tabSize ) -
							GetExpandedCharactersLength( loopInfo.StartLine, tabSize ) );

						yield return Tuple.Create( new SnapshotSpan( loopInfo.StartLine.End, 0 ), new PositionAffinity?( PositionAffinity.Predecessor ), new LoopTag
						{
							LoopStartLineNumber = loopInfo.StartLine.LineNumber + 1,
							LineHeight = View.LineHeight,
							LeftMargin = leftMargin
						} );

						tempLoops.Remove( loopInfo );
						break;
					}
				}
			}
		}

		private List<LoopInfo> GetLoops( ITextSnapshot textSnapshot )
		{
			List<LoopInfo> result;
			int versionNumber = textSnapshot.Version.VersionNumber;
			if ( _loopsCache.TryGetValue( versionNumber, out result ) )
			{
				return result;
			}

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
						LoopSpan = new SnapshotSpan(
							new SnapshotPoint( textSnapshot, firstTokenSpan.Start ),
							new SnapshotPoint( textSnapshot, lastTokenSpan.End ) ),
						LinesHeight = loopEndLine.LineNumber - loopStartLine.LineNumber + 1,
						LongestLoopLine = longestLine
					} );
				}
			}

			_loopsCache[versionNumber] = loopInfos;

			List<int> previousVersions = _loopsCache.Keys.Where( k => k < versionNumber ).ToList();

			foreach ( int previousVersion in previousVersions )
			{
				_loopsCache.Remove( previousVersion );
			}

			return loopInfos;
		}

		private static int GetExpandedCharactersLength( ITextSnapshotLine line, int tabSize )
		{
			return GetExpandedCharactersLength( line.GetText(), tabSize );
		}

		private static int GetExpandedCharactersLength( string str, int tabSize )
		{
			int tabsCount = str.Count( c => c == '\t' );
			return tabsCount * tabSize + str.Length - tabsCount;
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
			// todo brinchuk 
		}
	}
}