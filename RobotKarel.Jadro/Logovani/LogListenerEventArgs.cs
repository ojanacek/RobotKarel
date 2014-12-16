using System;

namespace RobotKarel.Jadro.Logovani
{
    public class LogListenerEventArgs
    {
        public string Message { get; private set; }

        public LogListenerEventArgs(string message)
        {
            Message = message;
        }
    }
}
