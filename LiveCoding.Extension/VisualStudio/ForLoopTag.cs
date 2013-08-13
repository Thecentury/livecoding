using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class ForLoopTag : ITag
	{
		public int RowsCount { get; set; }

		public double LineHeight { get; set; }

		public double LoopHeight
		{
			get { return LineHeight*RowsCount; }
		}
	}
}