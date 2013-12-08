using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Methods
    {
        public void M()
        {
            Console.WriteLine(new Random().NextDouble());

            Console.WriteLine("{0} {1}", Proxy.Pass(1 + 1), Proxy.Pass(true == false));

            int i = Proxy.Pass(1 + 2); Proxy.Pass(1 + 1); 
            Proxy.Pass(2 + "3");

            ClassWithProperty<int>.P = Proxy.Pass(1 + 2);
        }
    }

    static class ClassWithProperty<T>
    {
        public static T P
        {
            get { return default(T); }
            set { }
        }
    }

    static class Proxy
    {
        public static T Pass<T>(T value)
        {
            return value;
        }
    }
}
