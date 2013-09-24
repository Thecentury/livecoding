using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LiveCoding.Extension.Support;
using LiveCoding.Extension.ViewModels;
using LiveCoding.Extension.Views;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio.NunitTestCase
{
	internal sealed class NunitTestCaseTagger : IntraTextAdornmentTagger<NunitTestCaseTag, ExecuteMethodControl>
	{
		private static readonly Regex _regex = new Regex( @"\[\s*TestCase\s*\((?<arguments>.+)\)\s*\]", RegexOptions.Compiled );

		public NunitTestCaseTagger( IWpfTextView view )
			: base( view )
		{
		}

		protected override ExecuteMethodControl CreateAdornment( NunitTestCaseTag data, SnapshotSpan span )
		{
			return new ExecuteMethodControl
			{
				DataContext = new MethodExecutionViewModel( new MethodExecutionData( span, MethodExecutionKind.TestCase ) { Call = data.Arguments }, View )
			};
		}

		protected override bool UpdateAdornment( ExecuteMethodControl adornment, NunitTestCaseTag data, SnapshotSpan snapshotSpan )
		{
			// todo brinchuk update
			return true;
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, NunitTestCaseTag>> GetAdornmentData( NormalizedSnapshotSpanCollection spans )
		{
			foreach ( var span in spans )
			{
				Match match = _regex.Match( span.GetText() );
				if ( !match.Success )
				{
					continue;
				}

				string arguments = match.Groups["arguments"].Value;
				SnapshotSpan snapshotSpan = new SnapshotSpan( span.Start, 0 );

				yield return Tuple.Create( snapshotSpan, (PositionAffinity?)PositionAffinity.Predecessor, new NunitTestCaseTag { Arguments = arguments } );
			}
		}
	}
}