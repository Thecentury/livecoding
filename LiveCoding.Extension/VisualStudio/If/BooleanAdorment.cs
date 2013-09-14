using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Annotations;
using LiveCoding.Core;
using LiveCoding.Extension.Extensions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace LiveCoding.Extension.VisualStudio.If
{
	internal sealed class BooleanAdorment
	{
		private readonly IWpfTextView _textView;

		private readonly Brush _trueBrush = new SolidColorBrush( Color.FromArgb( 0xFF, 0xAB, 0xEA, 0xC6 ) ).AsFrozen();

		private readonly Brush _falseBrush = new SolidColorBrush( Color.FromArgb( 0xFF, 0xFF, 0x85, 0x70 ) ).AsFrozen();

		public BooleanAdorment( [NotNull] IWpfTextView textView )
		{
			if ( textView == null )
			{
				throw new ArgumentNullException( "textView" );
			}
			_textView = textView;
			_textView.LayoutChanged += OnTextViewLayoutChanged;
			_textView.TextBuffer.Changed += OnTextBufferChanged;
			_textView.Closed += OnTextViewClosed;
			_layer = textView.GetAdornmentLayer( BooleanAdornmentFactory.AdornmentLayerName );
		}

		private void OnTextViewClosed( object sender, EventArgs e )
		{
			if ( _events != null )
			{
				_events.Clear();
			}
		}

		private void OnTextBufferChanged( object sender, TextContentChangedEventArgs e )
		{
			Clear();
		}

		private List<IfEvaluationEvent> _events;
		private readonly IAdornmentLayer _layer;

		public void AddVariableChange( IfEvaluationEvent evt, SnapshotSpan span )
		{
			if ( _events == null )
			{
				_events = new List<IfEvaluationEvent>();
			}

			_events.Add( evt );
			var line = _textView.GetTextViewLineContainingBufferPosition( span.Start );
			if ( line != null )
			{
				CreateAdornment( line, evt );
			}
		}

		private void OnTextViewLayoutChanged( object sender, TextViewLayoutChangedEventArgs e )
		{
			if ( _events == null || _events.Count == 0 )
			{
				return;
			}

			foreach ( var line in e.NewOrReformattedLines )
			{
				CreateVisuals( line );
			}
		}

		private void CreateVisuals( ITextViewLine line )
		{
			if ( _events == null || _events.Count == 0 )
			{
				return;
			}

			int lineNumber = line.Snapshot.GetLineNumberFromPosition( line.Start );
			var evt = _events.LastOrDefault( e => e.StartIfLine == lineNumber );
			if ( evt == null )
			{
				return;
			}

			CreateAdornment( line, evt );
		}

		private void CreateAdornment( ITextViewLine line, IfEvaluationEvent evt )
		{
			var conditionSpan = new SnapshotSpan( line.Snapshot, evt.ConditionStartPosition,
				evt.ConditionEndPosition - evt.ConditionStartPosition );
			var markerGeometry = _textView.TextViewLines.GetMarkerGeometry( conditionSpan );
			if ( markerGeometry == null )
			{
				return;
			}

			Brush brush = evt.ConditionValue ? _trueBrush : _falseBrush;

			GeometryDrawing drawing = new GeometryDrawing( brush, null, markerGeometry ).AsFrozen();

			DrawingImage drawingImage = new DrawingImage( drawing ).AsFrozen();

			Image adornment = new Image { Source = drawingImage };

			Canvas.SetLeft( adornment, markerGeometry.Bounds.Left );
			Canvas.SetTop( adornment, markerGeometry.Bounds.Top );

			_layer.AddAdornment( AdornmentPositioningBehavior.TextRelative, conditionSpan, null, adornment, null );
		}

		public void Clear()
		{
			_layer.RemoveAllAdornments();
			if ( _events != null )
			{
				_events.Clear();
			}
		}
	}
}