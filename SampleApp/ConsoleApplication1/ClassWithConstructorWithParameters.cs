using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ClassWithConstructorWithParameters
    {
        private readonly int _i;
        public ClassWithConstructorWithParameters(int i)
        {
            _i = i + 1;
        }

        public void Method()
        {
            int q = _i;
        }
    }
}
