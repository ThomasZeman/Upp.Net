using System;
using Upp.Net.Serialization;

namespace Upp.Net
{
    public sealed class TypedSequencedConnection<T> : ITypedSequencedConnection<T> where T : ISequencedMessage, ISerializableMessage
    {
        private readonly TypedConnection<T> _typedConnection;
        public event Action<ITypedSequencedConnection<T>, T> NewMessage;

        public TypedSequencedConnection(Connection innnerConnection, Serializer<T> serializer)
        {
            _typedConnection = new TypedConnection<T>(innnerConnection, serializer);
            _typedConnection.NewMessage += _typedConnection_NewMessage;
        }

        private void _typedConnection_NewMessage(ITypedConnection<T> arg1, T arg2, ushort arg3)
        {
            arg2.SequenceId = arg3;
            NewMessage?.Invoke(this, arg2);
        }

        public bool Send(T message)
        {
            return _typedConnection.Send(message);
        }
    }
}