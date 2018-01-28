using System.Net.Sockets;

namespace Upp.Net.Platform
{
    public static class SocketFactory
    {
        public static Socket Create()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true
            };
            return socket;
        }
    }
}