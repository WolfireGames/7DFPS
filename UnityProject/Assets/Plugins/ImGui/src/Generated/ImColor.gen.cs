using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe partial struct ImColor
    {
        public Vector4 Value;
    }
    public unsafe partial struct ImColorPtr
    {
        public ImColor* NativePtr { get; }
        public ImColorPtr(ImColor* nativePtr) { NativePtr = nativePtr; }
        public ImColorPtr(IntPtr nativePtr) { NativePtr = (ImColor*)nativePtr; }
        public static implicit operator ImColorPtr(ImColor* nativePtr) { return new ImColorPtr(nativePtr); }
        public static implicit operator ImColor* (ImColorPtr wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator ImColorPtr(IntPtr nativePtr) { return new ImColorPtr(nativePtr); }
        public Vector4 Value { get { return (Vector4)NativePtr->Value; } set { NativePtr->Value = value; } }
        public void SetHSV(float h, float s, float v)
        {
            float a = 1.0f;
            ImGuiNative.ImColor_SetHSV(NativePtr, h, s, v, a);
        }
        public void SetHSV(float h, float s, float v, float a)
        {
            ImGuiNative.ImColor_SetHSV(NativePtr, h, s, v, a);
        }
        public ImColor HSV(float h, float s, float v)
        {
            float a = 1.0f;
            ImColor ret = ImGuiNative.ImColor_HSV(NativePtr, h, s, v, a);
            return ret;
        }
        public ImColor HSV(float h, float s, float v, float a)
        {
            ImColor ret = ImGuiNative.ImColor_HSV(NativePtr, h, s, v, a);
            return ret;
        }
    }
}
