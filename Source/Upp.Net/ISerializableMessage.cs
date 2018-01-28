namespace Upp.Net
{
    public interface ISerializableMessage
    {
        byte TypeId { get; }
        void Serialize(Paket paket);
        void Deserialize(Paket paket);
    }
}