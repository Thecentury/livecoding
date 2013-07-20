using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Formatting;

namespace LiveCoding.Extension.VisualStudio
{
	public sealed class MethodGlyphTag
	{
		public MethodGlyphTag( SnapshotSpan snapshotSpan )
		{
			SnapshotSpan = snapshotSpan;
		}

		public SnapshotSpan SnapshotSpan { get; private set; }
	}
}