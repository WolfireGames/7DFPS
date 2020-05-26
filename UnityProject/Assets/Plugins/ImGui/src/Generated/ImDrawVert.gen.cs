using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe partial struct ImDrawVert
    {
        public Vector2 pos;
        public Vector2 uv;
        public uint col;
    }
    public unsafe partial struct ImDrawVertPtr
    {
        public ImDrawVert* NativePtr { get; }
        public ImDrawVertPtr(ImDrawVert* nativePtr) { NativePtr = nativePtr; }
        public ImDrawVertPtr(IntPtr nativePtr) { NativePtr = (ImDrawVert*)nativePtr; }
        public static implicit operator ImDrawVertPtr(ImDrawVert* nativePtr) { return new ImDrawVertPtr(nativePtr); }
        public static implicit operator ImDrawVert* (ImDrawVertPtr wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator ImDrawVertPtr(IntPtr nativePtr) { return new ImDrawVertPtr(nativePtr); }
        public Vector2 pos { get { return (Vector2)NativePtr->pos; } set { NativePtr->pos = value; } }
        public Vector2 uv { get { return (Vector2)NativePtr->uv; } set { NativePtr->uv = value; } }
        public uint col { get { return (uint)NativePtr->col; } set { NativePtr->col = value; } }
    }
}
