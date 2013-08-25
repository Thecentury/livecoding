using System;
using System.Collections.Generic;
using System.Threading;
using IntraTextAdornmentSample;
using LiveCoding.Extension.Support;
using LiveCoding.Extension.ViewModels;
using LiveCoding.Extension.Views;
using LiveCoding.Extension.VisualStudio.Method;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class MethodTagger : IntraTextAdornmentTagger<MethodTag, ExecuteMethodControl>
	{
		private readonly IClassifier _сlassifier;
		private readonly IWpfTextView _view;

		internal MethodTagger( IClassifier classifier, IWpfTextView view )
			: base( view )
		{
			_сlassifier = classifier;
			_view = view;
		}

		protected override ExecuteMethodControl CreateAdornment( MethodTag data, SnapshotSpan span )
		{
			return new ExecuteMethodControl
			{
				DataContext = new MethodExecutionViewModel( new MethodExecutionData( span ), _view )
			};
		}

		protected override bool UpdateAdornment( ExecuteMethodControl adornment, MethodTag data, SnapshotSpan snapshotSpan )
		{
			adornment.DataContext = new MethodExecutionViewModel( new MethodExecutionData( snapshotSpan ), _view );
			return true;
		}

		private static bool ContainsAccessModifier( string code )
		{
			return code.Contains( "public" ) || code.Contains( "private" ) || code.Contains( "internal" ) || code.Contains( "protected" );
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, MethodTag>> GetAdornmentData( NormalizedSnapshotSpanCollection spans )
		{
			if ( spans.Count == 0 )
			{
				yield break;
			}

			var currentSnapshot = _view.TextBuffer.CurrentSnapshot;

			foreach ( SnapshotSpan span in spans )
			{
				bool yielded = false;

				if ( currentSnapshot.Length <= span.Start.Position )
				{
					continue;
				}

				string fullSpanLineText = currentSnapshot.GetLineFromPosition( span.Start ).GetText();

				bool probablyIsMethod = fullSpanLineText.Contains( "(" );
				if ( !probablyIsMethod )
				{
					continue;
				}

				if ( !ContainsAccessModifier( fullSpanLineText ) )
				{
					continue;
				}

				foreach ( ClassificationSpan classification in _сlassifier.GetClassificationSpans( span ) )
				{
					if ( classification.ClassificationType.Classification.ToLower().Contains( "keyword" ) )
					{
						if ( !yielded )
						{
							yielded = true;
							var snapshotSpan = new SnapshotSpan( span.Start, 0 );
							yield return Tuple.Create( snapshotSpan, (PositionAffinity?)PositionAffinity.Predecessor, new MethodTag() );
						}
					}
				}
			}
		}
	}
}