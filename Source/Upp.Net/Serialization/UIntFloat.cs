using System.Runtime.InteropServices;

namespace Upp.Net.Serialization
{
    [StructLayout(LayoutKind.Explicit)]
    struct UIntFloat
    {
        [FieldOffset(0)]
        public float FloatValue;

        [FieldOffset(0)]
        public uint IntValue;
    }
}