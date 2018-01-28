using System.Collections.Generic;

namespace Upp.Net
{
    public enum AddResult
    {
        Stale,
        Current,
        TooFarInFuture
    }

    public class ByFramePlayoutBuffer<T> where T : ISequencedMessage
    {
        private readonly T[] _buffer;
        private int _bufferBaseIndex;
        private int _capacity;
        private ushort _bufferBaseSequence;
        private int _capacityBitMask;
        private int _fillLevel;


        public ByFramePlayoutBuffer(int capacityAsIndexOfMsb)
        {
            _capacity = capacityAsIndexOfMsb;
            _capacityBitMask = (1 << _capacity) - 1;
            _buffer = new T[1 << _capacity];
        }

        public int GetFillLevel()
        {
            return _fillLevel;
        }

        public void ResetWithBaseId(ushort baseSequenceId)
        {
            _bufferBaseSequence = baseSequenceId;
            _fillLevel = 0;
            _bufferBaseIndex = 0;
        }

        public AddResult Add(T item)
        {
            var delta = (65536 + item.SequenceId - _bufferBaseSequence) & 65535;
            // TODO: what do the following two cases really mean?
            if ((delta >> _capacity) != 0)
            {
                return AddResult.TooFarInFuture;
            }
            if (delta < 0)
            {
                return AddResult.Stale;
            }
            _fillLevel++;
            _buffer[(_bufferBaseIndex + delta) & _capacityBitMask] = item;
            return AddResult.Current;
        }

        public T GetNext()
        {

            var item = _buffer[_bufferBaseIndex];
            _buffer[_bufferBaseIndex] = default(T);
            _bufferBaseIndex = (_bufferBaseIndex + 1) & _capacityBitMask;
            _bufferBaseSequence = (ushort)(_bufferBaseSequence + 1);
            if (!EqualityComparer<T>.Default.Equals(item, default(T)))
            {
                _fillLevel--;
            }
            return item;
        }

    }
}
