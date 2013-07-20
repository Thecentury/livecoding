using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Formatting;

namespace LiveCoding.Extension.VisualStudio
{
	public sealed class MethodExecutionData
	{
		public MethodExecutionData( SnapshotSpan snapshotSpan )
		{
			SnapshotSpan = snapshotSpan;
		}

		public SnapshotSpan SnapshotSpan { get; private set; }

		public string Call { get; set; }
	}
}