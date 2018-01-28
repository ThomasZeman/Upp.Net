using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public sealed class UnreliableOrderedConnection : Connection
    {
        private int _sequenceCounter;
        private int _highestSequenceIdReceived;
        private object _lock = new object();

        public UnreliableOrderedConnection(IUdpSend channel, byte connectionId, ITrace trace) : base(channel, connectionId, ServiceTypes.UnreliableOrdered, trace)
        {
        }

        public override void MessageReceived(Paket paket)
        {
            var receivedSequenceId = (ushort)(paket.Array[1] + (paket.Array[2] << 8));
            paket.SeqId = receivedSequenceId;
            ushort delta = (ushort)(65536 + receivedSequenceId - _highestSequenceIdReceived);
            if ((delta & 32768) != 0) // means "delta > 32768" which means negative value
            {
                return;
            }
            _highestSequenceIdReceived = receivedSequenceId;
            paket.Offset = 3;
            OnNewPaket(this, paket);
        }

        public override bool Send(Paket paket)
        {
            lock (_lock)
            {
                paket.Array[1] = (byte)(_sequenceCounter & 255);
                paket.Array[2] = (byte)(_sequenceCounter >> 8);
                _sequenceCounter++;
                Channel.Send(paket.Array, 0, paket.Count);
            }
            return true;
        }

        public override Paket CreatePaket()
        {
            var paket = new Paket
            {
                Offset = 3,
                Count = 3,
                Array = { [0] = GetByte0() }
            };
            return paket;
        }
    }
}