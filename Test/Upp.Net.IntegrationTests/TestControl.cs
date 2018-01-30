namespace Upp.Net.IntegrationTests
{
    public class TestControl : ISerializableMessage
    {
        public byte TypeId => 200;
        public void Serialize(Paket paket)
        {
        }

        public void Deserialize(Paket paket)
        {
        }
    }
}