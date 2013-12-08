using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class BaseClass
    {
        public event EventHandler E;
        public void Raise()
        {
            var e = E;
            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }
    }

    public class Events : BaseClass
    {
        public void M()
        {
            E += Events_E;
            Raise();
        }

        void Events_E(object sender, EventArgs e)
        {
            int i = 1;
        }
    }
}
