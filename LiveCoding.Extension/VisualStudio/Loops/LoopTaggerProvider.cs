using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace LiveCoding.Extension.VisualStudio.Loops
{
	[Export( typeof( IViewTaggerProvider ) )]
	[ContentType( "code" )]
	[TagType( typeof( IntraTextAdornmentTag ) )]
	internal sealed class LoopTaggerProvider : IViewTaggerProvider
	{
		static LoopTaggerProvider()
		{
			AssemblyResolver.Attach();
		}

		public ITagger<T> CreateTagger<T>( ITextView textView, ITextBuffer buffer ) where T : ITag
		{
			var tagger = (ITagger<T>)(object)buffer.Properties.GetOrCreateSingletonProperty( () => new LoopTagger( (IWpfTextView)textView ) );
			return tagger;
		}
	}
}