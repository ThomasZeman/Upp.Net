using Upp.Net.Platform;

namespace Upp.Net
{
    public class ServerConfiguration
    {
        public IpEndpoint IpEndpoint { get; }

        public ServerConfiguration(int port)
        {
            IpEndpoint = new IpEndpoint(IpAddress.AnyAddress, port);
        }

        public ServerConfiguration(IpEndpoint ipEndpoint)
        {
            IpEndpoint = ipEndpoint;
        }
    }
}