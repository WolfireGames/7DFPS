using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe partial struct ImDrawChannel
    {
        public ImVector/*<ImDrawCmd>*/ CmdBuffer;
        public ImVector/*<ImDrawIdx>*/ IdxBuffer;
    }
    public unsafe partial struct ImDrawChannelPtr
    {
        public ImDrawChannel* NativePtr { get; }
        public ImDrawChannelPtr(ImDrawChannel* nativePtr) { NativePtr = nativePtr; }
        public ImDrawChannelPtr(IntPtr nativePtr) { NativePtr = (ImDrawChannel*)nativePtr; }
        public static implicit operator ImDrawChannelPtr(ImDrawChannel* nativePtr) { return new ImDrawChannelPtr(nativePtr); }
        public static implicit operator ImDrawChannel* (ImDrawChannelPtr wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator ImDrawChannelPtr(IntPtr nativePtr) { return new ImDrawChannelPtr(nativePtr); }
        public ImVector<ImDrawCmd> CmdBuffer => new ImVector<ImDrawCmd>(NativePtr->CmdBuffer);
        public ImVector_ushort IdxBuffer => new ImVector_ushort(NativePtr->IdxBuffer);
    }
}
