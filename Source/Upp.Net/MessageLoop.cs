using System;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public class MessageLoop<TSocket> : IDisposable where TSocket : IDisposable
    {
        private readonly TSocket _socket;
        private readonly ITrace _trace;
        private readonly Action<TSocket> _operation;
        private readonly Action _stopOperation;
        private volatile bool _stop;
        private bool _disposed;
        private bool _started;

        internal static readonly string StartingLoopMessage = "Starting Loop";
        internal static readonly string StoppingLoopMessage = "Stopping Loop";

        public MessageLoop(TSocket socket, ITrace trace, Action<TSocket> operation, Action stopOperation)
        {
            _socket = socket;
            _trace = trace;
            _operation = operation;
            _stopOperation = stopOperation;
        }

        public virtual void Start()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Server");
            }

            if (_started)
            {
                throw new InvalidOperationException("StartReceiving can only be called once");
            }

            _started = true;
            _stop = false;
            _trace.Info("Creating UdpListener");
            Thread.Start("ReceiverThreadServer", Run);
        }

        private void Run()
        {
            _trace.Info(StartingLoopMessage);
            try
            {
                for (; !_stop;)
                {
                    try
                    {
                        _operation(_socket);
                    }
                    catch (UdpSocketException exception)
                    {
                        // TODO: this could be thrown when remote socket is closed. Do something more meaningful.
                        _trace.Info(exception.ToString());
                    }
                    catch (ObjectDisposedException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        // TODO: naeh. at least store last 100 traces and send for analysis
                        _trace.Exception(exception);
                    }
                }
            }
            catch (Exception exception)
            {
                // TODO: naeh. at least store last 100 traces and send for analysis
                _trace.Exception(exception);
            }
            finally
            {
                _trace.Info(StoppingLoopMessage);
                _started = false;
                _socket.Dispose();
            }
        }

        public void Stop()
        {
            _stop = true;
            _stopOperation();
        }

        public void Dispose()
        {
            _stop = true;
            _disposed = true;
            _stopOperation();
        }
    }
}