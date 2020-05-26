using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    // Unity doesn't let us create pointer to generic type (T*)
    // so we have to make an explicit type for it
    public unsafe struct ImVector_ushort
    {
        public readonly int Size;
        public readonly int Capacity;
        public readonly ushort *Data;

        public ImVector_ushort(ImVector vector)
        {
            Size = vector.Size;
            Capacity = vector.Capacity;
            Data = (ushort*) vector.Data;
        }

        public ImVector_ushort(int size, int capacity, IntPtr data)
        {
            Size = size;
            Capacity = capacity;
            Data = (ushort*) data;
        }

        public ushort this[int index] {
            get
            {
                if (index < 0 || index >= Size)
                {
                    throw new IndexOutOfRangeException();
                }
                return Data[index];
            }
        }
    }
}
