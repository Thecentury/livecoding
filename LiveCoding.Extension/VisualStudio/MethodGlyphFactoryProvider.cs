using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace LiveCoding.Extension.VisualStudio
{
	[Export( typeof( IGlyphFactoryProvider ) )]
	[Name( "LiveCodingMethodGlyph" )]
	[Order( Before = PredefinedMarginNames.LineNumber )]
	[ContentType( "code" )]
	[TagType( typeof( MethodTag ) )]
	//[MarginContainer( PredefinedMarginNames.Right )]
	internal sealed class MethodGlyphFactoryProvider : IGlyphFactoryProvider
	{
		public IGlyphFactory GetGlyphFactory( IWpfTextView view, IWpfTextViewMargin margin )
		{
			return new MethodGlyphFactory( view );
		}
	}
}