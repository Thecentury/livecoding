using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCoding.Core;
using LiveCoding.Extension.ViewModels;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Roslyn.Compilers.CSharp;
using Roslyn.Scripting.CSharp;
using VSLangProj;

namespace LiveCoding.Extension
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