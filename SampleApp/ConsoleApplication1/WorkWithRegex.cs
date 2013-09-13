using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Loops
    {
        public void ForLoopWithOneLineBody()
        {
            int j = 0;
            for (int i = 0; i < 10; i++)
                j += i;
        }
    }

    class BinarySearch
    {
        public void PerformBinarySearch()
        {
            const int count = 100;

            int[] array = new int[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = i;
                string s = i.ToString();
            }

            int needle = count / 2 - 1;
            int min = 0;
            int max = count;

            while (min <= max)
            {
                int mid = (max + min) / 2;
                int value = array[mid];
                if (value == needle)
                {
                    break;
                }
                else if (value < needle)
                {
                    min = mid + 1;
                }
                else
                {
                    max = mid - 1;
                }
            }
        }

        public void SomeLoop()
        {
            for (int i = 0; i < 1; i++)
            {
                Regex r = new Regex(@"\d");
            }
        }
    }

    class WorkWithRegex
    {
        public void Method()
        {
            object o = null;

            string s = "123";

            Regex r = new Regex(@"(?<g>\d+)\s+");

            var matches = r.Matches("1 2 3 123 ");

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var group = match.Groups["g"];

                var value = group.Value;
            }
        }
    }
}
