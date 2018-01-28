using Xunit;

namespace Upp.Net.UnitTests
{
    public class ByFramePlayoutBufferTests
    {
        [Fact]
        public void GetFillLevel_Empty_Zero()
        {
            var sut = new ByFramePlayoutBuffer<ISequencedMessage>(5);
            Assert.Equal(0, sut.GetFillLevel());
        }

        [Fact]
        public void Get_Empty_Null()
        {
            var sut = new ByFramePlayoutBuffer<ISequencedMessage>(5);
            Assert.Null(sut.GetNext());
        }

        [Fact]
        public void Get_TwiceSecondElementSet_Returned()
        {
            var sut = new ByFramePlayoutBuffer<ISequencedMessage>(5);
            Assert.Equal(0, sut.GetFillLevel());
            Assert.Null(sut.GetNext());
            Assert.Equal(0, sut.GetFillLevel());        
            var sequencedMessage = new Test(1);
            sut.Add(sequencedMessage);
            Assert.Equal(1, sut.GetFillLevel());
            Assert.Equal(sequencedMessage, sut.GetNext());
            Assert.Equal(0,sut.GetFillLevel());
        }

        [Fact]
        public void GetFillLevel_TwoAdded_Two()
        {
            var sut = new ByFramePlayoutBuffer<ISequencedMessage>(5);
            sut.Add(new Test(5));
            sut.Add(new Test(6));
            Assert.Equal(2, sut.GetFillLevel());
        }

        [Fact]
        public void Get_BatchesOfFiveAdded_CorrectOrder()
        {
            var sut = new ByFramePlayoutBuffer<ISequencedMessage>(5);
            ushort writeIndex = 0;
            ushort readIndex = 0;
            for (int i = 0; i < 30000; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Assert.Equal(AddResult.Current, sut.Add(new Test(writeIndex++)));
                }
                for (int j = 0; j < 5; j++)
                {
                    Assert.Equal(readIndex++, sut.GetNext().SequenceId);
                }
            }
        }

        class Test : ISequencedMessage
        {
            public Test(ushort sequenceId)
            {
                SequenceId = sequenceId;
            }

            public ushort SequenceId { get; set; }
        }
    }
}