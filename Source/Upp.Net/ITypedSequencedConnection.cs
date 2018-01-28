using System;

namespace Upp.Net
{
    public interface ITypedSequencedConnection<T> where T : ISequencedMessage, ISerializableMessage
    {
        event Action<ITypedSequencedConnection<T>, T> NewMessage;
        bool Send(T message);
    }
}