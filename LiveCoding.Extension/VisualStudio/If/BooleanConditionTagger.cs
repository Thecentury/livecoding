using System;
using System.Collections.Generic;
using System.Linq;
using LiveCoding.Core;
using LiveCoding.Extension.Support;
using LiveCoding.Extension.Views;
using LiveCoding.Extension.VisualStudio.VariableValues;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio.If
{
	internal sealed class BooleanConditionTagger : LiveCodingTagger<BooleanConditionTag, ConditionView>
	{
		public BooleanConditionTagger( IWpfTextView view )
			: base( view )
		{
		}

		protected override ConditionView CreateAdornment( BooleanConditionTag data, SnapshotSpan span )
		{
			return new ConditionView( data.Value );
		}

		protected override bool UpdateAdornment( ConditionView adornment, BooleanConditionTag data, SnapshotSpan snapshotSpan )
		{
			adornment.SetValue( data.Value );

			return true;
		}

		private List<IfEvaluationEvent> _events;

		public void AddVariableChange( IfEvaluationEvent evt, SnapshotSpan span )
		{
			if ( _events == null )
			{
				_events = new List<IfEvaluationEvent>();
			}

			_events.Add( evt );
			RaiseTagsChanged( span );
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, BooleanConditionTag>> GetAdornmentData( NormalizedSnapshotSpanCollection spans )
		{
			if ( _events == null || _events.Count == 0 )
			{
				yield break;
			}

			foreach ( var span in spans )
			{
				var snapshot = span.Snapshot;

				int lineNumber = snapshot.GetLineNumberFromPosition( span.Start.Position );

				var evt = _events.LastOrDefault( c => c.StartIfLine == lineNumber );
				if ( evt != null )
				{
					var snapshotSpan = new SnapshotSpan( new SnapshotPoint( snapshot, evt.ConditionStartPosition ), new SnapshotPoint( snapshot, evt.ConditionEndPosition ) );
					yield return Tuple.Create( snapshotSpan, new PositionAffinity?(), new BooleanConditionTag( evt.ConditionValue ) );
				}
			}
		}

		public void Clear()
		{
			// todo brinchuk 
		}
	}
}