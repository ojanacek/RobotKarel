using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotKarel.Jadro.Android;
using RobotKarel.Jadro.Logovani;
using System.Diagnostics;

namespace RobotKarel.UdpMonoKlient
{
    class Program
    {
        static void Main(string[] args)
        {
            LogListener listener = new LogListener();
            Trace.Listeners.Add(listener);
            listener.OnWriteLine += new LogListener.LogListenerHandler(listener_OnWriteLine);

            Fotografie fotografie = new Fotografie();

            fotografie.StahnoutFotografii();

            Console.ReadLine();
        }

        static void listener_OnWriteLine(object sender, LogListenerEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
    }
}
