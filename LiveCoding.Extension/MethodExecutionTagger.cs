using System;
using System.Collections.Generic;
using System.Windows.Controls;
using IntraTextAdornmentSample;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace LiveCoding.Extension
{
    internal sealed class MethodExecutionTagger : IntraTextAdornmentTagger<MethodExecutionTag, Image>
    {
        private readonly IClassifier _classifier;

        public MethodExecutionTagger(IClassifier classifier, IWpfTextView textView)
            : base(textView)
        {
            if (classifier == null)
            {
                throw new ArgumentNullException("classifier");
            }
            _classifier = classifier;
        }

        //public IEnumerable<ITagSpan<MethodExecutionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        //{
        //    foreach (var span in spans)
        //    {
        //        foreach (var classificationSpan in _classifier.GetClassificationSpans(span))
        //        {
        //            string spanText = span.ToString();

        //            if (
        //                classificationSpan.ClassificationType.Classification.ToLower().Contains(ClassificationConstants.Comment) &&
        //                spanText.StartsWith("//@"))
        //            {
        //                yield return new TagSpan<MethodExecutionTag>(new SnapshotSpan(classificationSpan.Span.Start, classificationSpan.Span.Length), new MethodExecutionTag());
        //            }
        //        }
        //    }
        //}

        protected override Image CreateAdornment(MethodExecutionTag data, SnapshotSpan span)
        {
            throw new NotImplementedException();
        }

        protected override bool UpdateAdornment(Image adornment, MethodExecutionTag data)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, MethodExecutionTag>> GetAdornmentData(NormalizedSnapshotSpanCollection spans)
        {
            throw new NotImplementedException();
        }
    }
}