using System;

namespace Upp.Net.Serialization
{
    public class Serializer<TBase> where TBase : ISerializableMessage
    {
        private Func<TBase>[] _factories = new Func<TBase>[8];

        public void AddType<T>() where T : TBase, new()
        {
            var instance = new T();
            var id = instance.TypeId;
            while (id >= _factories.Length)
            {
                Array.Resize(ref _factories, _factories.Length * 2);
            }
            if (_factories[id] != null)
            {
                throw new ArgumentException($"Type with TypeId:{id} already registered");
            }
            _factories[id] = () => new T();
        }

        public TBase Deserialize(Paket paket)
        {
            var id = paket.Array[paket.Offset++];
#if DEBUG
            if (_factories[id] == null)
            {
                throw new ArgumentException($"Cannot deserialize TypeId:{id} because type not registered with serializer");
            }
#endif
            var instance = _factories[id]();
            instance.Deserialize(paket);
            return instance;
        }

        public void Serialize(TBase serializableMessage, Paket paket)
        {
            SimpleTypeWriter.Write(serializableMessage.TypeId, paket);
            serializableMessage.Serialize(paket);
        }
    }
}