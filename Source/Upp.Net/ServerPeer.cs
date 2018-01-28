using System;
using System.Collections.Generic;
using System.Threading;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public interface IServerPeer
    {
        event Action<ServerPeer, Connection> NewConnection;
        Connection CreateConnection(ServiceTypes serviceType, int connectionId);
    }

    public class ServerPeer : IServerPeer
    {
        private readonly IpEndpoint _ipEndpoint;
        private readonly IUdpSend _udpSend;
        private readonly ITrace _trace;
        private readonly Dictionary<byte, Connection> _connectionDictionary = new Dictionary<byte, Connection>();
        private readonly object _lock = new object();

        internal ServerPeer(IpEndpoint ipEndpoint, IUdpSend udpSend, ITrace trace)
        {
            _ipEndpoint = ipEndpoint;
            _udpSend = udpSend;
            _trace = trace;
        }

        public Connection CreateConnection(ServiceTypes serviceType, int connectionId)
        {
            Monitor.Enter(_lock);
            try
            {
                var connection = ConnectionFactory.Create(serviceType, connectionId, _udpSend, _trace);
                _connectionDictionary.Add(connection.GetByte0(), connection);
                return connection;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        internal void MessageReceived(Paket paket)
        {

            Monitor.Enter(_lock);
            Connection connection;
            if (!_connectionDictionary.TryGetValue(paket.Array[0], out connection))
            {
                try
                {
                    var typeId = (paket.Array[0] >> 3) & 3;
                    var connectionId = (byte)((paket.Array[0] >> 5) & 7);
                    _trace.Info("Unknown connection. Remote: {0} Type: {1} ConnectionId: {2}", _ipEndpoint, (ServiceTypes)typeId, connectionId);
                    connection = ConnectionFactory.Create((ServiceTypes)typeId, connectionId, _udpSend, _trace);
                    _connectionDictionary.Add(paket.Array[0], connection);
                }
                finally 
                {
                    Monitor.Exit(_lock);
                }
                OnNewConnection(this, connection);
            }
            else
            {
                Monitor.Exit(_lock);
            }
            connection.MessageReceived(paket);
        }

        public event Action<ServerPeer, Connection> NewConnection;

        private void OnNewConnection(ServerPeer arg1, Connection arg2)
        {
            NewConnection?.Invoke(arg1, arg2);
        }
    }
}