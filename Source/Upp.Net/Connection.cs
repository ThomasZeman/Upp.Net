using System;
using System.Threading;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public abstract class Connection
    {
        private readonly ServiceTypes _serviceType;
        private static int _uidCounter;
        public event Action<Connection, Paket> NewPaket;

        protected ITrace Trace { get; }

        public IUdpSend Channel { get; }

        public static byte ProtocolVersion => 1;

        public byte ConnectionId { get; }

        protected Connection(IUdpSend channel, byte connectionId, ServiceTypes serviceType, ITrace trace)
        {
            Channel = channel;
            Trace = trace;
            _serviceType = serviceType;
            if (connectionId > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(connectionId), "connectionId must be less than 8");
            }
            ConnectionId = connectionId;
        }

        public abstract void MessageReceived(Paket paket);

        public abstract bool Send(Paket paket);

        public abstract Paket CreatePaket();

        protected void OnNewPaket(Connection arg1, Paket arg2)
        {
            NewPaket?.Invoke(arg1, arg2);
        }

        public uint GetUid()
        {
            return (uint)Interlocked.Increment(ref _uidCounter);
        }

        public byte GetByte0()
        {
            return (byte)(ProtocolVersion | ((byte)(_serviceType) << 3) | (ConnectionId << 5));
        }
    }
}