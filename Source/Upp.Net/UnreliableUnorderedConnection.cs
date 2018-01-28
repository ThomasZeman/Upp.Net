using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public sealed class UnreliableUnorderedConnection : Connection
    {
        public UnreliableUnorderedConnection(IUdpSend channel, byte connectionId, ITrace trace) : base(channel, connectionId, ServiceTypes.UnreliableUnordered, trace)
        {
        }


        public override void MessageReceived(Paket paket)
        {
            paket.SeqId = 0;
            paket.Offset = 1;
            OnNewPaket(this, paket);
        }

        public override bool Send(Paket paket)
        {
            // TODO: So geht das nicht. Send kann werfen aber hier gehts mit bool weiter.
            // Bugs -> weiterwerfen / SocketExceptions entsprechend Railroad
            Channel.Send(paket.Array, 0, paket.Count);
            return true;
        }

        public override Paket CreatePaket()
        {
            var paket = new Paket
            {
                Offset = 1,
                Count = 1,
                Array = { [0] = GetByte0() }
            };
            return paket;
        }
    }
}