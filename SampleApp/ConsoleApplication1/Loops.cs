using System;
using System.Linq;
namespace ConsoleApplication1
{
    class Loops
    {
        public void ForLoopWithOneLineBody()
        {
            int j = 1;
            for (int i = 1; i < 10; i++)
                j += i;
        }

        public void ForWithIf()
        {
            int j = 0;
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    j += i;
                }
                else
                {
                    j *= i;
                }
            }
        }

        public void ForeachLoop()
        {
            foreach (var item in Enumerable.Range(3, 5))
            {
                Console.WriteLine(item);
            }
        }

        public void While()
        {
            int i = 0;
            while (i < 10)
            {
                i++;
                Console.WriteLine(i + "!");
            }
        }

        public void DoWhile()
        {
            int i = 0;
            do
            {
                i++;
            } while (i < 10);
        }
    }
}