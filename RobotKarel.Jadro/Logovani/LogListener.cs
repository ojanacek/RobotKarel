using System;
using System.Diagnostics;

namespace RobotKarel.Jadro.Logovani
{
    public class LogListener : TextWriterTraceListener
    {
        public delegate void LogListenerHandler(object sender, LogListenerEventArgs args);
        public event LogListenerHandler OnWriteLine;

        public override void WriteLine(string message)
        {
            var args = new LogListenerEventArgs(message);

            if (OnWriteLine != null)
                OnWriteLine(this, args);
        }
    }
}
