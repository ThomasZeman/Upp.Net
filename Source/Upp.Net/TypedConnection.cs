using System;
using Upp.Net.Serialization;

namespace Upp.Net
{
    public sealed class TypedConnection<T> : ITypedConnection<T> where T : ISerializableMessage
    {
        private readonly Connection _innnerConnection;
        private readonly Serializer<T> _serializer;
        public event Action<ITypedConnection<T>, T, ushort> NewMessage;

        public TypedConnection(Connection innnerConnection, Serializer<T> serializer)
        {
            _innnerConnection = innnerConnection;
            _serializer = serializer;
            _innnerConnection.NewPaket += _innnerConnection_NewPaket;
        }

        private void _innnerConnection_NewPaket(Connection arg1, Paket arg2)
        {
            var item = _serializer.Deserialize(arg2);
            NewMessage?.Invoke(this, item, arg2.SeqId);
        }

        public bool Send(T message)
        {
            var paket = _innnerConnection.CreatePaket();
            _serializer.Serialize(message, paket);
            return _innnerConnection.Send(paket);
        }
    }
}