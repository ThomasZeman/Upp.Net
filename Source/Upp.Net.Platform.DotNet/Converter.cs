using System.Net;

namespace Upp.Net.Platform
{
    public static class Converter
    {
        public static IPEndPoint Convert(IpEndpoint ipEndpoint)
        {
            return new IPEndPoint(ipEndpoint.IpAddress.Ipv4Address, ipEndpoint.Port);
        }
    }
}