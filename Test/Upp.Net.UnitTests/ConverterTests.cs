using Upp.Net.Trace;
using Xunit;

namespace Upp.Net.UnitTests
{
    public class ConverterTests
    {
        [Fact]
        public void ConvertZero()
        {
            Assert.Equal("0000 0000", Converter.ToBinary(0));
        }

        [Fact]
        public void ConvertOne()
        {
            Assert.Equal("0000 0001", Converter.ToBinary(1));
        }

        [Fact]
        public void ConvertTwo()
        {
            Assert.Equal("0000 0010", Converter.ToBinary(2));
        }

        [Fact]
        public void Convert32678()
        {
            Assert.Equal("1000 0000 0000 0000", Converter.ToBinary(32768));
        }

        [Fact]
        public void Convert14643()
        {
            Assert.Equal("0011 1001 0011 0011", Converter.ToBinary(14643));
        }
    }
}
