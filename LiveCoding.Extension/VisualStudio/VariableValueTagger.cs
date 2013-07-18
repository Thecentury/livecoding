using System;
using System.Collections.Generic;
using System.Linq;
using LiveCoding.Core;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class VariableValueTagger : ITagger<VariableValueTag>
	{
		private readonly IClassifier _classifier;

		public VariableValueTagger( IClassifier classifier )
		{
			_classifier = classifier;
		}

		private List<ValueChange> _changes;

		public void SetVariableValues( IEnumerable<ValueChange> changes, List<SnapshotSpan> spans )
		{
			_changes = new List<ValueChange>( changes );

			foreach ( var snapshotSpan in spans )
			{
				TagsChanged.Raise( this, new SnapshotSpanEventArgs( snapshotSpan ) );
			}
		}

		public IEnumerable<ITagSpan<VariableValueTag>> GetTags( NormalizedSnapshotSpanCollection spans )
		{
			if ( _changes == null || _changes.Count == 0 )
			{
				yield break;
			}

			foreach ( var span in spans.Take( 1 ) )
			{
				int lineNumber = span.Snapshot.GetLineNumberFromPosition( span.Start.Position );
				var line = span.Snapshot.GetLineFromLineNumber( lineNumber );

				var change = _changes.FirstOrDefault( c => c.OriginalLineNumber == lineNumber );
				if ( change != null )
				{
					var snapshotSpan = new SnapshotSpan( line.Start, line.End );
					yield return new TagSpan<VariableValueTag>( snapshotSpan, new VariableValueTag( change ) );
				}
			}
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
	}
}