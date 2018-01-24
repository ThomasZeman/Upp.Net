using System;

namespace Upp.Net.Platform
{
    public static class Thread
    {
        public static void Start(string name, Action action)
        {
            BaitAndSwitch.Throw();
        }
    }
}