using System;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public sealed class ListenerBase : IDisposable
    {
        // This could be redesigned in such a way that instead of running its own thread it allows
        // perodic calling of a method which does the work and returns. To achieve highest performance
        // usage of async/await (construction of Tasks, capturing SyncContext ...) is currently not
        // considered and concret system threads are used.

        private readonly IpEndpoint _ipEndPoint;
        private readonly ITrace _trace;
        private readonly UdpListener _listener;

        public UdpListener Listener => _listener;
        private readonly MessageLoop<UdpListener> _messageLoop;
        private bool _stopped = false;

        /// <summary>
        /// Receives incoming data in a loop running in its own thread.
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="trace"></param>
        /// <exception cref="UdpSocketException">Thrown when socket cannot be bound to endpoint</exception>
        public ListenerBase(IpEndpoint ipEndPoint, ITrace trace)
        {
            _ipEndPoint = ipEndPoint;
            _trace = trace;
            _listener = new UdpListener(ipEndPoint);
            _messageLoop = new MessageLoop<UdpListener>(_listener, trace, Operation, StopOperation);
        }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (Disposed || _listener == null)
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
            _stopped = true;
            _messageLoop.Stop();
        }

        private void StopOperation()
        {
            using (var client = new UdpClient())
            {
                var endpoint = _ipEndPoint.IpAddress.Equals(IpAddress.AnyAddress)
                    ? new IpEndpoint(IpAddress.LoopbackAddress, _listener.LocalIpEndpoint.Port)
                    : _ipEndPoint;
                _trace.Info("StopOperation with endpoint: {0}", endpoint);
                client.Connect(endpoint);
                client.Send(new byte[1], 0, 1); // Sending 0/0/0 works on Windows but not on Linux
            }
        }

        private void Operation(UdpListener obj)
        {
            var paket = new Paket();
            var count = _listener.Receive(paket.Array, 0, paket.Array.Length, out var ipEndpoint);
            if (_stopped)
            {
                return;
            }

            paket.Count = count;
            MessageReceived?.Invoke(paket, ipEndpoint);
        }

        public event Action<Paket, IpEndpoint> MessageReceived;
    }
}