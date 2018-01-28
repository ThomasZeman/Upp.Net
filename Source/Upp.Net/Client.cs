using System;
using System.Collections.Generic;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public class Client : IDisposable
    {
        private readonly ITrace _trace;
        private readonly UdpClient _udpClient;
        private bool _stop;
        readonly Dictionary<byte, Connection> _connections = new Dictionary<byte, Connection>();
        private readonly MessageLoop<UdpClient> _messageLoop;

        public IpEndpoint ServerEndpoint { get; }

        public Client(IpEndpoint serverEndpoint, ITrace trace)
        {
            _trace = trace;
            ServerEndpoint = serverEndpoint;
            _udpClient = new UdpClient();
            _udpClient.Connect(serverEndpoint);
            _messageLoop = new MessageLoop<UdpClient>(_udpClient, _trace, Operation, StopOperation);
        }

        public void Start()
        {
            _messageLoop.Start();
        }

        private void StopOperation()
        {
        }

        private void Operation(UdpClient obj)
        {
            var dataAvailable = _udpClient.Pool(78 * 1000); // 78ms
            if (!dataAvailable)
            {
                return;
            }
            var paket = new Paket();
            var count = _udpClient.Receive(paket.Array, 0, paket.Array.Length);
            paket.Count = count;
            MessageReceived(paket);
        }

        private void MessageReceived(Paket paket)
        {
            paket.Offset = 7;
            if (paket.Count < 6)
            {
                _trace.Error("Received datagram with invalid count: {0}", new object[] { paket.Count });
                return;
            }
            if ((paket.Array[0] & 7) != Connection.ProtocolVersion)
            {
                return;
            }
            Connection connection;
            if (!_connections.TryGetValue(paket.Array[0], out connection))
            {
                byte connectionId = (byte)((paket.Array[0] >> 5) & 7);
                _trace.Error($"Incoming data for unknown connection id: {connectionId}");
            }
            else
            {
                connection.MessageReceived(paket);
            }
        }

        public Connection CreateConnection(ServiceTypes serviceType, int connectionId)
        {
            var connection = ConnectionFactory.Create(serviceType, connectionId, _udpClient, _trace);
            var byte0 = connection.GetByte0();
            if (_connections.ContainsKey(byte0))
            {
                throw new ArgumentException($"Connection with ServiceType {serviceType} and ConnectionId {connectionId} already exists");
            }
            _connections.Add(byte0, connection);
            return connection;
        }

        protected virtual void Dispose(bool disposing)
        {
            _messageLoop.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
