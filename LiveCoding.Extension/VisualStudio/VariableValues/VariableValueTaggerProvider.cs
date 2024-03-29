﻿using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace LiveCoding.Extension.VisualStudio.VariableValues
{
	[Export( typeof( IViewTaggerProvider ) )]
	[ContentType( "code" )]
	[TagType( typeof( IntraTextAdornmentTag ) )]
	internal sealed class VariableValueTaggerProvider : IViewTaggerProvider
	{
		static VariableValueTaggerProvider()
		{
			AssemblyResolver.Attach();
		}

		[Import]
		internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService;

		public ITagger<T> CreateTagger<T>( ITextView textView, ITextBuffer buffer ) where T : ITag
		{
			if ( buffer == null )
			{
				throw new ArgumentNullException( "buffer" );
			}

			var tagAggregator = BufferTagAggregatorFactoryService.CreateTagAggregator<VariableValueTag>( textView.TextBuffer );

			ITagger<T> tagger;
			bool found = buffer.Properties.TryGetProperty( typeof( VariableValueTagger ), out tagger );
			if ( tagger == null || !found )
			{
				tagger = new VariableValueTagger( (IWpfTextView)textView, tagAggregator ) as ITagger<T>;
				buffer.Properties[typeof( VariableValueTagger )] = tagger;
			}
			return tagger;
		}
	}
}