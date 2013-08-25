using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestFixture]
    public class TestWithTestCases
    {
        [TestCase(1)]
        [TestCase(2)]
        public void Method(int i)
        {
            int j = i;
        }
    }
}
