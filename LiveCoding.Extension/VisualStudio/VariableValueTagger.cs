﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using IntraTextAdornmentSample;
using LiveCoding.Core;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class VariableValueTagger : IntraTextAdornmentTagger<VariableValueTag, TextBlock>, IDisposable
	{
		private readonly ITagAggregator<VariableValueTag> _tagAggregator;

		public VariableValueTagger( IWpfTextView view, ITagAggregator<VariableValueTag> tagAggregator )
			: base( view )
		{
			_tagAggregator = tagAggregator;
		}

		private List<ValueChange> _changes;

		public void SetVariableValues( IEnumerable<ValueChange> changes, List<SnapshotSpan> spans )
		{
			_changes = new List<ValueChange>( changes );

			foreach ( var snapshotSpan in spans )
			{
				RaiseTagsChanged( snapshotSpan );
			}
		}

		protected override TextBlock CreateAdornment( VariableValueTag data, SnapshotSpan span )
		{
			return new TextBlock { Text = data.Change.GetValueString() };
		}

		protected override bool UpdateAdornment( TextBlock adornment, VariableValueTag data )
		{
			adornment.Text = data.Change.GetValueString();
			return true;
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, VariableValueTag>> GetAdornmentData( NormalizedSnapshotSpanCollection spans )
		{
			if ( _changes == null || _changes.Count == 0 )
			{
				yield break;
			}

			foreach ( var span in spans )
			{
				int lineNumber = span.Snapshot.GetLineNumberFromPosition( span.Start.Position );
				var line = span.Snapshot.GetLineFromLineNumber( lineNumber );

				var change = _changes.FirstOrDefault( c => c.OriginalLineNumber == lineNumber );
				if ( change != null )
				{
					var snapshotSpan = new SnapshotSpan( line.End, 0 );
					yield return Tuple.Create( snapshotSpan, (PositionAffinity?)PositionAffinity.Successor, new VariableValueTag( change ) );
				}
			}
		}

		public void Dispose()
		{
			_tagAggregator.Dispose();
			view.Properties.RemoveProperty( typeof( VariableValueTagger ) );
		}
	}
}