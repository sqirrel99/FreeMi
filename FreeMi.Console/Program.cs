using FreeMi.Core;
using FreeMi.Console.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeMi.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Device = new Device();
            Device.Start();
            try
            {
                System.Console.WriteLine(Resources.PressEnterToQuit);
                System.Console.ReadLine();
            }
            finally
            {
                if (Device != null)
                {
                    Device.Stop();
                    Device = null;
                }
            }
        }

        private static Device Device { get; set; }
    }
}
