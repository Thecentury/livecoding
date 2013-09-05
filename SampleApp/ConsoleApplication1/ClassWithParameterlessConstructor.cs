using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class ClassWithParameterlessConstructor
    {
        public ClassWithParameterlessConstructor()
        {
        }

        [NotNull]
        public object AnnotatedMethod()
        {
            return new object();
        }

        private void Method()
        {
            int q = 1;
        }
    }
}
