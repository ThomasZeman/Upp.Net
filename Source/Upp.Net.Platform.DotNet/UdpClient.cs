using System;
using System.Net;
using System.Net.Sockets;

namespace Upp.Net.Platform
{
    public class UdpClient : IUdpSend, IDisposable
    {
        private readonly Socket _socket;
        private IPEndPoint _ipEndpoint;

        public IpEndpoint LocalIpEndpoint { get; private set; }

        public UdpClient()
        {
            _socket = SocketFactory.Create();
        }

        public void Connect(IpEndpoint ipEndpoint)
        {
            _ipEndpoint = Converter.Convert(ipEndpoint);
            _socket.Connect(_ipEndpoint);
#pragma warning disable 618
            var socketLocalEndPoint = (IPEndPoint)_socket.LocalEndPoint;
            LocalIpEndpoint = new IpEndpoint(new IpAddress((uint)socketLocalEndPoint.Address.Address), socketLocalEndPoint.Port);
#pragma warning restore 618
        }

        public bool Pool(int microSeconds)
        {
            return _socket.Poll(microSeconds, SelectMode.SelectRead);
        }

        public int Receive(byte[] buffer, int offset, int count)
        {
            try
            {
                return _socket.Receive(buffer, offset, count, SocketFlags.None);
            }
            catch (SocketException socketException)
            {
                throw new UdpSocketException(socketException.Message, socketException.ErrorCode);
            }
        }

        public void Send(byte[] buffer, int offset, int count)
        {
            try
            {
                _socket.Send(buffer, offset, count, SocketFlags.None);
            }
            catch (SocketException socketException)
            {
                throw new UdpSocketException(socketException.Message, socketException.ErrorCode);
            }
        }

        public void Dispose()
        {
            _socket.Dispose();
        }
    }
}