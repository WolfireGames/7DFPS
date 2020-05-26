using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiPayload
    {
        public void* Data;
        public int DataSize;
        public uint SourceId;
        public uint SourceParentId;
        public int DataFrameCount;
        public fixed byte DataType[33];
        public byte Preview;
        public byte Delivery;
    }
    public unsafe partial struct ImGuiPayloadPtr
    {
        public ImGuiPayload* NativePtr { get; }
        public ImGuiPayloadPtr(ImGuiPayload* nativePtr) { NativePtr = nativePtr; }
        public ImGuiPayloadPtr(IntPtr nativePtr) { NativePtr = (ImGuiPayload*)nativePtr; }
        public static implicit operator ImGuiPayloadPtr(ImGuiPayload* nativePtr) { return new ImGuiPayloadPtr(nativePtr); }
        public static implicit operator ImGuiPayload* (ImGuiPayloadPtr wrappedPtr) { return wrappedPtr.NativePtr; }
        public static implicit operator ImGuiPayloadPtr(IntPtr nativePtr) { return new ImGuiPayloadPtr(nativePtr); }
        public IntPtr Data { get { return (IntPtr)NativePtr->Data; } set { NativePtr->Data = (void*)value; } }
        public int DataSize { get { return (int)NativePtr->DataSize; } set { NativePtr->DataSize = value; } }
        public uint SourceId { get { return (uint)NativePtr->SourceId; } set { NativePtr->SourceId = value; } }
        public uint SourceParentId { get { return (uint)NativePtr->SourceParentId; } set { NativePtr->SourceParentId = value; } }
        public int DataFrameCount { get { return (int)NativePtr->DataFrameCount; } set { NativePtr->DataFrameCount = value; } }
        public RangeAccessor<byte> DataType => new RangeAccessor<byte>(NativePtr->DataType, 33);
        public Bool8 Preview { get { return new Bool8(NativePtr->Preview); } set { NativePtr->Preview = (byte)(value ? 1 : 0); } }
        public Bool8 Delivery { get { return new Bool8(NativePtr->Delivery); } set { NativePtr->Delivery = (byte)(value ? 1 : 0); } }
        public void Clear()
        {
            ImGuiNative.ImGuiPayload_Clear(NativePtr);
        }
        public bool IsPreview()
        {
            byte ret = ImGuiNative.ImGuiPayload_IsPreview(NativePtr);
            return ret != 0;
        }
        public bool IsDataType(string type)
        {
            int type_byteCount = (type != null) ? Encoding.UTF8.GetByteCount(type) : 0;
            byte* native_type = stackalloc byte[type_byteCount + 1];
            fixed (char* type_ptr = type)
            {
                int native_type_offset = (type != null) ? Encoding.UTF8.GetBytes(type_ptr, type.Length, native_type, type_byteCount) : 0;
                native_type[native_type_offset] = 0;
            }
            native_type = (type != null) ? native_type : null;
            byte ret = ImGuiNative.ImGuiPayload_IsDataType(NativePtr, native_type);
            return ret != 0;
        }
        public bool IsDelivery()
        {
            byte ret = ImGuiNative.ImGuiPayload_IsDelivery(NativePtr);
            return ret != 0;
        }
    }
}
