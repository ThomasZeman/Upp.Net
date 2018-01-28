using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Upp.Net.Serialization
{
    public class SimpleTypeWriter
    {
        public static void Write(string data, Paket paket)
        {
            if (data == null)
            {
                Write(0x7fffffff, paket);
                return;
            }
            if (data.Equals(String.Empty))
            {
                Write(0x00000000, paket);
                return;
            }
            if (data.Length > 0x0000ffff)
            {
                throw new ArgumentException("len must not exceed 0x0000ffff", nameof(data));
            }
            if (data.Length != 0)
            {
                byte[] buf = Encoding.UTF8.GetBytes(data);
                Write(buf.Length, paket);
                paket.ReserveSpace(buf.Length);
                Buffer.BlockCopy(buf, 0, paket.Array, paket.Offset, buf.Length);
                paket.Offset += buf.Length;
            }
        }

        public static void Write(string[] array, Paket paket)
        {
            if (array.Length > 0x0000ffff)
            {
                throw new ArgumentException("len must not exceed 0x0000ffff", nameof(array));
            }
            Write((ushort)array.Length, paket);
            for (var index = 0; index < (ushort)array.Length; index++)
            {
                Write(array[index], paket);
            }
        }

        public static void Write(ushort[] array, Paket paket)
        {
            if (array.Length > 0x0000ffff)
            {
                throw new ArgumentException("len must not exceed 0x0000ffff", nameof(array));
            }
            Write((ushort)array.Length, paket);
            for (int index = 0; index < (ushort)array.Length; index++)
            {
                Write(array[index], paket);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(byte data, Paket paket)
        {
            paket.ReserveSpace(1);
            paket.Array[paket.Offset++] = data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(short value, Paket paket)
        {
            paket.ReserveSpace(2);
            paket.Array[paket.Offset++] = (byte)(value >> 8);
            paket.Array[paket.Offset++] = (byte)(value & 255);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ushort value, Paket paket)
        {
            paket.ReserveSpace(2);
            paket.Array[paket.Offset++] = (byte)(value >> 8);
            paket.Array[paket.Offset++] = (byte)(value & 255);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(int value, Paket paket)
        {
            paket.ReserveSpace(4);
            paket.Array[paket.Offset++] = (byte)((value >> 24) & 255);
            paket.Array[paket.Offset++] = (byte)((value >> 16) & 255);
            paket.Array[paket.Offset++] = (byte)((value >> 8) & 255);
            paket.Array[paket.Offset++] = (byte)(value & 255);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(uint value, Paket paket)
        {
            paket.ReserveSpace(4);
            paket.Array[paket.Offset++] = (byte)((value >> 24) & 255);
            paket.Array[paket.Offset++] = (byte)((value >> 16) & 255);
            paket.Array[paket.Offset++] = (byte)((value >> 8) & 255);
            paket.Array[paket.Offset++] = (byte)(value & 255);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(float value, Paket paket)
        {
            UIntFloat floaty = default(UIntFloat);
            floaty.FloatValue = value;
            Write(floaty.IntValue, paket);
        }

    }
}