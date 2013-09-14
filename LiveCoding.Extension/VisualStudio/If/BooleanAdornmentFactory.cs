using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace LiveCoding.Extension.VisualStudio.If
{
	[Export( typeof( IWpfTextViewCreationListener ) )]
	[ContentType( "text" )]
	[TextViewRole( PredefinedTextViewRoles.Document )]
	internal sealed class BooleanAdornmentFactory : IWpfTextViewCreationListener
	{
		public const string AdornmentLayerName = "BooleanAdornment";

		[Export( typeof( AdornmentLayerDefinition ) )]
		[Name( AdornmentLayerName )]
		[Order( After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text )]
		public AdornmentLayerDefinition EditorAdornmentLayer = null;

		public void TextViewCreated( IWpfTextView textView )
		{
			textView.TextBuffer.Properties.GetOrCreateSingletonProperty( () => new BooleanAdorment( textView ) );
		}
	}
}