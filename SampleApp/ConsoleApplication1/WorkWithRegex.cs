using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class BinarySearch
    {
        public void PerformBinarySearch()
        {
            const int count = 10;

            int[] array = new int[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = i;
                string s = i.ToString();
            }

            int needle = count / 2 - 1;
        }
    }

    class WorkWithRegex
    {
        public void Method()
        {
            object o = null;

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
