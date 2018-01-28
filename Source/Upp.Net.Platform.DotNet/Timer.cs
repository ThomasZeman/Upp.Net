using System;

namespace Upp.Net.Platform
{
    public class Timer
    {
        public static IDisposable CreateTimer(Action action, TimeSpan interval)
        {
            var foo = new LockedAction(action);
            var timer = new System.Threading.Timer(foo.Callback, null, TimeSpan.Zero, interval);
            foo.Timer = timer;
            return foo;
        }

        private class LockedAction : IDisposable
        {
            private readonly Action _action;
            private readonly object _lock = new object();
            public System.Threading.Timer Timer { get; set; }

            public LockedAction(Action action)
            {
                _action = action;
            }

            public void Callback(object state)
            {               
                lock (_lock)
                {
                    _action();                    
                }
            }

            public void Dispose()
            {
                Timer.Dispose();
            }
        }
    }
}