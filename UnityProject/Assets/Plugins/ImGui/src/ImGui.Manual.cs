using System;
using System.Text;
using UnityEngine;

namespace ImGuiNET
{
    public static unsafe partial class ImGui
    {
        public static bool InputText(string label, char[] buf, ImGuiInputTextFlags flags = 0, ImGuiInputTextCallback callback = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int buf_byteCount = (buf != null) ? Encoding.UTF8.GetByteCount(buf) : 0;
            byte* native_buf = stackalloc byte[buf_byteCount + 1];
            fixed (char* buf_ptr = buf)
            {
                int native_buf_offset = (buf != null) ? Encoding.UTF8.GetBytes(buf_ptr, buf.Length, native_buf, buf_byteCount) : 0;
                native_buf[native_buf_offset] = 0;
            }
            native_buf = (buf != null) ? native_buf : null;
            void* native_user_data = null;
            byte ret = ImGuiNative.igInputText(native_label, native_buf, (uint)buf.Length, flags, callback, native_user_data);

            // Nonzero return value means ImGui has changed buffer contents and it needs to be copied back
            // If EnterReturnsTrue is set buffer contents might change regardless of return value
            if (ret != 0 || flags.HasFlag(ImGuiInputTextFlags.EnterReturnsTrue))
            {
                fixed (char* buf_ptr = &buf[0])
                {
                    int buf_charCount = Encoding.UTF8.GetCharCount(native_buf, buf_byteCount);
                    Encoding.UTF8.GetChars(native_buf, buf_byteCount, buf_ptr, buf_charCount);
                }
            }
            return ret != 0;
        }

        public static bool InputTextMultiline(string label, char[] buf, Vector2 size, ImGuiInputTextFlags flags = 0, ImGuiInputTextCallback callback = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int buf_byteCount = (buf != null) ? Encoding.UTF8.GetByteCount(buf) : 0;
            byte* native_buf = stackalloc byte[buf_byteCount + 1];
            fixed (char* buf_ptr = buf)
            {
                int native_buf_offset = (buf != null) ? Encoding.UTF8.GetBytes(buf_ptr, buf.Length, native_buf, buf_byteCount) : 0;
                native_buf[native_buf_offset] = 0;
            }
            native_buf = (buf != null) ? native_buf : null;
            void* user_data = null;
            byte ret = ImGuiNative.igInputTextMultiline(native_label, native_buf, (uint)buf.Length, size, flags, callback, user_data);

            // Nonzero return value means ImGui has changed buffer contents and it needs to be copied back
            // If EnterReturnsTrue is set buffer contents might change regardless of return value
            if (ret != 0 || flags.HasFlag(ImGuiInputTextFlags.EnterReturnsTrue))
            {
                fixed (char* buf_ptr = buf)
                {
                    int buf_charCount = Encoding.UTF8.GetCharCount(native_buf, buf_byteCount);
                    Encoding.UTF8.GetChars(native_buf, buf_byteCount, buf_ptr, buf_charCount);
                }
            }
            return ret != 0;
        }

