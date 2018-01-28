using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Upp.Net.Serialization
{
    public class SimpleTypeReader
    {
        public static string ReadString(Paket paket)
        {
            int len = ReadInt(paket);
            switch (len)
            {
                case 0x7fffffff:
                    return null;
                case 0:
                    return "";
            }
            if (len < 0)
            {
                throw new IndexOutOfRangeException("len must not be below 0");
            }
            if (len > 0x000fffff)
            {
                throw new IndexOutOfRangeException("len must not exceed 0x000fffff");
            }
            var buf = new byte[len];
            Buffer.BlockCopy(paket.Array, paket.Offset, buf, 0, len);
            paket.Offset += len;
            return Encoding.UTF8.GetString(buf, 0, len);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(Paket paket)
        {
            return paket.Array[paket.Offset++];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt(Paket paket)
        {
            return (paket.Array[paket.Offset++] << 24) | paket.Array[paket.Offset++] << 16 | paket.Array[paket.Offset++] << 8 | paket.Array[paket.Offset++];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt(Paket paket)
        {
            return (uint)((paket.Array[paket.Offset++] << 24) | paket.Array[paket.Offset++] << 16 | paket.Array[paket.Offset++] << 8 | paket.Array[paket.Offset++]);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUShort(Paket paket)
        {
            return (ushort)(paket.Array[paket.Offset++] << 8 | paket.Array[paket.Offset++]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShort(Paket paket)
        {
            return (short)(paket.Array[paket.Offset++] << 8 | paket.Array[paket.Offset++]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloat(Paket paket)
        {
            UIntFloat floaty = default(UIntFloat);
            floaty.IntValue = ReadUInt(paket);
            return floaty.FloatValue;
        }

        public static string[] ReadStrings(Paket paket)
        {
            var length = ReadUShort(paket);
            string[] array = new string[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadString(paket);
            }
            return array;
        }

        public static ushort[] ReadUShorts(Paket paket)
        {
            var length = ReadUShort(paket);
            ushort[] array = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadUShort(paket);
            }
            return array;
        }
    }
}