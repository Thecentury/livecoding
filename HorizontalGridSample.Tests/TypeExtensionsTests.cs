using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HorizontalGridSample.Tests
{
	[TestFixture]
	public sealed class TypeExtensionsTests
	{
		[TestCase( typeof( int[] ), Result = true )]
		[TestCase( typeof( List<int> ), Result = true )]
		public bool IsCollection( Type type )
		{
			return type.IsCollection();
		}
	}
}
