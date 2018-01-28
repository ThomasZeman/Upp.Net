using System;

namespace Upp.Net
{
    public interface ITypedConnection<T> where T : ISerializableMessage
    {
        event Action<ITypedConnection<T>, T, ushort> NewMessage;
        bool Send(T message);
    }
}