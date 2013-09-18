using System;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LiveCoding.Core;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio.Invocations
{
	internal sealed class InvocationTag : IGlyphTag
	{
		private readonly InvocationEvent _invocation;

		public InvocationTag( [NotNull] InvocationEvent invocation )
		{
			if ( invocation == null )
			{
				throw new ArgumentNullException( "invocation" );
			}
			_invocation = invocation;
		}

		public InvocationEvent Invocation
		{
			get { return _invocation; }
		}
	}
}
