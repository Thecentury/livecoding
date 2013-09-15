using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Ifs
    {
        public void IfInsideOfLoop()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                if (rnd.NextDouble() > 0.5)
                {
                    bool f = true;
                }
            }
        }
        
        public void If1()
        {
            int i = 0;

            Random rnd = new Random();
            if (rnd.NextDouble() > 0.5)
            { 
            }

            if (false)
            {
            }
        }
    }
}
