using System;
using System.Collections.Generic;
using IntraTextAdornmentSample;
using LiveCoding.Extension.ViewModels;
using LiveCoding.Extension.Views;
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
				DataContext = new MethodExecutionViewModel( new MethodGlyphTag( span ), _view )
			};
		}

		protected override bool UpdateAdornment( ExecuteMethodControl adornment, MethodTag data )
		{
			return false;
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, MethodTag>> GetAdornmentData( NormalizedSnapshotSpanCollection spans )
		{
			foreach ( SnapshotSpan span in spans )
			{
				bool yielded = false;
				foreach ( ClassificationSpan classification in _сlassifier.GetClassificationSpans( span ) )
				{
					string spanText = span.ToString();
					if ( classification.ClassificationType.Classification.ToLower().Contains( "keyword" ) && spanText.Contains( "public" ) || spanText.Contains( "private" ) || spanText.Contains( "internal" ) || spanText.Contains( "protected" ) )
					{
						if ( !yielded )
						{
							yielded = true;
							yield return Tuple.Create( new SnapshotSpan( span.Start, 0 ), (PositionAffinity?)PositionAffinity.Predecessor, new MethodTag() );
						}
					}
				}
			}
		}
	}
}