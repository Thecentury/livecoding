﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCoding.Core;
using LiveCoding.Extension.Support;
using LiveCoding.Extension.Views;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension.VisualStudio.VariableValues
{
	internal sealed class VariableValueTagger : IntraTextAdornmentTagger<VariableValueTag, ObjectView>, IDisposable
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

		public void AddVariableChange( ValueChange change, SnapshotSpan span )
		{
			if ( _changes == null )
			{
				_changes = new List<ValueChange>();
			}

			_changes.Add( change );
			RaiseTagsChanged( span );
		}

		protected override ObjectView CreateAdornment( VariableValueTag data, SnapshotSpan span )
		{
			ValueChange change = data.Change;
			return new ObjectView( change.CapturedValue ) { Margin = new Thickness( 20, 0, 0, 0 ), ToolTip = change.TimestampUtc };
		}

		protected override bool UpdateAdornment( ObjectView adornment, VariableValueTag data, SnapshotSpan snapshotSpan )
		{
			var change = data.Change;
			adornment.SetRootObject( change.CapturedValue );
			adornment.ToolTip = change.TimestampUtc;
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

				var change = _changes.LastOrDefault( c => c.OriginalLineNumber == lineNumber );
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
			View.Properties.RemoveProperty( typeof( VariableValueTagger ) );
		}

		public void ClearVariableChanges()
		{
			_changes = null;

			var snapshot = View.TextBuffer.CurrentSnapshot;
			RaiseTagsChanged( new SnapshotSpan( snapshot, 0, snapshot.Length ) );
		}
	}
}