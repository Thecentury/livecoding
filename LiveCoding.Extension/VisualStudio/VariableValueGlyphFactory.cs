using System.Windows;
using System.Windows.Controls;
using LiveCoding.Core;
using LiveCoding.Extension.VisualStudio.VariableValues;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class VariableValueGlyphFactory : IGlyphFactory
	{
		public UIElement GenerateGlyph( IWpfTextViewLine line, IGlyphTag tag )
		{
			var valueTag = tag as VariableValueTag;
			if ( valueTag == null )
			{
				return null;
			}

			return new TextBlock { Text = valueTag.Change.GetValueString() };
		}
	}
}