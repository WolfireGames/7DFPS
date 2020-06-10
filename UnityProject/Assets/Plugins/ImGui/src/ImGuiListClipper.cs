using System;

namespace ImGuiNET
{
    public unsafe struct ImGuiListClipperNative
    {
        public float StartPosY;
        public float ItemsHeight;
        public int ItemsCount;
        public int StepNo;
        public int DisplayStart;
        public int DisplayEnd;
    };


    public unsafe class ImGuiListClipper : IDisposable
    {
        ImGuiListClipperNative *nativePtr;


        public float StartPosY    { get { return nativePtr->StartPosY;    } set { nativePtr->StartPosY    = value; } }
        public float ItemsHeight  { get { return nativePtr->ItemsHeight;  } set { nativePtr->ItemsHeight  = value; } }
        public int   ItemsCount   { get { return nativePtr->ItemsCount;   } set { nativePtr->ItemsCount   = value; } }
        public int   StepNo       { get { return nativePtr->StepNo;       } set { nativePtr->StepNo       = value; } }
        public int   DisplayStart { get { return nativePtr->DisplayStart; } set { nativePtr->DisplayStart = value; } }
        public int   DisplayEnd   { get { return nativePtr->DisplayEnd;   } set { nativePtr->DisplayEnd   = value; } }


        public ImGuiListClipper(int items_count = -1, float items_height = -1.0f)
        {
            nativePtr = ImGuiNative.ImGuiListClipper_ImGuiListClipper(items_count, items_height);
        }


        public bool Step()
        {
            byte retval = ImGuiNative.ImGuiListClipper_Step(nativePtr);
            return retval != 0;
        }


        public void Begin(int items_count, float items_height = -1.0f)
        {
            ImGuiNative.ImGuiListClipper_Begin(nativePtr, items_count, items_height);
        }


        public void End()
        {
            ImGuiNative.ImGuiListClipper_End(nativePtr);
        }


        public void Dispose()
        {
            if (nativePtr != null)
            {
                ImGuiNative.ImGuiListClipper_destroy(nativePtr);
                nativePtr = null;
            }
        }
    }
}
