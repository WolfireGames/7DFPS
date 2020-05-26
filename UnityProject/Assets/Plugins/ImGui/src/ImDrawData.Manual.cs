using System;

namespace ImGuiNET
{

    public unsafe partial struct ImDrawDataPtr
    {
        public ImDrawListPtr getDrawListPtr(int index)
        {
            if (index < 0 || index >= NativePtr->CmdListsCount)
            {
                throw new IndexOutOfRangeException();
            }

            return NativePtr->CmdLists[index];
        }

    }
}
