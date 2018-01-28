using System;
using System.Runtime.CompilerServices;

namespace Upp.Net
{
    public class Paket
    {
        public byte[] Array;
        public int Offset;
        public int Count;

        public ushort SeqId;

        public uint Uid;

        public Paket()
        {
            Array = new byte[1024];
        }

        public Paket(byte[] data, int length)
        {
            Array = data;
            Count = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReserveSpace(int amount)
        {
            var requiredCount = Offset + amount;
            //todo: auto increase array if necessary
            if (requiredCount > Count)
            {
                if (requiredCount > Array.Length)
                {
                    throw new ArgumentException("Cannot reserve space");
                }
                Count = requiredCount;
            }
        }

        public override string ToString()
        {
#if UID
            return $"Offset: {Offset}, Count: {Count}, SeqId: {SeqId}, Uid: {Uid}";
#else
            return $"Offset: {Offset}, Count: {Count}, SeqId: {SeqId}";
#endif
        }
    }
}