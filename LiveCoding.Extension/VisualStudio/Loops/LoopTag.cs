using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension.VisualStudio.Loops
{
	internal sealed class LoopTag : ITag
	{
		public int LoopStartLineNumber { get; set; }

		public double LineHeight { get; set; }

		public double LeftMargin { get; set; }
		
		public SnapshotSpan SnapshotSpan { get; set; }
	}
}