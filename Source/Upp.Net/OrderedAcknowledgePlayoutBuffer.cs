using System;
using Upp.Net.Trace;

namespace Upp.Net
{
    public enum AckResult
    {
        EarlyAck = 0,
        Match = 1,
        LateAck = 2,
        OutOfWindow = 3,
        AlreadyAcked = 4,
        Unknown = 255
    }

    public class OrderedAcknowledgePlayoutBuffer<T>
    {
        private readonly Action<T> _playout;
        private readonly ITrace _trace;
        private readonly T[] _buffer = new T[16];
        private int _bufferBaseIndex;

        private ushort _ackBase;
        private ushort _ackField;

        public OrderedAcknowledgePlayoutBuffer(Action<T> playout) : this(playout, new NullTrace())
        {
        }

        public OrderedAcknowledgePlayoutBuffer(Action<T> playout, ITrace trace)
        {
            _playout = playout;
            _trace = trace;
        }

        public ushort AckBase => _ackBase;

        public ushort AckField => _ackField;

        public AckResult Add(ushort id, T item)
        {
            // 255 - 250 = 5
            // 1 - 250 = -249 --> + 255 = 6
            var delta = (65536 + id - _ackBase) & 65535;
            _trace.Debug("Add: id {0} delta: {1} ackField: {2} ackBase: {3}", id, delta, Converter.ToBinary(_ackField), _ackBase );
            // can the following be determined by using an and mask?
            if ((delta >> 4) != 0)
            {
                return AckResult.OutOfWindow;
            }
            if (delta < 0)
            {
                return AckResult.OutOfWindow;
            }
            var bit = (ushort)(1 << delta);
            if ((_ackField & bit) == 0)
            {
                _buffer[(_bufferBaseIndex + delta) & 15] = item;
                _ackField |= bit;
                int counter = 0;
                while ((_ackField & 1) == 1)
                {
                    _trace.Debug("Playout: " + _ackBase + " ackField: " + _ackField + " bufferBaseIndex: " + _bufferBaseIndex);
                    _playout(_buffer[_bufferBaseIndex]);
                    _bufferBaseIndex = (_bufferBaseIndex + 1) & 15;
                    _ackBase++;
                    counter++;
                    _ackField = (ushort)(_ackField >> 1);
                }
                // consider changing all these loe, goe to bit mask logic for consistency
                return counter > 1 ? AckResult.LateAck : (AckResult)counter;
            }
            return AckResult.AlreadyAcked;
        }
    }
}