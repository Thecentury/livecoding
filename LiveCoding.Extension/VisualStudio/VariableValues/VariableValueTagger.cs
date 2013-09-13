using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LiveCoding.Core;
using LiveCoding.Extension.Views;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension.VisualStudio.VariableValues
{
	internal sealed class VariableValueTagger : LiveCodingTagger<VariableValueTag, FrameworkElement>, IDisposable
	{
		private readonly ITagAggregator<VariableValueTag> _tagAggregator;

		public VariableValueTagger( IWpfTextView view, ITagAggregator<VariableValueTag> tagAggregator )
			: base( view )
		{
			_tagAggregator = tagAggregator;
		}

		protected override void OnTextBufferChanged( TextContentChangedEventArgs e )
		{
			_changes = null;
		}

		private List<ValueChange> _changes;

		public void AddVariableChange( ValueChange change, SnapshotSpan span )
		{
			if ( _changes == null )
			{
				_changes = new List<ValueChange>();
			}

			_changes.Add( change );
			RaiseTagsChanged( span );
		}

		protected override FrameworkElement CreateAdornment( VariableValueTag data, SnapshotSpan span )
		{
			ValueChange change = data.Change;

			Thickness margin = new Thickness( 20, 0, 0, 0 );

			FrameworkElement adornment = ValueViewFactory.CreateView( change );

			adornment.Margin = margin;
			adornment.ToolTip = change.TimestampUtc.ToLocalTime();

			return adornment;
		}

		protected override bool UpdateAdornment( FrameworkElement adornment, VariableValueTag data, SnapshotSpan snapshotSpan )
		{
			var change = data.Change;

			ObjectViewContainer objectView = adornment as ObjectViewContainer;
			if ( objectView != null )
			{
				objectView.SetRootObject( change.CapturedValue );
			}

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