using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCoding.Core;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio.Invocations
{
	internal sealed class InvocationTagger : LiveCodingTagger<InvocationTag, TextBox>
	{
		public InvocationTagger( IWpfTextView view )
			: base( view )
		{
		}

		protected override TextBox CreateAdornment( InvocationTag data, SnapshotSpan span )
		{
			return new TextBox
			{
				IsReadOnly = true,
				Text = GetText( data ),
				BorderBrush = null,
				BorderThickness = new Thickness(),
				Background = Brushes.Transparent
			};
		}

		private static string GetText( InvocationTag data )
		{
			return String.Format( "( {0} )", String.Join( ", ", data.Invocation.Parameters.Select( i => i.ToString() ) ) );
		}

		protected override bool UpdateAdornment( TextBox adornment, InvocationTag data, SnapshotSpan snapshotSpan )
		{
			adornment.Text = GetText( data );
			return true;
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, InvocationTag>> GetAdornmentData( NormalizedSnapshotSpanCollection spans )
		{
			if ( _invocations == null || _invocations.Count == 0 )
			{
				yield break;
			}

			foreach ( var span in spans )
			{
				int lineNumber = span.Snapshot.GetLineNumberFromPosition( span.Start.Position );
				var line = span.Snapshot.GetLineFromLineNumber( lineNumber );

				var invocation = _invocations.LastOrDefault( c => c.LineNumber == lineNumber );
				if ( invocation != null )
				{
					var snapshotSpan = new SnapshotSpan( line.End, 0 );
					yield return Tuple.Create( snapshotSpan, (PositionAffinity?)PositionAffinity.Successor, new InvocationTag( invocation ) );
				}
			}
		}

		private List<InvocationEvent> _invocations;

		public void AddInvocation( InvocationEvent invocation, SnapshotSpan span )
		{
			if ( _invocations == null )
			{
				_invocations = new List<InvocationEvent>();
			}

			_invocations.Add( invocation );
			RaiseTagsChanged( span );
		}
	}
}