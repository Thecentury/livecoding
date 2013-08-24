using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ClassFromTheSameFile
    {
        public int I;

        public string S;
    }

    class ClassWithConstructorWithParameters
    {
        private readonly int _i;
        public ClassWithConstructorWithParameters(int i)
        {
            _i = i + 1;

            string q = Environment.CurrentDirectory;
        }

        public void Method()
        {
            int q = _i;

            ClassFromTheSameFile c = new ClassFromTheSameFile();
        }
    }
}
