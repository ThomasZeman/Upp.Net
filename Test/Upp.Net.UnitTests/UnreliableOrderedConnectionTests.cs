using System;
using System.Collections.Generic;
using Upp.Net.Platform;
using Upp.Net.Serialization;
using Upp.Net.Trace;
using Xunit;

namespace Upp.Net.UnitTests
{
    public class UnreliableOrderedConnectionTests
    {
        [Fact]
        public void Send_EverySecondLate_CorrectOrder()
        {
            var loopback = new Loopback(LoopbackTypes.EverySecondLate);
            var lhs = new UnreliableOrderedConnection(loopback.RhsUpdSend, 1, new NullTrace());
            var rhs = new UnreliableOrderedConnection(loopback.LhsUdpSend, 1, new NullTrace());
            loopback.RhsUpdSend.Sink = rhs;
            loopback.LhsUdpSend.Sink = lhs;
            var list = new List<Paket>();
            rhs.NewPaket += (p1, p2) => { list.Add(p2); };
            for (int i = 0; i < 100000; i++)
            {
                var paket = lhs.CreatePaket();
                SimpleTypeWriter.Write(i + "Test" + i, paket);
                SimpleTypeWriter.Write(i, paket);
                lhs.Send(paket);
            }
            Assert.Equal(50000, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var paket = list[i];
                var message = SimpleTypeReader.ReadString(paket);
                var number = SimpleTypeReader.ReadInt(paket);
                var j = i * 2 + 1;
                Assert.Equal(j, number);
                Assert.Equal(message, j + "Test" + j);
            }
        }

        [Fact]
        public void Send_AllInOrder_CorrectOrder()
        {
            var loopback = new Loopback(LoopbackTypes.AllInOrder);
            var lhs = new UnreliableOrderedConnection(loopback.RhsUpdSend, 1, new NullTrace());
            var rhs = new UnreliableOrderedConnection(loopback.LhsUdpSend, 1, new NullTrace());
            loopback.RhsUpdSend.Sink = rhs;
            loopback.LhsUdpSend.Sink = lhs;
            var list = new List<Paket>();
            rhs.NewPaket += (p1, p2) => { list.Add(p2); };
            for (int i = 0; i < 100000; i++)
            {
                var paket = lhs.CreatePaket();
                SimpleTypeWriter.Write(i + "Test" + i, paket);
                SimpleTypeWriter.Write(i, paket);
                lhs.Send(paket);
            }
            Assert.Equal(100000, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var paket = list[i];
                var message = SimpleTypeReader.ReadString(paket);
                var number = SimpleTypeReader.ReadInt(paket);
                Assert.Equal(i, number);
                Assert.Equal(message, i + "Test" + i);
            }
        }
    }

    enum LoopbackTypes
    {
        AllInOrder,
        EverySecondLate
    }

    class Loopback
    {
        public BaseLoopback LhsUdpSend { get; }

        public BaseLoopback RhsUpdSend { get; }


        public Loopback(LoopbackTypes loopbackType)
        {
            switch (loopbackType)
            {
                case LoopbackTypes.AllInOrder:
                    LhsUdpSend = new AllInOrder();
                    RhsUpdSend = new AllInOrder();
                    break;
                case LoopbackTypes.EverySecondLate:
                    LhsUdpSend = new EverySecondLate();
                    RhsUpdSend = new EverySecondLate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loopbackType), loopbackType, null);
            }

        }

        public abstract class BaseLoopback : IUdpSend
        {
            public Connection Sink { get; set; }
            public abstract void Send(byte[] buffer, int offset, int count);
        }

        public class AllInOrder : BaseLoopback
        {
            public override void Send(byte[] buffer, int offset, int count)
            {
                var paket = new Paket
                {
                    Array = buffer,
                    Count = count,
                    Offset = 0
                };
                Sink.MessageReceived(paket);
            }
        }

        public class EverySecondLate : BaseLoopback
        {
            private Paket _previousPaket;

            public override void Send(byte[] buffer, int offset, int count)
            {
                var paket = new Paket
                {
                    Array = buffer,
                    Count = count,
                    Offset = 0
                };
                if (_previousPaket == null)
                {
                    _previousPaket = paket;
                    return;
                }
                Sink.MessageReceived(paket);
                if (_previousPaket != null)
                {
                    Sink.MessageReceived(_previousPaket);
                    _previousPaket = null;
                }
            }
        }
    }
}
