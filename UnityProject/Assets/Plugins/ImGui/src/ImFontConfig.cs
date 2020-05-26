using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe struct ImFontConfigNative
    {
        public void* FontData;
        public int FontDataSize;
        public byte FontDataOwnedByAtlas;
        public int FontNo;
        public float SizePixels;
        public int OversampleH;
        public int OversampleV;
        public byte PixelSnapH;
        public Vector2 GlyphExtraSpacing;
        public Vector2 GlyphOffset;
        public ushort* GlyphRanges;
        public float GlyphMinAdvanceX;
        public float GlyphMaxAdvanceX;
        public byte MergeMode;
        public uint RasterizerFlags;
        public float RasterizerMultiply;
        public fixed byte Name[40];
        public ImFont* DstFont;
    }

    public unsafe class ImFontConfig : IDisposable
    {
        public ImFontConfigNative* NativePtr;

        public ImFontConfig(ImFontConfigNative* nativePtr) { NativePtr = nativePtr; }
        public ImFontConfig(IntPtr nativePtr) { NativePtr = (ImFontConfigNative*)nativePtr; }
        public static implicit operator ImFontConfig(ImFontConfigNative* nativePtr) { return new ImFontConfig(nativePtr); }
        public static implicit operator ImFontConfigNative* (ImFontConfig wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator ImFontConfig(IntPtr nativePtr) { return new ImFontConfig(nativePtr); }
        public IntPtr FontData { get { return (IntPtr)NativePtr->FontData; } set { NativePtr->FontData = (void*)value; } }
        public int FontDataSize { get { return (int)NativePtr->FontDataSize; } set { NativePtr->FontDataSize = value; } }
        public Bool8 FontDataOwnedByAtlas { get { return new Bool8(NativePtr->FontDataOwnedByAtlas); } set { NativePtr->FontDataOwnedByAtlas = (byte)(value ? 1 : 0); } }
        public int FontNo { get { return (int)NativePtr->FontNo; } set { NativePtr->FontNo = value; } }
        public float SizePixels { get { return (float)NativePtr->SizePixels; } set { NativePtr->SizePixels = value; } }
        public int OversampleH { get { return (int)NativePtr->OversampleH; } set { NativePtr->OversampleH = value; } }
        public int OversampleV { get { return (int)NativePtr->OversampleV; } set { NativePtr->OversampleV = value; } }
        public Bool8 PixelSnapH { get { return new Bool8(NativePtr->PixelSnapH); } set { NativePtr->PixelSnapH = (byte)(value ? 1 : 0); } }
        public Vector2 GlyphExtraSpacing { get { return (Vector2)NativePtr->GlyphExtraSpacing; } set { NativePtr->GlyphExtraSpacing = value; } }
        public Vector2 GlyphOffset { get { return (Vector2)NativePtr->GlyphOffset; } set { NativePtr->GlyphOffset = value; } }
        public IntPtr GlyphRanges { get { return (IntPtr)NativePtr->GlyphRanges; } set { NativePtr->GlyphRanges = (ushort*)value; } }
        public float GlyphMinAdvanceX { get { return (float)NativePtr->GlyphMinAdvanceX; } set { NativePtr->GlyphMinAdvanceX = value; } }
        public float GlyphMaxAdvanceX { get { return (float)NativePtr->GlyphMaxAdvanceX; } set { NativePtr->GlyphMaxAdvanceX = value; } }
        public Bool8 MergeMode { get { return new Bool8(NativePtr->MergeMode); } set { NativePtr->MergeMode = (byte)(value ? 1 : 0); } }
        public uint RasterizerFlags { get { return (uint)NativePtr->RasterizerFlags; } set { NativePtr->RasterizerFlags = value; } }
        public float RasterizerMultiply { get { return (float)NativePtr->RasterizerMultiply; } set { NativePtr->RasterizerMultiply = value; } }
        public RangeAccessor<byte> Name => new RangeAccessor<byte>(NativePtr->Name, 40);
        public ImFontPtr DstFont => new ImFontPtr(NativePtr->DstFont);

        public ImFontConfig()
        {
            NativePtr = ImGuiNative.ImFontConfig_ImFontConfig();
        }

        public void Dispose()
        {
            if (NativePtr != null)
            {
                ImGuiNative.ImFontConfig_destroy(NativePtr);
                NativePtr = null;
            }
        }
    }
}
