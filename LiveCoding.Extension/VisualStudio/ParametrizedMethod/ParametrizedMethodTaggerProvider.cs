using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace LiveCoding.Extension.VisualStudio.ParametrizedMethod
{
	[Export( typeof( IViewTaggerProvider ) )]
	[ContentType( "code" )]
	[TagType( typeof( IntraTextAdornmentTag ) )]
	internal sealed class ParametrizedMethodTaggerProvider : IViewTaggerProvider
	{
		[Import]
		internal IClassifierAggregatorService AggregatorService;

		public ITagger<T> CreateTagger<T>( ITextView textView, ITextBuffer buffer ) where T : ITag
		{
			return buffer.Properties.GetOrCreateSingletonProperty( () => new ParametrizedMethodTagger((IWpfTextView)textView ) ) as ITagger<T>;
		}
	}
}