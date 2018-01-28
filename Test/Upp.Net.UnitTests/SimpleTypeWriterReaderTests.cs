using Upp.Net.Serialization;
using Xunit;

namespace Upp.Net.UnitTests
{
    public class SimpleTypeWriterReaderTests
    {
        [Fact]
        public void WriteReadInt_12345678()
        {
            var paket = new Paket();
            paket.Offset = 5;
            SimpleTypeWriter.Write(12345678, paket);
            paket.Offset = 5;
            Assert.Equal(12345678, SimpleTypeReader.ReadInt(paket));
        }

        [Fact]
        public void WriteReadInt_1()
        {
            var paket = new Paket();
            paket.Offset = 8;
            SimpleTypeWriter.Write(1, paket);
            paket.Offset = 8;
            Assert.Equal(1, SimpleTypeReader.ReadInt(paket));
        }

        [Fact]
        public void UShort()
        {
            uint prev = 0;
            for (ushort i = 0; prev <= i; i +=13)
            {
                prev = i;
                var paket = new Paket();
                paket.Offset = 7;
                SimpleTypeWriter.Write(i, paket);
                paket.Offset = 7;
                Assert.Equal(i, SimpleTypeReader.ReadUShort(paket));
            }
        }

        [Fact]
        public void UInt()
        {
            uint prev = 0;
            for (uint i = 0; prev <= i; i += 29787)
            {
                prev = i;
                var paket = new Paket();
                paket.Offset = 7;
                SimpleTypeWriter.Write(i, paket);
                paket.Offset = 7;
                Assert.Equal(i, SimpleTypeReader.ReadUInt(paket));
            }
        }

        [Fact]
        public void Float()
        {
            for (float i = -1000000; i < 1000000; i+=1.37f)
            {
                var paket = new Paket();
                paket.Offset = 7;
                SimpleTypeWriter.Write(i, paket);
                paket.Offset = 7;
                Assert.Equal(i, SimpleTypeReader.ReadFloat(paket));
            }
        }
    }
}
