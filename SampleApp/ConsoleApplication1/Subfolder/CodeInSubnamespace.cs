using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Subfolder
{
    class CodeInSubnamespace
    {
        public CodeInSubnamespace()
        {
            var e = new Events();
            e.Raise();
        }
    }
}
