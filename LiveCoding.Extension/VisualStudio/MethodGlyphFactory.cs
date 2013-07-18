using System.Windows;
using LiveCoding.Extension.ViewModels;
using LiveCoding.Extension.Views;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace LiveCoding.Extension.VisualStudio
{
	internal sealed class MethodGlyphFactory : IGlyphFactory
	{
		private readonly IWpfTextView _view;

		public MethodGlyphFactory( IWpfTextView view )
		{
			_view = view;
		}

		const double GlyphSize = 12;

		public UIElement GenerateGlyph( IWpfTextViewLine line, IGlyphTag tag )
		{
			// Ensure we can draw a glyph for this marker. 
			if ( tag == null || !( tag is MethodTag ) )
			{
				return null;
			}

			string filePath = _view.GetFilePath();

			var methodGlyphTag = new MethodGlyphTag
			{
				Line = line
			};

			ExecuteMethodControl glyph = new ExecuteMethodControl
			{
				Height = GlyphSize,
				Width = GlyphSize,
				ToolTip = filePath,
				DataContext = new MethodExecutionViewModel( methodGlyphTag, _view )
			};

			return glyph;
		}
	}
}