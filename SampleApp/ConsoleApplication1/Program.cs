﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    class Program
    {
        public static void Main(string[] args)
        {
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
    }
}
