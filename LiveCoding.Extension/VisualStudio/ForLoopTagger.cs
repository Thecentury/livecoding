using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using IntraTextAdornmentSample;
using LiveCoding.Extension.Support;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Roslyn.Compilers.CSharp;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class ForLoopTagger : IntraTextAdornmentTagger<ForLoopTag, Canvas>
	{
		private sealed class LoopInfo
		{
			public ITextSnapshotLine StartLine { get; set; }

			public ITextSnapshotLine EndLine { get; set; }

			public SnapshotSpan LoopSpan { get; set; }

			public int LinesHeight { get; set; }
		}

		public ForLoopTagger( IWpfTextView view )
			: base( view )
		{
		}

		protected override Canvas CreateAdornment( ForLoopTag data, SnapshotSpan span )
		{
			Canvas canvas = new Canvas();
			var rectangle = new Rectangle
			{
				Fill = Brushes.LightGreen,
				Width = 30,
				Height = data.RowsCount * data.LineHeight
			};

			Canvas.SetTop( rectangle, -10 );
			Canvas.SetLeft( rectangle, 0 );

			canvas.Children.Add( rectangle );
			return canvas;
		}

		protected override bool UpdateAdornment( Canvas adornment, ForLoopTag data )
		{
			return true;
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, ForLoopTag>> GetAdornmentData( NormalizedSnapshotSpanCollection spans )
		{
			if ( spans.Count == 0 )
			{
				yield break;
			}

			if ( view.FormattedLineSource == null )
			{
				yield break;
			}

			var firstSpan = spans[0];

			var textSnapshot = firstSpan.Snapshot;
			string fullText = textSnapshot.GetText();
			var syntaxTree = SyntaxTree.ParseText( fullText );

			// todo brinchuk compilation errors handling

			var root = syntaxTree.GetRoot();
			var forLoops = root.DescendantNodes().OfType<ForStatementSyntax>().ToList();

			var loopInfos = new List<LoopInfo>( forLoops.Count );

			foreach ( var loop in forLoops )
			{
				var firstTokenSpan = loop.GetFirstToken().GetLocation().SourceSpan;
				var startLine = textSnapshot.GetLineFromPosition( firstTokenSpan.Start );

				var lastTokenSpan = loop.GetLastToken().GetLocation().SourceSpan;
				var endLine = textSnapshot.GetLineFromPosition( lastTokenSpan.End );

				if ( firstTokenSpan.Start < lastTokenSpan.End )
				{
					loopInfos.Add( new LoopInfo
					{
						StartLine = startLine,
						EndLine = endLine,
						LoopSpan =
							new SnapshotSpan( new SnapshotPoint( textSnapshot, firstTokenSpan.Start ),
								new SnapshotPoint( textSnapshot, lastTokenSpan.End ) ),
						LinesHeight = endLine.LineNumber - startLine.LineNumber + 1
					} );
				}
			}

			foreach ( var span in spans )
			{
				foreach ( var loopInfo in loopInfos )
				{
					if ( span.IntersectsWith( loopInfo.LoopSpan ) )
					{
						yield return Tuple.Create( new SnapshotSpan( loopInfo.StartLine.End, 0 ), new PositionAffinity?( PositionAffinity.Predecessor ), new ForLoopTag
						{
							LineHeight = view.LineHeight,
							RowsCount = loopInfo.LinesHeight
						} );
					}
				}
			}
		}
	}
}