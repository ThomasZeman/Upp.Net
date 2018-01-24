using System;

namespace Upp.Net.Platform
{
    public class UdpClient : IUdpSend, IDisposable
    {
        public IpEndpoint LocalIpEndpoint { get; }

        public UdpClient()
        {
            LocalIpEndpoint = new IpEndpoint(IpAddress.AnyAddress, 0);
            BaitAndSwitch.Throw();
        }

        public void Connect(IpEndpoint ipEndpoint)
        {
            BaitAndSwitch.Throw();
        }

        public bool Pool(int microSeconds)
        {
            BaitAndSwitch.Throw();
            return false;
        }

        public int Receive(byte[] buffer, int offset, int count)
        {
            BaitAndSwitch.Throw();
            return 0;
        }

        public void Send(byte[] buffer, int offset, int count)
        {
            BaitAndSwitch.Throw();
        }

        public void Dispose()
        {
            BaitAndSwitch.Throw();
        }
    }
}