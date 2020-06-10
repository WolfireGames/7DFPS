using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe struct ImGuiInputTextCallbackDataNative
    {
        public ImGuiInputTextFlags EventFlag;
        public ImGuiInputTextFlags Flags;
        public void* UserData;
        public ushort EventChar;
        public ImGuiKey EventKey;
        public byte* Buf;
        public int BufTextLen;
        public int BufSize;
        public byte BufDirty;
        public int CursorPos;
        public int SelectionStart;
        public int SelectionEnd;
    };

    public unsafe struct ImGuiInputTextCallbackData
    {
        public ImGuiInputTextCallbackDataNative* NativePtr { get; }
        public ImGuiInputTextCallbackData(ImGuiInputTextCallbackDataNative* nativePtr) { NativePtr = nativePtr; }
        public ImGuiInputTextCallbackData(IntPtr nativePtr) { NativePtr = (ImGuiInputTextCallbackDataNative*)nativePtr; }
        public static implicit operator ImGuiInputTextCallbackData(ImGuiInputTextCallbackDataNative* nativePtr) { return new ImGuiInputTextCallbackData(nativePtr); }
        public static implicit operator ImGuiInputTextCallbackDataNative* (ImGuiInputTextCallbackData wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator ImGuiInputTextCallbackData(IntPtr nativePtr) { return new ImGuiInputTextCallbackData(nativePtr); }
        public ImGuiInputTextFlags EventFlag { get { return (ImGuiInputTextFlags)NativePtr->EventFlag; } set { NativePtr->EventFlag = value; } }
        public ImGuiInputTextFlags Flags { get { return (ImGuiInputTextFlags)NativePtr->Flags; } set { NativePtr->Flags = value; } }
        public IntPtr UserData { get { return (IntPtr)NativePtr->UserData; } set { NativePtr->UserData = (void*)value; } }
        public ushort EventChar { get { return (ushort)NativePtr->EventChar; } set { NativePtr->EventChar = value; } }
        public ImGuiKey EventKey { get { return (ImGuiKey)NativePtr->EventKey; } set { NativePtr->EventKey = value; } }
        public IntPtr Buf { get { return (IntPtr)NativePtr->Buf; } set { NativePtr->Buf = (byte*)value; } }
        public int BufTextLen { get { return (int)NativePtr->BufTextLen; } set { NativePtr->BufTextLen = value; } }
        public int BufSize { get { return (int)NativePtr->BufSize; } set { NativePtr->BufSize = value; } }
        public Bool8 BufDirty { get { return new Bool8(NativePtr->BufDirty); } set { NativePtr->BufDirty = (byte)(value ? 1 : 0); } }
        public int CursorPos { get { return (int)NativePtr->CursorPos; } set { NativePtr->CursorPos = value; } }
        public int SelectionStart { get { return (int)NativePtr->SelectionStart; } set { NativePtr->SelectionStart = value; } }
        public int SelectionEnd { get { return (int)NativePtr->SelectionEnd; } set { NativePtr->SelectionEnd = value; } }

        public string GetString(int pos)
        {
            if (pos < 1)
            {
                return "";
            }

            char[] buf = new char[pos];
            fixed (char* buf_ptr = &buf[0])
            {
                int buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                int buf_charCount = Encoding.UTF8.GetCharCount(NativePtr->Buf, buf_byteCount);
                Encoding.UTF8.GetChars(NativePtr->Buf, buf_byteCount, buf_ptr, buf_charCount);
            }
            return new string(buf);
        }

        public void DeleteChars(int pos, int bytes_count)
        {
            ImGuiNative.ImGuiInputTextCallbackData_DeleteChars(NativePtr, pos, bytes_count);
        }

        public bool HasSelection()
        {
            byte ret = ImGuiNative.ImGuiInputTextCallbackData_HasSelection(NativePtr);
            return ret != 0;
        }

        public void InsertChars(int pos, string text)
        {
            int text_byteCount = (text != null) ? Encoding.UTF8.GetByteCount(text) : 0;
            byte* native_text = stackalloc byte[text_byteCount + 1];
            fixed (char* text_ptr = text)
            {
                int native_text_offset = (text != null) ? Encoding.UTF8.GetBytes(text_ptr, text.Length, native_text, text_byteCount) : 0;
                native_text[native_text_offset] = 0;
            }
            native_text = (text != null) ? native_text : null;
            byte* native_text_end = null;
            ImGuiNative.ImGuiInputTextCallbackData_InsertChars(NativePtr, pos, native_text, native_text_end);
        }
    }
}
