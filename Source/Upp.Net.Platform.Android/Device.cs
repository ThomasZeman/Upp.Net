using Android.App;
using Android.OS;
using Android.Provider;

namespace Upp.Net.Platform
{
    public static class Device
    {
        public static string GetBluetoothName()
        {
            return Settings.Secure.GetString(Application.Context.ContentResolver, "bluetooth_name");
        }

        public static string GetDeviceModelName()
        {
            return Build.Model;
        }

        public static string GetHostName()
        {
            return Build.Host;
        }
    }
}