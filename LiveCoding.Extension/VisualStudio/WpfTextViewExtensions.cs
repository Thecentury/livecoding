using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio
{
    internal static class WpfTextViewExtensions
    {
        // http://stackoverflow.com/questions/2489448/accessing-the-project-system-from-a-visual-studio-mef-editor-extension
        public static string GetFilePath( this IWpfTextView wpfTextView )
        {
            ITextDocument document;
            if ( ( wpfTextView == null ) ||
                 ( !wpfTextView.TextDataModel.DocumentBuffer.Properties.TryGetProperty( typeof( ITextDocument ),
                     out document ) ) )
            {
                return String.Empty;
            }

            // If we have no document, just ignore it.
            if ( ( document == null ) || ( document.TextBuffer == null ) )
                return String.Empty;

            return document.FilePath;
        }
    }
}