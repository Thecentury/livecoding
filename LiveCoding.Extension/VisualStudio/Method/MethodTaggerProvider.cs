using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Roslyn.Compilers;

namespace LiveCoding.Extension.VisualStudio
{
	[Export( typeof( IViewTaggerProvider ) )]
	[ContentType( "code" )]
	[TagType( typeof( IntraTextAdornmentTag ) )]
	internal sealed class MethodTaggerProvider : IViewTaggerProvider
	{
		static MethodTaggerProvider()
		{
			AssemblyResolver.Attach();
		}

		[Import]
		internal IClassifierAggregatorService AggregatorService;

		public ITagger<T> CreateTagger<T>( ITextView textView, ITextBuffer buffer ) where T : ITag
		{
			if ( buffer == null )
			{
				throw new ArgumentNullException( "buffer" );
			}

			return buffer.Properties.GetOrCreateSingletonProperty( typeof( MethodTagger ), () => new MethodTagger( AggregatorService.GetClassifier( buffer ), (IWpfTextView)textView ) as ITagger<T> );
		}
	}
}