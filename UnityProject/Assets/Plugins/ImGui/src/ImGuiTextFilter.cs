using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace ImGuiNET
{
    public struct ImGuiTextFilter
    {
        public struct TextRange
        {
            public string range;

            public TextRange(string text) { range = text; }

            public void split(char separator, List<TextRange> output)
            {
                int wb = 0;
                int we = wb;
                while (we < range.Length)
                {
                    if (range[we] == separator)
                    {
                        output.Add(new TextRange(range.Substring(wb, we - wb)));
                        wb = we + 1;
                    }
                    we++;
                }
                if (wb != we)
                    output.Add(new TextRange(range.Substring(wb, we - wb)));
            }
        }

        static bool ImCharIsBlankA(char c) { return c == ' ' || c == '\t'; }

        public char[] InputBuf;
        public List<TextRange> Filters;
        public int CountGrep;

        public ImGuiTextFilter(string default_filter)
        {
            InputBuf = new char[256];
            Filters = new List<TextRange>();
            CountGrep = 0;
            if (default_filter != "" && default_filter.Length < 256)
            {
                Array.Copy(default_filter.ToCharArray(), InputBuf, default_filter.Length);
                Build();
            }
            else
            {
                InputBuf[0] = '\0';
            }
        }

        void Clear() { InputBuf[0] = '\0'; Build(); }

        public bool IsActive() { return Filters.Count > 0; }

        public bool Draw(string label = "Filter (inc,-exc)", float width = 0.0f)
        {
            if (width != 0.0f)
                ImGui.PushItemWidth(width);
            bool value_changed = ImGui.InputText(label, InputBuf);
            if (width != 0.0f)
                ImGui.PopItemWidth();
            if (value_changed)
                Build();
            return value_changed;
        }

        public bool PassFilter(string text = "")
        {
            if (Filters.Count == 0)
                return true;

            for (int i = 0; i != Filters.Count; i++)
            {
                TextRange f = Filters[i];
                if (f.range.Length == 0)
                    continue;
                if (f.range[0] == '-')
                {
                    // Subtract
                    if (text.IndexOf(f.range.Substring(1), StringComparison.CurrentCultureIgnoreCase) != -1)
                        return false;
                }
                else
                {
                    // Grep
                    if (text.IndexOf(f.range, StringComparison.CurrentCultureIgnoreCase) != -1)
                        return true;
                }
            }

            // Implicit * grep
            if (CountGrep == 0)
                return true;

            return false;
        }

        public void Build()
        {
            TextRange input_range = new TextRange(new string(InputBuf));
            input_range.split(',', Filters);

            CountGrep = 0;
            for (int i = 0; i != Filters.Count; i++)
            {
                TextRange f = Filters[i];
                int b = 0;
                int e = f.range.Length;
                while (b < e && ImCharIsBlankA(f.range[b]))
                    b++;
                while (e > b && ImCharIsBlankA(f.range[e - 1]))
                    e--;
                f.range = f.range.Substring(b, e - b);
                if (f.range.Length == 0)
                    continue;
                if (Filters[i].range[0] != '-')
                    CountGrep += 1;
            }
        }
    }
}
