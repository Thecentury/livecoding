using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio.ParametrizedMethod
{
	internal sealed class ParametrizedMethodTag : IGlyphTag
	{
		public ParametrizedMethodTag( string call )
		{
			Call = call;
		}

		public string Call { get; private set; }
	}
}