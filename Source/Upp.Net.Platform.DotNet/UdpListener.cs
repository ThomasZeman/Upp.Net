using System;
using System.Net;
using System.Net.Sockets;

namespace Upp.Net.Platform
{
    public class UdpListener : IDisposable
    {
        private readonly Socket _socket;
        private readonly EndPoint _anyEndPoint;
        public IpEndpoint LocalIpEndpoint { get; }

        public UdpListener(IpEndpoint ipEndPoint)
        {
            _socket = SocketFactory.Create();
            _socket.Bind(new IPEndPoint(ipEndPoint.IpAddress.Ipv4Address, ipEndPoint.Port));
            _anyEndPoint = new IPEndPoint(IPAddress.Any, 0);
#pragma warning disable 618
            var socketLocalEndPoint = (IPEndPoint)_socket.LocalEndPoint;
            LocalIpEndpoint = new IpEndpoint(new IpAddress((uint)socketLocalEndPoint.Address.Address), socketLocalEndPoint.Port);
#pragma warning restore 618
        }

        public int Receive(byte[] buffer, int offset, int count, out IpEndpoint ipEndpoint)
        {
            EndPoint endpoint = _anyEndPoint;
            int read;
            try
            {
                read = _socket.ReceiveFrom(buffer, offset, count, SocketFlags.None, ref endpoint);
            }
            catch (SocketException socketException)
            {
                // TODO: consider exceptions for bugs only
                // TODO: this fails when remote is closed?!?
                throw new UdpSocketException(socketException.Message, socketException.ErrorCode);
            }
            var ipEndPoint2 = ((IPEndPoint)endpoint);
#pragma warning disable 618
            ipEndpoint = new IpEndpoint(new IpAddress((uint)ipEndPoint2.Address.Address), ipEndPoint2.Port);
#pragma warning restore 618
            return read;
        }

        public IUdpSend ForkSendTo(IpEndpoint ipEndPoint)
        {
            return new UdpSend(_socket, ipEndPoint);
        }

        public void Dispose()
        {
            _socket.Dispose();
        }

        public class UdpSend : IUdpSend
        {
            private readonly Socket _socket;
            private readonly IPEndPoint _ipEndPoint;

            public UdpSend(Socket socket, IpEndpoint ipEndPoint)
            {
                _socket = socket;
                _ipEndPoint = Converter.Convert(ipEndPoint);
            }

            public void Send(byte[] buffer, int offset, int count)
            {
                try
                {                   
                    _socket.SendTo(buffer, offset, count, SocketFlags.None, _ipEndPoint);
                }
                catch (SocketException socketException)
                {                  
                    throw new UdpSocketException(socketException.Message, socketException.ErrorCode);
                }
            }
        }
    }
}
