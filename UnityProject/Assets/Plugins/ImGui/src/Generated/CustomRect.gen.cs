using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe partial struct CustomRect
    {
        public uint ID;
        public ushort Width;
        public ushort Height;
        public ushort X;
        public ushort Y;
        public float GlyphAdvanceX;
        public Vector2 GlyphOffset;
        public ImFont* Font;
    }
    public unsafe partial struct CustomRectPtr
    {
        public CustomRect* NativePtr { get; }
        public CustomRectPtr(CustomRect* nativePtr) { NativePtr = nativePtr; }
        public CustomRectPtr(IntPtr nativePtr) { NativePtr = (CustomRect*)nativePtr; }
        public static implicit operator CustomRectPtr(CustomRect* nativePtr) { return new CustomRectPtr(nativePtr); }
        public static implicit operator CustomRect* (CustomRectPtr wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator CustomRectPtr(IntPtr nativePtr) { return new CustomRectPtr(nativePtr); }
        public uint ID { get { return (uint)NativePtr->ID; } set { NativePtr->ID = value; } }
        public ushort Width { get { return (ushort)NativePtr->Width; } set { NativePtr->Width = value; } }
        public ushort Height { get { return (ushort)NativePtr->Height; } set { NativePtr->Height = value; } }
        public ushort X { get { return (ushort)NativePtr->X; } set { NativePtr->X = value; } }
        public ushort Y { get { return (ushort)NativePtr->Y; } set { NativePtr->Y = value; } }
        public float GlyphAdvanceX { get { return (float)NativePtr->GlyphAdvanceX; } set { NativePtr->GlyphAdvanceX = value; } }
        public Vector2 GlyphOffset { get { return (Vector2)NativePtr->GlyphOffset; } set { NativePtr->GlyphOffset = value; } }
        public ImFontPtr Font => new ImFontPtr(NativePtr->Font);
        public bool IsPacked()
        {
            byte ret = ImGuiNative.CustomRect_IsPacked(NativePtr);
            return ret != 0;
        }
    }
}
