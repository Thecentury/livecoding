using LiveCoding.Core;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio.VariableValues
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