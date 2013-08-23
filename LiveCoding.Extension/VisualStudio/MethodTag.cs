using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class MethodTag : IGlyphTag
	{
		private readonly SnapshotSpan _snapshotSpan;

		public MethodTag( SnapshotSpan snapshotSpan )
		{
			_snapshotSpan = snapshotSpan;
		}

		public SnapshotSpan SnapshotSpan
		{
			get { return _snapshotSpan; }
		}
	}
}