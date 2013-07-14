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
    [TagType( typeof( MethodTag ) )]
    internal sealed class MethodTaggerProvider : ITaggerProvider
    {
	    static MethodTaggerProvider()
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

            return new MethodTagger( AggregatorService.GetClassifier( buffer ) ) as ITagger<T>;
        }
    }
}