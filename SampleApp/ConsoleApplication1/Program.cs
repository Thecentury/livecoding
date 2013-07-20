using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args == null)
            {
                Debugger.Break();
            }
        }

        public static void Q()
        {
            int i = 1;
            var rnd = new Random();
            if (rnd.NextDouble() < 2)
            {
                i += 3;
                i++;
                ++i;
            }

            MessageBox.Show("123");
        }

        public static void SlowMethodWithLoop()
        {
            int i = 0;

            for (int j = 0; j < 10; j++)
            {
                i++;

                Thread.Sleep(1000);
            }
        }

        private static void SlowMethod()
        {
            int i = 0;

            Thread.Sleep(1000);

            i++;

            Thread.Sleep(1000);

            i++;

            Thread.Sleep(1000);

            i++;

            Console.WriteLine(i);
        }
    }
}
