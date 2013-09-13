using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;

namespace LiveCoding.Extension.VisualStudio.If
{
	internal sealed class BooleanConditionTag : IGlyphTag
	{
		public bool Value { get; private set; }

		public BooleanConditionTag( bool value )
		{
			Value = value;
		}
	}
}
