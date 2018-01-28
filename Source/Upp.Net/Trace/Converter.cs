using System.Text;

namespace Upp.Net.Trace
{
    public static class Converter
    {
        public static string ToBinary(ushort value)
        {
            return ToBinary(value, 16);
        }

        public static string ToBinary(byte value)
        {
            return ToBinary(value, 8);
        }

        private static string ToBinary(uint value, int width)
        {
            var mask = 1 << (width - 1);
            var sb = new StringBuilder();
            for (int i = 0; i < width; i++)
            {
                if ((value & mask) != 0)
                {
                    sb.Append('1');
                }
                else
                {
                    sb.Append('0');
                }
                if (i % 4 == 3 && (i + 1 != width))
                {
                    sb.Append(' ');
                }
                mask = mask >> 1;
            }
            return sb.ToString();
        }
    }
}