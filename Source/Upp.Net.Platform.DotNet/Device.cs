using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upp.Net.Platform
{
    // This is not implemented correctly for Windows
    public static class Device
    {
        public static string GetBluetoothName()
        {           
            return Environment.MachineName;
        }

        public static string GetDeviceModelName()
        {
            return "GenericDotNet";
        }

        public static string GetHostName()
        {
            return Environment.MachineName;
        }
    }
}
