using System.Collections.Generic;
using System.Threading;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public sealed class ReliableOrderedConnection : Connection
    {
        // What about connection established and lost detection. EchoRequest/Reply part of this here? If analogy to TCP/IP then: no
        // Batching is missing. Here or higher layer? All connections could benefit from that

        // This protocol is most likely not ideal for high throughput, low latency scenarios because its "send window" is only 16 pakets wide. Not enough
        // for high throughput concerning the round trip time for the necessary paket confirmations.
        // It might be a better approach for paket loss > 1% scenarios and lower paket send rates

        // 0 protocolId [ConnectionNo2 . ConnectionNo1 . ConnectioNo0  . TypeConnection1 . TypeConnection0 . Version2 . Version1 . Version0 ]
        // 1 bitfield [ 1 = Empty, 2 = RequireAckImmediately ]
        // 2 seqid 0 
        // 3 seqid 1
        // 4 ackbase 0
        // 5 ackbase 1
        // 6 ackfield 0
        // 7 ackfield 1

        public static readonly byte Empty = 1;
        public static readonly byte RequireAck = 2;
        private readonly AcknowledgeSendBuffer<Paket> _acknowledgeSendBuffer;
        private readonly object _lock = new object();
        private readonly OrderedAcknowledgePlayoutBuffer<Paket> _receivedOrderedAcknowledgePlayoutBuffers;
        private readonly List<Paket> _unconfirmed = new List<Paket>(16);

        private AckResult _lastAckResult = AckResult.Unknown;
        private int _messageReceivedCounter;

        public ReliableOrderedConnection(IUdpSend channel, byte connectionId, ITrace trace)
            : base(channel, connectionId, ServiceTypes.ReliableOrdered, trace)
        {
            _receivedOrderedAcknowledgePlayoutBuffers = new OrderedAcknowledgePlayoutBuffer<Paket>(PlayOut, trace);
            _acknowledgeSendBuffer = new AcknowledgeSendBuffer<Paket>(trace);
        }

        private void PlayOut(Paket obj)
        {
            OnNewPaket(this, obj);
        }

        public override void MessageReceived(Paket paket)
        {
            var sequenceId = (ushort)(paket.Array[2] + (paket.Array[3] << 8));
            paket.SeqId = sequenceId;
#if UID
            paket.Offset = 8;
            paket.Uid = (uint) SimpleTypeReader.ReadInt(paket);
#else
            paket.Offset = 8;
#endif
            // TODO: playoutBuffer calls Playout into user code while having this lock. High deadlock danger when user calls back into send while the lock is held here
            lock (_lock)
            {
                Trace.Debug(">>> Received: {0}", paket);
                var sendAck = (paket.Array[1] & RequireAck) != 0;
                if ((paket.Array[1] & Empty) == 0)
                {
                    _messageReceivedCounter++;
                    var ackResult = _receivedOrderedAcknowledgePlayoutBuffers.Add(paket.SeqId, paket);
                    // not sure whether lateAck is a reason to send acks. Following scenario:
                    // 001
                    // 101 early ack
                    // 111 late ack
                    var ack = ackResult == AckResult.AlreadyAcked || ackResult == AckResult.EarlyAck || ackResult == AckResult.OutOfWindow;
                    if (_messageReceivedCounter++ == 4)
                    {
                        _messageReceivedCounter = 0;
                        _lastAckResult = AckResult.Unknown;
                    }
                    // Avoid flooding with empty packages. This could happen when for example a paket is missing but pakets
                    // keep on arriving they all would come back here with "EarlyAck". However after 4 pakets we reset.
                    if (ackResult != _lastAckResult)
                    {
                        sendAck |= ack;
                    }
                    _lastAckResult = ackResult;
                    if (sendAck)
                    {
                        Trace.Debug("SendingAck because AckResult: {0}", ackResult);
                    }
                }
                else
                {
                    Trace.Debug("Received empty paket. Not adding to acks");
                }
                if (ResendUnconfirmed(paket))
                {
                    sendAck = false;
                }
                if (sendAck)
                {
                    SendEmptyPaket(Empty);
                }
                Trace.Debug("<<< Received: {0}", paket);
                Monitor.Pulse(_lock);
            }
        }

        private void SendEmptyPaket(byte byte1)
        {
            lock (_lock)
            {
                var ackOnlyPaket = CreatePaket();
                ackOnlyPaket.Array[1] = byte1;
                ackOnlyPaket.Array[2] = 0;
                ackOnlyPaket.Array[3] = 0;
                Trace.Debug("Sending empty paket byte1: {0}", byte1);
                Channel.Send(ackOnlyPaket.Array, 0, ackOnlyPaket.Count);
            }
        }

        private bool ResendUnconfirmed(Paket paket)
        {
            var ackSent = true;
            _acknowledgeSendBuffer.Ack(GetReceivedAckField(paket), GetReceivedAckBase(paket));
            _unconfirmed.Clear();
            _acknowledgeSendBuffer.GetAllUnconfirmed(_unconfirmed);
            if (_unconfirmed.Count == 0)
            {
                ackSent = false;
            }
            for (var i = 0; i < _unconfirmed.Count; i++)
            {
                SetAcksInPaket(_unconfirmed[i]);
#if UID
                SetUid(_unconfirmed[i]);
#endif
                Trace.Debug("Resending paket: {0}", _unconfirmed[i]);
                Channel.Send(_unconfirmed[i].Array, 0, _unconfirmed[i].Count);
            }
            return ackSent;
        }

        public override bool Send(Paket paket)
        {
            lock (_lock)
            {
                int delta;
                int result;
                result = _acknowledgeSendBuffer.Add(paket, out delta);
                if (result == -1)
                {
                    // No space left in ack buffer. Remote side still there? We are not waiting, we are not buffering. Leave it to the user to decide what to do.
                    return false;
                }
                paket.SeqId = (ushort)result;
                if (delta == 3 || delta == 7 || delta == 11 || delta == 15)
                {
                    paket.Array[1] = RequireAck;
                }
                paket.Array[2] = (byte)(result & 255);
                paket.Array[3] = (byte)(result >> 8);
                Trace.Debug("Sending paket: {0}", paket);
                Channel.Send(paket.Array, 0, paket.Count);
                return true;
            }
        }

        private void SetAcksInPaket(Paket paket)
        {
            lock (_lock)
            {
                paket.Array[4] = (byte)(_receivedOrderedAcknowledgePlayoutBuffers.AckBase & 255);
                paket.Array[5] = (byte)(_receivedOrderedAcknowledgePlayoutBuffers.AckBase >> 8);

                paket.Array[6] = (byte)(_receivedOrderedAcknowledgePlayoutBuffers.AckField & 255);
                paket.Array[7] = (byte)(_receivedOrderedAcknowledgePlayoutBuffers.AckField >> 8);
            }
        }

        public override Paket CreatePaket()
        {
            var paket = new Paket
            {
                Offset = 8,
                Count = 8,
                Array = { [0] = GetByte0() }
            };
#if UID
            SetUid(paket);
#endif
            SetAcksInPaket(paket);
            return paket;
        }

#if UID
        private void SetUid(Paket paket)
        {
            paket.Offset = 8;
            paket.Uid = GetUid();
            SimpleTypeWriter.Write((int) paket.Uid, paket);
        }
#endif

        private static ushort GetReceivedAckBase(Paket paket)
        {
            return (ushort)(paket.Array[4] + (paket.Array[5] << 8));
        }

        private static ushort GetReceivedAckField(Paket paket)
        {
            return (ushort)(paket.Array[6] + (paket.Array[7] << 8));
        }
    }
}