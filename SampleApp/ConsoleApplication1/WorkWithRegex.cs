using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class WorkWithRegex
    {
        public void Method()
        {
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
