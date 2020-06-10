using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiStyle
    {
        public float Alpha;
        public Vector2 WindowPadding;
        public float WindowRounding;
        public float WindowBorderSize;
        public Vector2 WindowMinSize;
        public Vector2 WindowTitleAlign;
        public float ChildRounding;
        public float ChildBorderSize;
        public float PopupRounding;
        public float PopupBorderSize;
        public Vector2 FramePadding;
        public float FrameRounding;
        public float FrameBorderSize;
        public Vector2 ItemSpacing;
        public Vector2 ItemInnerSpacing;
        public Vector2 TouchExtraPadding;
        public float IndentSpacing;
        public float ColumnsMinSpacing;
        public float ScrollbarSize;
        public float ScrollbarRounding;
        public float GrabMinSize;
        public float GrabRounding;
        public Vector2 ButtonTextAlign;
        public Vector2 DisplayWindowPadding;
        public Vector2 DisplaySafeAreaPadding;
        public float MouseCursorScale;
        public byte AntiAliasedLines;
        public byte AntiAliasedFill;
        public float CurveTessellationTol;
        public Vector4 Colors_0;
        public Vector4 Colors_1;
        public Vector4 Colors_2;
        public Vector4 Colors_3;
        public Vector4 Colors_4;
        public Vector4 Colors_5;
        public Vector4 Colors_6;
        public Vector4 Colors_7;
        public Vector4 Colors_8;
        public Vector4 Colors_9;
        public Vector4 Colors_10;
        public Vector4 Colors_11;
        public Vector4 Colors_12;
        public Vector4 Colors_13;
        public Vector4 Colors_14;
        public Vector4 Colors_15;
        public Vector4 Colors_16;
        public Vector4 Colors_17;
        public Vector4 Colors_18;
        public Vector4 Colors_19;
        public Vector4 Colors_20;
        public Vector4 Colors_21;
        public Vector4 Colors_22;
        public Vector4 Colors_23;
        public Vector4 Colors_24;
        public Vector4 Colors_25;
        public Vector4 Colors_26;
        public Vector4 Colors_27;
        public Vector4 Colors_28;
        public Vector4 Colors_29;
        public Vector4 Colors_30;
        public Vector4 Colors_31;
        public Vector4 Colors_32;
        public Vector4 Colors_33;
        public Vector4 Colors_34;
        public Vector4 Colors_35;
        public Vector4 Colors_36;
        public Vector4 Colors_37;
        public Vector4 Colors_38;
        public Vector4 Colors_39;
        public Vector4 Colors_40;
        public Vector4 Colors_41;
        public Vector4 Colors_42;
    }
    public unsafe partial struct ImGuiStylePtr
    {
        public ImGuiStyle* NativePtr { get; }
        public ImGuiStylePtr(ImGuiStyle* nativePtr) { NativePtr = nativePtr; }
        public ImGuiStylePtr(IntPtr nativePtr) { NativePtr = (ImGuiStyle*)nativePtr; }
        public static implicit operator ImGuiStylePtr(ImGuiStyle* nativePtr) { return new ImGuiStylePtr(nativePtr); }
        public static implicit operator ImGuiStyle* (ImGuiStylePtr wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator ImGuiStylePtr(IntPtr nativePtr) { return new ImGuiStylePtr(nativePtr); }
        public float Alpha { get { return (float)NativePtr->Alpha; } set { NativePtr->Alpha = value; } }
        public Vector2 WindowPadding { get { return (Vector2)NativePtr->WindowPadding; } set { NativePtr->WindowPadding = value; } }
        public float WindowRounding { get { return (float)NativePtr->WindowRounding; } set { NativePtr->WindowRounding = value; } }
        public float WindowBorderSize { get { return (float)NativePtr->WindowBorderSize; } set { NativePtr->WindowBorderSize = value; } }
        public Vector2 WindowMinSize { get { return (Vector2)NativePtr->WindowMinSize; } set { NativePtr->WindowMinSize = value; } }
        public Vector2 WindowTitleAlign { get { return (Vector2)NativePtr->WindowTitleAlign; } set { NativePtr->WindowTitleAlign = value; } }
        public float ChildRounding { get { return (float)NativePtr->ChildRounding; } set { NativePtr->ChildRounding = value; } }
        public float ChildBorderSize { get { return (float)NativePtr->ChildBorderSize; } set { NativePtr->ChildBorderSize = value; } }
        public float PopupRounding { get { return (float)NativePtr->PopupRounding; } set { NativePtr->PopupRounding = value; } }
        public float PopupBorderSize { get { return (float)NativePtr->PopupBorderSize; } set { NativePtr->PopupBorderSize = value; } }
        public Vector2 FramePadding { get { return (Vector2)NativePtr->FramePadding; } set { NativePtr->FramePadding = value; } }
        public float FrameRounding { get { return (float)NativePtr->FrameRounding; } set { NativePtr->FrameRounding = value; } }
        public float FrameBorderSize { get { return (float)NativePtr->FrameBorderSize; } set { NativePtr->FrameBorderSize = value; } }
        public Vector2 ItemSpacing { get { return (Vector2)NativePtr->ItemSpacing; } set { NativePtr->ItemSpacing = value; } }
        public Vector2 ItemInnerSpacing { get { return (Vector2)NativePtr->ItemInnerSpacing; } set { NativePtr->ItemInnerSpacing = value; } }
        public Vector2 TouchExtraPadding { get { return (Vector2)NativePtr->TouchExtraPadding; } set { NativePtr->TouchExtraPadding = value; } }
        public float IndentSpacing { get { return (float)NativePtr->IndentSpacing; } set { NativePtr->IndentSpacing = value; } }
        public float ColumnsMinSpacing { get { return (float)NativePtr->ColumnsMinSpacing; } set { NativePtr->ColumnsMinSpacing = value; } }
        public float ScrollbarSize { get { return (float)NativePtr->ScrollbarSize; } set { NativePtr->ScrollbarSize = value; } }
        public float ScrollbarRounding { get { return (float)NativePtr->ScrollbarRounding; } set { NativePtr->ScrollbarRounding = value; } }
        public float GrabMinSize { get { return (float)NativePtr->GrabMinSize; } set { NativePtr->GrabMinSize = value; } }
        public float GrabRounding { get { return (float)NativePtr->GrabRounding; } set { NativePtr->GrabRounding = value; } }
        public Vector2 ButtonTextAlign { get { return (Vector2)NativePtr->ButtonTextAlign; } set { NativePtr->ButtonTextAlign = value; } }
        public Vector2 DisplayWindowPadding { get { return (Vector2)NativePtr->DisplayWindowPadding; } set { NativePtr->DisplayWindowPadding = value; } }
        public Vector2 DisplaySafeAreaPadding { get { return (Vector2)NativePtr->DisplaySafeAreaPadding; } set { NativePtr->DisplaySafeAreaPadding = value; } }
        public float MouseCursorScale { get { return (float)NativePtr->MouseCursorScale; } set { NativePtr->MouseCursorScale = value; } }
        public Bool8 AntiAliasedLines { get { return new Bool8(NativePtr->AntiAliasedLines); } set { NativePtr->AntiAliasedLines = (byte)(value ? 1 : 0); } }
        public Bool8 AntiAliasedFill { get { return new Bool8(NativePtr->AntiAliasedFill); } set { NativePtr->AntiAliasedFill = (byte)(value ? 1 : 0); } }
        public float CurveTessellationTol { get { return (float)NativePtr->CurveTessellationTol; } set { NativePtr->CurveTessellationTol = value; } }
        public RangeAccessor<Vector4> Colors => new RangeAccessor<Vector4>(&NativePtr->Colors_0, 43);
        public void ScaleAllSizes(float scale_factor)
        {
            ImGuiNative.ImGuiStyle_ScaleAllSizes(NativePtr, scale_factor);
        }
    }
}
