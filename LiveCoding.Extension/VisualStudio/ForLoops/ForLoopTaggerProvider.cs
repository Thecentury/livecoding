using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace LiveCoding.Extension.VisualStudio.ForLoops
{
	[Export( typeof( IViewTaggerProvider ) )]
	[ContentType( "code" )]
	[TagType( typeof( IntraTextAdornmentTag ) )]
	internal sealed class ForLoopTaggerProvider : IViewTaggerProvider
	{
		static ForLoopTaggerProvider()
		{
			AssemblyResolver.Attach();
		}

		public ITagger<T> CreateTagger<T>( ITextView textView, ITextBuffer buffer ) where T : ITag
		{
			var tagger = (ITagger<T>)(object)buffer.Properties.GetOrCreateSingletonProperty( () => new ForLoopTagger( (IWpfTextView)textView ) );
			return tagger;
		}
	}
}