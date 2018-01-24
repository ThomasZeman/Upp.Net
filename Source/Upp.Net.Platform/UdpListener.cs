using System;

namespace Upp.Net.Platform
{
    public class UdpListener : IDisposable
    {
        public IpEndpoint LocalIpEndpoint { get; }

        public UdpListener(IpEndpoint ipEndPoint)
        {
            LocalIpEndpoint = new IpEndpoint(IpAddress.AnyAddress, ipEndPoint.Port);
            BaitAndSwitch.Throw();
        }

        public int Receive(byte[] buffer, int offset, int count, out IpEndpoint ipEndpoint)
        {
            BaitAndSwitch.Throw();
            ipEndpoint = null;
            return 0;
        }

        public IUdpSend ForkSendTo(IpEndpoint ipEndPoint)
        {
            BaitAndSwitch.Throw();
            return null;
        }

        public void Dispose()
        {
            BaitAndSwitch.Throw();
        }
    }
}
