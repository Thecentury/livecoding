﻿	using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class MethodTagger : ITagger<MethodTag>
	{
		private readonly IClassifier _сlassifier;

		internal MethodTagger( IClassifier classifier )
		{
			_сlassifier = classifier;
		}

		IEnumerable<ITagSpan<MethodTag>> ITagger<MethodTag>.GetTags( NormalizedSnapshotSpanCollection spans )
		{
			foreach ( SnapshotSpan span in spans )
			{
				//look at each classification span

				bool yielded = false;
				foreach ( ClassificationSpan classification in _сlassifier.GetClassificationSpans( span ) )
				{
					string spanText = span.ToString();
					if ( classification.ClassificationType.Classification.ToLower().Contains( "keyword" ) && spanText.Contains( "public" ) || spanText.Contains( "private" ) || spanText.Contains( "internal" ) || spanText.Contains( "protected" ) )
					{
						if ( !yielded )
						{
							yielded = true;
							yield return new TagSpan<MethodTag>( new SnapshotSpan( classification.Span.Start, classification.Span.Length ), new MethodTag() );
						}
					}
				}
			}
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
	}
}