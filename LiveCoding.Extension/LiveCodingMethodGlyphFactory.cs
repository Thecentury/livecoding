using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace LiveCoding.Extension
{
    internal sealed class LiveCodingMethodGlyphFactory : IGlyphFactory
    {
        private readonly IWpfTextView _view;

        public LiveCodingMethodGlyphFactory( IWpfTextView view )
        {
            _view = view;
        }

        const double GlyphSize = 12;

        public UIElement GenerateGlyph( IWpfTextViewLine line, IGlyphTag tag )
        {
            // Ensure we can draw a glyph for this marker. 
            if ( tag == null || !( tag is LiveCodingMethodTag ) )
            {
                return null;
            }

            string filePath = _view.GetFilePath();

            Ellipse ellipse = new Ellipse
            {
                Fill = Brushes.LightBlue,
                StrokeThickness = 1,
                Stroke = Brushes.DarkBlue,
                Height = GlyphSize,
                Width = GlyphSize,
                ToolTip = filePath
            };

            ellipse.MouseLeftButtonDown += OnGlyphMouseLeftButtonDown;

            return ellipse;
        }

        private void OnGlyphMouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            MessageBox.Show( "111" );
        }
    }
}