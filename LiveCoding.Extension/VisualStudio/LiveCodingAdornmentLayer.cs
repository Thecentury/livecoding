using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace LiveCoding.Extension.VisualStudio
{
	[Export( typeof( IWpfTextViewCreationListener ) )]
	[ContentType( "csharp" )]
	[TextViewRole( PredefinedTextViewRoles.Document )]
	internal sealed class LiveCodingAdornmentLayerProvider : IWpfTextViewCreationListener
	{
		public void TextViewCreated( IWpfTextView textView )
		{
			LiveCodingLayerDefinition = new AdornmentLayerDefinition();
		}

		[Export]
		[Name( LiveCodingAdornmentLayers.LiveCodingLayer )]
		[Order( After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text )]
		internal AdornmentLayerDefinition LiveCodingLayerDefinition;
	}
}