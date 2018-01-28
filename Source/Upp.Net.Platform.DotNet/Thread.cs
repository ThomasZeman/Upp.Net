using System;

namespace Upp.Net.Platform
{
    public class Thread
    {
        public static void Start(string name, Action action)
        {
            var thread = new System.Threading.Thread(_ => action()) { Name = name, IsBackground = true };
            thread.Start();
        }
    }
}