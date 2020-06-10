using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe struct ImVector
    {
        public readonly int Size;
        public readonly int Capacity;
        public readonly IntPtr Data;

        public T Ref<T>(int index) where T : struct
        {
            throw new NotImplementedException();  // FIXME: find AsRef equivalent
            //return Unsafe.AsRef<T>((byte*)Data + index * UnsafeUtility.SizeOf<T>());
        }

        public IntPtr Address<T>(int index) where T : struct
        {
            return (IntPtr)((byte*)Data + index * UnsafeUtility.SizeOf<T>());
        }
    }

    public unsafe struct ImVector<T>
    {
        public readonly int Size;
        public readonly int Capacity;
        public readonly IntPtr Data;

        public ImVector(ImVector vector)
        {
            Size = vector.Size;
            Capacity = vector.Capacity;
            Data = vector.Data;
        }

        public ImVector(int size, int capacity, IntPtr data)
        {
            Size = size;
            Capacity = capacity;
            Data = data;
        }

        public T this[int index] {
            get
            {
                if (index < 0 || index >= Size)
                {
                    throw new IndexOutOfRangeException();
                }
                return UnsafeUtility.ReadArrayElement<T>((byte*)Data, index);
            }

            set
            {
                if (index < 0 || index >= Size)
                {
                    throw new IndexOutOfRangeException();
                }
                UnsafeUtility.WriteArrayElement<T>((byte*)Data, index, value);
            }
        }
    }

    public unsafe struct ImPtrVector<T>
    {
        public readonly int Size;
        public readonly int Capacity;
        public readonly IntPtr Data;
        private readonly int _stride;

        public ImPtrVector(ImVector vector, int stride)
            : this(vector.Size, vector.Capacity, vector.Data, stride)
        { }

        public ImPtrVector(int size, int capacity, IntPtr data, int stride)
        {
            Size = size;
            Capacity = capacity;
            Data = data;
            _stride = stride;
        }

        public T this[int index]
        {
            get
            {
                T ret = UnsafeUtility.ReadArrayElementWithStride<T>((byte*)Data, index, _stride);
                return ret;
            }
        }
    }
}
