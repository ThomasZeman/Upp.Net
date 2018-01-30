using System;
using Upp.Net.Serialization;

namespace Upp.Net.IntegrationTests
{
    public class TestData : ISerializableMessage
    {
        public String TestString { get; private set; }
        public int TestNumber { get; private set; }

        public TestData()
        {
        }

        public override string ToString()
        {
            return $"TestString: {TestString}, TestNumber: {TestNumber}";
        }

        public TestData(string testString, int testNumber)
        {
            TestString = testString;
            TestNumber = testNumber;
        }

        public byte TypeId => 1;
        public void Serialize(Paket paket)
        {
            SimpleTypeWriter.Write(TestString, paket);
            SimpleTypeWriter.Write(TestNumber, paket);
        }

        public void Deserialize(Paket paket)
        {
            TestString = SimpleTypeReader.ReadString(paket);
            TestNumber = SimpleTypeReader.ReadInt(paket);
        }
    }
}