using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension
{
    internal sealed class LiveCodingMethodTagger : ITagger<LiveCodingMethodTag>
    {
        private readonly IClassifier _сlassifier;

        internal LiveCodingMethodTagger( IClassifier classifier )
        {
            _сlassifier = classifier;
        }

        IEnumerable<ITagSpan<LiveCodingMethodTag>> ITagger<LiveCodingMethodTag>.GetTags( NormalizedSnapshotSpanCollection spans )
        {
            foreach ( SnapshotSpan span in spans )
            {
                //look at each classification span \ 
                foreach ( ClassificationSpan classification in _сlassifier.GetClassificationSpans( span ) )
                {
                    string spanText = span.ToString();
                    if ( classification.ClassificationType.Classification.ToLower().Contains( "keyword" ) && spanText.Contains( "public" ) || spanText.Contains( "private" ) || spanText.Contains( "internal" ) || spanText.Contains( "protected" ) )
                    {
                        yield return new TagSpan<LiveCodingMethodTag>( new SnapshotSpan( classification.Span.Start, classification.Span.Length ), new LiveCodingMethodTag() );
                    }
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}