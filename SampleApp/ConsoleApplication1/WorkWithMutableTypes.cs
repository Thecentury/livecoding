using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class WorkWithMutableTypes
    {
        public void StringBuilder()
        {
            StringBuilder builder1 = null;

            StringBuilder builder2 = new StringBuilder();

            builder2.Append("1");

            builder2.Append("2");

            builder2.Append("3");
        }

        public void List()
        {
            List<int> list = new List<int>();

            list.Add(1);
            list.Add(2);
            list.Add(3);
        }
    }
}
