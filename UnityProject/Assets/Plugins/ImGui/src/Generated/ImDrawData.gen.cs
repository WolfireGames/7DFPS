using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe partial struct ImDrawData
    {
        public byte Valid;
        public ImDrawList** CmdLists;
        public int CmdListsCount;
        public int TotalIdxCount;
        public int TotalVtxCount;
        public Vector2 DisplayPos;
        public Vector2 DisplaySize;
    }
    public unsafe partial struct ImDrawDataPtr
    {
        public ImDrawData* NativePtr { get; }
        public ImDrawDataPtr(ImDrawData* nativePtr) { NativePtr = nativePtr; }
        public ImDrawDataPtr(IntPtr nativePtr) { NativePtr = (ImDrawData*)nativePtr; }
        public static implicit operator ImDrawDataPtr(ImDrawData* nativePtr) { return new ImDrawDataPtr(nativePtr); }
        public static implicit operator ImDrawData* (ImDrawDataPtr wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator ImDrawDataPtr(IntPtr nativePtr) { return new ImDrawDataPtr(nativePtr); }
        public Bool8 Valid { get { return new Bool8(NativePtr->Valid); } set { NativePtr->Valid = (byte)(value ? 1 : 0); } }
        public IntPtr CmdLists { get { return (IntPtr)NativePtr->CmdLists; } set { NativePtr->CmdLists = (ImDrawList**)value; } }
        public int CmdListsCount { get { return (int)NativePtr->CmdListsCount; } set { NativePtr->CmdListsCount = value; } }
        public int TotalIdxCount { get { return (int)NativePtr->TotalIdxCount; } set { NativePtr->TotalIdxCount = value; } }
        public int TotalVtxCount { get { return (int)NativePtr->TotalVtxCount; } set { NativePtr->TotalVtxCount = value; } }
        public Vector2 DisplayPos { get { return (Vector2)NativePtr->DisplayPos; } set { NativePtr->DisplayPos = value; } }
        public Vector2 DisplaySize { get { return (Vector2)NativePtr->DisplaySize; } set { NativePtr->DisplaySize = value; } }
        public void ScaleClipRects(Vector2 sc)
        {
            ImGuiNative.ImDrawData_ScaleClipRects(NativePtr, sc);
        }
        public void DeIndexAllBuffers()
        {
            ImGuiNative.ImDrawData_DeIndexAllBuffers(NativePtr);
        }
        public void Clear()
        {
            ImGuiNative.ImDrawData_Clear(NativePtr);
        }
    }
}
