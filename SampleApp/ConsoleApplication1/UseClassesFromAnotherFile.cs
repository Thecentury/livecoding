using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class UseClassesFromAnotherFile
    {
        public UseClassesFromAnotherFile()
        {
            var m = new Methods();
            m.M();
        }
    }
}
