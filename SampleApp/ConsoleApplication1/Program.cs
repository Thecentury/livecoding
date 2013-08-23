using NLog;
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
    internal class Program
    {
        private readonly int _field;

        public void CommunicateWithClassFromExternalReferencedAssembly()
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Debug("message");
        }

        //@ Program.Main("123");
        public static void Main(params string[] args)
        {
            if (args != null && args.Length > 0)
            {
                MessageBox.Show(args[0]);
            }
        }

        public static void WithException()
        {
            //in i;
            try
            {
                int i = 1;

                throw new Exception();

                int j = 1;
            }
            catch (Exception exc)
            {
                string s = exc.StackTrace;
                //throw;
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

            //MessageBox.Show("123");
        }

        public static void ForLoop()
        {
            for (int i = 0; i < 10; i++)
            {

            }
        }

        public static void ForeachLoop()
        {
            int j = 1;
            for (int i = 0; i < 10; i++)
            {
                j++;
            }

            int[] array = new[] { 1, 2, 3, 4 };
            foreach (var item in array)
            {
                Console.WriteLine(item);
            }
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

        public static void LoopWithStrings()
        {
            string s = null;

            for (int i = 0; i < 10; i++)
            {
                s += i + " ";
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
