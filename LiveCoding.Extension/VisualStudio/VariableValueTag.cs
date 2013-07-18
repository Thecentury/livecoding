using LiveCoding.Core;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class VariableValueTag : IGlyphTag
	{
		private readonly ValueChange _change;

		public VariableValueTag( ValueChange change )
		{
			_change = change;
		}

		public ValueChange Change
		{
			get { return _change; }
		}
	}
}