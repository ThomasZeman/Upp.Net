using System;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public sealed class ListenerBase : IDisposable
    {
        // This could be redesigned in such a way that instead of running its own thread it allows
        // perodic calling of a method which does the work and returns

        private readonly IpEndpoint _ipEndPoint;
        private readonly UdpListener _client;

        public UdpListener Client => _client;
        private readonly MessageLoop<UdpListener> _messageLoop;

        /// <summary>
        /// Receives incoming data in a loop running in its own thread.
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="trace"></param>
        /// <exception cref="UdpSocketException">Thrown when socket cannot be bound to endpoint</exception>
        public ListenerBase(IpEndpoint ipEndPoint, ITrace trace)
        {
            _ipEndPoint = ipEndPoint;
            _client = new UdpListener(ipEndPoint);
            _messageLoop = new MessageLoop<UdpListener>(_client, trace, Operation, StopOperation);
        }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (Disposed || _client == null)
            {
                return;
            }
            _messageLoop.Dispose();
            Disposed = true;
        }

        public void Start()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException("Server");
            }
            _messageLoop.Start();
        }

        public void Stop()
        {
            _messageLoop.Stop();
        }

        private void StopOperation()
        {
            using (var client = new UdpClient())
            {
                IpEndpoint endpoint;
                if (_ipEndPoint.IpAddress.Equals(IpAddress.AnyAddress))
                {
                    endpoint = new IpEndpoint(IpAddress.LoopbackAddress, _client.LocalIpEndpoint.Port);
                }
                else
                {
                    endpoint = _ipEndPoint;
                }
                client.Connect(endpoint);
                client.Send(new byte[0], 0, 0);
            }
        }

        private void Operation(UdpListener obj)
        {
            var paket = new Paket();
            IpEndpoint ipEndpoint;
            var count = _client.Receive(paket.Array, 0, paket.Array.Length, out ipEndpoint);
            if (count == 0)
            {
                return;
            }
            paket.Count = count;
            MessageReceived?.Invoke(paket, ipEndpoint);
        }

        public event Action<Paket,IpEndpoint> MessageReceived;
    }
}