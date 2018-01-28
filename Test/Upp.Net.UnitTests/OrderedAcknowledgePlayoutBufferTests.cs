using System.Collections.Generic;
using Xunit;

namespace Upp.Net.UnitTests
{
    public class OrderedAcknowledgePlayoutBufferTests
    {
        [Fact]
        public void AddSequence_BitFieldEmpty()
        {
            var list = new List<int>();
            var sut = new OrderedAcknowledgePlayoutBuffer<int>(list.Add);
            for (ushort i = 0; i < 99; i++)
            {
                var result = sut.Add(i, i * 2);
                Assert.True(result == AckResult.Match, i.ToString());
            }
            Assert.Equal(0, sut.AckField);
            Assert.Equal(99, sut.AckBase);
        }

        [Fact]
        public void AddSequenceButNotFirst_AllSetExceptForFirst()
        {
            var list = new List<int>();
            var sut = new OrderedAcknowledgePlayoutBuffer<int>(list.Add);
            for (ushort i = 1; i < 99; i++)
            {
                var result = sut.Add(i, i * 2);
                if (i < 16)
                {
                    Assert.True(result == AckResult.EarlyAck, i.ToString());
                }
                else
                {
                    Assert.True(result == AckResult.OutOfWindow, i.ToString());
                }
            }
            Assert.Equal(65534, sut.AckField);
            Assert.Equal(0, sut.AckBase);
        }

        [Fact]
        public void AddFirstToAlreadyFull_AllEmpty()
        {
            var list = new List<int>();
            var sut = new OrderedAcknowledgePlayoutBuffer<int>(list.Add);
            for (ushort i = 1; i < 16; i++)
            {
                sut.Add(i, i * 2);
            }
            Assert.Equal(65534, sut.AckField);
            Assert.True(sut.Add(0, 99) == AckResult.LateAck);
            Assert.Equal(0, sut.AckField);
            Assert.Equal(16, sut.AckBase);
        }
    }
}