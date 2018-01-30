using System;

namespace Upp.Net.IntegrationTests
{
    public static class Wait
    {
        public static bool UntilTrue(Func<bool> func)
        {
            for (int i = 0; i < 600; i++)
            {
                if (func())
                {
                    return true;
                }
                System.Threading.Thread.Sleep(10);
            }
            return false;
        }
    }
}