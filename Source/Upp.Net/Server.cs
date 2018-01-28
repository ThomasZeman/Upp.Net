using System;
using System.Collections.Generic;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public interface IServer : IDisposable
    {
        void StartReceiving();

        void StopReceiving();
    }

    public sealed class Server : IServer
    {
        private readonly ITrace _trace;
        private readonly Dictionary<IpEndpoint, ServerPeer> _serverPeers = new Dictionary<IpEndpoint, ServerPeer>();
        private readonly ListenerBase _listenerBase;

        public Server(ServerConfiguration serverConfiguration, ITrace trace)
        {
            _trace = trace;
            _listenerBase = new ListenerBase(serverConfiguration.IpEndpoint, trace);
            _listenerBase.MessageReceived += MessageReceived;
        }

        public void StartReceiving()
        {
            _listenerBase.Start();
        }

        public void StopReceiving()
        {
            _listenerBase.Stop();
            _serverPeers.Clear();
        }

        private void MessageReceived(Paket paket, IpEndpoint ipEndpoint)
        {
            if (paket.Count < 2 || paket.Count > 1024)
            {
                _trace.Error("Received datagram with invalid count: {0}", paket.Count);
                return;
            }
            var protocolVersion = paket.Array[0] & 7;
            if (protocolVersion != Connection.ProtocolVersion)
            {
                _trace.Error("Discarding Message with different protocol version. Was: {0} Expected: {1}", protocolVersion, Connection.ProtocolVersion);
                return;
            }
            ServerPeer serverPeer;
            if (!_serverPeers.TryGetValue(ipEndpoint, out serverPeer))
            {
                serverPeer = new ServerPeer(ipEndpoint, _listenerBase.Client.ForkSendTo(ipEndpoint), _trace);
                _serverPeers.Add(ipEndpoint, serverPeer);
                OnNewServerPeer(this, serverPeer);
            }
            serverPeer.MessageReceived(paket);
        }

        public event Action<Server, IServerPeer> NewServerPeer;

        private void OnNewServerPeer(Server arg1, IServerPeer arg2)
        {
            NewServerPeer?.Invoke(arg1, arg2);
        }

        public void Dispose()
        {
            _listenerBase?.Dispose();
        }
    }
}