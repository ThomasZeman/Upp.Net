using System;

namespace Upp.Net
{
    public class NullTypedConnection<T> : ITypedConnection<T> where T : ISerializableMessage
    {
        public event Action<ITypedConnection<T>, T, ushort> NewMessage;
        public bool Send(T message)
        {
            return true;
        }
    }
}