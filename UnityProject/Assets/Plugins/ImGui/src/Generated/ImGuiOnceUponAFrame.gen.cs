using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiOnceUponAFrame
    {
        public int RefFrame;
    }
    public unsafe partial struct ImGuiOnceUponAFramePtr
    {
        public ImGuiOnceUponAFrame* NativePtr { get; }
        public ImGuiOnceUponAFramePtr(ImGuiOnceUponAFrame* nativePtr) { NativePtr = nativePtr; }
        public ImGuiOnceUponAFramePtr(IntPtr nativePtr) { NativePtr = (ImGuiOnceUponAFrame*)nativePtr; }
        public static implicit operator ImGuiOnceUponAFramePtr(ImGuiOnceUponAFrame* nativePtr) { return new ImGuiOnceUponAFramePtr(nativePtr); }
        public static implicit operator ImGuiOnceUponAFrame* (ImGuiOnceUponAFramePtr wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator ImGuiOnceUponAFramePtr(IntPtr nativePtr) { return new ImGuiOnceUponAFramePtr(nativePtr); }
        public int RefFrame { get { return (int)NativePtr->RefFrame; } set { NativePtr->RefFrame = value; } }
    }
}
