using ImGuiNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ImGUIDemoWindow : MonoBehaviour
{
    public bool showDemoWindow = true;

    // Examples Apps (accessible from the "Examples" menu)
    bool show_app_main_menu_bar = false;
    bool show_app_console = false;
    bool show_app_log = false;
    bool show_app_layout = false;
    bool show_app_property_editor = false;
    bool show_app_long_text = false;
    bool show_app_auto_resize = false;
    bool show_app_constrained_resize = false;
    bool show_app_simple_overlay = false;
    bool show_app_window_titles = false;
    bool show_app_custom_rendering = false;


    // Demonstrate the various window flags. Typically you would just use the default!
    bool no_titlebar = false;
    bool no_scrollbar = false;
    bool no_menu = false;
    bool no_move = false;
    bool no_resize = false;
    bool no_collapse = false;
    bool no_close = false;
    bool no_nav = false;


    // Dear ImGui Apps (accessible from the "Help" menu)
    static bool show_app_metrics = false;
    static bool show_app_style_editor = false;
    static bool show_app_about = false;


    // Static variables lifted from below for C#
    int clicked = 0;
    bool check = true;
    int e = 0;
    int counter = 0;
    float[] arr = { 0.6f, 0.1f, 1.0f, 0.5f, 0.92f, 0.1f, 0.2f };
    int item_current = 0;
    char[] str0 = "Hello, world!".ToCharArray();
    int i0 = 123;
    float f0 = 0.001f;
    double d0 = 999999.00000001;
    float f1 = 1e10f;
    Vector3 vec4a = new Vector3(0.10f, 0.20f, 0.30f);
    int i1 = 50, i2 = 42;
    float f11 = 1.00f, f2 = 0.0067f;
    int i11 = 0;
    float f13 = 0.123f, f21 = 0.0f;
    float angle = 0.0f;
    Vector3 col1 = new Vector3(1.0f, 0.0f, 0.2f);
    Vector4 col2 = new Vector4(0.4f, 0.7f, 0.0f, 0.5f);
    string[] listbox_items = { "Apple", "Banana", "Cherry", "Kiwi", "Mango", "Orange", "Pineapple", "Strawberry", "Watermelon" };
    int listbox_item_current = 1;
    bool align_label_with_current_x_position = false;
    int selection_mask = (1 << 2); // Dumb representation of what may be user-side selection state. You may carry selection state inside or outside your objects in whatever format you see fit.
    bool closable_group = true;

    int selected = 0;

    bool auto_resize = false;
    int type = 0;
    int display_lines = 10;

    int lines_autoresize = 10;

    int corner = 0;

    // ShowExampleMenuFile()
    bool menu_enabled = true;
    float menu_f = 0.5f;
    int menu_n = 0;
    bool b = true;

    float wrap_width = 200.0f;
    char[] buf = "\xe6\x97\xa5\xe6\x9c\xac\xe8\xaa\x9e".ToCharArray();
    int pressed_count = 0;

    float sz = 36.0f;
    float thickness = 4.0f;
    Vector3 col = new Vector3(1.0f, 1.0f, 0.4f);
    uint flags = 0;
    static string[] items = { "AAAA", "BBBB", "CCCC", "DDDD", "EEEE", "FFFF", "GGGG", "HHHH", "IIII", "JJJJ", "KKKK", "LLLLLLL", "MMMM", "OOOOOOO" };
    string item_current_1 = items[0];            // Here our selection is a single pointer stored outside the object.
    int item_current_2 = 0;
    int item_current_3 = -1; // If the selection isn't within 0..count, Combo won't display a preview
    bool[] selection = { false, true, false, false, false };
    int treenode_selected = -1;
    bool[] selection_1 = { false, false, false, false, false };
    bool[] selected_1 = { false, false, false };
    bool[] selected_2 = { false };
    bool[] selected_3 = { true, false, false, false, false, true, false, false, false, false, true, false, false, false, false, true };
    char[] buf1 = new char[64];
    char[] buf2 = new char[64];
    char[] buf3 = new char[64];
    char[] buf4 = new char[64];
    char[] buf5 = new char[64];
    char[] buf6 = new char[64];
    char[] bufpass = "password123".ToCharArray();
    bool read_only = false;
    char[] text = "/*\nThe Pentium F00F bug, shorthand for F0 0F C7 C8,\nthe hexadecimal encoding of one offending instruction,\nmore formally, the invalid operand with locked CMPXCHG8B\ninstruction bug, is a design flaw in the majority of\nIntel Pentium, Pentium MMX, and Pentium OverDrive\nprocessors (all in the P5 microarchitecture).\n*/\n\nlabel:\n\tlock cmpxchg8b eax\n".ToCharArray();
    bool animate = true;
    float[] values = new float[90];
    int values_offset = 0;
    double refresh_time = 0.0;
    float phase = 0.0f;
    float progress = 0.0f, progress_dir = 1.0f;
    Vector3 color3 = new Vector4(114, 144, 154);
    Vector4 color = new Vector4(114, 144, 154, 200);
    bool alpha_preview = true;
    bool alpha_half_preview = false;
    bool drag_and_drop = true;
    bool options_menu = true;
    bool hdr = false;
    bool saved_palette_inited = false;
    Vector4[] saved_palette = new Vector4[32];
    Vector4 backup_color;
    bool alpha = true;
    bool alpha_bar = true;
    bool side_preview = true;
    bool ref_color = false;
    Vector4 ref_color_v = new Vector4(1.0f, 0.0f, 1.0f, 0.5f);
    int inputs_mode = 2;
    int picker_mode = 0;
    float begin = 10, end = 90;
    int begin_i = 100, end_i = 1000;

    Vector2 vec2f = new Vector2(0.10f, 0.20f);
    Vector3 vec3f = new Vector3(0.10f, 0.20f, 0.30f);
    Vector4 vec4f = new Vector4(0.10f, 0.20f, 0.30f, 0.44f);
    int[] vec2i = { 1, 5 };
    int[] vec3i = { 1, 5, 100 };
    int[] vec4i = { 1, 5, 100, 255 };
    const float spacing = 4;
    int int_value = 0;
    float[] values_1 = { 0.0f, 0.60f, 0.35f, 0.9f, 0.70f, 0.20f, 0.0f };
    float[] values2 = { 0.20f, 0.80f, 0.40f, 0.25f };
    const int rows = 3;
    Vector2 small_slider_size = new Vector2(18, (160.0f - (rows - 1) * spacing) / rows);

    int item_type = 1;
    Vector4 col4f = new Vector4(1.0f, 0.5f, 0.0f, 1.0f);
    string[] items2 = { "Apple", "Banana", "Cherry", "Kiwi" };
    int current = 1;
    bool embed_all_inside_a_child_window = false;
    bool test_window = false;

    bool disable_mouse_wheel = false;
    bool disable_menu = false;
    int line = 50;
    float f = 0.0f;
    bool c1 = false, c2 = false, c3 = false, c4 = false;
    float f02 = 1.0f, f12 = 2.0f, f22 = 3.0f;
    string[] items3 = { "AAAA", "BBBB", "CCCC", "DDDD" };
    int item = -1;
    int[] selection2 = { 0, 1, 2, 3 };
    Vector2 button_sz = new Vector2(40, 40);
    bool track = true;
    int track_line = 50, scroll_to_px = 200;
    int lines = 7;
    Vector2 size = new Vector2(100, 100);
    Vector2 offset = new Vector2(50, 20);
    int selected_fish = -1;
    string[] names = { "Bream", "Haddock", "Mackerel", "Pollock", "Tilefish" };
    bool[] toggles = { true, false, false, false, false };
    float value_context = 0.5f;
    char[] name_context = "Label1".ToCharArray();
    bool dont_ask_me_next_time = false;
    int item_modal = 1;
    Vector4 color_modal = new Vector4(0.4f, 0.7f, 0.0f, 0.5f);
    int selected_columns = -1;
    float foo = 1.0f;
    float bar = 1.0f;
    bool h_borders = true;
    bool v_borders = true;

    List<Vector2> points = new List<Vector2>();
    bool adding_line = false;

    char[] buf_tabbing = stringToSizedCharArray("dummy", 32);

    char[] buf_focus = stringToSizedCharArray("click on a button to set focus", 128);
    Vector3 f3 = new Vector3(0.0f, 0.0f, 0.0f);

    Vector3 col1_dnd = new Vector3(1.0f, 0.0f, 0.2f);
    Vector4 col2_dnd = new Vector4(0.4f, 0.7f, 0.0f, 0.5f);
    string[] names_dnd = { "Bobby", "Beatrice", "Betty", "Brianna", "Barry", "Bernard", "Bibi", "Blaine", "Bryn" };

    enum ModeDnd
    {
        Mode_Copy,
        Mode_Move,
        Mode_Swap
    };
    ModeDnd mode_dnd = ModeDnd.Mode_Copy;

    static int FilterImGuiLetters(ImGuiInputTextCallbackData data)
    {
        string imgui = "imgui";
        char[] eventchar = new char[1];
        eventchar[0] = (char)data.EventChar;
        if (data.EventChar < 256 && imgui.Contains(new string(eventchar)))
            return 0;
        return 1;
    }
    ImGuiInputTextCallback filterImGuiLetters = FilterImGuiLetters;

    // State
    int s32_v = -1;
    uint u32_v = uint.MaxValue;
    long s64_v = -1;
    ulong u64_v = ulong.MaxValue;
    float f32_v = 0.123f;
    double f64_v = 90000.01234567890123456789;
    bool drag_clamp = false;
    bool inputs_step = true;

    int test_type = 0;
    StringBuilder log;
    int log_lines = 0;


    static char[] stringToSizedCharArray(string str, int size)
    {
        var arr = new char[size];

        for (int i = 0; i < Math.Min(str.Length, size); i++)
        {
            arr[i] = str[i];
        }

        return arr;
    }


    unsafe struct CustomConstraints // Helper functions to demonstrate programmatic constraints
    {
        public static void Square(ImGuiSizeCallbackData* data) { data->DesiredSize = new Vector2(Math.Max(data->DesiredSize.x, data->DesiredSize.y), Math.Max(data->DesiredSize.x, data->DesiredSize.y)); }
        public static void Step(ImGuiSizeCallbackData* data) { float step = (float)(int)(IntPtr)data->UserData; data->DesiredSize = new Vector2((int)(data->DesiredSize.x / step + 0.5f) * step, (int)(data->DesiredSize.y / step + 0.5f) * step); }

        public static ImGuiSizeCallback SquareDelegate = new ImGuiSizeCallback(Square);
        public static ImGuiSizeCallback StepDelegate = new ImGuiSizeCallback(Step);
    };


    static ExampleAppConsole console = new ExampleAppConsole();
    static System.Random rand = new System.Random();
    static ExampleAppLog applog = new ExampleAppLog();
    double last_time = -1.0;


    // Use this for initialization
    void Start()
    {
        log = new StringBuilder();
        applog.Clear();
    }


    // Update is called once per frame
    void Update()
    {
        ShowDemoWindow(ref showDemoWindow);
    }

    // Helper to display a little (?) mark which shows a tooltip when hovered.
    void ShowHelpMarker(string desc)
    {
        ImGui.TextDisabled("(?)");
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
            ImGui.TextUnformatted(desc);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }

    // Helper to display basic user controls.
    void ShowUserGuide()
    {
        ImGui.BulletText("Double-click on title bar to collapse window.");
        ImGui.BulletText("Click and drag on lower right corner to resize window\n(double-click to auto fit window to its contents).");
        ImGui.BulletText("Click and drag on any empty space to move window.");
        ImGui.BulletText("TAB/SHIFT+TAB to cycle through keyboard editable fields.");
        ImGui.BulletText("CTRL+Click on a slider or drag box to input value as text.");
        if (ImGui.GetIO().FontAllowUserScaling)
            ImGui.BulletText("CTRL+Mouse Wheel to zoom window contents.");
        ImGui.BulletText("Mouse Wheel to scroll.");
        ImGui.BulletText("While editing text:\n");
        ImGui.Indent();
        ImGui.BulletText("Hold SHIFT or use mouse to select text.");
        ImGui.BulletText("CTRL+Left/Right to word jump.");
        ImGui.BulletText("CTRL+A or double-click to select all.");
        ImGui.BulletText("CTRL+X,CTRL+C,CTRL+V to use clipboard.");
        ImGui.BulletText("CTRL+Z,CTRL+Y to undo/redo.");
        ImGui.BulletText("ESCAPE to revert.");
        ImGui.BulletText("You can apply arithmetic operators +,*,/ on numerical values.\nUse +- to subtract.");
        ImGui.Unindent();
    }

    //-----------------------------------------------------------------------------
    // [SECTION] Demo Window / ShowDemoWindow()
    //-----------------------------------------------------------------------------

    // Demonstrate most Dear ImGui features (this is big function!)
    // You may execute this function to experiment with the UI and understand what it does. You may then search for keywords in the code when you are interested by a specific feature.
    void ShowDemoWindow(ref bool p_open)
    {
        // Examples Apps (accessible from the "Examples" menu)
        if (show_app_main_menu_bar) ShowExampleAppMainMenuBar();
        if (show_app_console) ShowExampleAppConsole(ref show_app_console);
        if (show_app_log) ShowExampleAppLog(ref show_app_log);
        if (show_app_layout) ShowExampleAppLayout(ref show_app_layout);
        if (show_app_property_editor) ShowExampleAppPropertyEditor(ref show_app_property_editor);
        if (show_app_long_text) ShowExampleAppLongText(ref show_app_long_text);
        if (show_app_auto_resize) ShowExampleAppAutoResize(ref show_app_auto_resize);
        if (show_app_constrained_resize) ShowExampleAppConstrainedResize(ref show_app_constrained_resize);
        if (show_app_simple_overlay) ShowExampleAppSimpleOverlay(ref show_app_simple_overlay);
        if (show_app_window_titles) ShowExampleAppWindowTitles(ref show_app_window_titles);
        if (show_app_custom_rendering) ShowExampleAppCustomRendering(ref show_app_custom_rendering);

        if (show_app_metrics) { ImGui.ShowMetricsWindow(ref show_app_metrics); }
        if (show_app_style_editor) { ImGui.Begin("Style Editor", ref show_app_style_editor); ImGui.ShowStyleEditor(); ImGui.End(); }
        if (show_app_about)
        {
            ImGui.Begin("About Dear ImGui", ref show_app_about, ImGuiWindowFlags.AlwaysAutoResize);
            ImGui.Text("Dear ImGui " + ImGui.GetVersion());
            ImGui.Separator();
            ImGui.Text("By Omar Cornut and all dear imgui contributors.");
            ImGui.Text("Dear ImGui is licensed under the MIT License, see LICENSE for more information.");
            ImGui.End();
        }

        // Demonstrate the various window flags. Typically you would just use the default!
        ImGuiWindowFlags window_flags = 0;
        if (no_titlebar) window_flags |= ImGuiWindowFlags.NoTitleBar;
        if (no_scrollbar) window_flags |= ImGuiWindowFlags.NoScrollbar;
        if (!no_menu) window_flags |= ImGuiWindowFlags.MenuBar;
        if (no_move) window_flags |= ImGuiWindowFlags.NoMove;
        if (no_resize) window_flags |= ImGuiWindowFlags.NoResize;
        if (no_collapse) window_flags |= ImGuiWindowFlags.NoCollapse;
        if (no_nav) window_flags |= ImGuiWindowFlags.NoNav;

        // We specify a default position/size in case there's no data in the .ini file. Typically this isn't required! We only do it to make the Demo applications a little more welcoming.
        ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowSize(new Vector2(550, 680), ImGuiCond.FirstUseEver);

        // c# doesn't allow passing null reference, need some hacks
        bool mainWindowBegan;
        if (no_close)
        {
            mainWindowBegan = !ImGui.Begin("ImGui C# Demo", window_flags);
        }
        else
        {
            mainWindowBegan = !ImGui.Begin("ImGui C# Demo", ref p_open, window_flags);
        }

        // Main body of the Demo window starts here.
        if (mainWindowBegan)
        {
            // Early out if the window is collapsed, as an optimization.
            ImGui.End();
            return;
        }
        ImGui.Text($"dear imgui says hello. ({ImGui.GetVersion()})");

        // Most "big" widgets share a common width settings by default.
        //ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.65f);    // Use 2/3 of the space for widgets and 1/3 for labels (default)
        ImGui.PushItemWidth(ImGui.GetFontSize() * -12);           // Use fixed width for labels (by passing a negative value), the rest goes to widgets. We choose a width proportional to our font size.

        // Menu
        if (ImGui.BeginMenuBar())
        {
            if (ImGui.BeginMenu("Menu"))
            {
                ShowExampleMenuFile();
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Examples"))
            {
                ImGui.MenuItem("Main menu bar", null, ref show_app_main_menu_bar);
                ImGui.MenuItem("Console", null, ref show_app_console);
                ImGui.MenuItem("Log", null, ref show_app_log);
                ImGui.MenuItem("Simple layout", null, ref show_app_layout);
                ImGui.MenuItem("Property editor", null, ref show_app_property_editor);
                ImGui.MenuItem("Long text display", null, ref show_app_long_text);
                ImGui.MenuItem("Auto-resizing window", null, ref show_app_auto_resize);
                ImGui.MenuItem("Constrained-resizing window", null, ref show_app_constrained_resize);
                ImGui.MenuItem("Simple overlay", null, ref show_app_simple_overlay);
                ImGui.MenuItem("Manipulating window titles", null, ref show_app_window_titles);
                ImGui.MenuItem("Custom rendering", null, ref show_app_custom_rendering);
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Help"))
            {
                ImGui.MenuItem("Metrics", null, ref show_app_metrics);
                ImGui.MenuItem("Style Editor", null, ref show_app_style_editor);
                ImGui.MenuItem("About Dear ImGui", null, ref show_app_about);
                ImGui.EndMenu();
            }
            ImGui.EndMenuBar();
        }

        ImGui.Spacing();
        if (ImGui.CollapsingHeader("Help"))
        {
            ImGui.Text("PROGRAMMER GUIDE:");
            ImGui.BulletText("Please see the ShowDemoWindow() code in imgui_demo.cpp. <- you are here!");
            ImGui.BulletText("Please see the comments in imgui.cpp.");
            ImGui.BulletText("Please see the examples/ in application.");
            ImGui.BulletText("Enable 'io.ConfigFlags |= NavEnableKeyboard' for keyboard controls.");
            ImGui.BulletText("Enable 'io.ConfigFlags |= NavEnableGamepad' for gamepad controls.");
            ImGui.Separator();

            ImGui.Text("USER GUIDE:");
            ImGui.ShowUserGuide();
        }

        if (ImGui.CollapsingHeader("Configuration"))
        {
            ImGuiIOPtr io = ImGui.GetIO();

            if (ImGui.TreeNode("Configuration##2"))
            {
                uint configFlags = (uint)io.ConfigFlags;
                ImGui.CheckboxFlags("io.ConfigFlags: NavEnableKeyboard", ref configFlags, (uint)ImGuiConfigFlags.NavEnableKeyboard);
                ImGui.CheckboxFlags("io.ConfigFlags: NavEnableGamepad", ref configFlags, (uint)ImGuiConfigFlags.NavEnableGamepad);
                ImGui.SameLine(); ShowHelpMarker("Required back-end to feed in gamepad inputs in io.NavInputs[] and set io.BackendFlags |= ImGuiBackendFlags_HasGamepad.\n\nRead instructions in imgui.cpp for details.");
                ImGui.CheckboxFlags("io.ConfigFlags: NavEnableSetMousePos", ref configFlags, (uint)ImGuiConfigFlags.NavEnableSetMousePos);
                ImGui.SameLine(); ShowHelpMarker("Instruct navigation to move the mouse cursor. See comment for ImGuiConfigFlags_NavEnableSetMousePos.");
                ImGui.CheckboxFlags("io.ConfigFlags: NoMouse", ref configFlags, (uint)ImGuiConfigFlags.NoMouse);
                if ((io.ConfigFlags & ImGuiConfigFlags.NoMouse) != 0) // Create a way to restore this flag otherwise we could be stuck completely!
                {
                    if (((float)ImGui.GetTime() % 0.40f) < 0.20f)
                    {
                        ImGui.SameLine();
                        ImGui.Text("<<PRESS SPACE TO DISABLE>>");
                    }
                    if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Space)))
                        configFlags &= ~((uint)ImGuiConfigFlags.NoMouse);
                }
                ImGui.CheckboxFlags("io.ConfigFlags: NoMouseCursorChange", ref configFlags, (uint)ImGuiConfigFlags.NoMouseCursorChange);
                ImGui.SameLine(); ShowHelpMarker("Instruct back-end to not alter mouse cursor shape and visibility.");
                bool configInputTextCursorBlink = io.ConfigInputTextCursorBlink;
                ImGui.Checkbox("io.ConfigInputTextCursorBlink", ref configInputTextCursorBlink);
                io.ConfigInputTextCursorBlink = configInputTextCursorBlink;
                ImGui.SameLine(); ShowHelpMarker("Set to false to disable blinking cursor, for users who consider it distracting");
                bool configResizeWindowsFromEdges = io.ConfigResizeWindowsFromEdges;
                ImGui.Checkbox("io.ConfigResizeWindowsFromEdges [beta]", ref configResizeWindowsFromEdges);
                io.ConfigResizeWindowsFromEdges = configResizeWindowsFromEdges;
                ImGui.SameLine(); ShowHelpMarker("Enable resizing of windows from their edges and from the lower-left corner.\nThis requires (io.BackendFlags & ImGuiBackendFlags_HasMouseCursors) because it needs mouse cursor feedback.");
                bool mouseDrawCursor = io.MouseDrawCursor;
                ImGui.Checkbox("io.MouseDrawCursor", ref mouseDrawCursor);
                io.MouseDrawCursor = mouseDrawCursor;
                ImGui.SameLine(); ShowHelpMarker("Instruct Dear ImGui to render a mouse cursor for you. Note that a mouse cursor rendered via your application GPU rendering path will feel more laggy than hardware cursor, but will be more in sync with your other visuals.\n\nSome desktop applications may use both kinds of cursors (e.g. enable software cursor only when resizing/dragging something).");
                ImGui.TreePop();
                ImGui.Separator();
                io.ConfigFlags = (ImGuiConfigFlags)configFlags;
            }

            if (ImGui.TreeNode("Backend Flags"))
            {
                uint backend_flags = (uint)io.BackendFlags; // Make a local copy to avoid modifying the back-end flags.
                ImGui.CheckboxFlags("io.BackendFlags: HasGamepad", ref backend_flags, (uint)ImGuiBackendFlags.HasGamepad);
                ImGui.CheckboxFlags("io.BackendFlags: HasMouseCursors", ref backend_flags, (uint)ImGuiBackendFlags.HasMouseCursors);
                ImGui.CheckboxFlags("io.BackendFlags: HasSetMousePos", ref backend_flags, (uint)ImGuiBackendFlags.HasSetMousePos);
                ImGui.TreePop();
                ImGui.Separator();
            }

            if (ImGui.TreeNode("Style"))
            {
                ImGui.ShowStyleEditor();
                ImGui.TreePop();
                ImGui.Separator();
            }

            if (ImGui.TreeNode("Capture/Logging"))
            {
                ImGui.TextWrapped("The logging API redirects all text output so you can easily capture the content of a window or a block. Tree nodes can be automatically expanded.");
                ShowHelpMarker("Try opening any of the contents below in this window and then click one of the \"Log To\" button.");
                ImGui.LogButtons();
                ImGui.TextWrapped("You can also call ImGui.LogText() to output directly to the log without a visual output.");
                if (ImGui.Button("Copy \"Hello, world!\" to clipboard"))
                {
                    ImGui.LogToClipboard();
                    ImGui.LogText("Hello, world!");
                    ImGui.LogFinish();
                }
                ImGui.TreePop();
            }
        }

        if (ImGui.CollapsingHeader("Window options"))
        {
            ImGui.Checkbox("No titlebar", ref no_titlebar); ImGui.SameLine(150);
            ImGui.Checkbox("No scrollbar", ref no_scrollbar); ImGui.SameLine(300);
            ImGui.Checkbox("No menu", ref no_menu);
            ImGui.Checkbox("No move", ref no_move); ImGui.SameLine(150);
            ImGui.Checkbox("No resize", ref no_resize); ImGui.SameLine(300);
            ImGui.Checkbox("No collapse", ref no_collapse);
            ImGui.Checkbox("No close", ref no_close); ImGui.SameLine(150);
            ImGui.Checkbox("No nav", ref no_nav);
        }

        if (ImGui.CollapsingHeader("Widgets"))
        {
            if (ImGui.TreeNode("Basic"))
            {
                if (ImGui.Button("Button"))
                    clicked++;
                if (clicked % 2 == 0)
                {
                    ImGui.SameLine();
                    ImGui.Text("Thanks for clicking me!");
                }

                ImGui.Checkbox("checkbox", ref check);

                ImGui.RadioButton("radio a", ref e, 0); ImGui.SameLine();
                ImGui.RadioButton("radio b", ref e, 1); ImGui.SameLine();
                ImGui.RadioButton("radio c", ref e, 2);

                // Color buttons, demonstrate using PushID() to add unique identifier in the ID stack, and changing style.
                for (int i = 0; i < 7; i++)
                {
                    if (i > 0) ImGui.SameLine();
                    ImGui.PushID(i);
                    ImGui.PushStyleColor(ImGuiCol.Button, Color.HSVToRGB(i / 7.0f, 0.6f, 0.6f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.HSVToRGB(i / 7.0f, 0.7f, 0.7f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonActive, Color.HSVToRGB(i / 7.0f, 0.8f, 0.8f));
                    ImGui.Button("Click");
                    ImGui.PopStyleColor(3);
                    ImGui.PopID();
                }

                // Arrow buttons
                float spacing = ImGui.GetStyle().ItemInnerSpacing.x;
                ImGui.PushButtonRepeat(true);
                if (ImGui.ArrowButton("##left", ImGuiDir.Left)) { counter--; }
                ImGui.SameLine(0.0f, spacing);
                if (ImGui.ArrowButton("##right", ImGuiDir.Right)) { counter++; }
                ImGui.PopButtonRepeat();
                ImGui.SameLine();
                ImGui.Text("" + counter);

                ImGui.Text("Hover over me");
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("I am a tooltip");

                ImGui.SameLine();
                ImGui.Text("- or me");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("I am a fancy tooltip");
                    ImGui.PlotLines("Curve", arr);
                    ImGui.EndTooltip();
                }

                ImGui.Separator();

                ImGui.LabelText("label", "Value");

                {
                    // Using the _simplified_ one-liner Combo() api here
                    // See "Combo" section for examples of how to use the more complete BeginCombo()/EndCombo() api.
                    string[] items = { "AAAA", "BBBB", "CCCC", "DDDD", "EEEE", "FFFF", "GGGG", "HHHH", "IIII", "JJJJ", "KKKK", "LLLLLLL", "MMMM", "OOOOOOO" };
                    ImGui.Combo("combo", ref item_current, items);
                    ImGui.SameLine(); ShowHelpMarker("Refer to the \"Combo\" section below for an explanation of the full BeginCombo/EndCombo API, and demonstration of various flags.\n");
                }

                {
                    ImGui.InputText("input text", str0);
                    ImGui.SameLine(); ShowHelpMarker("USER:\nHold SHIFT or use mouse to select text.\nCTRL+Left/Right to word jump.\nCTRL+A or double-click to select all.\nCTRL+X,CTRL+C,CTRL+V clipboard.\nCTRL+Z,CTRL+Y undo/redo.\nESCAPE to revert.\n\nPROGRAMMER:\nYou can use the ImGuiInputTextFlags_CallbackResize facility if you need to wire InputText() to a dynamic string type. See misc/stl/imgui_stl.h for an example (this is not demonstrated in imgui_demo.cpp).");

                    ImGui.InputInt("input int", ref i0);
                    ImGui.SameLine(); ShowHelpMarker("You can apply arithmetic operators +,*,/ on numerical values.\n  e.g. [ 100 ], input \'*2\', result becomes [ 200 ]\nUse +- to subtract.\n");

                    ImGui.InputFloat("input float", ref f0, 0.01f, 1.0f);

                    ImGui.InputDouble("input double", ref d0, 0.01f, 1.0f, "%.8f");

                    ImGui.InputFloat("input scientific", ref f1, 0.0f, 0.0f, "%e");
                    ImGui.SameLine(); ShowHelpMarker("You can input value using the scientific notation,\n  e.g. \"1e+8\" becomes \"100000000\".\n");

                    ImGui.InputFloat3("input float3", ref vec4a);
                }

                {
                    ImGui.DragInt("drag int", ref i1, 1);
                    ImGui.SameLine(); ShowHelpMarker("Click and drag to edit value.\nHold SHIFT/ALT for faster/slower edit.\nDouble-click or CTRL+click to input value.");

                    ImGui.DragInt("drag int 0..100", ref i2, 1, 0, 100, "%d%%");

                    ImGui.DragFloat("drag float", ref f11, 0.005f);
                    ImGui.DragFloat("drag small float", ref f2, 0.0001f, 0.0f, 0.0f, "%.06f ns");
                }

                {
                    ImGui.SliderInt("slider int", ref i11, -1, 3);
                    ImGui.SameLine(); ShowHelpMarker("CTRL+click to input value.");

                    ImGui.SliderFloat("slider float", ref f13, 0.0f, 1.0f, "ratio = %.3f");
                    ImGui.SliderFloat("slider float (curve)", ref f21, -10.0f, 10.0f, "%.4f", 2.0f);
                    ImGui.SliderAngle("slider angle", ref angle);
                }

                {
                    ImGui.ColorEdit3("color 1", ref col1);
                    ImGui.SameLine(); ShowHelpMarker("Click on the colored square to open a color picker.\nClick and hold to use drag and drop.\nRight-click on the colored square to show options.\nCTRL+click on individual component to input value.\n");

                    ImGui.ColorEdit4("color 2", ref col2);
                }

                {
                    // List box
                    ImGui.ListBox("listbox\n(single select)", ref listbox_item_current, listbox_items, listbox_items.Length, 4);

                    //static int listbox_item_current2 = 2;
                    //ImGui.PushItemWidth(-1);
                    //ImGui.ListBox("##listbox2", ref listbox_item_current2, listbox_items, IM_ARRAYSIZE(listbox_items), 4);
                    //ImGui.PopItemWidth();
                }

                ImGui.TreePop();
            }

            // Testing ImGuiOnceUponAFrame helper.
            //static ImGuiOnceUponAFrame once;
            //for (int i = 0; i < 5; i++)
            //    if (once)
            //        ImGui.Text("This will be displayed only once.");

            if (ImGui.TreeNode("Trees"))
            {
                if (ImGui.TreeNode("Basic trees"))
                {
                    for (int i = 0; i < 5; i++)
                        if (ImGui.TreeNode((IntPtr)i, "Child" + i))
                        {
                            ImGui.Text("blah blah");
                            ImGui.SameLine();
                            if (ImGui.SmallButton("button")) { };
                            ImGui.TreePop();
                        }
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Advanced, with Selectable nodes"))
                {
                    ShowHelpMarker("This is a more standard looking tree with selectable nodes.\nClick to select, CTRL+Click to toggle, click on arrows or double-click to open.");
                    ImGui.Checkbox("Align label with current X position)", ref align_label_with_current_x_position);
                    ImGui.Text("Hello!");
                    if (align_label_with_current_x_position)
                        ImGui.Unindent(ImGui.GetTreeNodeToLabelSpacing());

                    int node_clicked = -1;                // Temporary storage of what node we have clicked to process selection at the end of the loop. May be a pointer to your own node type, etc.
                    ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, ImGui.GetFontSize() * 3); // Increase spacing to differentiate leaves from expanded contents.
                    for (int i = 0; i < 6; i++)
                    {
                        // Disable the default open on single-click behavior and pass in Selected flag according to our selection state.
                        ImGuiTreeNodeFlags node_flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ((selection_mask & (1 << i)) == 0 ? ImGuiTreeNodeFlags.Selected : 0);
                        if (i < 3)
                        {
                            // Node
                            bool node_open = ImGui.TreeNodeEx((IntPtr)i, node_flags, "Selectable Node %d");
                            if (ImGui.IsItemClicked())
                                node_clicked = i;
                            if (node_open)
                            {
                                ImGui.Text("Blah blah\nBlah Blah");
                                ImGui.TreePop();
                            }
                        }
                        else
                        {
                            // Leaf: The only reason we have a TreeNode at all is to allow selection of the leaf. Otherwise we can use BulletText() or TreeAdvanceToLabelPos()+Text().
                            node_flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen; // ImGuiTreeNodeFlags.Bullet
                            ImGui.TreeNodeEx((IntPtr)i, node_flags, "Selectable Leaf %d");
                            if (ImGui.IsItemClicked())
                                node_clicked = i;
                        }
                    }
                    if (node_clicked != -1)
                    {
                        // Update selection state. Process outside of tree loop to avoid visual inconsistencies during the clicking-frame.
                        if (ImGui.GetIO().KeyCtrl)
                            selection_mask ^= (1 << node_clicked);          // CTRL+click to toggle
                        else //if (!(selection_mask & (1 << node_clicked))) // Depending on selection behavior you want, this commented bit preserve selection when clicking on item that is part of the selection
                            selection_mask = (1 << node_clicked);           // Click to single-select
                    }
                    ImGui.PopStyleVar();
                    if (align_label_with_current_x_position)
                        ImGui.Indent(ImGui.GetTreeNodeToLabelSpacing());
                    ImGui.TreePop();
                }
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Collapsing Headers"))
            {
                ImGui.Checkbox("Enable extra group", ref closable_group);
                if (ImGui.CollapsingHeader("Header"))
                {
                    ImGui.Text("IsItemHovered " + ImGui.IsItemHovered());
                    for (int i = 0; i < 5; i++)
                        ImGui.Text("Some content " + i);
                }
                if (ImGui.CollapsingHeader("Header with a close button", ref closable_group))
                {
                    ImGui.Text("IsItemHovered " + ImGui.IsItemHovered());
                    for (int i = 0; i < 5; i++)
                        ImGui.Text("More content " + i);
                }
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Bullets"))
            {
                ImGui.BulletText("Bullet point 1");
                ImGui.BulletText("Bullet point 2\nOn multiple lines");
                ImGui.Bullet(); ImGui.Text("Bullet point 3 (two calls)");
                ImGui.Bullet(); ImGui.SmallButton("Button");
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Text"))
            {
                if (ImGui.TreeNode("Colored Text"))
                {
                    // Using shortcut. You can use PushStyleColor()/PopStyleColor() for more flexibility.
                    ImGui.TextColored(new Vector4(1.0f, 0.0f, 1.0f, 1.0f), "Pink");
                    ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), "Yellow");
                    ImGui.TextDisabled("Disabled");
                    ImGui.SameLine(); ShowHelpMarker("The TextDisabled color is stored in ImGuiStyle.");
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Word Wrapping"))
                {
                    // Using shortcut. You can use PushTextWrapPos()/PopTextWrapPos() for more flexibility.
                    ImGui.TextWrapped("This text should automatically wrap on the edge of the window. The current implementation for text wrapping follows simple rules suitable for English and possibly other languages.");
                    ImGui.Spacing();

                    ImGui.SliderFloat("Wrap width", ref wrap_width, -20, 600, "%.0f");

                    ImGui.Text("Test paragraph 1:");
                    Vector2 pos = ImGui.GetCursorScreenPos();
                    ImGui.GetWindowDrawList().AddRectFilled(new Vector2(pos.x + wrap_width, pos.y), new Vector2(pos.x + wrap_width + 10, pos.y + ImGui.GetTextLineHeight()), ImGui.Col32(255, 0, 255, 255));
                    ImGui.PushTextWrapPos(ImGui.GetCursorPos().x + wrap_width);
                    ImGui.Text($"The lazy dog is a good dog. This paragraph is made to fit within {wrap_width:#.} pixels. Testing a 1 character word. The quick brown fox jumps over the lazy dog.");
                    ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), ImGui.Col32(255, 255, 0, 255));
                    ImGui.PopTextWrapPos();

                    ImGui.Text("Test paragraph 2:");
                    pos = ImGui.GetCursorScreenPos();
                    ImGui.GetWindowDrawList().AddRectFilled(new Vector2(pos.x + wrap_width, pos.y), new Vector2(pos.x + wrap_width + 10, pos.y + ImGui.GetTextLineHeight()), ImGui.Col32(255, 0, 255, 255));
                    ImGui.PushTextWrapPos(ImGui.GetCursorPos().x + wrap_width);
                    ImGui.Text("aaaaaaaa bbbbbbbb, c cccccccc,dddddddd. d eeeeeeee   ffffffff. gggggggg!hhhhhhhh");
                    ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), ImGui.Col32(255, 255, 0, 255));
                    ImGui.PopTextWrapPos();

                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("UTF-8 Text"))
                {
                    // UTF-8 test with Japanese characters
                    // (Needs a suitable font, try Noto, or Arial Unicode, or M+ fonts. Read misc/fonts/README.txt for details.)
                    // - From C++11 you can use the u8"my text" syntax to encode literal strings as UTF-8
                    // - For earlier compiler, you may be able to encode your sources as UTF-8 (e.g. Visual Studio save your file as 'UTF-8 without signature')
                    // - FOR THIS DEMO FILE ONLY, BECAUSE WE WANT TO SUPPORT OLD COMPILERS, WE ARE *NOT* INCLUDING RAW UTF-8 CHARACTERS IN THIS SOURCE FILE.
                    //   Instead we are encoding a few strings with hexadecimal constants. Don't do this in your application!
                    //   Please use u8"text in any language" in your application!
                    // Note that characters values are preserved even by InputText() if the font cannot be displayed, so you can safely copy & paste garbled characters into another application.
                    ImGui.TextWrapped("CJK text will only appears if the font was loaded with the appropriate CJK character ranges. Call io.Font->LoadFromFileTTF() manually to load extra character ranges. Read misc/fonts/README.txt for details.");
                    ImGui.Text("Hiragana: \xe3\x81\x8b\xe3\x81\x8d\xe3\x81\x8f\xe3\x81\x91\xe3\x81\x93 (kakikukeko)"); // Normally we would use u8"blah blah" with the proper characters directly in the string.
                    ImGui.Text("Kanjis: \xe6\x97\xa5\xe6\x9c\xac\xe8\xaa\x9e (nihongo)");
                    //static char buf[32] = u8"NIHONGO"; // <- this is how you would write it with C++11, using real kanjis
                    ImGui.InputText("UTF-8 input", buf);
                    ImGui.TreePop();
                }
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Images"))
            {
                ImGui.TextWrapped("Below we are displaying the font texture (which is the only texture we have access to in this demo). Use the 'ImTextureID' type as storage to pass pointers or identifier to your own texture data. Hover the texture for a zoomed view!");

                // Here we are grabbing the font texture because that's the only one we have access to inside the demo code.
                // Remember that ImTextureID is just storage for whatever you want it to be, it is essentially a value that will be passed to the render function inside the ImDrawCmd structure.
                // If you use one of the default imgui_impl_XXXX.cpp renderer, they all have comments at the top of their file to specify what they expect to be stored in ImTextureID.
                // (for example, the imgui_impl_dx11.cpp renderer expect a 'ID3D11ShaderResourceView*' pointer. The imgui_impl_glfw_gl3.cpp renderer expect a GLuint OpenGL texture identifier etc.)
                // If you decided that ImTextureID = MyEngineTexture*, then you can pass your MyEngineTexture* pointers to ImGui.Image(), and gather width/height through your own functions, etc.
                // Using ShowMetricsWindow() as a "debugger" to inspect the draw data that are being passed to your render will help you debug issues if you are confused about this.
                // Consider using the lower-level ImDrawList::AddImage() API, via ImGui.GetWindowDrawList()->AddImage().
#if false
                ImGuiIOPtr io = ImGui.GetIO();

                Texture2D my_tex = ImGuiUnity.fontTexture;
                float my_tex_w = (float)io.Fonts.TexWidth;
                float my_tex_h = (float)io.Fonts.TexHeight;

                ImGui.Text($"{my_tex_w:#.}x{my_tex_h:#.}");
                Vector2 pos = ImGui.GetCursorScreenPos();
                ImGui.Image(my_tex, new Vector2(my_tex_w, my_tex_h), new Vector2(0,0), new Vector2(1,1), new Vector4(255,255,255,255), new Vector4(255,255,255,128));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    float region_sz = 32.0f;
                    float region_x = io.MousePos.x - pos.x - region_sz * 0.5f; if (region_x < 0.0f) region_x = 0.0f; else if (region_x > my_tex_w - region_sz) region_x = my_tex_w - region_sz;
                    float region_y = io.MousePos.y - pos.y - region_sz * 0.5f; if (region_y < 0.0f) region_y = 0.0f; else if (region_y > my_tex_h - region_sz) region_y = my_tex_h - region_sz;
                    float zoom = 4.0f;
                    ImGui.Text($"Min: ({region_x:#.##}, {region_y:#.##})");
                    ImGui.Text($"Max: ({(region_x + region_sz):#.##}, {(region_y + region_sz):#.##})");
                    Vector2 uv0 = new Vector2((region_x) / my_tex_w, (region_y) / my_tex_h);
                    Vector2 uv1 = new Vector2((region_x + region_sz) / my_tex_w, (region_y + region_sz) / my_tex_h);
                    ImGui.Image(my_tex, new Vector2(region_sz * zoom, region_sz * zoom), uv0, uv1, new Vector4(255,255,255,255), new Vector4(255,255,255,128));
                    ImGui.EndTooltip();
                }
                ImGui.TextWrapped("And now some textured buttons..");
                for (int i = 0; i < 8; i++)
                {
                    ImGui.PushID(i);
                    int frame_padding = -1 + i;     // -1 = uses default padding
                    if (ImGui.ImageButton(my_tex, new Vector2(32,32), new Vector2(0,0), new Vector2(32.0f/my_tex_w,32/my_tex_h), frame_padding, new Vector4(0,0,0,255)))
                        pressed_count += 1;
                    ImGui.PopID();
                    ImGui.SameLine();
                }
#endif
                ImGui.NewLine();
                ImGui.Text("Pressed " + pressed_count + " times.");
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Combo"))
            {
                // Expose flags as checkbox for the demo
                ImGui.CheckboxFlags("ImGuiComboFlags_PopupAlignLeft", ref flags, (uint)ImGuiComboFlags.PopupAlignLeft);
                if (ImGui.CheckboxFlags("ImGuiComboFlags_NoArrowButton", ref flags, (uint)ImGuiComboFlags.NoArrowButton))
                    flags &= ~(uint)ImGuiComboFlags.NoPreview;     // Clear the other flag, as we cannot combine both
                if (ImGui.CheckboxFlags("ImGuiComboFlags_NoPreview", ref flags, (uint)ImGuiComboFlags.NoPreview))
                    flags &= ~(uint)ImGuiComboFlags.NoArrowButton; // Clear the other flag, as we cannot combine both

                // General BeginCombo() API, you have full control over your selection data and display type.
                // (your selection data could be an index, a pointer to the object, an id for the object, a flag stored in the object itself, etc.)
                if (ImGui.BeginCombo("combo 1", item_current_1, (ImGuiNET.ImGuiComboFlags)flags)) // The second parameter is the label previewed before opening the combo.
                {
                    for (int n = 0; n < items.Length; n++)
                    {
                        bool is_selected = (item_current_1 == items[n]);
                        if (ImGui.Selectable(items[n], is_selected))
                            item_current_1 = items[n];
                        if (is_selected)
                            ImGui.SetItemDefaultFocus();   // Set the initial focus when opening the combo (scrolling + for keyboard navigation support in the upcoming navigation branch)
                    }
                    ImGui.EndCombo();
                }

                // Simplified one-liner Combo() API, using values packed in a single constant string
                ImGui.Combo("combo 2 (one-liner)", ref item_current_2, "aaaa\0bbbb\0cccc\0dddd\0eeee\0\0");

                // Simplified one-liner Combo() using an array of const char*
                ImGui.Combo("combo 3 (array)", ref item_current_3, items);

#if false
                // Simplified one-liner Combo() using an accessor function
                struct FuncHolder { static bool ItemGetter(void* data, int idx, const char** out_str) { *out_str = ((const char**)data)[idx]; return true; } };
                static int item_current_4 = 0;
                ImGui.Combo("combo 4 (function)", ref item_current_4, ref FuncHolder::ItemGetter, items, IM_ARRAYSIZE(items));
#endif

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Selectables"))
            {
                // Selectable() has 2 overloads:
                // - The one taking "bool selected" as a read-only selection information. When Selectable() has been clicked is returns true and you can alter selection state accordingly.
                // - The one taking "bool* p_selected" as a read-write selection information (convenient in some cases)
                // The earlier is more flexible, as in real application your selection may be stored in a different manner (in flags within objects, as an external list, etc).
                if (ImGui.TreeNode("Basic"))
                {
                    ImGui.Selectable("1. I am selectable", selection[0]);
                    ImGui.Selectable("2. I am selectable", selection[1]);
                    ImGui.Text("3. I am not selectable");
                    ImGui.Selectable("4. I am selectable", selection[3]);
                    if (ImGui.Selectable("5. I am double clickable", selection[4], ImGuiSelectableFlags.AllowDoubleClick))
                        if (ImGui.IsMouseDoubleClicked(0))
                            selection[4] = !selection[4];
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("Selection State: Single Selection"))
                {
                    for (int n = 0; n < 5; n++)
                    {
                        string buf = "Object " + n;
                        if (ImGui.Selectable(buf, treenode_selected == n))
                            treenode_selected = n;
                    }
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("Selection State: Multiple Selection"))
                {
                    ShowHelpMarker("Hold CTRL and click to select multiple items.");
                    for (int n = 0; n < 5; n++)
                    {
                        string buf = "Object " + n;
                        if (ImGui.Selectable(buf, selection_1[n]))
                        {
                            if (!ImGui.GetIO().KeyCtrl)    // Clear selection when CTRL is not held
                            {
                                for (int i = 0; i < selection_1.Length; i++)
                                {
                                    selection_1[i] = false;
                                }
                            }
                            selection_1[n] ^= true;
                        }
                    }
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("Rendering more text into the same line"))
                {
                    // Using the Selectable() override that takes "bool* p_selected" parameter and toggle your booleans automatically.
                    ImGui.Selectable("main.c", selected_1[0]); ImGui.SameLine(300); ImGui.Text(" 2,345 bytes");
                    ImGui.Selectable("Hello.cpp", selected_1[1]); ImGui.SameLine(300); ImGui.Text("12,345 bytes");
                    ImGui.Selectable("Hello.h", selected_1[2]); ImGui.SameLine(300); ImGui.Text(" 2,345 bytes");
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("In columns"))
                {
                    ImGui.Columns(3, null, false);
                    for (int i = 0; i < 1; i++)
                    {
                        string label = "Item " + i;
                        if (ImGui.Selectable(label, selected_2[i])) { }
                        ImGui.NextColumn();
                    }
                    ImGui.Columns(1);
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("Grid"))
                {
                    for (int i = 0; i < 16; i++)
                    {
                        ImGui.PushID(i);
                        if (ImGui.Selectable("Sailor", selected_3[i], 0, new Vector2(50, 50)))
                        {
                            int x = i % 4, y = i / 4;
                            if (x > 0) selected_3[i - 1] ^= true;
                            if (x < 3) selected_3[i + 1] ^= true;
                            if (y > 0) selected_3[i - 4] ^= true;
                            if (y < 3) selected_3[i + 4] ^= true;
                        }
                        if ((i % 4) < 3) ImGui.SameLine();
                        ImGui.PopID();
                    }
                    ImGui.TreePop();
                }
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Filtered Text Input"))
            {
                ImGui.InputText("default", buf1);
                ImGui.InputText("decimal", buf2, ImGuiInputTextFlags.CharsDecimal);
                ImGui.InputText("hexadecimal", buf3, ImGuiInputTextFlags.CharsHexadecimal | ImGuiInputTextFlags.CharsUppercase);
                ImGui.InputText("uppercase", buf4, ImGuiInputTextFlags.CharsUppercase);
                ImGui.InputText("no blank", buf5, ImGuiInputTextFlags.CharsNoBlank);
                ImGui.InputText("\"imgui\" letters", buf6, ImGuiInputTextFlags.CallbackCharFilter, filterImGuiLetters);

                ImGui.Text("Password input");
                ImGui.InputText("password", bufpass, ImGuiInputTextFlags.Password | ImGuiInputTextFlags.CharsNoBlank);
                ImGui.SameLine(); ShowHelpMarker("Display all characters as '*'.\nDisable clipboard cut and copy.\nDisable logging.\n");
                ImGui.InputText("password (clear)", bufpass, ImGuiInputTextFlags.CharsNoBlank);

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Multi-line Text Input"))
            {
                ShowHelpMarker("You can use the ImGuiInputTextFlags_CallbackResize facility if you need to wire InputTextMultiline() to a dynamic string type. See misc/stl/imgui_stl.h for an example. (This is not demonstrated in imgui_demo.cpp)");
                ImGui.Checkbox("Read-only", ref read_only);
                ImGuiInputTextFlags flags = ImGuiInputTextFlags.AllowTabInput | (read_only ? ImGuiInputTextFlags.ReadOnly : 0);
                ImGui.InputTextMultiline("##source", text, new Vector2(-1.0f, ImGui.GetTextLineHeight() * 16), flags);
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Plots Widgets"))
            {
                ImGui.Checkbox("Animate", ref animate);

                ImGui.PlotLines("Frame Times", arr);

                // Create a dummy array of contiguous float values to plot
                // Tip: If your float aren't contiguous but part of a structure, you can pass a pointer to your first float and the sizeof() of your structure in the Stride parameter.
                if (!animate || refresh_time == 0.0f)
                    refresh_time = ImGui.GetTime();
                while (refresh_time < ImGui.GetTime()) // Create dummy data at fixed 60 hz rate for the demo
                {
                    values[values_offset] = (float)Math.Cos(phase);
                    values_offset = (values_offset + 1) % values.Length;
                    phase += 0.10f * values_offset;
                    refresh_time += 1.0f / 60.0f;
                }
                ImGui.PlotLines("Lines", values, values_offset, "avg 0.0", -1.0f, 1.0f, new Vector2(0, 80));
                ImGui.PlotHistogram("Histogram", arr, 0, "", 0.0f, 1.0f, new Vector2(0, 80));

#if false
                // Use functions to generate output
                // FIXME: This is rather awkward because current plot API only pass in indices. We probably want an API passing floats and user provide sample rate/count.
                struct Funcs
                {
                    static float Sin(void*, int i) { return sinf(i * 0.1f); }
                    static float Saw(void*, int i) { return (i & 1) ? 1.0f : -1.0f; }
                };
                static int func_type = 0, display_count = 70;
                ImGui.Separator();
                ImGui.PushItemWidth(100); ImGui.Combo("func", ref func_type, "Sin\0Saw\0"); ImGui.PopItemWidth();
                ImGui.SameLine();
                ImGui.SliderInt("Sample count", ref display_count, 1, 400);
                float (*func)(void*, int) = (func_type == 0) ? Funcs::Sin : Funcs::Saw;
                ImGui.PlotLines("Lines", func, null, display_count, 0, null, -1.0f, 1.0f, new Vector2(0,80));
                ImGui.PlotHistogram("Histogram", func, null, display_count, 0, null, -1.0f, 1.0f, new Vector2(0,80));
                ImGui.Separator();
#endif

                // Animate a simple progress bar
                if (animate)
                {
                    progress += progress_dir * 0.4f * ImGui.GetIO().DeltaTime;
                    if (progress >= +1.1f) { progress = +1.1f; progress_dir *= -1.0f; }
                    if (progress <= -0.1f) { progress = -0.1f; progress_dir *= -1.0f; }
                }

                // Typically we would use new Vector2(-1.0f,0.0f) to use all available width, or new Vector2(width,0.0f) for a specified width. new Vector2(0.0f,0.0f) uses ItemWidth.
                ImGui.ProgressBar(progress, new Vector2(0.0f, 0.0f));
                ImGui.SameLine(0.0f, ImGui.GetStyle().ItemInnerSpacing.x);
                ImGui.Text("Progress Bar");

                float progress_saturated = (progress < 0.0f) ? 0.0f : (progress > 1.0f) ? 1.0f : progress;
                string buf = "" + (int)(progress_saturated * 1753) + " " + 1753;
                ImGui.ProgressBar(progress, new Vector2(0.0f, 0.0f), buf);
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Color/Picker Widgets"))
            {
                ImGui.Checkbox("With Alpha Preview", ref alpha_preview);
                ImGui.Checkbox("With Half Alpha Preview", ref alpha_half_preview);
                ImGui.Checkbox("With Drag and Drop", ref drag_and_drop);
                ImGui.Checkbox("With Options Menu", ref options_menu); ImGui.SameLine(); ShowHelpMarker("Right-click on the individual color widget to show options.");
                ImGui.Checkbox("With HDR", ref hdr); ImGui.SameLine(); ShowHelpMarker("Currently all this does is to lift the 0..1 limits on dragging widgets.");
                int misc_flags = (hdr ? (int)ImGuiColorEditFlags.HDR : 0) | (drag_and_drop ? 0 : (int)ImGuiColorEditFlags.NoDragDrop) | (alpha_half_preview ? (int)ImGuiColorEditFlags.AlphaPreviewHalf : (alpha_preview ? (int)ImGuiColorEditFlags.AlphaPreview : 0)) | (options_menu ? 0 : (int)ImGuiColorEditFlags.NoOptions);

                ImGui.Text("Color widget:");
                ImGui.SameLine(); ShowHelpMarker("Click on the colored square to open a color picker.\nCTRL+click on individual component to input value.\n");
                ImGui.ColorEdit3("MyColor##1", ref color3, (ImGuiColorEditFlags)misc_flags);

                ImGui.Text("Color widget HSV with Alpha:");
                ImGui.ColorEdit4("MyColor##2", ref color, ImGuiColorEditFlags.HSV | (ImGuiColorEditFlags)misc_flags);

                ImGui.Text("Color widget with Float Display:");
                ImGui.ColorEdit4("MyColor##2f", ref color, ImGuiColorEditFlags.Float | (ImGuiColorEditFlags)misc_flags);

                ImGui.Text("Color button with Picker:");
                ImGui.SameLine(); ShowHelpMarker("With the ImGuiColorEditFlags_NoInputs flag you can hide all the slider/text inputs.\nWith the ImGuiColorEditFlags_NoLabel flag you can pass a non-empty label which will only be used for the tooltip and picker popup.");
                ImGui.ColorEdit4("MyColor##3", ref color, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel | (ImGuiColorEditFlags)misc_flags);

                ImGui.Text("Color button with Custom Picker Popup:");

                // Generate a dummy palette
                if (!saved_palette_inited)
                    for (int n = 0; n < saved_palette.Length; n++)
                    {
                        saved_palette[n] = new Vector4();
                        ImGui.ColorConvertHSVtoRGB(n / 31.0f, 0.8f, 0.8f, out saved_palette[n].x, out saved_palette[n].y, out saved_palette[n].z);
                        saved_palette[n].w = 1.0f; // Alpha
                    }
                saved_palette_inited = true;

                bool open_popup = ImGui.ColorButton("MyColor##3b", color, (ImGuiColorEditFlags)misc_flags);
                ImGui.SameLine();
                open_popup |= ImGui.Button("Palette");
                if (open_popup)
                {
                    ImGui.OpenPopup("mypicker");
                    backup_color = color;
                }
                if (ImGui.BeginPopup("mypicker"))
                {
                    // FIXME: Adding a drag and drop example here would be perfect!
                    ImGui.Text("MY CUSTOM COLOR PICKER WITH AN AMAZING PALETTE!");
                    ImGui.Separator();
                    ImGui.ColorPicker4("##picker", ref color, (ImGuiColorEditFlags)misc_flags | ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoSmallPreview);
                    ImGui.SameLine();
                    ImGui.BeginGroup();
                    ImGui.Text("Current");
                    ImGui.ColorButton("##current", color, ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.AlphaPreviewHalf, new Vector2(60, 40));
                    ImGui.Text("Previous");
                    if (ImGui.ColorButton("##previous", backup_color, ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.AlphaPreviewHalf, new Vector2(60, 40)))
                        color = backup_color;
                    ImGui.Separator();
                    ImGui.Text("Palette");
                    for (int n = 0; n < saved_palette.Length; n++)
                    {
                        ImGui.PushID(n);
                        if ((n % 8) != 0)
                            ImGui.SameLine(0.0f, ImGui.GetStyle().ItemSpacing.y);
                        if (ImGui.ColorButton("##palette", saved_palette[n], ImGuiColorEditFlags.NoAlpha | ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.NoTooltip, new Vector2(20, 20)))
                            color = new Vector4(saved_palette[n].x, saved_palette[n].y, saved_palette[n].z, color.w); // Preserve alpha!

                        if (ImGui.BeginDragDropTarget())
                        {
                            Vector3 color = new Vector3(0.0f, 0.0f, 0.0f);
                            if (ImGui.AcceptDragDropPayload("_COL3F", ref color))
                            {
                                saved_palette[n] = new Vector4(color.x, color.y, color.z, 1.0f);
                            }

                            Vector4 color4 = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                            if (ImGui.AcceptDragDropPayload("_COL4F", ref color4))
                            {
                                saved_palette[n] = color4;
                            }
                            ImGui.EndDragDropTarget();
                        }

                        ImGui.PopID();
                    }
                    ImGui.EndGroup();
                    ImGui.EndPopup();
                }

                ImGui.Text("Color button only:");
                ImGui.ColorButton("MyColor##3c", color, (ImGuiColorEditFlags)misc_flags, new Vector2(80, 80));

                ImGui.Text("Color picker:");
                ImGui.Checkbox("With Alpha", ref alpha);
                ImGui.Checkbox("With Alpha Bar", ref alpha_bar);
                ImGui.Checkbox("With Side Preview", ref side_preview);
                if (side_preview)
                {
                    ImGui.SameLine();
                    ImGui.Checkbox("With Ref Color", ref ref_color);
                    if (ref_color)
                    {
                        ImGui.SameLine();
                        ImGui.ColorEdit4("##RefColor", ref ref_color_v, ImGuiColorEditFlags.NoInputs | (ImGuiColorEditFlags)misc_flags);
                    }
                }
                ImGui.Combo("Inputs Mode", ref inputs_mode, "All Inputs\0No Inputs\0RGB Input\0HSV Input\0HEX Input\0");
                ImGui.Combo("Picker Mode", ref picker_mode, "Auto/Current\0Hue bar + SV rect\0Hue wheel + SV triangle\0");
                ImGui.SameLine(); ShowHelpMarker("User can right-click the picker to change mode.");
                ImGuiColorEditFlags flags = (ImGuiColorEditFlags)misc_flags;
                if (!alpha) flags |= ImGuiColorEditFlags.NoAlpha; // This is by default if you call ColorPicker3() instead of ColorPicker4()
                if (alpha_bar) flags |= ImGuiColorEditFlags.AlphaBar;
                if (!side_preview) flags |= ImGuiColorEditFlags.NoSidePreview;
                if (picker_mode == 1) flags |= ImGuiColorEditFlags.PickerHueBar;
                if (picker_mode == 2) flags |= ImGuiColorEditFlags.PickerHueWheel;
                if (inputs_mode == 1) flags |= ImGuiColorEditFlags.NoInputs;
                if (inputs_mode == 2) flags |= ImGuiColorEditFlags.RGB;
                if (inputs_mode == 3) flags |= ImGuiColorEditFlags.HSV;
                if (inputs_mode == 4) flags |= ImGuiColorEditFlags.HEX;
                ImGui.ColorPicker4("MyColor##4", ref color, flags);

                ImGui.Text("Programmatically set defaults:");
                ImGui.SameLine(); ShowHelpMarker("SetColorEditOptions() is designed to allow you to set boot-time default.\nWe don't have Push/Pop functions because you can force options on a per-widget basis if needed, and the user can change non-forced ones with the options menu.\nWe don't have a getter to avoid encouraging you to persistently save values that aren't forward-compatible.");
                if (ImGui.Button("Default: Uint8 + HSV + Hue Bar"))
                    ImGui.SetColorEditOptions(ImGuiColorEditFlags.Uint8 | ImGuiColorEditFlags.HSV | ImGuiColorEditFlags.PickerHueBar);
                if (ImGui.Button("Default: Float + HDR + Hue Wheel"))
                    ImGui.SetColorEditOptions(ImGuiColorEditFlags.Float | ImGuiColorEditFlags.HDR | ImGuiColorEditFlags.PickerHueWheel);

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Range Widgets"))
            {
                ImGui.DragFloatRange2("range", ref begin, ref end, 0.25f, 0.0f, 100.0f, "Min: %.1f %%", "Max: %.1f %%");
                ImGui.DragIntRange2("range int (no bounds)", ref begin_i, ref end_i, 5, 0, 0, "Min: %d units", "Max: %d units");
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Data Types"))
            {
                // C++ has a fancy system to allow DragScalar/InputScalar/SliderScalar functions with various data types
                // C# can't express it so we have manually specified all possible combinations

                // Note that the SliderScalar function has a maximum usable range of half the natural type maximum, hence the /2 below.
                const long LLONG_MIN = -9223372036854775807L - 1;
                const long LLONG_MAX = 9223372036854775807L;
                const ulong ULLONG_MAX = (2UL * 9223372036854775807L + 1);
                const int s32_min = int.MinValue / 2, s32_max = int.MaxValue / 2, s32_hi_a = int.MaxValue / 2 - 100, s32_hi_b = int.MaxValue / 2;
                const uint u32_min = 0, u32_max = uint.MaxValue / 2, u32_hi_a = uint.MaxValue / 2 - 100, u32_hi_b = uint.MaxValue / 2;
                const long s64_min = LLONG_MIN / 2, s64_max = LLONG_MAX / 2, s64_hi_a = LLONG_MAX / 2 - 100, s64_hi_b = LLONG_MAX / 2;
                const ulong u64_min = 0, u64_max = ULLONG_MAX / 2, u64_hi_a = ULLONG_MAX / 2 - 100, u64_hi_b = ULLONG_MAX / 2;
                const float f32_lo_a = -10000000000.0f, f32_hi_a = +10000000000.0f;
                const double f64_lo_a = -1000000000000000.0, f64_hi_a = +1000000000000000.0;

                const float drag_speed = 0.2f;
                ImGui.Text("Drags:");
                ImGui.Checkbox("Clamp integers to 0..50", ref drag_clamp); ImGui.SameLine(); ShowHelpMarker("As with every widgets in dear imgui, we never modify values unless there is a user interaction.\nYou can override the clamping limits by using CTRL+Click to input a value.");
                if (drag_clamp)
                {
                    ImGui.DragScalar("drag s32", ref s32_v, drag_speed, 0, 50);
                    ImGui.DragScalar("drag u32", ref u32_v, drag_speed, 0, 50, 1.0f, "%u ms");
                    ImGui.DragScalar("drag s64", ref s64_v, drag_speed, 0, 50);
                    ImGui.DragScalar("drag u64", ref u64_v, drag_speed, 0, 50);
                }
                else
                {
                    ImGui.DragScalar("drag s32", ref s32_v, drag_speed);
                    ImGui.DragScalar("drag u32", ref u32_v, drag_speed, 1.0f, "%u ms");
                    ImGui.DragScalar("drag s64", ref s64_v, drag_speed);
                    ImGui.DragScalar("drag u64", ref u64_v, drag_speed);
                }
                ImGui.DragScalar("drag float", ref f32_v, 0.005f, 0.0f, 1.0f, 1.0f, "%f");
                ImGui.DragScalar("drag float ^2", ref f32_v, 0.005f, 0.0f, 1.0f, 2.0f, "%f"); ImGui.SameLine(); ShowHelpMarker("You can use the 'power' parameter to increase tweaking precision on one side of the range.");
                ImGui.DragScalar("drag double", ref f64_v, 0.0005f, 0.0, double.MaxValue, 1.0f, "%.10f grams");
                ImGui.DragScalar("drag double ^2", ref f64_v, 0.0005f, 0.0, 1.0, 2.0f, "0 < %.10f < 1");

                ImGui.Text("Sliders");
                ImGui.SliderScalar("slider s32 low", ref s32_v, 0, 50, 1.0f, "%d");
                ImGui.SliderScalar("slider s32 high", ref s32_v, s32_hi_a, s32_hi_b, 1.0f, "%d");
                ImGui.SliderScalar("slider s32 full", ref s32_v, s32_min, s32_max, 1.0f, "%d");
                ImGui.SliderScalar("slider u32 low", ref u32_v, 0, 50, 1.0f, "%u");
                ImGui.SliderScalar("slider u32 high", ref u32_v, u32_hi_a, u32_hi_b, 1.0f, "%u");
                ImGui.SliderScalar("slider u32 full", ref u32_v, u32_min, u32_max, 1.0f, "%u");
                ImGui.SliderScalar("slider s64 low", ref s64_v, 0, 50, 1.0f, "%I64d");
                ImGui.SliderScalar("slider s64 high", ref s64_v, s64_hi_a, s64_hi_b, 1.0f, "%I64d");
                ImGui.SliderScalar("slider s64 full", ref s64_v, s64_min, s64_max, 1.0f, "%I64d");
                ImGui.SliderScalar("slider u64 low", ref u64_v, 0, 50, 1.0f, "%I64u ms");
                ImGui.SliderScalar("slider u64 high", ref u64_v, u64_hi_a, u64_hi_b, 1.0f, "%I64u ms");
                ImGui.SliderScalar("slider u64 full", ref u64_v, u64_min, u64_max, 1.0f, "%I64u ms");
                ImGui.SliderScalar("slider float low", ref f32_v, 0, 1.0f);
                ImGui.SliderScalar("slider float low^2", ref f32_v, 0, 1.0f, 2.0f, "%.10f");
                ImGui.SliderScalar("slider float high", ref f32_v, f32_lo_a, f32_hi_a, 1.0f, "%e");
                ImGui.SliderScalar("slider double low", ref f64_v, 0, 1.0, 1.0f, "%.10f grams");
                ImGui.SliderScalar("slider double low^2", ref f64_v, 0, 1.0, 2.0f, "%.10f");
                ImGui.SliderScalar("slider double high", ref f64_v, f64_lo_a, f64_hi_a, 1.0f, "%e grams");

                ImGui.Text("Inputs");
                ImGui.Checkbox("Show step buttons", ref inputs_step);
                if (inputs_step)
                {
                    ImGui.InputScalar("input s32", ref s32_v, 1, ImGuiInputTextFlags.None, "%d");
                    ImGui.InputScalar("input s32 hex", ref s32_v, 1, ImGuiInputTextFlags.CharsHexadecimal, "%08X");
                    ImGui.InputScalar("input u32", ref u32_v, 1, ImGuiInputTextFlags.None, "%u");
                    ImGui.InputScalar("input u32 hex", ref u32_v, 1, ImGuiInputTextFlags.CharsHexadecimal, "%08X");
                    ImGui.InputScalar("input s64", ref s64_v, 1);
                    ImGui.InputScalar("input u64", ref u64_v, 1);
                    ImGui.InputScalar("input float", ref f32_v, 1);
                    ImGui.InputScalar("input double", ref f64_v, 1);
                }
                else
                {
                    ImGui.InputScalar("input s32", ref s32_v, ImGuiInputTextFlags.None, "%d");
                    ImGui.InputScalar("input s32 hex", ref s32_v, ImGuiInputTextFlags.CharsHexadecimal, "%08X");
                    ImGui.InputScalar("input u32", ref u32_v, ImGuiInputTextFlags.None, "%u");
                    ImGui.InputScalar("input u32 hex", ref u32_v, ImGuiInputTextFlags.CharsHexadecimal, "%08X");
                    ImGui.InputScalar("input s64", ref s64_v);
                    ImGui.InputScalar("input u64", ref u64_v);
                    ImGui.InputScalar("input float", ref f32_v);
                    ImGui.InputScalar("input double", ref f64_v);
                }

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Multi-component Widgets"))
            {
                ImGui.InputFloat2("input float2", ref vec2f);
                ImGui.DragFloat2("drag float2", ref vec2f, 0.01f, 0.0f, 1.0f);
                ImGui.SliderFloat2("slider float2", ref vec2f, 0.0f, 1.0f);
                ImGui.InputInt2("input int2", vec2i);
                ImGui.DragInt2("drag int2", vec2i, 1, 0, 255);
                ImGui.SliderInt2("slider int2", vec2i, 0, 255);
                ImGui.Spacing();

                ImGui.InputFloat3("input float3", ref vec3f);
                ImGui.DragFloat3("drag float3", ref vec3f, 0.01f, 0.0f, 1.0f);
                ImGui.SliderFloat3("slider float3", ref vec3f, 0.0f, 1.0f);
                ImGui.InputInt3("input int3", vec3i);
                ImGui.DragInt3("drag int3", vec3i, 1, 0, 255);
                ImGui.SliderInt3("slider int3", vec3i, 0, 255);
                ImGui.Spacing();

                ImGui.InputFloat4("input float4", ref vec4f);
                ImGui.DragFloat4("drag float4", ref vec4f, 0.01f, 0.0f, 1.0f);
                ImGui.SliderFloat4("slider float4", ref vec4f, 0.0f, 1.0f);
                ImGui.InputInt4("input int4", vec4i);
                ImGui.DragInt4("drag int4", vec4i, 1, 0, 255);
                ImGui.SliderInt4("slider int4", vec4i, 0, 255);

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Vertical Sliders"))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(spacing, spacing));

                ImGui.VSliderInt("##int", new Vector2(18, 160), ref int_value, 0, 5);
                ImGui.SameLine();
                ImGui.PushID("set1");
                for (int i = 0; i < 7; i++)
                {
                    if (i > 0) ImGui.SameLine();
                    ImGui.PushID(i);
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, (Vector4)Color.HSVToRGB(i / 7.0f, 0.5f, 0.5f));
                    ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, (Vector4)Color.HSVToRGB(i / 7.0f, 0.6f, 0.5f));
                    ImGui.PushStyleColor(ImGuiCol.FrameBgActive, (Vector4)Color.HSVToRGB(i / 7.0f, 0.7f, 0.5f));
                    ImGui.PushStyleColor(ImGuiCol.SliderGrab, (Vector4)Color.HSVToRGB(i / 7.0f, 0.9f, 0.9f));
                    ImGui.VSliderFloat("##v", new Vector2(18, 160), ref values_1[i], 0.0f, 1.0f, "");
                    if (ImGui.IsItemActive() || ImGui.IsItemHovered())
                        ImGui.SetTooltip(values_1[i].ToString("0.000"));
                    ImGui.PopStyleColor(4);
                    ImGui.PopID();
                }
                ImGui.PopID();

                ImGui.SameLine();
                ImGui.PushID("set2");
                for (int nx = 0; nx < 4; nx++)
                {
                    if (nx > 0) ImGui.SameLine();
                    ImGui.BeginGroup();
                    for (int ny = 0; ny < rows; ny++)
                    {
                        ImGui.PushID(nx * rows + ny);
                        ImGui.VSliderFloat("##v", small_slider_size, ref values2[nx], 0.0f, 1.0f, "");
                        if (ImGui.IsItemActive() || ImGui.IsItemHovered())
                            ImGui.SetTooltip(values2[nx].ToString("0.000"));
                        ImGui.PopID();
                    }
                    ImGui.EndGroup();
                }
                ImGui.PopID();

                ImGui.SameLine();
                ImGui.PushID("set3");
                for (int i = 0; i < 4; i++)
                {
                    if (i > 0) ImGui.SameLine();
                    ImGui.PushID(i);
                    ImGui.PushStyleVar(ImGuiStyleVar.GrabMinSize, 40);
                    ImGui.VSliderFloat("##v", new Vector2(40, 160), ref values[i], 0.0f, 1.0f, "%.2f\nsec");
                    ImGui.PopStyleVar();
                    ImGui.PopID();
                }
                ImGui.PopID();
                ImGui.PopStyleVar();
                ImGui.TreePop();
            }
            if (ImGui.TreeNode("Drag and Drop"))
            {
                {
                    // ColorEdit widgets automatically act as drag source and drag target.
                    // They are using standardized payload strings IMGUI_PAYLOAD_TYPE_COLOR_3F and IMGUI_PAYLOAD_TYPE_COLOR_4F to allow your own widgets
                    // to use colors in their drag and drop interaction. Also see the demo in Color Picker -> Palette demo.
                    ImGui.BulletText("Drag and drop in standard widgets");
                    ImGui.Indent();
                    ImGui.ColorEdit3("color 1", ref col1_dnd);
                    ImGui.ColorEdit4("color 2", ref col2_dnd);
                    ImGui.Unindent();
                }

                {
                    ImGui.BulletText("Drag and drop to copy/swap items");
                    ImGui.Indent();

                    if (ImGui.RadioButton("Copy", mode_dnd == ModeDnd.Mode_Copy)) { mode_dnd = ModeDnd.Mode_Copy; }
                    ImGui.SameLine();
                    if (ImGui.RadioButton("Move", mode_dnd == ModeDnd.Mode_Move)) { mode_dnd = ModeDnd.Mode_Move; }
                    ImGui.SameLine();
                    if (ImGui.RadioButton("Swap", mode_dnd == ModeDnd.Mode_Swap)) { mode_dnd = ModeDnd.Mode_Swap; }
                    for (int n = 0; n < names_dnd.Length; n++)
                    {
                        ImGui.PushID(n);
                        if ((n % 3) != 0)
                            ImGui.SameLine();
                        ImGui.Button(names_dnd[n], new Vector2(60, 60));

                        // Our buttons are both drag sources and drag targets here!
                        if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.None))
                        {
                            ImGui.SetDragDropPayload("DND_DEMO_CELL", n);                  // Set payload to carry the index of our item (could be anything)
                            if (mode_dnd == ModeDnd.Mode_Copy) { ImGui.Text("Copy " + names_dnd[n]); }        // Display preview (could be anything, e.g. when dragging an image we could decide to display the filename and a small preview of the image, etc.)
                            if (mode_dnd == ModeDnd.Mode_Move) { ImGui.Text("Move " + names_dnd[n]); }
                            if (mode_dnd == ModeDnd.Mode_Swap) { ImGui.Text("Swap " + names_dnd[n]); }
                            ImGui.EndDragDropSource();
                        }
                        if (ImGui.BeginDragDropTarget())
                        {
                            int payload_n = -1;
                            if (ImGui.AcceptDragDropPayload("DND_DEMO_CELL", ref payload_n))
                            {
                                if (mode_dnd == ModeDnd.Mode_Copy)
                                {
                                    names_dnd[n] = names_dnd[payload_n];
                                }
                                if (mode_dnd == ModeDnd.Mode_Move)
                                {
                                    names_dnd[n] = names_dnd[payload_n];
                                    names_dnd[payload_n] = "";
                                }
                                if (mode_dnd == ModeDnd.Mode_Swap)
                                {
                                    string tmp = names_dnd[n];
                                    names_dnd[n] = names_dnd[payload_n];
                                    names_dnd[payload_n] = tmp;
                                }
                            }
                            ImGui.EndDragDropTarget();
                        }
                        ImGui.PopID();
                    }
                    ImGui.Unindent();
                }

                ImGui.TreePop();
            }
            if (ImGui.TreeNode("Querying Status (Active/Focused/Hovered etc.)"))
            {
                // Display the value of IsItemHovered() and other common item state functions. Note that the flags can be combined.
                // (because BulletText is an item itself and that would affect the output of IsItemHovered() we pass all state in a single call to simplify the code).
                ImGui.RadioButton("Text", ref item_type, 0);
                ImGui.RadioButton("Button", ref item_type, 1);
                ImGui.RadioButton("CheckBox", ref item_type, 2);
                ImGui.RadioButton("SliderFloat", ref item_type, 3);
                ImGui.RadioButton("ColorEdit4", ref item_type, 4);
                ImGui.RadioButton("ListBox", ref item_type, 5);
                ImGui.Separator();
                bool ret = false;
                if (item_type == 0) { ImGui.Text("ITEM: Text"); }                                              // Testing text items with no identifier/interaction
                if (item_type == 1) { ret = ImGui.Button("ITEM: Button"); }                                    // Testing button
                if (item_type == 2) { ret = ImGui.Checkbox("ITEM: CheckBox", ref b); }                            // Testing checkbox
                if (item_type == 3) { ret = ImGui.SliderFloat("ITEM: SliderFloat", ref col4f.x, 0.0f, 1.0f); }   // Testing basic item
                if (item_type == 4) { ret = ImGui.ColorEdit4("ITEM: ColorEdit4", ref col4f); }                     // Testing multi-component items (IsItemXXX flags are reported merged)
                if (item_type == 5) { ret = ImGui.ListBox("ITEM: ListBox", ref current, items2, items2.Length, items2.Length); }
                ImGui.BulletText(
                    "Return value = " + ret + "\nIsItemFocused() = " + ImGui.IsItemFocused() +
                    "\nIsItemHovered() = " + ImGui.IsItemHovered() +
                    "\nIsItemHovered(_AllowWhenBlockedByPopup) = " + ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup) +
                    "\nIsItemHovered(_AllowWhenBlockedByActiveItem) = " + ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenBlockedByActiveItem) +
                    "\nIsItemHovered(_AllowWhenOverlapped) = " + ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenOverlapped) +
                    "\nIsItemHovered(_RectOnly) = " + ImGui.IsItemHovered(ImGuiHoveredFlags.RectOnly) +
                    "\nIsItemActive() = " + ImGui.IsItemActive() +
                    "\nIsItemEdited() = " + ImGui.IsItemEdited() +
                    "\nIsItemDeactivated() = " + ImGui.IsItemDeactivated() +
                    "\nIsItemDeactivatedEdit() = " + ImGui.IsItemDeactivatedAfterEdit() +
                    "\nIsItemVisible() = " + ImGui.IsItemVisible() +
                    "\nGetItemRectMin() = " + ImGui.GetItemRectMin() +
                    "\nGetItemRectMax() = " + ImGui.GetItemRectMax() +
                    "\nGetItemRectSize() = " + ImGui.GetItemRectSize()
                );

                ImGui.Checkbox("Embed everything inside a child window (for additional testing)", ref embed_all_inside_a_child_window);
                if (embed_all_inside_a_child_window)
                    ImGui.BeginChild("outer_child", new Vector2(0, ImGui.GetFontSize() * 20), true);

                // Testing IsWindowFocused() function with its various flags. Note that the flags can be combined.
                ImGui.BulletText(
                    "IsWindowFocused() = " + ImGui.IsWindowFocused() +
                    "\nIsWindowFocused(_ChildWindows) = " + ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows) +
                    "\nIsWindowFocused(_ChildWindows|_RootWindow) = " + ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows | ImGuiFocusedFlags.RootWindow) +
                    "\nIsWindowFocused(_RootWindow) = " + ImGui.IsWindowFocused(ImGuiFocusedFlags.RootWindow) +
                    "\nIsWindowFocused(_AnyWindow) = " + ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow)
                );

                // Testing IsWindowHovered() function with its various flags. Note that the flags can be combined.
                ImGui.BulletText(
                    "IsWindowHovered() = " + ImGui.IsWindowHovered() +
                    "\nIsWindowHovered(_AllowWhenBlockedByPopup) = " + ImGui.IsWindowHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup) +
                    "\nIsWindowHovered(_AllowWhenBlockedByActiveItem) = " + ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows) +
                    "\nIsWindowHovered(_ChildWindows) = " + ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows) +
                    "\nIsWindowHovered(_ChildWindows|_RootWindow) = " + ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows | ImGuiHoveredFlags.RootWindow) +
                    "\nIsWindowHovered(_RootWindow) = " + ImGui.IsWindowHovered(ImGuiHoveredFlags.RootWindow) +
                    "\nIsWindowHovered(_AnyWindow) = " + ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow)
                );

                ImGui.BeginChild("child", new Vector2(0, 50), true);
                ImGui.Text("This is another child window for testing with the _ChildWindows flag.");
                ImGui.EndChild();
                if (embed_all_inside_a_child_window)
                    ImGui.EndChild();

                // Calling IsItemHovered() after begin returns the hovered status of the title bar. 
                // This is useful in particular if you want to create a context menu (with BeginPopupContextItem) associated to the title bar of a window.
                ImGui.Checkbox("Hovered/Active tests after Begin() for title bar testing", ref test_window);
                if (test_window)
                {
                    ImGui.Begin("Title bar Hovered/Active tests", ref test_window);
                    if (ImGui.BeginPopupContextItem()) // <-- This is using IsItemHovered()
                    {
                        if (ImGui.MenuItem("Close")) { test_window = false; }
                        ImGui.EndPopup();
                    }
                    ImGui.Text(
                        "IsItemHovered() after begin = " + ImGui.IsItemHovered() + "(== is title bar hovered)\n" +
                        "IsItemActive() after begin = " + ImGui.IsItemActive() + "(== is window being clicked/moved)\n"
                    );
                    ImGui.End();
                }

                ImGui.TreePop();
            }
        }

        if (ImGui.CollapsingHeader("Layout"))
        {
            if (ImGui.TreeNode("Child regions"))
            {
                ImGui.Checkbox("Disable Mouse Wheel", ref disable_mouse_wheel);
                ImGui.Checkbox("Disable Menu", ref disable_menu);

                bool goto_line = ImGui.Button("Goto");
                ImGui.SameLine();
                ImGui.PushItemWidth(100);
                goto_line |= ImGui.InputInt("##Line", ref line, 0, 0, ImGuiInputTextFlags.EnterReturnsTrue);
                ImGui.PopItemWidth();

                // Child 1: no border, enable horizontal scrollbar
                {
                    ImGui.BeginChild("Child1", new Vector2(ImGui.GetWindowContentRegionWidth() * 0.5f, 300), false, ImGuiWindowFlags.HorizontalScrollbar | (disable_mouse_wheel ? ImGuiWindowFlags.NoScrollWithMouse : 0));
                    for (int i = 0; i < 100; i++)
                    {
                        ImGui.Text(i + "scrollable region");
                        if (goto_line && line == i)
                            ImGui.SetScrollHere();
                    }
                    if (goto_line && line >= 100)
                        ImGui.SetScrollHere();
                    ImGui.EndChild();
                }

                ImGui.SameLine();

                // Child 2: rounded border
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 5.0f);
                    ImGui.BeginChild("Child2", new Vector2(0, 300), true, (disable_mouse_wheel ? ImGuiWindowFlags.NoScrollWithMouse : 0) | (disable_menu ? 0 : ImGuiWindowFlags.MenuBar));
                    if (!disable_menu && ImGui.BeginMenuBar())
                    {
                        if (ImGui.BeginMenu("Menu"))
                        {
                            ShowExampleMenuFile();
                            ImGui.EndMenu();
                        }
                        ImGui.EndMenuBar();
                    }
                    ImGui.Columns(2);
                    for (int i = 0; i < 100; i++)
                    {
                        string buf = i.ToString();
                        ImGui.Button(buf, new Vector2(-1.0f, 0.0f));
                        ImGui.NextColumn();
                    }
                    ImGui.EndChild();
                    ImGui.PopStyleVar();
                }

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Widgets Width"))
            {
                ImGui.Text("PushItemWidth(100)");
                ImGui.SameLine(); ShowHelpMarker("Fixed width.");
                ImGui.PushItemWidth(100);
                ImGui.DragFloat("float##1", ref f);
                ImGui.PopItemWidth();

                ImGui.Text("PushItemWidth(GetWindowWidth() * 0.5f)");
                ImGui.SameLine(); ShowHelpMarker("Half of window width.");
                ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
                ImGui.DragFloat("float##2", ref f);
                ImGui.PopItemWidth();

                ImGui.Text("PushItemWidth(GetContentRegionAvailWidth() * 0.5f)");
                ImGui.SameLine(); ShowHelpMarker("Half of available width.\n(~ right-cursor_pos)\n(works within a column set)");
                ImGui.PushItemWidth(ImGui.GetContentRegionAvailWidth() * 0.5f);
                ImGui.DragFloat("float##3", ref f);
                ImGui.PopItemWidth();

                ImGui.Text("PushItemWidth(-100)");
                ImGui.SameLine(); ShowHelpMarker("Align to right edge minus 100");
                ImGui.PushItemWidth(-100);
                ImGui.DragFloat("float##4", ref f);
                ImGui.PopItemWidth();

                ImGui.Text("PushItemWidth(-1)");
                ImGui.SameLine(); ShowHelpMarker("Align to right edge");
                ImGui.PushItemWidth(-1);
                ImGui.DragFloat("float##5", ref f);
                ImGui.PopItemWidth();

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Basic Horizontal Layout"))
            {
                ImGui.TextWrapped("(Use ImGui.SameLine() to keep adding items to the right of the preceding item)");

                // Text
                ImGui.Text("Two items: Hello"); ImGui.SameLine();
                ImGui.TextColored(new Vector4(1, 1, 0, 1), "Sailor");

                // Adjust spacing
                ImGui.Text("More spacing: Hello"); ImGui.SameLine(0, 20);
                ImGui.TextColored(new Vector4(1, 1, 0, 1), "Sailor");

                // Button
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Normal buttons"); ImGui.SameLine();
                ImGui.Button("Banana"); ImGui.SameLine();
                ImGui.Button("Apple"); ImGui.SameLine();
                ImGui.Button("Corniflower");

                // Button
                ImGui.Text("Small buttons"); ImGui.SameLine();
                ImGui.SmallButton("Like this one"); ImGui.SameLine();
                ImGui.Text("can fit within a text block.");

                // Aligned to arbitrary position. Easy/cheap column.
                ImGui.Text("Aligned");
                ImGui.SameLine(150); ImGui.Text("x=150");
                ImGui.SameLine(300); ImGui.Text("x=300");
                ImGui.Text("Aligned");
                ImGui.SameLine(150); ImGui.SmallButton("x=150");
                ImGui.SameLine(300); ImGui.SmallButton("x=300");

                // Checkbox
                ImGui.Checkbox("My", ref c1); ImGui.SameLine();
                ImGui.Checkbox("Tailor", ref c2); ImGui.SameLine();
                ImGui.Checkbox("Is", ref c3); ImGui.SameLine();
                ImGui.Checkbox("Rich", ref c4);

                // Various
                ImGui.PushItemWidth(80);
                ImGui.Combo("Combo", ref item, items3); ImGui.SameLine();
                ImGui.SliderFloat("X", ref f02, 0.0f, 5.0f); ImGui.SameLine();
                ImGui.SliderFloat("Y", ref f12, 0.0f, 5.0f); ImGui.SameLine();
                ImGui.SliderFloat("Z", ref f22, 0.0f, 5.0f);
                ImGui.PopItemWidth();

                ImGui.PushItemWidth(80);
                ImGui.Text("Lists:");
                for (int i = 0; i < 4; i++)
                {
                    if (i > 0) ImGui.SameLine();
                    ImGui.PushID(i);
                    ImGui.ListBox("", ref selection2[i], items3, items3.Length);
                    ImGui.PopID();
                    //if (ImGui.IsItemHovered()) ImGui.SetTooltip("ListBox %d hovered", i);
                }
                ImGui.PopItemWidth();

                // Dummy
                ImGui.Button("A", button_sz); ImGui.SameLine();
                ImGui.Dummy(button_sz); ImGui.SameLine();
                ImGui.Button("B", button_sz);

                // Manually wrapping (we should eventually provide this as an automatic layout feature, but for now you can do it manually)
                ImGui.Text("Manually wrapping:");
                ImGuiStylePtr style = ImGui.GetStyle();
                int buttons_count = 20;
                float window_visible_x2 = ImGui.GetWindowPos().x + ImGui.GetWindowContentRegionMax().x;
                for (int n = 0; n < buttons_count; n++)
                {
                    ImGui.PushID(n);
                    ImGui.Button("Box", button_sz);
                    float last_button_x2 = ImGui.GetItemRectMax().x;
                    float next_button_x2 = last_button_x2 + style.ItemSpacing.x + button_sz.x; // Expected position if next button was on same line
                    if (n + 1 < buttons_count && next_button_x2 < window_visible_x2)
                        ImGui.SameLine();
                    ImGui.PopID();
                }

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Groups"))
            {
                ImGui.TextWrapped("(Using ImGui.BeginGroup()/EndGroup() to layout items. BeginGroup() basically locks the horizontal position. EndGroup() bundles the whole group so that you can use functions such as IsItemHovered() on it.)");
                ImGui.BeginGroup();
                {
                    ImGui.BeginGroup();
                    ImGui.Button("AAA");
                    ImGui.SameLine();
                    ImGui.Button("BBB");
                    ImGui.SameLine();
                    ImGui.BeginGroup();
                    ImGui.Button("CCC");
                    ImGui.Button("DDD");
                    ImGui.EndGroup();
                    ImGui.SameLine();
                    ImGui.Button("EEE");
                    ImGui.EndGroup();
                    if (ImGui.IsItemHovered())
                        ImGui.SetTooltip("First group hovered");
                }
                Vector2 size = ImGui.GetItemRectSize();

                // Capture the group size and create widgets using the same size
                float[] values3 = { 0.5f, 0.20f, 0.80f, 0.60f, 0.25f };
                ImGui.PlotHistogram("##values", values3, 0, "", 0.0f, 1.0f, size);

                ImGui.Button("ACTION", new Vector2((size.x - ImGui.GetStyle().ItemSpacing.x) * 0.5f, size.y));
                ImGui.SameLine();
                ImGui.Button("REACTION", new Vector2((size.x - ImGui.GetStyle().ItemSpacing.x) * 0.5f, size.y));
                ImGui.EndGroup();
                ImGui.SameLine();

                ImGui.Button("LEVERAGE\nBUZZWORD", size);
                ImGui.SameLine();

                if (ImGui.ListBoxHeader("List", size))
                {
                    ImGui.Selectable("Selected", true);
                    ImGui.Selectable("Not Selected", false);
                    ImGui.ListBoxFooter();
                }

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Text Baseline Alignment"))
            {
                ImGui.TextWrapped("(This is testing the vertical alignment that occurs on text to keep it at the same baseline as widgets. Lines only composed of text or \"small\" widgets fit in less vertical spaces than lines with normal widgets)");

                ImGui.Text("One\nTwo\nThree"); ImGui.SameLine();
                ImGui.Text("Hello\nWorld"); ImGui.SameLine();
                ImGui.Text("Banana");

                ImGui.Text("Banana"); ImGui.SameLine();
                ImGui.Text("Hello\nWorld"); ImGui.SameLine();
                ImGui.Text("One\nTwo\nThree");

                ImGui.Button("HOP##1"); ImGui.SameLine();
                ImGui.Text("Banana"); ImGui.SameLine();
                ImGui.Text("Hello\nWorld"); ImGui.SameLine();
                ImGui.Text("Banana");

                ImGui.Button("HOP##2"); ImGui.SameLine();
                ImGui.Text("Hello\nWorld"); ImGui.SameLine();
                ImGui.Text("Banana");

                ImGui.Button("TEST##1"); ImGui.SameLine();
                ImGui.Text("TEST"); ImGui.SameLine();
                ImGui.SmallButton("TEST##2");

                ImGui.AlignTextToFramePadding(); // If your line starts with text, call this to align it to upcoming widgets.
                ImGui.Text("Text aligned to Widget"); ImGui.SameLine();
                ImGui.Button("Widget##1"); ImGui.SameLine();
                ImGui.Text("Widget"); ImGui.SameLine();
                ImGui.SmallButton("Widget##2"); ImGui.SameLine();
                ImGui.Button("Widget##3");

                // Tree
                float spacing2 = ImGui.GetStyle().ItemInnerSpacing.x;
                ImGui.Button("Button##1");
                ImGui.SameLine(0.0f, spacing2);
                if (ImGui.TreeNode("Node##1")) { for (int i = 0; i < 6; i++) ImGui.BulletText("Item " + i); ImGui.TreePop(); }    // Dummy tree data

                ImGui.AlignTextToFramePadding();         // Vertically align text node a bit lower so it'll be vertically centered with upcoming widget. Otherwise you can use SmallButton (smaller fit).
                bool node_open = ImGui.TreeNode("Node##2");  // Common mistake to avoid: if we want to SameLine after TreeNode we need to do it before we add child content.
                ImGui.SameLine(0.0f, spacing2); ImGui.Button("Button##2");
                if (node_open) { for (int i = 0; i < 6; i++) ImGui.BulletText("Item " + i); ImGui.TreePop(); }   // Dummy tree data

                // Bullet
                ImGui.Button("Button##3");
                ImGui.SameLine(0.0f, spacing2);
                ImGui.BulletText("Bullet text");

                ImGui.AlignTextToFramePadding();
                ImGui.BulletText("Node");
                ImGui.SameLine(0.0f, spacing2); ImGui.Button("Button##4");

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Scrolling"))
            {
                ImGui.TextWrapped("(Use SetScrollHere() or SetScrollFromPosY() to scroll to a given position.)");
                ImGui.Checkbox("Track", ref track);
                ImGui.PushItemWidth(100);
                ImGui.SameLine(130); track |= ImGui.DragInt("##line", ref track_line, 0.25f, 0, 99, "Line = %d");
                bool scroll_to = ImGui.Button("Scroll To Pos");
                ImGui.SameLine(130); scroll_to |= ImGui.DragInt("##pos_y", ref scroll_to_px, 1.00f, 0, 9999, "Y = %d px");
                ImGui.PopItemWidth();
                if (scroll_to) track = false;

                for (int i = 0; i < 5; i++)
                {
                    if (i > 0) ImGui.SameLine();
                    ImGui.BeginGroup();
                    ImGui.Text("" + (i == 0 ? "Top" : i == 1 ? "25%" : i == 2 ? "Center" : i == 3 ? "75%" : "Bottom"));
                    ImGui.BeginChild(ImGui.GetID(i.ToString()), new Vector2(ImGui.GetWindowWidth() * 0.17f, 200.0f), true);
                    if (scroll_to)
                        ImGui.SetScrollFromPosY(ImGui.GetCursorStartPos().y + scroll_to_px, i * 0.25f);
                    for (int line = 0; line < 100; line++)
                    {
                        if (track && line == track_line)
                        {
                            ImGui.TextColored(new Vector4(255, 255, 0), "Line " + line);
                            ImGui.SetScrollHere(i * 0.25f); // 0.0f:top, 0.5f:center, 1.0f:bottom
                        }
                        else
                        {
                            ImGui.Text("Line " + line);
                        }
                    }
                    float scroll_y = ImGui.GetScrollY(), scroll_max_y = ImGui.GetScrollMaxY();
                    ImGui.EndChild();
                    ImGui.Text(scroll_y + " " + scroll_max_y);
                    ImGui.EndGroup();
                }
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Horizontal Scrolling"))
            {
                ImGui.Bullet(); ImGui.TextWrapped("Horizontal scrolling for a window has to be enabled explicitly via the ImGuiWindowFlags.HorizontalScrollbar flag.");
                ImGui.Bullet(); ImGui.TextWrapped("You may want to explicitly specify content width by calling SetNextWindowContentWidth() before Begin().");
                ImGui.SliderInt("Lines", ref lines, 1, 15);
                ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(2.0f, 1.0f));
                ImGui.BeginChild("scrolling", new Vector2(0, ImGui.GetFrameHeightWithSpacing() * 7 + 30), true, ImGuiWindowFlags.HorizontalScrollbar);
                for (int line = 0; line < lines; line++)
                {
                    // Display random stuff (for the sake of this trivial demo we are using basic Button+SameLine. If you want to create your own time line for a real application you may be better off
                    // manipulating the cursor position yourself, aka using SetCursorPos/SetCursorScreenPos to position the widgets yourself. You may also want to use the lower-level ImDrawList API)
                    int num_buttons = 10 + ((line % 2 == 0) ? line * 9 : line * 3);
                    for (int n = 0; n < num_buttons; n++)
                    {
                        if (n > 0) ImGui.SameLine();
                        ImGui.PushID(n + line * 1000);
                        string num_buf = n.ToString();
                        string label = ((n % 15) != 0) ? "FizzBuzz" : ((n % 3) != 0) ? "Fizz" : ((n % 5) != 0) ? "Buzz" : num_buf;
                        float hue = n * 0.05f;
                        ImGui.PushStyleColor(ImGuiCol.Button, (Vector4)Color.HSVToRGB(hue, 0.6f, 0.6f));
                        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, (Vector4)Color.HSVToRGB(hue, 0.7f, 0.7f));
                        ImGui.PushStyleColor(ImGuiCol.ButtonActive, (Vector4)Color.HSVToRGB(hue, 0.8f, 0.8f));
                        ImGui.Button(label, new Vector2(40.0f + (float)Math.Sin((line + n)) * 20.0f, 0.0f));
                        ImGui.PopStyleColor(3);
                        ImGui.PopID();
                    }
                }
                float scroll_x = ImGui.GetScrollX(), scroll_max_x = ImGui.GetScrollMaxX();
                ImGui.EndChild();
                ImGui.PopStyleVar(2);
                float scroll_x_delta = 0.0f;
                ImGui.SmallButton("<<"); if (ImGui.IsItemActive()) scroll_x_delta = -ImGui.GetIO().DeltaTime * 1000.0f; ImGui.SameLine();
                ImGui.Text("Scroll from code"); ImGui.SameLine();
                ImGui.SmallButton(">>"); if (ImGui.IsItemActive()) scroll_x_delta = +ImGui.GetIO().DeltaTime * 1000.0f; ImGui.SameLine();
                ImGui.Text(scroll_x + " " + scroll_max_x);
                if (scroll_x_delta != 0.0f)
                {
                    ImGui.BeginChild("scrolling"); // Demonstrate a trick: you can use Begin to set yourself in the context of another window (here we are already out of your child window)
                    ImGui.SetScrollX(ImGui.GetScrollX() + scroll_x_delta);
                    ImGui.End();
                }
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Clipping"))
            {
                ImGui.TextWrapped("On a per-widget basis we are occasionally clipping text CPU-side if it won't fit in its frame. Otherwise we are doing coarser clipping + passing a scissor rectangle to the renderer. The system is designed to try minimizing both execution and CPU/GPU rendering cost.");
                ImGui.DragFloat2("size", ref size, 0.5f, 0.0f, 200.0f, "%.0f");
                ImGui.TextWrapped("(Click and drag)");
                Vector2 pos = ImGui.GetCursorScreenPos();
                Vector4 clip_rect = new Vector4(pos.x, pos.y, pos.x + size.x, pos.y + size.y);
                ImGui.InvisibleButton("##dummy", size);
                if (ImGui.IsItemActive() && ImGui.IsMouseDragging()) { offset.x += ImGui.GetIO().MouseDelta.x; offset.y += ImGui.GetIO().MouseDelta.y; }
                ImGui.GetWindowDrawList().AddRectFilled(pos, new Vector2(pos.x + size.x, pos.y + size.y), ImGui.Col32(90, 90, 120, 255));
                ImGui.GetWindowDrawList().AddText(ImGui.GetFont(), ImGui.GetFontSize() *2.0f, new Vector2(pos.x+offset.x, pos.y+offset.y), ImGui.Col32(255,255,255,255), "Line 1 hello\nLine 2 clip me!", 0.0f, clip_rect);
                ImGui.TreePop();
            }
        }
        if (ImGui.CollapsingHeader("Popups & Modal windows"))
        {

            if (ImGui.TreeNode("Popups"))
            {
                ImGui.TextWrapped("When a popup is active, it inhibits interacting with windows that are behind the popup. Clicking outside the popup closes it.");

                // Simple selection popup
                // (If you want to show the current selection inside the Button itself, you may want to build a string using the "###" operator to preserve a constant ID with a variable label)
                if (ImGui.Button("Select.."))
                    ImGui.OpenPopup("select");
                ImGui.SameLine();
                ImGui.TextUnformatted(selected_fish == -1 ? "<None>" : names[selected_fish]);
                if (ImGui.BeginPopup("select"))
                {
                    ImGui.Text("Aquarium");
                    ImGui.Separator();
                    for (int i = 0; i < names.Length; i++)
                        if (ImGui.Selectable(names[i]))
                            selected_fish = i;
                    ImGui.EndPopup();
                }

                // Showing a menu with toggles
                if (ImGui.Button("Toggle.."))
                    ImGui.OpenPopup("toggle");
                if (ImGui.BeginPopup("toggle"))
                {
                    for (int i = 0; i < names.Length; i++)
                        ImGui.MenuItem(names[i], "", ref toggles[i]);
                    if (ImGui.BeginMenu("Sub-menu"))
                    {
                        ImGui.MenuItem("Click me");
                        ImGui.EndMenu();
                    }

                    ImGui.Separator();
                    ImGui.Text("Tooltip here");
                    if (ImGui.IsItemHovered())
                        ImGui.SetTooltip("I am a tooltip over a popup");

                    if (ImGui.Button("Stacked Popup"))
                        ImGui.OpenPopup("another popup");
                    if (ImGui.BeginPopup("another popup"))
                    {
                        for (int i = 0; i < names.Length; i++)
                            ImGui.MenuItem(names[i], "", ref toggles[i]);
                        if (ImGui.BeginMenu("Sub-menu"))
                        {
                            ImGui.MenuItem("Click me");
                            ImGui.EndMenu();
                        }
                        ImGui.EndPopup();
                    }
                    ImGui.EndPopup();
                }

                if (ImGui.Button("Popup Menu.."))
                    ImGui.OpenPopup("FilePopup");
                if (ImGui.BeginPopup("FilePopup"))
                {
                    ShowExampleMenuFile();
                    ImGui.EndPopup();
                }

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Context menus"))
            {
                // BeginPopupContextItem() is a helper to provide common/simple popup behavior of essentially doing:
                //    if (IsItemHovered() && IsMouseClicked(0))
                //       OpenPopup(id);
                //    return BeginPopup(id);
                // For more advanced uses you may want to replicate and cuztomize this code. This the comments inside BeginPopupContextItem() implementation.
                ImGui.Text("Value = " + value_context.ToString("0.000") + "(<-- right-click here)");
                if (ImGui.BeginPopupContextItem("item context menu"))
                {
                    if (ImGui.Selectable("Set to zero")) value_context = 0.0f;
                    if (ImGui.Selectable("Set to PI")) value_context = 3.1415f;
                    ImGui.PushItemWidth(-1);
                    ImGui.DragFloat("##Value", ref value_context, 0.1f, 0.0f, 0.0f);
                    ImGui.PopItemWidth();
                    ImGui.EndPopup();
                }

                string buf = "Button: " + name_context + "###Button"; // ### operator override ID ignoring the preceding label
                ImGui.Button(buf);
                if (ImGui.BeginPopupContextItem()) // When used after an item that has an ID (here the Button), we can skip providing an ID to BeginPopupContextItem().
                {
                    ImGui.Text("Edit name:");
                    ImGui.InputText("##edit", name_context);
                    if (ImGui.Button("Close"))
                        ImGui.CloseCurrentPopup();
                    ImGui.EndPopup();
                }
                ImGui.SameLine(); ImGui.Text("(<-- right-click here)");

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Modals"))
            {
                ImGui.TextWrapped("Modal windows are like popups but the user cannot close them by clicking outside the window.");

                if (ImGui.Button("Delete.."))
                    ImGui.OpenPopup("Delete?");
                bool open = true;
                if (ImGui.BeginPopupModal("Delete?", ref open, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    ImGui.Text("All those beautiful files will be deleted.\nThis operation cannot be undone!\n\n");
                    ImGui.Separator();

                    //static int dummy_i = 0;
                    //ImGui.Combo("Combo", ref dummy_i, "Delete\0Delete harder\0");

                    ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
                    ImGui.Checkbox("Don't ask me next time", ref dont_ask_me_next_time);
                    ImGui.PopStyleVar();

                    if (ImGui.Button("OK", new Vector2(120, 0))) { ImGui.CloseCurrentPopup(); }
                    ImGui.SetItemDefaultFocus();
                    ImGui.SameLine();
                    if (ImGui.Button("Cancel", new Vector2(120, 0))) { ImGui.CloseCurrentPopup(); }
                    ImGui.EndPopup();
                }

                if (ImGui.Button("Stacked modals.."))
                    ImGui.OpenPopup("Stacked 1");
                if (ImGui.BeginPopupModal("Stacked 1"))
                {
                    ImGui.Text("Hello from Stacked The First\nUsing style.Colors[ImGuiCol.ModalWindowDimBg] behind it.");

                    ImGui.Combo("Combo", ref item_modal, "aaaa\0bbbb\0cccc\0dddd\0eeee\0\0");
                    ImGui.ColorEdit4("color", ref color_modal);  // This is to test behavior of stacked regular popups over a modal

                    if (ImGui.Button("Add another modal.."))
                        ImGui.OpenPopup("Stacked 2");
                    if (ImGui.BeginPopupModal("Stacked 2"))
                    {
                        ImGui.Text("Hello from Stacked The Second!");
                        if (ImGui.Button("Close"))
                            ImGui.CloseCurrentPopup();
                        ImGui.EndPopup();
                    }

                    if (ImGui.Button("Close"))
                        ImGui.CloseCurrentPopup();
                    ImGui.EndPopup();
                }

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Menus inside a regular window"))
            {
                ImGui.TextWrapped("Below we are testing adding menu items to a regular window. It's rather unusual but should work!");
                ImGui.Separator();
                // NB: As a quirk in this very specific example, we want to differentiate the parent of this menu from the parent of the various popup menus above.
                // To do so we are encloding the items in a PushID()/PopID() block to make them two different menusets. If we don't, opening any popup above and hovering our menu here
                // would open it. This is because once a menu is active, we allow to switch to a sibling menu by just hovering on it, which is the desired behavior for regular menus.
                ImGui.PushID("foo");
                ImGui.MenuItem("Menu item", "CTRL+M");
                if (ImGui.BeginMenu("Menu inside a regular window"))
                {
                    ShowExampleMenuFile();
                    ImGui.EndMenu();
                }
                ImGui.PopID();
                ImGui.Separator();
                ImGui.TreePop();
            }
        }
        if (ImGui.CollapsingHeader("Columns"))
        {
            ImGui.PushID("Columns");

            // Basic columns
            if (ImGui.TreeNode("Basic"))
            {
                ImGui.Text("Without border:");
                ImGui.Columns(3, "mycolumns3", false);  // 3-ways, no border
                ImGui.Separator();
                for (int n = 0; n < 14; n++)
                {
                    string label = "Item " + n;
                    if (ImGui.Selectable(label)) { }
                    //if (ImGui.Button(label, new Vector2(-1,0))) {}
                    ImGui.NextColumn();
                }
                ImGui.Columns(1);
                ImGui.Separator();

                ImGui.Text("With border:");
                ImGui.Columns(4, "mycolumns"); // 4-ways, with border
                ImGui.Separator();
                ImGui.Text("ID"); ImGui.NextColumn();
                ImGui.Text("Name"); ImGui.NextColumn();
                ImGui.Text("Path"); ImGui.NextColumn();
                ImGui.Text("Hovered"); ImGui.NextColumn();
                ImGui.Separator();
                string[] names = { "One", "Two", "Three" };
                string[] paths = { "/path/one", "/path/two", "/path/three" };

                for (int i = 0; i < 3; i++)
                {
                    string label = i.ToString();
                    if (ImGui.Selectable(label, selected_columns == i, ImGuiSelectableFlags.SpanAllColumns))
                        selected_columns = i;
                    bool hovered = ImGui.IsItemHovered();
                    ImGui.NextColumn();
                    ImGui.Text(names[i]); ImGui.NextColumn();
                    ImGui.Text(paths[i]); ImGui.NextColumn();
                    ImGui.Text("" + hovered); ImGui.NextColumn();
                }
                ImGui.Columns(1);
                ImGui.Separator();
                ImGui.TreePop();
            }

            // Create multiple items in a same cell before switching to next column
            if (ImGui.TreeNode("Mixed items"))
            {
                ImGui.Columns(3, "mixed");
                ImGui.Separator();

                ImGui.Text("Hello");
                ImGui.Button("Banana");
                ImGui.NextColumn();

                ImGui.Text("ImGui");
                ImGui.Button("Apple");
                ImGui.InputFloat("red", ref foo, 0.05f, 0, "%.3f");
                ImGui.Text("An extra line here.");
                ImGui.NextColumn();

                ImGui.Text("Sailor");
                ImGui.Button("Corniflower");
                ImGui.InputFloat("blue", ref bar, 0.05f, 0, "%.3f");
                ImGui.NextColumn();

                if (ImGui.CollapsingHeader("Category A")) { ImGui.Text("Blah blah blah"); }
                ImGui.NextColumn();
                if (ImGui.CollapsingHeader("Category B")) { ImGui.Text("Blah blah blah"); }
                ImGui.NextColumn();
                if (ImGui.CollapsingHeader("Category C")) { ImGui.Text("Blah blah blah"); }
                ImGui.NextColumn();
                ImGui.Columns(1);
                ImGui.Separator();
                ImGui.TreePop();
            }

            // Word wrapping
            if (ImGui.TreeNode("Word-wrapping"))
            {
                ImGui.Columns(2, "word-wrapping");
                ImGui.Separator();
                ImGui.TextWrapped("The quick brown fox jumps over the lazy dog.");
                ImGui.TextWrapped("Hello Left");
                ImGui.NextColumn();
                ImGui.TextWrapped("The quick brown fox jumps over the lazy dog.");
                ImGui.TextWrapped("Hello Right");
                ImGui.Columns(1);
                ImGui.Separator();
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Borders"))
            {
                // NB: Future columns API should allow automatic horizontal borders.
                ImGui.Checkbox("horizontal", ref h_borders);
                ImGui.SameLine();
                ImGui.Checkbox("vertical", ref v_borders);
                ImGui.Columns(4, null, v_borders);
                for (int i = 0; i < 4 * 3; i++)
                {
                    if (h_borders && ImGui.GetColumnIndex() == 0)
                        ImGui.Separator();
                    ImGui.Text("a" + i + "a" + i + "a" + i);
                    ImGui.Text("Width " + ImGui.GetColumnWidth().ToString("0.00") + "\nOffset " + ImGui.GetColumnOffset().ToString("0.00"));
                    ImGui.NextColumn();
                }
                ImGui.Columns(1);
                if (h_borders)
                    ImGui.Separator();
                ImGui.TreePop();
            }

            // Scrolling columns
            /*
            if (ImGui.TreeNode("Vertical Scrolling"))
            {
                ImGui.BeginChild("##header", new Vector2(0, ImGui.GetTextLineHeightWithSpacing()+ImGui.GetStyle().ItemSpacing.y));
                ImGui.Columns(3);
                ImGui.Text("ID"); ImGui.NextColumn();
                ImGui.Text("Name"); ImGui.NextColumn();
                ImGui.Text("Path"); ImGui.NextColumn();
                ImGui.Columns(1);
                ImGui.Separator();
                ImGui.EndChild();
                ImGui.BeginChild("##scrollingregion", new Vector2(0, 60));
                ImGui.Columns(3);
                for (int i = 0; i < 10; i++)
                {
                    ImGui.Text("%04d", i); ImGui.NextColumn();
                    ImGui.Text("Foobar"); ImGui.NextColumn();
                    ImGui.Text("/path/foobar/%04d/", i); ImGui.NextColumn();
                }
                ImGui.Columns(1);
                ImGui.EndChild();
                ImGui.TreePop();
            }
            */

            if (ImGui.TreeNode("Horizontal Scrolling"))
            {
                ImGui.SetNextWindowContentSize(new Vector2(1500.0f, 0.0f));
                ImGui.BeginChild("##ScrollingRegion", new Vector2(0, ImGui.GetFontSize() * 20), false, ImGuiWindowFlags.HorizontalScrollbar);
                ImGui.Columns(10);
                int ITEMS_COUNT = 2000;
                using (ImGuiListClipper clipper = new ImGuiListClipper())
                {  // Also demonstrate using the clipper for large list
                    clipper.ItemsCount = ITEMS_COUNT;
                    while (clipper.Step())
                    {
                        for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                            for (int j = 0; j < 10; j++)
                            {
                                ImGui.Text("Line " + i + " Column " + j + "...");
                                ImGui.NextColumn();
                            }
                    }
                }
                ImGui.Columns(1);
                ImGui.EndChild();
                ImGui.TreePop();
            }

            bool node_open = ImGui.TreeNode("Tree within single cell");
            ImGui.SameLine(); ShowHelpMarker("NB: Tree node must be poped before ending the cell. There's no storage of state per-cell.");
            if (node_open)
            {
                ImGui.Columns(2, "tree items");
                ImGui.Separator();
                if (ImGui.TreeNode("Hello")) { ImGui.BulletText("Sailor"); ImGui.TreePop(); }
                ImGui.NextColumn();
                if (ImGui.TreeNode("Bonjour")) { ImGui.BulletText("Marin"); ImGui.TreePop(); }
                ImGui.NextColumn();
                ImGui.Columns(1);
                ImGui.Separator();
                ImGui.TreePop();
            }
            ImGui.PopID();
        }

        if (ImGui.CollapsingHeader("Filtering"))
        {
            ImGuiTextFilter filter = new ImGuiTextFilter("");
            ImGui.Text("Filter usage:\n"
                       + "  \"\"         display all lines\n"
                       + "  \"xxx\"      display lines containing \"xxx\"\n"
                       + "  \"xxx,yyy\"  display lines containing \"xxx\" or \"yyy\"\n"
                       + "  \"-xxx\"     hide lines containing \"xxx\"");
            filter.Draw();
            string[] lines = { "aaa1.c", "bbb1.c", "ccc1.c", "aaa2.cpp", "bbb2.cpp", "ccc2.cpp", "abc.h", "hello, world" };
            for (int i = 0; i < lines.Length; i++)
                if (filter.PassFilter(lines[i]))
                    ImGui.BulletText(lines[i]);
        }

        if (ImGui.CollapsingHeader("Inputs, Navigation & Focus"))
        {
            ImGuiIOPtr io = ImGui.GetIO();

            ImGui.Text($"WantCaptureMouse: {io.WantCaptureMouse}");
            ImGui.Text($"WantCaptureKeyboard: {io.WantCaptureKeyboard}");
            ImGui.Text($"WantTextInput: {io.WantTextInput}");
            ImGui.Text($"WantSetMousePos: {io.WantSetMousePos}");
            ImGui.Text($"NavActive: {io.NavActive}, NavVisible: {io.NavVisible}");

            if (ImGui.TreeNode("Keyboard, Mouse & Navigation State"))
            {
                if (ImGui.IsMousePosValid())
                    ImGui.Text($"Mouse pos: ({io.MousePos.x}, {io.MousePos.y})");
                else
                    ImGui.Text("Mouse pos: <INVALID>");
                ImGui.Text($"Mouse delta: ({ io.MouseDelta.x}, {io.MouseDelta.y})");
                ImGui.Text("Mouse down:"); for (int i = 0; i < io.MouseDown.Count; i++) if (io.MouseDownDuration[i] >= 0.0f) { ImGui.SameLine(); ImGui.Text($"b{i} ({io.MouseDownDuration[i]:#.##} secs)"); }
                ImGui.Text("Mouse clicked:"); for (int i = 0; i < io.MouseDown.Count; i++) if (ImGui.IsMouseClicked(i)) { ImGui.SameLine(); ImGui.Text($"b{i}"); }
                ImGui.Text("Mouse dbl-clicked:"); for (int i = 0; i < io.MouseDown.Count; i++) if (ImGui.IsMouseDoubleClicked(i)) { ImGui.SameLine(); ImGui.Text($"b{i}"); }
                ImGui.Text("Mouse released:"); for (int i = 0; i < io.MouseDown.Count; i++) if (ImGui.IsMouseReleased(i)) { ImGui.SameLine(); ImGui.Text($"b{i}"); }
                ImGui.Text($"Mouse wheel: {io.MouseWheel:#.#}");

                ImGui.Text("Keys down:"); for (int i = 0; i < io.KeysDown.Count; i++) if (io.KeysDownDuration[i] >= 0.0f) { ImGui.SameLine(); ImGui.Text($"{i} ({io.KeysDownDuration[i]:#.##} secs)"); }
                ImGui.Text("Keys pressed:"); for (int i = 0; i < io.KeysDown.Count; i++) if (ImGui.IsKeyPressed(i)) { ImGui.SameLine(); ImGui.Text($"{i}"); }
                ImGui.Text("Keys release:"); for (int i = 0; i < io.KeysDown.Count; i++) if (ImGui.IsKeyReleased(i)) { ImGui.SameLine(); ImGui.Text($"{i}"); }
                ImGui.Text($"Keys mods: {(io.KeyCtrl ? "CTRL " : "")}{(io.KeyShift ? "SHIFT " : "")}{(io.KeyAlt ? "ALT " : "")}{(io.KeySuper ? "SUPER " : "")}");

                ImGui.Text("NavInputs down:"); for (int i = 0; i < io.NavInputs.Count; i++) if (io.NavInputs[i] > 0.0f) { ImGui.SameLine(); ImGui.Text($"[{i}] {io.NavInputs[i]:#.##}"); }
                ImGui.Text("NavInputs pressed:"); for (int i = 0; i < io.NavInputs.Count; i++) if (io.NavInputsDownDuration[i] == 0.0f) { ImGui.SameLine(); ImGui.Text($"[{i}]"); }
                ImGui.Text("NavInputs duration:"); for (int i = 0; i < io.NavInputs.Count; i++) if (io.NavInputsDownDuration[i] >= 0.0f) { ImGui.SameLine(); ImGui.Text($"[{i}] {io.NavInputsDownDuration[i]:#.##}"); }

                ImGui.Button("Hovering me sets the\nkeyboard capture flag");
                if (ImGui.IsItemHovered())
                    ImGui.CaptureKeyboardFromApp(true);
                ImGui.SameLine();
                ImGui.Button("Holding me clears the\nthe keyboard capture flag");
                if (ImGui.IsItemActive())
                    ImGui.CaptureKeyboardFromApp(false);

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Tabbing"))
            {
                ImGui.Text("Use TAB/SHIFT+TAB to cycle through keyboard editable fields.");
                ImGui.InputText("1", buf_tabbing);
                ImGui.InputText("2", buf_tabbing);
                ImGui.InputText("3", buf_tabbing);
                ImGui.PushAllowKeyboardFocus(false);
                ImGui.InputText("4 (tab skip)", buf_tabbing);
                //ImGui.SameLine(); ShowHelperMarker("Use ImGui.PushAllowKeyboardFocus(bool)\nto disable tabbing through certain widgets.");
                ImGui.PopAllowKeyboardFocus();
                ImGui.InputText("5", buf_tabbing);
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Focus from code"))
            {
                bool focus_1 = ImGui.Button("Focus on 1"); ImGui.SameLine();
                bool focus_2 = ImGui.Button("Focus on 2"); ImGui.SameLine();
                bool focus_3 = ImGui.Button("Focus on 3");
                int has_focus = 0;

                if (focus_1) ImGui.SetKeyboardFocusHere();
                ImGui.InputText("1", buf_focus);
                if (ImGui.IsItemActive()) has_focus = 1;

                if (focus_2) ImGui.SetKeyboardFocusHere();
                ImGui.InputText("2", buf_focus);
                if (ImGui.IsItemActive()) has_focus = 2;

                ImGui.PushAllowKeyboardFocus(false);
                if (focus_3) ImGui.SetKeyboardFocusHere();
                ImGui.InputText("3 (tab skip)", buf_focus);
                if (ImGui.IsItemActive()) has_focus = 3;
                ImGui.PopAllowKeyboardFocus();

                if (has_focus != 0)
                    ImGui.Text($"Item with focus: {has_focus}");
                else
                    ImGui.Text("Item with focus: <none>");

                // Use >= 0 parameter to SetKeyboardFocusHere() to focus an upcoming item
                int focus_ahead = -1;
                if (ImGui.Button("Focus on X")) focus_ahead = 0; ImGui.SameLine();
                if (ImGui.Button("Focus on Y")) focus_ahead = 1; ImGui.SameLine();
                if (ImGui.Button("Focus on Z")) focus_ahead = 2;
                if (focus_ahead != -1) ImGui.SetKeyboardFocusHere(focus_ahead);
                ImGui.SliderFloat3("Float3", ref f3, 0.0f, 1.0f);

                ImGui.TextWrapped("NB: Cursor & selection are preserved when refocusing last used item in code.");
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Dragging"))
            {
                ImGui.TextWrapped("You can use ImGui.GetMouseDragDelta(0) to query for the dragged amount on any widget.");
                for (int button = 0; button < 3; button++)
                    ImGui.Text($"IsMouseDragging({button}):\n  w/ default threshold: {ImGui.IsMouseDragging(button)},\n  w/ zero threshold: {ImGui.IsMouseDragging(button, 0.0f)}\n  w/ large threshold: {ImGui.IsMouseDragging(button, 20.0f)}");
                ImGui.Button("Drag Me");
                if (ImGui.IsItemActive())
                {
                    // Draw a line between the button and the mouse cursor
                    var draw_list = ImGui.GetWindowDrawList();
                    draw_list.PushClipRectFullScreen();
                    draw_list.AddLine(io.MouseClickedPos[0], io.MousePos, ImGui.GetColorU32(ImGuiCol.Button), 4.0f);
                    draw_list.PopClipRect();

                    // Drag operations gets "unlocked" when the mouse has moved past a certain threshold (the default threshold is stored in io.MouseDragThreshold)
                    // You can request a lower or higher threshold using the second parameter of IsMouseDragging() and GetMouseDragDelta()
                    Vector2 value_raw = ImGui.GetMouseDragDelta(0, 0.0f);
                    Vector2 value_with_lock_threshold = ImGui.GetMouseDragDelta(0);
                    Vector2 mouse_delta = io.MouseDelta;
                    ImGui.SameLine(); ImGui.Text($"Raw ({value_raw.x:#.#}, {value_raw.y:#.#}), WithLockThresold ({value_with_lock_threshold.x:#.#}, {value_with_lock_threshold.y:#.#}), MouseDelta ({mouse_delta.x:#.#}, {mouse_delta.y:#.#})");
                }
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Mouse cursors"))
            {
                string[] mouse_cursors_names = { "Arrow", "TextInput", "Move", "ResizeNS", "ResizeEW", "ResizeNESW", "ResizeNWSE", "Hand" };
                Debug.Assert(mouse_cursors_names.Length == (int)ImGuiMouseCursor.COUNT);

                ImGui.Text($"Current mouse cursor = {ImGui.GetMouseCursor()}: {mouse_cursors_names[(int)ImGui.GetMouseCursor()]}");
                ImGui.Text("Hover to see mouse cursors:");
                ImGui.SameLine(); ShowHelpMarker("Your application can render a different mouse cursor based on what ImGui.GetMouseCursor() returns. If software cursor rendering (io.MouseDrawCursor) is set ImGui will draw the right cursor for you, otherwise your backend needs to handle it.");
                for (int i = 0; i < (int)ImGuiMouseCursor.COUNT; i++)
                {
                    string label = $"Mouse cursor {i}: {mouse_cursors_names[i]}";
                    ImGui.Bullet(); ImGui.Selectable(label, false);
                    if (ImGui.IsItemHovered() || ImGui.IsItemFocused())
                        ImGui.SetMouseCursor((ImGuiMouseCursor)i);
                }
                ImGui.TreePop();
            }
        }

        // End of ShowDemoWindow()
        ImGui.End();
    }


    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Main Menu Bar / ShowExampleAppMainMenuBar()
    //-----------------------------------------------------------------------------

    // Demonstrate creating a fullscreen menu bar and populating it.
    void ShowExampleAppMainMenuBar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                ShowExampleMenuFile();
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Undo", "CTRL+Z")) { }
                if (ImGui.MenuItem("Redo", "CTRL+Y", false, false)) { }  // Disabled item
                ImGui.Separator();
                if (ImGui.MenuItem("Cut", "CTRL+X")) { }
                if (ImGui.MenuItem("Copy", "CTRL+C")) { }
                if (ImGui.MenuItem("Paste", "CTRL+V")) { }
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
    }


    void ShowExampleMenuFile()
    {
        ImGui.MenuItem("(dummy menu)", null, false, false);
        if (ImGui.MenuItem("New")) { }
        if (ImGui.MenuItem("Open", "Ctrl+O")) { }
        if (ImGui.BeginMenu("Open Recent"))
        {
            ImGui.MenuItem("fish_hat.c");
            ImGui.MenuItem("fish_hat.inl");
            ImGui.MenuItem("fish_hat.h");
            if (ImGui.BeginMenu("More.."))
            {
                ImGui.MenuItem("Hello");
                ImGui.MenuItem("Sailor");
                if (ImGui.BeginMenu("Recurse.."))
                {
                    ShowExampleMenuFile();
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
            ImGui.EndMenu();
        }
        if (ImGui.MenuItem("Save", "Ctrl+S")) { }
        if (ImGui.MenuItem("Save As..")) { }
        ImGui.Separator();
        if (ImGui.BeginMenu("Options"))
        {
            ImGui.MenuItem("Enabled", "", ref menu_enabled);
            ImGui.BeginChild("child", new Vector2(0, 60), true);
            for (int i = 0; i < 10; i++)
                ImGui.Text($"Scrolling Text {i}");
            ImGui.EndChild();
            ImGui.SliderFloat("Value", ref menu_f, 0.0f, 1.0f);
            ImGui.InputFloat("Input", ref menu_f, 0.1f);
            ImGui.Combo("Combo", ref menu_n, "Yes\0No\0Maybe\0\0");
            ImGui.Checkbox("Check", ref b);
            ImGui.EndMenu();
        }
        if (ImGui.BeginMenu("Colors"))
        {
            float sz = ImGui.GetTextLineHeight();
            for (int i = 0; i < (int)ImGuiCol.COUNT; i++)
            {
                string name = ImGui.GetStyleColorName((ImGuiCol)i);
                Vector2 p = ImGui.GetCursorScreenPos();
                ImGui.GetWindowDrawList().AddRectFilled(p, new Vector2(p.x + sz, p.y + sz), ImGui.GetColorU32((ImGuiCol)i));
                ImGui.Dummy(new Vector2(sz, sz));
                ImGui.SameLine();
                ImGui.MenuItem(name);
            }
            ImGui.EndMenu();
        }
        if (ImGui.BeginMenu("Disabled", false)) // Disabled
        {
            Debug.LogError("Impossible thing just happened");
        }
        if (ImGui.MenuItem("Checked", null, true)) { }
        if (ImGui.MenuItem("Quit", "Alt+F4")) { }
    }



    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Debug Console / ShowExampleAppConsole()
    //-----------------------------------------------------------------------------

    // Demonstrate creating a simple console window, with scrolling, filtering, completion and history.
    // For the console example, here we are using a more C++ like approach of declaring a class to hold the data and the functions.
    public class ExampleAppConsole
    {
        char[] InputBuf = new char[256];
        List<string> Items = new List<string>();
        bool ScrollToBottom = false;
        List<string> History = new List<string>();
        int HistoryPos = -1;    // -1: new line, 0..History.Size-1 browsing history.
        List<string> Commands = new List<string>();
        ImGuiTextFilter filter;

        ImGuiInputTextCallback textEditCallback;

        public ExampleAppConsole()
        {
            textEditCallback = data => { return this.TextEditCallback(data); };

            ClearLog();
            InputBuf[0] = '\0';
            HistoryPos = -1;
            Commands.Add("HELP");
            Commands.Add("HISTORY");
            Commands.Add("CLEAR");
            Commands.Add("CLASSIFY");  // "classify" is only here to provide an example of "C"+[tab] completing to "CL" and displaying matches.
            AddLog("Welcome to Dear ImGui!");
        }

        void ClearLog()
        {
            Items.Clear();
            ScrollToBottom = true;
        }

        void AddLog(string buf)
        {
            Items.Add(buf);
            ScrollToBottom = true;
        }

        public void Draw(string title, ref bool p_open)
        {
            filter = new ImGuiTextFilter("");

            ImGui.SetNextWindowSize(new Vector2(520, 600), ImGuiCond.FirstUseEver);
            if (!ImGui.Begin(title, ref p_open))
            {
                ImGui.End();
                return;
            }

            // As a specific feature guaranteed by the library, after calling Begin() the last Item represent the title bar. So e.g. IsItemHovered() will return true when hovering the title bar.
            // Here we create a context menu only available from the title bar.
            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.MenuItem("Close Console"))
                    p_open = false;
                ImGui.EndPopup();
            }

            ImGui.TextWrapped("This example implements a console with basic coloring, completion and history. A more elaborate implementation may want to store entries along with extra data such as timestamp, emitter, etc.");
            ImGui.TextWrapped("Enter 'HELP' for help, press TAB to use text completion.");

            // TODO: display items starting from the bottom

            if (ImGui.SmallButton("Add Dummy Text")) { AddLog(Items.Count + " some text"); AddLog("some more text"); AddLog("display very important message here!"); }
            ImGui.SameLine();
            if (ImGui.SmallButton("Add Dummy Error")) { AddLog("[error] something went wrong"); }
            ImGui.SameLine();
            if (ImGui.SmallButton("Clear")) { ClearLog(); }
            ImGui.SameLine();
            bool copy_to_clipboard = ImGui.SmallButton("Copy"); ImGui.SameLine();
            if (ImGui.SmallButton("Scroll to bottom")) ScrollToBottom = true;
            //static float t = 0.0f; if (ImGui.GetTime() - t > 0.02f) { t = ImGui.GetTime(); AddLog("Spam %f", t); }

            ImGui.Separator();

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
            filter.Draw("Filter (\"incl,-excl\") (\"error\")", 180);
            ImGui.PopStyleVar();
            ImGui.Separator();

            float footer_height_to_reserve = ImGui.GetStyle().ItemSpacing.y + ImGui.GetFrameHeightWithSpacing(); // 1 separator, 1 input text
            ImGui.BeginChild("ScrollingRegion", new Vector2(0, -footer_height_to_reserve), false, ImGuiWindowFlags.HorizontalScrollbar); // Leave room for 1 separator + 1 InputText
            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.Selectable("Clear")) ClearLog();
                ImGui.EndPopup();
            }

            // Display every line as a separate entry so we can change their color or add custom widgets. If you only want raw text you can use ImGui.TextUnformatted(log.begin(), log.end());
            // NB- if you have thousands of entries this approach may be too inefficient and may require user-side clipping to only process visible items.
            // You can seek and display only the lines that are visible using the ImGuiListClipper helper, if your elements are evenly spaced and you have cheap random access to the elements.
            // To use the clipper we could replace the 'for (int i = 0; i < Items.Size; i++)' loop with:
            //     ImGuiListClipper clipper(Items.Size);
            //     while (clipper.Step())
            //         for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
            // However, note that you can not use this code as is if a filter is active because it breaks the 'cheap random-access' property. We would need random-access on the post-filtered list.
            // A typical application wanting coarse clipping and filtering may want to pre-compute an array of indices that passed the filtering test, recomputing this array when user changes the filter,
            // and appending newly elements as they are inserted. This is left as a task to the user until we can manage to improve this example code!
            // If your items are of variable size you may want to implement code similar to what ImGuiListClipper does. Or split your data into fixed height items to allow random-seeking into your list.
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1)); // Tighten spacing
            if (copy_to_clipboard)
                ImGui.LogToClipboard();
            Vector4 col_default_text = ImGui.GetStyleColorVec4(ImGuiCol.Text);
            for (int i = 0; i < Items.Count; i++)
            {
                string item = Items[i];
                if (!filter.PassFilter(item))
                    continue;
                Vector4 col = col_default_text;
                if (item.Contains("[error]")) col = new Vector4(1.0f, 0.4f, 0.4f, 1.0f);
                else if (string.CompareOrdinal(item, 0, "# ", 0, 2) == 0) col = new Vector4(1.0f, 0.78f, 0.58f, 1.0f);
                ImGui.PushStyleColor(ImGuiCol.Text, col);
                ImGui.TextUnformatted(item);
                ImGui.PopStyleColor();
            }
            if (copy_to_clipboard)
                ImGui.LogFinish();
            if (ScrollToBottom)
                ImGui.SetScrollHere(1.0f);
            ScrollToBottom = false;
            ImGui.PopStyleVar();
            ImGui.EndChild();
            ImGui.Separator();

            // Command-line
            bool reclaim_focus = false;
            if (ImGui.InputText("Input", InputBuf, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion | ImGuiInputTextFlags.CallbackHistory, textEditCallback))
            {
                string s = new string(InputBuf);
                s.Trim();
                if (s.Length > 0)
                    ExecCommand(s);
                InputBuf[0] = '\0';
                reclaim_focus = true;
            }

            // Auto-focus on window apparition
            ImGui.SetItemDefaultFocus();
            if (reclaim_focus)
                ImGui.SetKeyboardFocusHere(-1); // Auto focus previous widget

            ImGui.End();
        }

        void ExecCommand(string command_line)
        {
            AddLog("# " + command_line + "\n");

            // Insert into history. First find match and delete it so it can be pushed to the back. This isn't trying to be smart or optimal.
            HistoryPos = -1;
            for (int i = History.Count - 1; i >= 0; i--)
                if (string.Compare(History[i], command_line, true) == 0)
                {
                    History.RemoveAt(i);
                    break;
                }
            History.Add(command_line);

            // Process command
            if (string.Compare(command_line, "CLEAR", true) == 0)
            {
                ClearLog();
            }
            else if (string.Compare(command_line, "HELP", true) == 0)
            {
                AddLog("Commands:");
                for (int i = 0; i < Commands.Count; i++)
                    AddLog("- " + Commands[i]);
            }
            else if (string.Compare(command_line, "HISTORY", true) == 0)
            {
                int first = History.Count - 10;
                for (int i = first > 0 ? first : 0; i < History.Count; i++)
                    AddLog(i.ToString("000") + ": " + History[i] + "\n");
            }
            else
            {
                AddLog("Unknown command: " + command_line + "\n");
            }
        }

        int TextEditCallback(ImGuiInputTextCallbackData data)
        {
            //AddLog("cursor: %d, selection: %d-%d", data->CursorPos, data->SelectionStart, data->SelectionEnd);
            switch (data.EventFlag)
            {
                case ImGuiInputTextFlags.CallbackCompletion:
                    {
                        // Example of TEXT COMPLETION

                        // Locate beginning of current word
                        string word = data.GetString(data.CursorPos);
                        int word_start = word.Length;
                        while (word_start > 0)
                        {
                            char c = word[word_start - 1];
                            if (c == ' ' || c == '\t' || c == ',' || c == ';')
                                break;
                            word_start--;
                        }
                        word = word.Substring(word_start);

                        // Build a list of candidates
                        List<string> candidates = new List<string>();
                        for (int i = 0; i < Commands.Count; i++)
                            if (Commands[i].Contains(word.ToUpper()))
                                candidates.Add(Commands[i]);

                        if (candidates.Count == 0)
                        {
                            // No match
                            AddLog("No match for \"" + word + "\"!\n");
                        }
                        else if (candidates.Count == 1)
                        {
                            // Single match. Delete the beginning of the word and replace it entirely so we've got nice casing
                            data.DeleteChars(word_start, word.Length);
                            data.InsertChars(data.CursorPos, candidates[0]);
                        }
                        else
                        {
                            // Multiple matches. Complete as much as we can, so inputing "C" will complete to "CL" and display "CLEAR" and "CLASSIFY"
                            int match_len = word.Length;
                            for (; ; )
                            {
                                int c = 0;
                                bool all_candidates_matches = true;
                                for (int i = 0; i < candidates.Count && all_candidates_matches; i++)
                                    if (i == 0)
                                        c = char.ToUpper(candidates[i][match_len]);
                                    else if (c == 0 || c != char.ToUpper(candidates[i][match_len]))
                                        all_candidates_matches = false;
                                if (!all_candidates_matches)
                                    break;
                                match_len++;
                            }

                            if (match_len > 0)
                            {
                                data.DeleteChars(word_start, word.Length);
                                data.InsertChars(data.CursorPos, candidates[0]);
                            }

                            // List matches
                            AddLog("Possible matches:\n");
                            for (int i = 0; i < candidates.Count; i++)
                                AddLog("-" + candidates[i] + "\n");
                        }

                        break;
                    }
                case ImGuiInputTextFlags.CallbackHistory:
                    {
                        // Example of HISTORY
                        int prev_history_pos = HistoryPos;
                        if (data.EventKey == ImGuiKey.UpArrow)
                        {
                            if (HistoryPos == -1)
                                HistoryPos = History.Count - 1;
                            else if (HistoryPos > 0)
                                HistoryPos--;
                        }
                        else if (data.EventKey == ImGuiKey.DownArrow)
                        {
                            if (HistoryPos != -1)
                                if (++HistoryPos >= History.Count)
                                    HistoryPos = -1;
                        }

                        // A better implementation would preserve the data on the current input line along with cursor position.
                        if (prev_history_pos != HistoryPos)
                        {
                            string history_str = (HistoryPos >= 0) ? History[HistoryPos] : "";
                            data.DeleteChars(0, data.BufTextLen);
                            data.InsertChars(0, history_str);
                        }
                        break;
                    }
            }
            return 0;
        }
    }

    void ShowExampleAppConsole(ref bool p_open)
    {
        console.Draw("Example: Console", ref p_open);
    }



//-----------------------------------------------------------------------------
// [SECTION] Example App: Debug Log / ShowExampleAppLog()
//-----------------------------------------------------------------------------

// Usage:
//  static ExampleAppLog my_log;
//  my_log.AddLog("Hello %d world\n", 123);
//  my_log.Draw("title");
public class ExampleAppLog
{
    StringBuilder Buf = new StringBuilder();
    ImGuiTextFilter Filter;
    List<int> LineOffsets = new List<int>();        // Index to lines offset
    bool ScrollToBottom = false;

    public void Clear() { Buf.Clear(); LineOffsets.Clear(); LineOffsets.Add(0); }

    public void AddLog(string fmt)
    {
        int old_size = Buf.Length;
        Buf.Append(fmt);
        for (int new_size = Buf.Length; old_size < new_size; old_size++)
            if (Buf[old_size] == '\n')
                LineOffsets.Add(old_size);
        ScrollToBottom = true;
    }

    public void Draw(string title, ref bool p_open)
    {
        Filter = new ImGuiTextFilter("");

        ImGui.SetNextWindowSize(new Vector2(500,400), ImGuiCond.FirstUseEver);
        if (!ImGui.Begin(title, ref p_open))
        {
            ImGui.End();
            return;
        }
        if (ImGui.Button("Clear")) Clear();
            ImGui.SameLine();
        bool copy = ImGui.Button("Copy");
        ImGui.SameLine();
        Filter.Draw("Filter", -100.0f);
        ImGui.Separator();
        ImGui.BeginChild("scrolling", new Vector2(0,0), false, ImGuiWindowFlags.HorizontalScrollbar);
        if (copy) ImGui.LogToClipboard();

        if (Filter.IsActive())
        {
            string buf = Buf.ToString();
            for (int line_no = 1; line_no < LineOffsets.Count && LineOffsets[line_no] < buf.Length; line_no++)
            {
                string line = buf.Substring(LineOffsets[line_no - 1], LineOffsets[line_no] - LineOffsets[line_no - 1]);
                if (Filter.PassFilter(line))
                    ImGui.TextUnformatted(line);
            }
        }
        else
        {
            ImGui.TextUnformatted(Buf.ToString());
        }

        if (ScrollToBottom)
            ImGui.SetScrollHere(1.0f);
        ScrollToBottom = false;
        ImGui.EndChild();
        ImGui.End();
    }
};


    // Demonstrate creating a simple log window with basic filtering.
    void ShowExampleAppLog(ref bool p_open)
    {
        // Demo: add random items (unless Ctrl is held)
        double time = ImGui.GetTime();
        if (time - last_time >= 0.20f && !ImGui.GetIO().KeyCtrl)
        {
            string[] random_words = { "system", "info", "warning", "error", "fatal", "notice", "log" };
            applog.AddLog($"{random_words[rand.Next(random_words.Length)]} Hello, time is {time.ToString("0.0")}, frame count is {ImGui.GetFrameCount()}\n");
            last_time = time;
        }

        applog.Draw("Example: Log", ref p_open);
    }


    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Simple Layout / ShowExampleAppLayout()
    //-----------------------------------------------------------------------------

    // Demonstrate create a window with multiple child windows.
    void ShowExampleAppLayout(ref bool p_open)
    {
        ImGui.SetNextWindowSize(new Vector2(500, 440), ImGuiCond.FirstUseEver);
        if (ImGui.Begin("Example: Layout", ref p_open, ImGuiWindowFlags.MenuBar))
        {
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Close")) p_open = false;
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            // left
            ImGui.BeginChild("left pane", new Vector2(150, 0), true);
            for (int i = 0; i < 100; i++)
            {
                if (ImGui.Selectable($"MyObject {i}", selected == i))
                    selected = i;
            }
            ImGui.EndChild();
            ImGui.SameLine();

            // right
            ImGui.BeginGroup();
            ImGui.BeginChild("item view", new Vector2(0, -ImGui.GetFrameHeightWithSpacing())); // Leave room for 1 line below us
            ImGui.Text($"MyObject: {selected}");
            ImGui.Separator();
            ImGui.TextWrapped("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. ");
            ImGui.EndChild();
            if (ImGui.Button("Revert")) { }
            ImGui.SameLine();
            if (ImGui.Button("Save")) { }
            ImGui.EndGroup();
        }
        ImGui.End();
    }


    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Property Editor / ShowExampleAppPropertyEditor()
    //-----------------------------------------------------------------------------


        struct funcs
        {
            static float[] dummy_members = { 0.0f, 0.0f, 1.0f, 3.1416f, 100.0f, 999.0f, 0.0f, 0.0f };


            public static void ShowDummyObject(string prefix, int uid)
            {
                ImGui.PushID(uid);                      // Use object uid as identifier. Most commonly you could also use the object pointer as a base ID.
                ImGui.AlignTextToFramePadding();  // Text and Tree nodes are less high than regular widgets, here we add vertical spacing to make the tree lines equal high.
                bool node_open = ImGui.TreeNode("Object", $"{prefix}_{uid}");
                ImGui.NextColumn();
                ImGui.AlignTextToFramePadding();
                ImGui.Text("my sailor is rich");
                ImGui.NextColumn();
                if (node_open)
                {
                    for (int i = 0; i< 8; i++)
                    {
                        ImGui.PushID(i); // Use field index as identifier.
                        if (i< 2)
                        {
                            ShowDummyObject("Child", 424242);
                        }
                        else
                        {
                            // Here we use a TreeNode to highlight on hover (we could use e.g. Selectable as well)
                            ImGui.AlignTextToFramePadding();
                            ImGui.TreeNodeEx("Field", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet, $"Field_{i}");
                            ImGui.NextColumn();
                            ImGui.PushItemWidth(-1);
                            if (i >= 5)
                                ImGui.InputFloat("##value", ref dummy_members[i], 1.0f);
                            else
                                ImGui.DragFloat("##value", ref dummy_members[i], 0.01f);
                            ImGui.PopItemWidth();
                            ImGui.NextColumn();
                        }
                        ImGui.PopID();
                    }
                    ImGui.TreePop();
                }
                ImGui.PopID();
            }
        };


    // Demonstrate create a simple property editor.
    void ShowExampleAppPropertyEditor(ref bool p_open)
    {
        ImGui.SetNextWindowSize(new Vector2(430, 450), ImGuiCond.FirstUseEver);
        if (!ImGui.Begin("Example: Property editor", ref p_open))
        {
            ImGui.End();
            return;
        }

        ShowHelpMarker("This example shows how you may implement a property editor using two columns.\nAll objects/fields data are dummies here.\nRemember that in many simple cases, you can use ImGui.SameLine(xxx) to position\nyour cursor horizontally instead of using the Columns() API.");

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(2, 2));
        ImGui.Columns(2);
        ImGui.Separator();

        // Iterate dummy objects with dummy members (all the same data)
        for (int obj_i = 0; obj_i< 3; obj_i++)
            funcs.ShowDummyObject("Object", obj_i);

        ImGui.Columns(1);
        ImGui.Separator();
        ImGui.PopStyleVar();
        ImGui.End();
    }


    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Long Text / ShowExampleAppLongText()
    //-----------------------------------------------------------------------------

    // Demonstrate/test rendering huge amount of text, and the incidence of clipping.
    void ShowExampleAppLongText(ref bool p_open)
    {
        ImGui.SetNextWindowSize(new Vector2(520, 600), ImGuiCond.FirstUseEver);
        if (!ImGui.Begin("Example: Long text display", ref p_open))
        {
            ImGui.End();
            return;
        }

        ImGui.Text("Printing unusually long amount of text.");
        ImGui.Combo("Test type", ref test_type, "Single call to TextUnformatted()\0Multiple calls to Text(), clipped manually\0Multiple calls to Text(), not clipped (slow)\0");
        ImGui.Text("Buffer contents: " + log_lines + " lines, " + log.Length + " bytes");
        if (ImGui.Button("Clear")) { log.Clear(); log_lines = 0; }
        ImGui.SameLine();
        if (ImGui.Button("Add 1000 lines"))
        {
            for (int i = 0; i < 1000; i++)
                log.Append((log_lines + i) + " The quick brown fox jumps over the lazy dog\n");
            log_lines += 1000;
        }
        ImGui.BeginChild("Log");
        switch (test_type)
        {
            case 0:
                // Single call to TextUnformatted() with a big buffer
                ImGui.TextUnformatted(log.ToString());
                break;
            case 1:
                {
                    // Multiple calls to Text(), manually coarsely clipped - demonstrate how to use the ImGuiListClipper helper.
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
                    using (ImGuiListClipper clipper = new ImGuiListClipper(log_lines))
                    {
                        while (clipper.Step())
                            for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                                ImGui.Text($"{i} The quick brown fox jumps over the lazy dog");
                    }
                    ImGui.PopStyleVar();
                    break;
                }
            case 2:
                // Multiple calls to Text(), not clipped (slow)
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
                for (int i = 0; i < log_lines; i++)
                    ImGui.Text(i + " The quick brown fox jumps over the lazy dog");
                ImGui.PopStyleVar();
                break;
        }
        ImGui.EndChild();
        ImGui.End();
    }


    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Auto Resize / ShowExampleAppAutoResize()
    //-----------------------------------------------------------------------------

    // Demonstrate creating a window which gets auto-resized according to its content.
    void ShowExampleAppAutoResize(ref bool p_open)
    {
        if (!ImGui.Begin("Example: Auto-resizing window", ref p_open, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.End();
            return;
        }

        ImGui.Text("Window will resize every-frame to the size of its content.\nNote that you probably don't want to query the window size to\noutput your content because that would create a feedback loop.");
        ImGui.SliderInt("Number of lines", ref lines_autoresize, 1, 20);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < lines_autoresize; i++)
        {
            sb.Clear();
            sb.Append(' ', i * 4);
            sb.Append("This is line ");
            sb.Append(i);
            ImGui.Text(sb.ToString()); // Pad with space to extend size horizontally
        }
        ImGui.End();
    }


    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Constrained Resize / ShowExampleAppConstrainedResize()
    //-----------------------------------------------------------------------------

    // Demonstrate creating a window with custom resize constraints.
    void ShowExampleAppConstrainedResize(ref bool p_open)
    {
        if (type == 0) ImGui.SetNextWindowSizeConstraints(new Vector2(-1, 0), new Vector2(-1, float.MaxValue));      // Vertical only
        if (type == 1) ImGui.SetNextWindowSizeConstraints(new Vector2(0, -1), new Vector2(float.MaxValue, -1));      // Horizontal only
        if (type == 2) ImGui.SetNextWindowSizeConstraints(new Vector2(100, 100), new Vector2(float.MaxValue, float.MaxValue)); // Width > 100, Height > 100
        if (type == 3) ImGui.SetNextWindowSizeConstraints(new Vector2(400, -1), new Vector2(500, -1));          // Width 400-500
        if (type == 4) ImGui.SetNextWindowSizeConstraints(new Vector2(-1, 400), new Vector2(-1, 500));          // Height 400-500
        if (type == 5) ImGui.SetNextWindowSizeConstraints(new Vector2(0, 0), new Vector2(float.MaxValue, float.MaxValue), CustomConstraints.SquareDelegate);          // Always Square
        if (type == 6) ImGui.SetNextWindowSizeConstraints(new Vector2(0, 0), new Vector2(float.MaxValue, float.MaxValue), CustomConstraints.StepDelegate, (IntPtr)100);// Fixed Step

        ImGuiWindowFlags flags = auto_resize ? ImGuiWindowFlags.AlwaysAutoResize : 0;
        if (ImGui.Begin("Example: Constrained Resize", ref p_open, flags))
        {
            string[] desc =
            {
            "Resize vertical only",
            "Resize horizontal only",
            "Width > 100, Height > 100",
            "Width 400-500",
            "Height 400-500",
            "Custom: Always Square",
            "Custom: Fixed Steps (100)",
        };
            if (ImGui.Button("200x200")) { ImGui.SetWindowSize(new Vector2(200, 200)); }
            ImGui.SameLine();
            if (ImGui.Button("500x500")) { ImGui.SetWindowSize(new Vector2(500, 500)); }
            ImGui.SameLine();
            if (ImGui.Button("800x200")) { ImGui.SetWindowSize(new Vector2(800, 200)); }
            ImGui.PushItemWidth(200);
            ImGui.Combo("Constraint", ref type, desc);
            ImGui.DragInt("Lines", ref display_lines, 0.2f, 1, 100);
            ImGui.PopItemWidth();
            ImGui.Checkbox("Auto-resize", ref auto_resize);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < display_lines; i++)
            {
                sb.Clear();
                sb.Append(' ', i * 4);
                sb.Append("Hello, sailor! Making this line long enough for the example.");
                ImGui.Text(sb.ToString());
            }
        }
        ImGui.End();
    }


    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Simple Overlay / ShowExampleAppSimpleOverlay()
    //-----------------------------------------------------------------------------

    // Demonstrate creating a simple static window with no decoration + a context-menu to choose which corner of the screen to use.
    void ShowExampleAppSimpleOverlay(ref bool p_open)
    {
        const float DISTANCE = 10.0f;
        bool corner1 = (corner & 1) != 0;
        bool corner2 = (corner & 2) != 0;
        Vector2 window_pos = new Vector2(corner1 ? ImGui.GetIO().DisplaySize.x - DISTANCE : DISTANCE, corner2 ? ImGui.GetIO().DisplaySize.y - DISTANCE : DISTANCE);
        Vector2 window_pos_pivot = new Vector2(corner1 ? 1.0f : 0.0f, corner2 ? 1.0f : 0.0f);
        if (corner != -1)
            ImGui.SetNextWindowPos(window_pos, ImGuiCond.Always, window_pos_pivot);
        ImGui.SetNextWindowBgAlpha(0.3f); // Transparent background
        if (ImGui.Begin("Example: Simple Overlay", ref p_open, (corner != -1 ? ImGuiWindowFlags.NoMove : 0) | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav))
        {
            ImGui.Text("Simple overlay\n" + "in the corner of the screen.\n" + "(right-click to change position)");
            ImGui.Separator();
            if (ImGui.IsMousePosValid())
                ImGui.Text($"Mouse Position: ({ImGui.GetIO().MousePos.x:#,#},{ImGui.GetIO().MousePos.y:#,#})");
            else
                ImGui.Text("Mouse Position: <invalid>");
            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.MenuItem("Custom", null, corner == -1)) corner = -1;
                if (ImGui.MenuItem("Top-left", null, corner == 0)) corner = 0;
                if (ImGui.MenuItem("Top-right", null, corner == 1)) corner = 1;
                if (ImGui.MenuItem("Bottom-left", null, corner == 2)) corner = 2;
                if (ImGui.MenuItem("Bottom-right", null, corner == 3)) corner = 3;
                if (p_open && ImGui.MenuItem("Close")) p_open = false;
                ImGui.EndPopup();
            }
        }
        ImGui.End();
    }


    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Manipulating Window Titles / ShowExampleAppWindowTitles()
    //-----------------------------------------------------------------------------

    // Demonstrate using "##" and "###" in identifiers to manipulate ID generation.
    // This apply to all regular items as well. Read FAQ section "How can I have multiple widgets with the same label? Can I have widget without a label? (Yes). A primer on the purpose of labels/IDs." for details.
    void ShowExampleAppWindowTitles(ref bool p_open)
    {
        // By default, Windows are uniquely identified by their title.
        // You can use the "##" and "###" markers to manipulate the display/ID.

        // Using "##" to display same title but have unique identifier.
        ImGui.SetNextWindowPos(new Vector2(100, 100), ImGuiCond.FirstUseEver);
        ImGui.Begin("Same title as another window##1");
        ImGui.Text("This is window 1.\nMy title is the same as window 2, but my identifier is unique.");
        ImGui.End();

        ImGui.SetNextWindowPos(new Vector2(100, 200), ImGuiCond.FirstUseEver);
        ImGui.Begin("Same title as another window##2");
        ImGui.Text("This is window 2.\nMy title is the same as window 1, but my identifier is unique.");
        ImGui.End();

        // Using "###" to display a changing title but keep a static identifier "AnimatedTitle"
        string temp = "|/-\\";
        string buf = $"Animated title  {temp[(int)(ImGui.GetTime() / 0.25f) & 3]} {ImGui.GetFrameCount()}###AnimatedTitle";
        ImGui.SetNextWindowPos(new Vector2(100, 300), ImGuiCond.FirstUseEver);
        ImGui.Begin(buf);
        ImGui.Text("This window has a changing title.");
        ImGui.End();
    }


    //-----------------------------------------------------------------------------
    // [SECTION] Example App: Custom Rendering using ImDrawList API / ShowExampleAppCustomRendering()
    //-----------------------------------------------------------------------------

    // Demonstrate using the low-level ImDrawList to draw custom shapes.
    void ShowExampleAppCustomRendering(ref bool p_open)
    {
        ImGui.SetNextWindowSize(new Vector2(350, 560), ImGuiCond.FirstUseEver);
        if (!ImGui.Begin("Example: Custom rendering", ref p_open))
        {
            ImGui.End();
            return;
        }

        // Primitives
        ImGui.Text("Primitives");
        ImGui.DragFloat("Size", ref sz, 0.2f, 2.0f, 72.0f, "%.0f");
        ImGui.DragFloat("Thickness", ref thickness, 0.05f, 1.0f, 8.0f, "%.02f");
        ImGui.ColorEdit3("Color", ref col);

        // Tip: If you do a lot of custom rendering, you probably want to use your own geometrical types and benefit of overloaded operators, etc.
        // Define IM_VEC2_CLASS_EXTRA in imconfig.h to create implicit conversions between your types and new Vector2/Vector4.
        // ImGui defines overloaded operators but they are internal to imgui.cpp and not exposed outside (to avoid messing with your types)
        // In this example we are not using the maths operators!
        ImDrawListPtr draw_list = ImGui.GetWindowDrawList();

        {
            Vector2 p = ImGui.GetCursorScreenPos();
            uint col32 = ImGui.Col32(col);
            float x = p.x + 4.0f, y = p.y + 4.0f, spacing = 8.0f;
            for (int n = 0; n < 2; n++)
            {
                float curr_thickness = (n == 0) ? 1.0f : thickness;
                draw_list.AddCircle(new Vector2(x + sz * 0.5f, y + sz * 0.5f), sz * 0.5f, col32, 20, curr_thickness); x += sz + spacing;
                draw_list.AddRect(new Vector2(x, y), new Vector2(x + sz, y + sz), col32, 0.0f, (int)ImDrawCornerFlags.All, curr_thickness); x += sz + spacing;
                draw_list.AddRect(new Vector2(x, y), new Vector2(x + sz, y + sz), col32, 10.0f, (int)ImDrawCornerFlags.All, curr_thickness); x += sz + spacing;
                draw_list.AddRect(new Vector2(x, y), new Vector2(x + sz, y + sz), col32, 10.0f, (int)(ImDrawCornerFlags.TopLeft | ImDrawCornerFlags.BotRight), curr_thickness); x += sz + spacing;
                draw_list.AddTriangle(new Vector2(x + sz * 0.5f, y), new Vector2(x + sz, y + sz - 0.5f), new Vector2(x, y + sz - 0.5f), col32, curr_thickness); x += sz + spacing;
                draw_list.AddLine(new Vector2(x, y), new Vector2(x + sz, y), col32, curr_thickness); x += sz + spacing;   // Horizontal line (note: drawing a filled rectangle will be faster!)
                draw_list.AddLine(new Vector2(x, y), new Vector2(x, y + sz), col32, curr_thickness); x += spacing;      // Vertical line (note: drawing a filled rectangle will be faster!)
                draw_list.AddLine(new Vector2(x, y), new Vector2(x + sz, y + sz), col32, curr_thickness); x += sz + spacing;   // Diagonal line
                draw_list.AddBezierCurve(new Vector2(x, y), new Vector2(x + sz * 1.3f, y + sz * 0.3f), new Vector2(x + sz - sz * 1.3f, y + sz - sz * 0.3f), new Vector2(x + sz, y + sz), col32, curr_thickness);
                x = p.x + 4;
                y += sz + spacing;
            }
            draw_list.AddCircleFilled(new Vector2(x + sz * 0.5f, y + sz * 0.5f), sz * 0.5f, col32, 32); x += sz + spacing;
            draw_list.AddRectFilled(new Vector2(x, y), new Vector2(x + sz, y + sz), col32); x += sz + spacing;
            draw_list.AddRectFilled(new Vector2(x, y), new Vector2(x + sz, y + sz), col32, 10.0f); x += sz + spacing;
            draw_list.AddRectFilled(new Vector2(x, y), new Vector2(x + sz, y + sz), col32, 10.0f, (int)(ImDrawCornerFlags.TopLeft | ImDrawCornerFlags.BotRight)); x += sz + spacing;
            draw_list.AddTriangleFilled(new Vector2(x + sz * 0.5f, y), new Vector2(x + sz, y + sz - 0.5f), new Vector2(x, y + sz - 0.5f), col32); x += sz + spacing;
            draw_list.AddRectFilled(new Vector2(x, y), new Vector2(x + sz, y + thickness), col32); x += sz + spacing;          // Horizontal line (faster than AddLine, but only handle integer thickness)
            draw_list.AddRectFilled(new Vector2(x, y), new Vector2(x + thickness, y + sz), col32); x += spacing + spacing;     // Vertical line (faster than AddLine, but only handle integer thickness)
            draw_list.AddRectFilled(new Vector2(x, y), new Vector2(x + 1, y + 1), col32); x += sz;                  // Pixel (faster than AddLine)
            draw_list.AddRectFilledMultiColor(new Vector2(x, y), new Vector2(x + sz, y + sz), ImGui.Col32(0, 0, 0, 255), ImGui.Col32(255, 0, 0, 255), ImGui.Col32(255, 255, 0, 255), ImGui.Col32(0, 255, 0, 255));
            ImGui.Dummy(new Vector2((sz + spacing) * 8, (sz + spacing) * 3));
        }
        ImGui.Separator();

        {
            ImGui.Text("Canvas example");
            if (ImGui.Button("Clear")) points.Clear();
            if (points.Count >= 2) { ImGui.SameLine(); if (ImGui.Button("Undo")) { points.RemoveAt(points.Count - 1); points.RemoveAt(points.Count - 1); } }
            ImGui.Text("Left-click and drag to add lines,\nRight-click to undo");

            // Here we are using InvisibleButton() as a convenience to 1) advance the cursor and 2) allows us to use IsItemHovered()
            // But you can also draw directly and poll mouse/keyboard by yourself. You can manipulate the cursor using GetCursorPos() and SetCursorPos().
            // If you only use the ImDrawList API, you can notify the owner window of its extends by using SetCursorPos(max).
            Vector2 canvas_pos = ImGui.GetCursorScreenPos();            // ImDrawList API uses screen coordinates!
            Vector2 canvas_size = ImGui.GetContentRegionAvail();        // Resize canvas to what's available
            if (canvas_size.x < 50.0f) canvas_size.x = 50.0f;
            if (canvas_size.y < 50.0f) canvas_size.y = 50.0f;
            draw_list.AddRectFilledMultiColor(canvas_pos, new Vector2(canvas_pos.x + canvas_size.x, canvas_pos.y + canvas_size.y), ImGui.Col32(50, 50, 50, 255), ImGui.Col32(50, 50, 60, 255), ImGui.Col32(60, 60, 70, 255), ImGui.Col32(50, 50, 60, 255));
            draw_list.AddRect(canvas_pos, new Vector2(canvas_pos.x + canvas_size.x, canvas_pos.y + canvas_size.y), ImGui.Col32(255, 255, 255, 255));

            bool adding_preview = false;
            ImGui.InvisibleButton("canvas", canvas_size);
            Vector2 mouse_pos_in_canvas = new Vector2(ImGui.GetIO().MousePos.x - canvas_pos.x, ImGui.GetIO().MousePos.y - canvas_pos.y);
            if (adding_line)
            {
                adding_preview = true;
                points.Add(mouse_pos_in_canvas);
                if (!ImGui.IsMouseDown(0))
                    adding_line = adding_preview = false;
            }
            if (ImGui.IsItemHovered())
            {
                if (!adding_line && ImGui.IsMouseClicked(0))
                {
                    points.Add(mouse_pos_in_canvas);
                    adding_line = true;
                }
                if (ImGui.IsMouseClicked(1) && points.Count > 0)
                {
                    adding_line = adding_preview = false;
                    points.RemoveAt(points.Count - 1);
                    points.RemoveAt(points.Count - 1);
                }
            }
            draw_list.PushClipRect(canvas_pos, new Vector2(canvas_pos.x + canvas_size.x, canvas_pos.y + canvas_size.y), true);      // clip lines within the canvas (if we resize it, etc.)
            for (int i = 0; i < points.Count - 1; i += 2)
                draw_list.AddLine(new Vector2(canvas_pos.x + points[i].x, canvas_pos.y + points[i].y), new Vector2(canvas_pos.x + points[i + 1].x, canvas_pos.y + points[i + 1].y), ImGui.Col32(255, 255, 0, 255), 2.0f);
            draw_list.PopClipRect();
            if (adding_preview)
                points.RemoveAt(points.Count - 1);
        }

        ImGui.End();
    }

}
