using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace LiveCoding.Extension
{
    [Export( typeof( IGlyphFactoryProvider ) )]
    [Name( "LiveCodingMethodGlyph" )]
    [Order( After = "VsTextMarker" )]
    [ContentType( "code" )]
    [TagType( typeof( LiveCodingMethodTag ) )]
    internal sealed class TodoGlyphFactoryProvider : IGlyphFactoryProvider
    {
        public IGlyphFactory GetGlyphFactory( IWpfTextView view, IWpfTextViewMargin margin )
        {
            return new LiveCodingMethodGlyphFactory( view );
        }
    }
}