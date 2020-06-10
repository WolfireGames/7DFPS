using System;
using System.Text;
using UnityEngine;

namespace ImGuiNET
{
    public unsafe partial struct ImDrawListPtr
    {
        public void AddImageQuad(Texture texture, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            Vector2 uv_a = new Vector2();
            AddImageQuad(texture, a, b, c, d, uv_a);
        }

        public void AddImageQuad(Texture texture, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 uv_a)
        {
            Vector2 uv_b = new Vector2(1, 0);
            AddImageQuad(texture, a, b, c, d, uv_a, uv_b);
        }

        public void AddImageQuad(Texture texture, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 uv_a, Vector2 uv_b)
        {
            Vector2 uv_c = new Vector2(1, 1);
            AddImageQuad(texture, a, b, c, d, uv_a, uv_b, uv_c);
        }

        public void AddImageQuad(Texture texture, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 uv_a, Vector2 uv_b, Vector2 uv_c)
        {
            Vector2 uv_d = new Vector2(0, 1);
            AddImageQuad(texture, a, b, c, d, uv_a, uv_b, uv_c, uv_d);
        }

        public void AddImageQuad(Texture texture, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 uv_a, Vector2 uv_b, Vector2 uv_c, Vector2 uv_d)
        {
            uint col = 0xFFFFFFFF;
            AddImageQuad(texture, a, b, c, d, uv_a, uv_b, uv_c, uv_d, col);
        }

        public void AddImageQuad(Texture texture, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 uv_a, Vector2 uv_b, Vector2 uv_c, Vector2 uv_d, uint col)
        {
            ImGuiUnity.AddDrawListImageQuad(NativePtr, texture, a, b, c, d, uv_a, uv_b, uv_c, uv_d, col);
        }

        public void AddImage(Texture texture, Vector2 a, Vector2 b)
        {
            Vector2 uv_a = new Vector2();
            AddImage(texture, a, b, uv_a);
        }

        public void AddImage(Texture texture, Vector2 a, Vector2 b, Vector2 uv_a)
        {
            Vector2 uv_b = new Vector2(1, 1);
            AddImage(texture, a, b, uv_a, uv_b);
        }

        public void AddImage(Texture texture, Vector2 a, Vector2 b, Vector2 uv_a, Vector2 uv_b)
        {
            uint col = 0xFFFFFFFF;
            AddImage(texture, a, b, uv_a, uv_b, col);
        }

        public void AddImage(Texture texture, Vector2 a, Vector2 b, Vector2 uv_a, Vector2 uv_b, uint col)
        {
            ImGuiUnity.AddDrawListImage(NativePtr, texture, a, b, uv_a, uv_b, col);
        }

        public void AddImageRounded(Texture texture, Vector2 a, Vector2 b, Vector2 uv_a, Vector2 uv_b, uint col, float rounding)
        {
            int rounding_corners = (int)ImDrawCornerFlags.All;
            AddImageRounded(texture, a, b, uv_a, uv_b, col, rounding, rounding_corners);
        }

        public void AddImageRounded(Texture texture, Vector2 a, Vector2 b, Vector2 uv_a, Vector2 uv_b, uint col, float rounding, int rounding_corners)
        {
            ImGuiUnity.AddDrawListImageRounded(NativePtr, texture, a, b, uv_a, uv_b, col, rounding, rounding_corners);
         }

        public void AddText(Vector2 pos, uint col, string text_begin)
        {
            int text_begin_byteCount = Encoding.UTF8.GetByteCount(text_begin);
            byte* native_text_begin = stackalloc byte[text_begin_byteCount + 1];
            fixed (char* text_begin_ptr = text_begin)
            {
                int native_text_begin_offset = Encoding.UTF8.GetBytes(text_begin_ptr, text_begin.Length, native_text_begin, text_begin_byteCount);
                native_text_begin[native_text_begin_offset] = 0;
            }
            byte* native_text_end = null;
            ImGuiNative.ImDrawList_AddText(NativePtr, pos, col, native_text_begin, native_text_end);
        }

        public void AddText(ImFontPtr font, float font_size, Vector2 pos, uint col, string text_begin)
        {
            ImFont* native_font = font.NativePtr;
            int text_begin_byteCount = Encoding.UTF8.GetByteCount(text_begin);
            byte* native_text_begin = stackalloc byte[text_begin_byteCount + 1];
            fixed (char* text_begin_ptr = text_begin)
            {
                int native_text_begin_offset = Encoding.UTF8.GetBytes(text_begin_ptr, text_begin.Length, native_text_begin, text_begin_byteCount);
                native_text_begin[native_text_begin_offset] = 0;
            }
            byte* native_text_end = null;
            float wrap_width = 0.0f;
            Vector4* cpu_fine_clip_rect = null;
            ImGuiNative.ImDrawList_AddTextFontPtr(NativePtr, native_font, font_size, pos, col, native_text_begin, native_text_end, wrap_width, cpu_fine_clip_rect);
        }

        public void AddText(ImFontPtr font, float font_size, Vector2 pos, uint col, string text_begin, float wrap_width, Vector4 clip_rect)
        {
            ImFont* native_font = font.NativePtr;
            int text_begin_byteCount = Encoding.UTF8.GetByteCount(text_begin);
            byte* native_text_begin = stackalloc byte[text_begin_byteCount + 1];
            fixed (char* text_begin_ptr = text_begin)
            {
                int native_text_begin_offset = Encoding.UTF8.GetBytes(text_begin_ptr, text_begin.Length, native_text_begin, text_begin_byteCount);
                native_text_begin[native_text_begin_offset] = 0;
            }
            byte* native_text_end = null;
            Vector4* cpu_fine_clip_rect = &clip_rect;
            ImGuiNative.ImDrawList_AddTextFontPtr(NativePtr, native_font, font_size, pos, col, native_text_begin, native_text_end, wrap_width, cpu_fine_clip_rect);
        }
    }
}
