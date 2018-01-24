using System;

namespace Upp.Net.Platform
{
    public static class Timer
    {
        public static IDisposable CreateTimer(Action action, TimeSpan interval)
        {
            BaitAndSwitch.Throw();
            return null;
        }
    }
}