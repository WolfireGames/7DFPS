using System;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiIOPtr
    {
        // ImGuiIO only stores a pointer and we have to manage the lifetime of its contents
        static NativeArray<byte> IniFileNameArray;

        // Sets the Ini file name
        // If you set it to non-empty you must also set it to empty at shutdown or Unity will complain about memory leak
        // ImGuiRenderer will do that for you, you only need to care if you use this directly
        public void SetIniFile(string filename)
        {
            if (IniFileNameArray.IsCreated)
            {
                IniFileNameArray.Dispose();
            }

            if (filename == null || filename == "")
            {
                NativePtr->IniFilename = null;
                return;
            }

            IniFileNameArray = new NativeArray<byte>(filename.Length + 1, Allocator.Persistent);
            for (int i = 0; i < filename.Length; i++)
            {
                IniFileNameArray[i] = (byte) filename[i];
            }
            IniFileNameArray[filename.Length] = 0;
            NativePtr->IniFilename = (byte *) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(IniFileNameArray);
        }
    }
}
