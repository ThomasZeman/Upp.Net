using System;
using System.Collections.Generic;
using Upp.Net.Trace;

namespace Upp.Net
{
    internal class AcknowledgeSendBuffer<T> where T : class
    {
        private readonly ITrace _trace;
        private readonly T[] _buffer = new T[16];
        private ushort _ackBaseLocal; // base seqId
        private int _bufferBaseIndex; // which element in _buffer (index) equals the seqId stored in ackBaseLocal.
        private ushort _filled;
        private int _fillLevel;
        private ushort _confirmedPakets;
        private ushort _id;

        public AcknowledgeSendBuffer() : this(new NullTrace())
        {
        }

        public AcknowledgeSendBuffer(ITrace trace)
        {
            _trace = trace;
        }

        public int Add(T item, out int delta)
        {
            delta = (65536 + _id - _ackBaseLocal) & 65535;
            if ((delta >> 4) != 0)
            {
                return -1;
            }
            if (delta < 0)
            {
                throw new ArgumentException($"This is a bug in Upp.Net. Paket with Id:{_id} already added");
            }
            var deltaIndex = (delta + _bufferBaseIndex) & 15;
            if ((_filled & (1 << delta)) != 0)
            {
                throw new ArgumentException($"This is a bug in Upp.Net. Paket with Id:{_id} already added");
            }
            _buffer[deltaIndex] = item;
            _filled |= (ushort)(1 << delta);
            _fillLevel++;
            var currentId = _id;
            _id++;
            WriteDiagnostics();
            return currentId;
        }

        private void WriteDiagnostics()
        {
#if DEBUG
            _trace.Debug("SendBuffer: sendBase: " + _ackBaseLocal + " bufferBase: " + _bufferBaseIndex + " filled: " + Converter.ToBinary(_filled) + " confirmed: " + Converter.ToBinary(_confirmedPakets));
#endif
        }

        public void Ack(ushort ackField, ushort ackBaseRemote)
        {
            ushort delta = (ushort)(65536 + ackBaseRemote - _ackBaseLocal);
            if ((delta & 32768) != 0) // means "delta > 32768" which means negative value
            {
                delta = (ushort)~delta;
                delta++;
                _trace.Debug("Sendbuffer: 1Ack for: " + Converter.ToBinary(ackField) + " ackBaseRemote: " + ackBaseRemote + " delta: -" + delta);
                WriteDiagnostics();
                ackField = (ushort)(ackField >> delta);
            }
            else
            {
                _trace.Debug("Sendbuffer: 2Ack for: " + Converter.ToBinary(ackField) + " ackBaseRemote: " + ackBaseRemote + " delta: " + delta);
                WriteDiagnostics();
                for (int i = 0; i < delta; i++)
                {
                    _bufferBaseIndex = (_bufferBaseIndex + 1) & 15;
                    _confirmedPakets = (ushort)(_confirmedPakets >> 1);
                    _filled = (ushort)(_filled >> 1);
                    _fillLevel--;
                    _ackBaseLocal++;
                }
            }
            _confirmedPakets |= ackField;
            WriteDiagnostics();
        }

        public bool HigherOrderPaketsUnconfirmed()
        {
            var unconfirmedMask = _filled & (~_confirmedPakets);
            return ((unconfirmedMask & 65280) != 0);
        }

        public void GetAllUnconfirmed(List<T> unconfirmedPakets)
        {
            // Get all which are not confirmed except the leading ones
            // because they have just been sent and might be confirmed
            // any time soon. In other words: get only the gaps
            //
            // filled:    0000 0111 1111 1111
            // confirmed: 0000 0001 1101 1110
            // unconf:    0000 0110 0010 0001

            // result:    0000 0000 0010 0001

            // This is optimized by scanning through the high and low byte seperately. 
            var unconfirmedMask = _filled & (~_confirmedPakets);
            if (unconfirmedMask == 0)
            {
                return;
            }

            int firstConfirmed = -1;
            var startIndex = _fillLevel - 1;

            // If possible avoid scanning through low byte
            if ((_confirmedPakets & 65280) == 0)
            {
                startIndex = Math.Min(startIndex, 7);
            }
            for (int i = startIndex; i >= 0; i--)
            {
                if ((_confirmedPakets & (1 << i)) != 0)
                {
                    firstConfirmed = i;
                    break;
                }
            }
            if (firstConfirmed == -1)
            {
                if (_fillLevel == 16)
                {
                    firstConfirmed = 15;
                }
                else
                {
                    return;
                }
            }
            for (int i = firstConfirmed; i >= 0; i--)
            {
                if ((unconfirmedMask & (1 << i)) != 0)
                {
                    unconfirmedPakets.Add(_buffer[(i + _bufferBaseIndex) & 15]);
                }
            }
        }
    }
}