        public static bool Begin(string name, ImGuiWindowFlags flags)
        {
            int name_byteCount = Encoding.UTF8.GetByteCount(name);
            byte* native_name = stackalloc byte[name_byteCount + 1];
            fixed (char* name_ptr = name)
            {
                int native_name_offset = Encoding.UTF8.GetBytes(name_ptr, name.Length, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            byte* p_open = null;
            byte ret = ImGuiNative.igBegin(native_name, p_open, flags);
            return ret != 0;
        }

        public static bool MenuItem(string label, bool enabled)
        {
            return MenuItem(label, string.Empty, false, enabled);
        }

        public static void Image(Texture texture)
        {
            Vector2 size = new Vector2(texture.width, texture.height);
            Image(texture, size);
        }

        public static void Image(Texture texture, Vector2 size)
        {
            Vector2 uv0 = new Vector2();
            Image(texture, size, uv0);
        }

        public static void Image(Texture texture, Vector2 size, Vector2 uv0)
        {
            Vector2 uv1 = new Vector2(1, 1);
            Image(texture, size, uv0, uv1);
        }

        public static void Image(Texture texture, Vector2 size, Vector2 uv0, Vector2 uv1)
        {
            Vector4 tint_col = new Vector4(1, 1, 1, 1);
            Image(texture, size, uv0, uv1, tint_col);
        }

        public static void Image(Texture texture, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 tint_col)
        {
            Vector4 border_col = new Vector4();
            Image(texture, size, uv0, uv1, tint_col, border_col);
        }

        public static void Image(Texture texture, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 tint_col, Vector4 border_col)
        {
            ImGuiUnity.Image(texture, size, uv0, uv1, tint_col, border_col);
        }

        public static bool ImageButton(Texture texture)
        {
            Vector2 size = new Vector2(texture.width, texture.height);
            return ImageButton(texture, size);
        }

        public static bool ImageButton(Texture texture, Vector2 size)
        {
            Vector2 uv0 = new Vector2();
            return ImageButton(texture, size, uv0);
        }

        public static bool ImageButton(Texture texture, Vector2 size, Vector2 uv0)
        {
            Vector2 uv1 = new Vector2(1, 1);
            return ImageButton(texture, size, uv0, uv1);
        }

        public static bool ImageButton(Texture texture, Vector2 size, Vector2 uv0, Vector2 uv1)
        {
            int frame_padding = -1;
            return ImageButton(texture, size, uv0, uv1, frame_padding);
        }

        public static bool ImageButton(Texture texture, Vector2 size, Vector2 uv0, Vector2 uv1, int frame_padding)
        {
            Vector4 bg_col = new Vector4();
            return ImageButton(texture, size, uv0, uv1, frame_padding, bg_col);
        }

        public static bool ImageButton(Texture texture, Vector2 size, Vector2 uv0, Vector2 uv1, int frame_padding, Vector4 bg_col)
        {
            Vector4 tint_col = new Vector4(1, 1, 1, 1);
            return ImageButton(texture, size, uv0, uv1, frame_padding, bg_col, tint_col);
        }

        public static bool ImageButton(Texture texture, Vector2 size, Vector2 uv0, Vector2 uv1, int frame_padding, Vector4 bg_col, Vector4 tint_col)
        {
            return ImGuiUnity.ImageButton(texture, size, uv0, uv1, frame_padding, bg_col, tint_col);
        }

        public static uint Col32(int r, int g, int b, int a)
        {
            int IM_COL32_R_SHIFT = 0;
            int IM_COL32_G_SHIFT = 8;
            int IM_COL32_B_SHIFT = 16;
            int IM_COL32_A_SHIFT = 24;
            return (uint) ((a << IM_COL32_A_SHIFT) | (b << IM_COL32_B_SHIFT) | (g << IM_COL32_G_SHIFT) | (r << IM_COL32_R_SHIFT));
        }


        public static uint Col32(Vector3 c)
        {
            int r = ((int) (c.x * 255.0f)) & 0xFF;
            int g = ((int) (c.y * 255.0f)) & 0xFF;
            int b = ((int) (c.z * 255.0f)) & 0xFF;
            return Col32(r, g, b, 255);
        }


        public static void PlotLines(string label, float[] values, int values_offset = 0, string overlay_text = null, float scale_min = float.MaxValue, float scale_max = float.MaxValue, Vector2 graph_size = new Vector2())
        {
            if (values_offset >= values.Length && values.Length != 0)
            {
                throw new ArgumentException("Offset is past end of array", $"Offset { values_offset } is past end of array { values.Length }");
            }
            else if (values_offset < 0)
            {
                throw new ArgumentException("Offset is negative", $"Offset { values_offset } is negative");
            }

            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;
            int overlay_text_byteCount = (overlay_text != null) ? Encoding.UTF8.GetByteCount(overlay_text) : 0;
            byte* native_overlay_text = stackalloc byte[overlay_text_byteCount + 1];
            fixed (char* overlay_text_ptr = overlay_text)
            {
                int native_overlay_text_offset = (overlay_text != null) ? Encoding.UTF8.GetBytes(overlay_text_ptr, overlay_text.Length, native_overlay_text, overlay_text_byteCount) : 0;
                native_overlay_text[native_overlay_text_offset] = 0;
            }
            native_overlay_text = (overlay_text != null) ? native_overlay_text : null;

            fixed (float* native_values = values)
            {
                ImGuiNative.igPlotLines(native_label, native_values, values.Length, values_offset, native_overlay_text, scale_min, scale_max, graph_size, sizeof(float));
            }
        }


        public static void PlotHistogram(string label, float[] values, int values_offset = 0, string overlay_text = null, float scale_min = float.MaxValue, float scale_max = float.MaxValue, Vector2 graph_size = new Vector2())
        {
            if (values_offset >= values.Length && values.Length != 0)
            {
                throw new ArgumentException("Offset is past end of array", $"Offset { values_offset } is past end of array { values.Length }");
            }
            else if (values_offset < 0)
            {
                throw new ArgumentException("Offset is negative", $"Offset { values_offset } is negative");
            }

            // C# stackalloc is stupid
            // it must be immediately assigned to a new variable
            // can't declare a variable and allocate inside a conditional block
            // so when label is null we still need a useless stack alloc and lots of conditionals
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int overlay_text_byteCount = (overlay_text != null) ? Encoding.UTF8.GetByteCount(overlay_text) : 0;
            byte* native_overlay_text = stackalloc byte[overlay_text_byteCount + 1];
            fixed (char* overlay_text_ptr = overlay_text)
            {
                int native_overlay_text_offset = (overlay_text != null) ? Encoding.UTF8.GetBytes(overlay_text_ptr, overlay_text.Length, native_overlay_text, overlay_text_byteCount) : 0;
                native_overlay_text[native_overlay_text_offset] = 0;
            }
            native_overlay_text = (overlay_text != null) ? native_overlay_text : null;

            fixed (float* native_values = values)
            {
                ImGuiNative.igPlotHistogramFloatPtr(native_label, native_values, values.Length, values_offset, native_overlay_text, scale_min, scale_max, graph_size, sizeof(float));
            }
        }


        public static bool Combo(string label, ref int current_item, string[] items, int popup_max_height_in_items = -1)
        {
            if (items.Length == 0)
            {
                throw new ArgumentException("Zero length items array", $"Zero length items array");
            }
            if (current_item >= items.Length)
            {
                throw new ArgumentException("Current item is past end of array", $"Current item { current_item } is past end of array { items.Length }");
            }

            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;
            int* items_byteCounts = stackalloc int[items.Length];
            int items_byteCount = 0;
            for (int i = 0; i < items.Length; i++)
            {
                string s = items[i];
                items_byteCounts[i] = Encoding.UTF8.GetByteCount(s);
                items_byteCount += items_byteCounts[i] + 1;
            }
            byte* native_items_data = stackalloc byte[items_byteCount];
            int offset = 0;
            for (int i = 0; i < items.Length; i++)
            {
                string s = items[i];
                fixed (char* sPtr = s)
                {
                    offset += Encoding.UTF8.GetBytes(sPtr, s.Length, native_items_data + offset, items_byteCounts[i]);
                    offset += 1;
                    native_items_data[offset] = 0;
                }
            }
            byte** native_items = stackalloc byte*[items.Length];
            offset = 0;
            for (int i = 0; i < items.Length; i++)
            {
                native_items[i] = &native_items_data[offset];
                offset += items_byteCounts[i] + 1;
            }
            fixed (int* native_current_item = &current_item)
            {
                byte ret = ImGuiNative.igCombo(native_label, native_current_item, native_items, items.Length, popup_max_height_in_items);
                return ret != 0;
            }
        }
        

        public static bool Combo(string label, ref int current_item, string items_separated_by_zeros, int popup_max_height_in_items = -1)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;
            int items_separated_by_zeros_byteCount = (items_separated_by_zeros != null) ? Encoding.UTF8.GetByteCount(items_separated_by_zeros) : 0;
            byte* native_items_separated_by_zeros = stackalloc byte[items_separated_by_zeros_byteCount + 1];
            fixed (char* items_separated_by_zeros_ptr = items_separated_by_zeros)
            {
                int native_items_separated_by_zeros_offset = (items_separated_by_zeros != null) ? Encoding.UTF8.GetBytes(items_separated_by_zeros_ptr, items_separated_by_zeros.Length, native_items_separated_by_zeros, items_separated_by_zeros_byteCount) : 0;
                native_items_separated_by_zeros[native_items_separated_by_zeros_offset] = 0;
            }
            native_items_separated_by_zeros = (items_separated_by_zeros != null) ? native_items_separated_by_zeros : null;
            fixed (int* native_current_item = &current_item)
            {
                byte ret = ImGuiNative.igComboStr(native_label, native_current_item, native_items_separated_by_zeros, popup_max_height_in_items);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref int value, float v_speed, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;
            void* v_min = null;
            void* v_max = null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (int* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.S32, native_v, v_speed, v_min, v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref int value, float v_speed, int v_min, int v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (int* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.S32, native_v, v_speed, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref uint value, float v_speed, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;
            void* v_min = null;
            void* v_max = null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (uint* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.U32, native_v, v_speed, v_min, v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref uint value, float v_speed, uint v_min, uint v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (uint* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.U32, native_v, v_speed, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref long value, float v_speed, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;
            void* v_min = null;
            void* v_max = null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (long* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.S64, native_v, v_speed, v_min, v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref long value, float v_speed, long v_min, long v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (long* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.S64, native_v, v_speed, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref ulong value, float v_speed, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;
            void* v_min = null;
            void* v_max = null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (ulong* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.U64, native_v, v_speed, v_min, v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref ulong value, float v_speed, ulong v_min, ulong v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (ulong* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.U64, native_v, v_speed, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref float value, float v_speed, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;
            void* v_min = null;
            void* v_max = null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (float* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.Float, native_v, v_speed, v_min, v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref float value, float v_speed, float v_min, float v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (float* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.Float, native_v, v_speed, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref double value, float v_speed, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;
            void* v_min = null;
            void* v_max = null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (double* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.Double, native_v, v_speed, v_min, v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool DragScalar(string label, ref double value, float v_speed, double v_min, double v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (double* native_v = &value)
            {
                byte ret = ImGuiNative.igDragScalar(native_label, ImGuiDataType.Double, native_v, v_speed, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref int value, int v_min, int v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (int* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.S32, native_v, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref int value, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (int* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.S32, native_v, null, null, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref uint value, uint v_min, uint v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (uint* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.U32, native_v, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref uint value, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (uint* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.U32, native_v, null, null, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref long value, long v_min, long v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (long* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.S64, native_v, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref long value, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (long* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.S64, native_v, null, null, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref ulong value, ulong v_min, ulong v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (ulong* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.U64, native_v, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref ulong value, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (ulong* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.U64, native_v, null, null, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref float value, float v_min, float v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (float* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.Float, native_v, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref float value, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (float* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.Float, native_v, null, null, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref double value, double v_min, double v_max, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (double* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.Double, native_v, &v_min, &v_max, native_format, power);
                return ret != 0;
            }
        }


        public static bool SliderScalar(string label, ref double value, float power = 1.0f, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (double* native_v = &value)
            {
                byte ret = ImGuiNative.igSliderScalar(native_label, ImGuiDataType.Double, native_v, null, null, native_format, power);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref int value, int step, int step_fast, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (int* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.S32, native_v, &step, &step_fast, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref int value, int step, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (int* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.S32, native_v, &step, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref int value, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (int* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.S32, native_v, null, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref uint value, uint step, uint step_fast, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (uint* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.U32, native_v, &step, &step_fast, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref uint value, uint step, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (uint* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.U32, native_v, &step, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref uint value, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (uint* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.U32, native_v, null, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref long value, long step, long step_fast, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (long* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.S64, native_v, &step, &step_fast, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref long value, long step, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (long* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.S64, native_v, &step, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref long value, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (long* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.S64, native_v, null, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref ulong value, ulong step, ulong step_fast, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (ulong* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.U64, native_v, &step, &step_fast, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref ulong value, ulong step, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (ulong* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.U64, native_v, &step, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref ulong value, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (ulong* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.U64, native_v, null, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref float value, float step, float step_fast, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (float* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.Float, native_v, &step, &step_fast, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref float value, float step, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (float* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.Float, native_v, &step, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref float value, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (float* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.Float, native_v, null, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref double value, double step, double step_fast, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (double* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.Double, native_v, &step, &step_fast, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref double value, double step, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (double* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.Double, native_v, &step, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static bool InputScalar(string label, ref double value, ImGuiInputTextFlags extra_flags = ImGuiInputTextFlags.None, string format = null)
        {
            int label_byteCount = (label != null) ? Encoding.UTF8.GetByteCount(label) : 0;
            byte* native_label = stackalloc byte[label_byteCount + 1];
            fixed (char* label_ptr = label)
            {
                int native_label_offset = (label != null) ? Encoding.UTF8.GetBytes(label_ptr, label.Length, native_label, label_byteCount) : 0;
                native_label[native_label_offset] = 0;
            }
            native_label = (label != null) ? native_label : null;

            int format_byteCount = (format != null) ? Encoding.UTF8.GetByteCount(format) : 0;
            byte* native_format = stackalloc byte[format_byteCount + 1];
            fixed (char* format_ptr = format)
            {
                int native_format_offset = (format != null) ? Encoding.UTF8.GetBytes(format_ptr, format.Length, native_format, format_byteCount) : 0;
                native_format[native_format_offset] = 0;
            }
            native_format = (format != null) ? native_format : null;

            fixed (double* native_v = &value)
            {
                byte ret = ImGuiNative.igInputScalar(native_label, ImGuiDataType.Double, native_v, null, null, native_format, extra_flags);
                return ret != 0;
            }
        }


        public static Vector4 GetStyleColorVec4(ImGuiCol idx)
        {
            Vector4* ret = ImGuiNative.igGetStyleColorVec4(idx);
            return *ret;
        }


        public static bool AcceptDragDropPayload(string type, ref Vector3 value, ImGuiDragDropFlags flags = ImGuiDragDropFlags.None)
        {
            int type_byteCount = (type != null) ? Encoding.UTF8.GetByteCount(type) : 0;
            if (type_byteCount > 32)
            {
                throw new ArgumentException("Type name too long", $"Type name \"{ type }\" is too long ({ type_byteCount } characters), maximum is 32");
            }
            byte* native_type = stackalloc byte[type_byteCount + 1];
            fixed (char* type_ptr = type)
            {
                int native_type_offset = (type != null) ? Encoding.UTF8.GetBytes(type_ptr, type.Length, native_type, type_byteCount) : 0;
                native_type[native_type_offset] = 0;
            }
            native_type = (type != null) ? native_type : null;
            ImGuiPayload* ret = ImGuiNative.igAcceptDragDropPayload(native_type, flags);

            if (ret == null)
            {
                return false;
            }

            if (ret->DataSize != 12)
            {
                return false;
            }

            float[] temp = new float[3];
            System.Runtime.InteropServices.Marshal.Copy((IntPtr) ret->Data, temp, 0, 3);
            value.x = temp[0];
            value.y = temp[1];
            value.z = temp[2];

            return true;
        }


        public static bool AcceptDragDropPayload(string type, ref Vector4 value, ImGuiDragDropFlags flags = ImGuiDragDropFlags.None)
        {
            int type_byteCount = (type != null) ? Encoding.UTF8.GetByteCount(type) : 0;
            if (type_byteCount > 32)
            {
                throw new ArgumentException("Type name too long", $"Type name \"{ type }\" is too long ({ type_byteCount } characters), maximum is 32");
            }
            byte* native_type = stackalloc byte[type_byteCount + 1];
            fixed (char* type_ptr = type)
            {
                int native_type_offset = (type != null) ? Encoding.UTF8.GetBytes(type_ptr, type.Length, native_type, type_byteCount) : 0;
                native_type[native_type_offset] = 0;
            }
            native_type = (type != null) ? native_type : null;
            ImGuiPayload* ret = ImGuiNative.igAcceptDragDropPayload(native_type, flags);

            if (ret == null)
            {
                return false;
            }

            if (ret->DataSize != 16)
            {
                return false;
            }

            float[] temp = new float[4];
            System.Runtime.InteropServices.Marshal.Copy((IntPtr) ret->Data, temp, 0, 4);
            value.x = temp[0];
            value.y = temp[1];
            value.z = temp[2];
            value.w = temp[3];

            return true;
        }


        public static bool AcceptDragDropPayload(string type, ref int value, ImGuiDragDropFlags flags = ImGuiDragDropFlags.None)
        {
            int type_byteCount = (type != null) ? Encoding.UTF8.GetByteCount(type) : 0;
            if (type_byteCount > 32)
            {
                throw new ArgumentException("Type name too long", $"Type name \"{ type }\" is too long ({ type_byteCount } characters), maximum is 32");
            }
            byte* native_type = stackalloc byte[type_byteCount + 1];
            fixed (char* type_ptr = type)
            {
                int native_type_offset = (type != null) ? Encoding.UTF8.GetBytes(type_ptr, type.Length, native_type, type_byteCount) : 0;
                native_type[native_type_offset] = 0;
            }
            native_type = (type != null) ? native_type : null;
            ImGuiPayload* ret = ImGuiNative.igAcceptDragDropPayload(native_type, flags);

            if (ret == null)
            {
                return false;
            }

            if (ret->DataSize != 4)
            {
                return false;
            }
            value = System.Runtime.InteropServices.Marshal.ReadInt32((IntPtr) ret->Data);
            return true;
        }


        public static bool SetDragDropPayload(string type, int value, ImGuiCond cond = 0)
        {
            int type_byteCount = (type != null) ? Encoding.UTF8.GetByteCount(type) : 0;
            byte* native_type = stackalloc byte[type_byteCount + 1];
            fixed (char* type_ptr = type)
            {
                int native_type_offset = (type != null) ? Encoding.UTF8.GetBytes(type_ptr, type.Length, native_type, type_byteCount) : 0;
                native_type[native_type_offset] = 0;
            }
            native_type = (type != null) ? native_type : null;
            void* native_data = &value;
            byte ret = ImGuiNative.igSetDragDropPayload(native_type, native_data, 4, cond);
            return ret != 0;
        }


        public static ImFontPtr AddFontFromFileTTF(ImFontAtlasPtr fonts, string filename, float size_pixels, ImFontConfig font_cfg, ushort[] glyph_ranges)
        {
            int filename_byteCount = (filename != null) ? Encoding.UTF8.GetByteCount(filename) : 0;
            byte* native_filename = stackalloc byte[filename_byteCount + 1];
            fixed (char* filename_ptr = filename)
            {
                int native_filename_offset = (filename != null) ? Encoding.UTF8.GetBytes(filename_ptr, filename.Length, native_filename, filename_byteCount) : 0;
                native_filename[native_filename_offset] = 0;
            }
            native_filename = (filename != null) ? native_filename : null;
            ImFontConfigNative* native_font_cfg = font_cfg.NativePtr;
            fixed (ushort* native_glyph_ranges = glyph_ranges)
            {
                ImFont* ret = ImGuiNative.ImFontAtlas_AddFontFromFileTTF(fonts.NativePtr, native_filename, size_pixels, native_font_cfg, native_glyph_ranges);
                return new ImFontPtr(ret);
            }
        }


    }
}
