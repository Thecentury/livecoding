using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension.VisualStudio.ForLoops
{
	internal sealed class ForLoopTag : ITag
	{
		public int LoopStartLineNumber { get; set; }

		public int RowsCount { get; set; }

		public double LineHeight { get; set; }

		public double LoopHeight
		{
			get { return LineHeight * RowsCount; }
		}
	}
}