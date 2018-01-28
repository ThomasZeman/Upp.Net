using System.Collections.Generic;
using Xunit;

namespace Upp.Net.UnitTests
{
    public class AcknowledgeSendBufferTests
    {

        [Fact]
        public void Add_NotUnconfirmedBecauseLeading()
        {
            var sut = new AcknowledgeSendBuffer<Holder>();
            var holder = new Holder(123);
            sut.Add(holder, out _);
            var list = new List<Holder>();
            sut.GetAllUnconfirmed(list);
            Assert.Empty(list);
        }

        [Fact]
        public void AddTwo_AckFirst_NoneUnconfirmed()
        {
            var sut = new AcknowledgeSendBuffer<Holder>();
            var holder = new Holder(123);
            sut.Add(holder, out _);
            var holder2 = new Holder(323);
            sut.Add(holder2, out _);
            sut.Ack(1, 0);
            var list = new List<Holder>();
            sut.GetAllUnconfirmed(list);
            Assert.Empty(list);
        }

        [Fact]
        public void AddTwo_AckSecond_FirstUnconfirmed()
        {
            var sut = new AcknowledgeSendBuffer<Holder>();
            var holder = new Holder(123);
            sut.Add(holder, out _);
            var holder2 = new Holder(323);
            sut.Add(holder2, out _);
            sut.Ack(2, 0);
            var list = new List<Holder>();
            sut.GetAllUnconfirmed(list);
            Assert.Single(list);
            Assert.Equal(holder, list[0]);
        }

        [Fact]
        public void Add200000_Ack()
        {
            var sut = new AcknowledgeSendBuffer<Holder>();
            var list = new List<Holder>();
            for (int i = 0; i < 200000; i++)
            {
                sut.Add(new Holder(i), out _);
                sut.Ack(1, (ushort)i);
                sut.GetAllUnconfirmed(list);
                Assert.Empty(list);
            }
        }

        [Fact]
        public void Add250000_AcksDelayed_NoneUnconfirmed()
        {
            // Tests that only really gaps in the confirmation mask
            // are returned as unconfirmed pakets. This test does not
            // produce gaps so there are no unconfirmed pakets

            var sut = new AcknowledgeSendBuffer<Holder>();
            var unconfirmedPakets = new List<Holder>();
            for (int i = 0; i < 5; i++)
            {
                var holder = new Holder(i);
                sut.Add(holder, out _);
            }

            //  0000 0000 00ss ssss
            //  0000 0000 0000 000c // base seqid 2 equals ->
            //  0000 0000 0000 0c00 // --> all below 2 are "auto" confirmed
            //  0000 0000 00ss s000
            
            for (int i = 5; i < 250000; i++)
            {
                var holder = new Holder(i);
                sut.Add(holder, out _);
                sut.Ack(1, (ushort)(i - 3));
                unconfirmedPakets.Clear();
                sut.GetAllUnconfirmed(unconfirmedPakets);
                Assert.Empty(unconfirmedPakets);
            }
        }

        [Fact]
        public void Add_LowerAckBase()
        {
            var sut = new AcknowledgeSendBuffer<Holder>();
            var sent = new List<Holder>();
            var unconfirmedPakets = new List<Holder>();
            for (int i = 0; i < 12; i++)
            {
                var holder = new Holder(i);
                sut.Add(holder, out _);
                sent.Add(holder);
            }
            // 0000 0110 0000 0000
            sut.Ack((1 + 2) << 9, 0);
            // 0000 1111 0000 0000
            sut.Ack((1 + 2 + 4 + 8) << 4, 4);
            // 0000 0000 1000 0000
            sut.Ack((1) << 7, 0);
            sut.GetAllUnconfirmed(unconfirmedPakets);
            Assert.Equal(3, unconfirmedPakets.Count);
            for (int i = 4; i < 7; i++)
            {
                Assert.Equal(sent[i], unconfirmedPakets[6 - i]);
            }
        }
    }
}