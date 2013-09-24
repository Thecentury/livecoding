using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LiveCoding.Extension.Support;
using LiveCoding.Extension.ViewModels;
using LiveCoding.Extension.Views;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio.ParametrizedMethod
{
	internal sealed class ParametrizedMethodTagger : IntraTextAdornmentTagger<ParametrizedMethodTag, ExecuteMethodControl>
	{
		private readonly IWpfTextView _view;

		private static readonly Regex _commentRegex = new Regex( "//@ (?<call>.+)", RegexOptions.Compiled );

		internal ParametrizedMethodTagger( IWpfTextView view )
			: base( view )
		{
			_view = view;
		}

		protected override ExecuteMethodControl CreateAdornment( ParametrizedMethodTag data, SnapshotSpan span )
		{
			return new ExecuteMethodControl
			{
				DataContext = new MethodExecutionViewModel( new MethodExecutionData( span, MethodExecutionKind.CommonMethod ) { Call = data.Call }, _view )
			};
		}

		protected override bool UpdateAdornment(ExecuteMethodControl adornment, ParametrizedMethodTag data, SnapshotSpan snapshotSpan)
		{
			adornment.DataContext = new MethodExecutionViewModel( new MethodExecutionData( snapshotSpan, MethodExecutionKind.CommonMethod ) { Call = data.Call }, _view );
			return true;
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, ParametrizedMethodTag>> GetAdornmentData( NormalizedSnapshotSpanCollection spans )
		{
			foreach ( SnapshotSpan span in spans )
			{
				var match = _commentRegex.Match( span.GetText() );
				if ( !match.Success )
				{
					continue;
				}

				string call = match.Groups["call"].Value;

				SnapshotSpan snapshotSpan = new SnapshotSpan( span.Start, 0 );
				yield return Tuple.Create( snapshotSpan, (PositionAffinity?)PositionAffinity.Predecessor, new ParametrizedMethodTag( call ) );
			}
		}
	}
}