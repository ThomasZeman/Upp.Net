using System;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public interface IBroadcastClient: IDisposable
    {
        void StartReceiving();

        void StopReceiving();

        UnreliableUnorderedConnection Connection { get; }

        IpEndpoint CurrentEndpoint { get; }
    }

    public sealed class BroadcastClient: IBroadcastClient 
    {
        private readonly ListenerBase _listenerBase;

        public BroadcastClient(IpEndpoint localEndpoint, int targetPort, byte connectionId, ITrace trace)
        {
            _listenerBase = new ListenerBase(localEndpoint, trace);
            _listenerBase.MessageReceived += MessageReceived;
            var broadcastClient = _listenerBase.Listener.ForkSendTo(new IpEndpoint(new IpAddress("255.255.255.255"), targetPort));
            Connection = new UnreliableUnorderedConnection(broadcastClient, connectionId, trace);
        }

        public void StartReceiving()
        {
            _listenerBase.Start();      
        }

        public void StopReceiving()
        {
            _listenerBase.Stop();
        }

        private void MessageReceived(Paket paket, IpEndpoint ipEndpoint)
        {
            CurrentEndpoint = ipEndpoint;
            Connection.MessageReceived(paket);
        }

        public void Dispose()
        {
            _listenerBase?.Dispose();
        }

        public UnreliableUnorderedConnection Connection { get; }
        public IpEndpoint CurrentEndpoint { get; private set; }
    }
}