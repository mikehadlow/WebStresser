using System;

namespace TestTimerService
{
    public class Program
    {
        static readonly Uri baseAddress = new Uri("http://localhost:8123/hello");

        public static void Main()
        {
            ConsoleServiceHost.Start<ITimerService, TimerService>(baseAddress);
        }
    }
}