using System.Windows;
using LiveCoding.Extension.Support;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio.VariableValues
{
	internal abstract class LiveCodingTagger<TTag, TVisual> : IntraTextAdornmentTagger<TTag, TVisual>
		where TVisual : UIElement
	{
		protected LiveCodingTagger( IWpfTextView view ) : base( view )
		{
			view.TextBuffer.Changed += OnTextBufferChanged;
		}

		private void OnTextBufferChanged( object sender, TextContentChangedEventArgs e )
		{
			OnTextBufferChanged( e );
			var currentSnapshot = e.AfterVersion.TextBuffer.CurrentSnapshot;
			RaiseTagsChanged( new SnapshotSpan( currentSnapshot, 0, currentSnapshot.Length ) );
		}

		protected virtual void OnTextBufferChanged( TextContentChangedEventArgs e )
		{
			
		}
	}
}