using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace LiveCoding.Extension.VisualStudio
{
	[Export( typeof( ITaggerProvider ) )]
	[ContentType( "code" )]
	[TagType( typeof( VariableValueTag ) )]
	internal sealed class VariableValueTaggerProvider : ITaggerProvider
	{
		static VariableValueTaggerProvider()
		{
			AssemblyResolver.Attach();
		}

		[Import]
		internal IClassifierAggregatorService AggregatorService;

		public ITagger<T> CreateTagger<T>( ITextBuffer buffer ) where T : ITag
		{
			if ( buffer == null )
			{
				throw new ArgumentNullException( "buffer" );
			}

			var tagger = buffer.Properties.GetOrCreateSingletonProperty( typeof( VariableValueTagger ), () => new VariableValueTagger( AggregatorService.GetClassifier( buffer ) ) as ITagger<T> );
			return tagger;
		}
	}
}