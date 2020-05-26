using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;
using IconFonts;

public class SampleUI : MonoBehaviour
{
    public List<Texture> uiTextures;

    // UI state
    private static float _f = 0.0f;
    private static int _counter = 0;
    private static int _dragInt = 0;
    private static bool _showDemoWindow = true;
    private static bool _showAnotherWindow = false;

    private ImGuiUnity imGui;

    void Start()
    {
        imGui = GameObject.Find("ImGuiUnity").GetComponent<ImGuiUnity>();
    }

    void Update()
    {
        // Demo code adapted from the official Dear ImGui demo program:
        // https://github.com/ocornut/imgui/blob/master/examples/example_win32_directx11/main.cpp#L172

        // 1. Show a simple window.
        // Tip: if we don't call ImGui.BeginWindow()/ImGui.EndWindow() the widgets automatically appears in a window called "Debug".
        {
            ImGui.Text("Hello, world!");                                        // Display some text (you can use a format string too)
            ImGui.SliderFloat("float", ref _f, 0, 1, _f.ToString("0.000"), 1);  // Edit 1 float using a slider from 0.0f to 1.0f    

            ImGui.Text($"Mouse position: {ImGui.GetMousePos()}");

            ImGui.Checkbox("Demo Window", ref _showDemoWindow);                 // Edit bools storing our windows open/close state
            ImGui.Checkbox("Another Window", ref _showAnotherWindow);
#if !UNITY_ANDROID
            ImGui.PushFont(imGui.imFonts[0]);                                   // Custom font example
#endif
            if (ImGui.Button("Button"))                                         // Buttons return true when clicked (NB: most widgets return true when edited/activated)
                _counter++;
            ImGui.SameLine(0, -1);
            ImGui.Text($"counter = {_counter}");

            ImGui.DragInt("Draggable Int", ref _dragInt);

            float framerate = ImGui.GetIO().Framerate;
            ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
#if !UNITY_ANDROID
            ImGui.PopFont();
#endif

            ImGui.Text("Some icons: " + FontAwesome5.Undo + FontAwesome5.FileAudio + FontAwesome5.FileExcel); // Icon font example
            ImGui.Button(FontAwesome5.PenFancy + " " + FontAwesome5.Rocket);

            //ImGui.ImageButton(uiTextures[1], new Vector2(100, 200), new Vector2(), new Vector2(1, 1), -1, new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 1));

            //ImGui.Image(uiTextures[0], new Vector2(100, 200));

            //ImGui.GetWindowDrawList().AddImage(uiTextures[0], new Vector2(200, 400), new Vector2(400, 500));
        }

        // 2. Show another simple window. In most cases you will use an explicit Begin/End pair to name your windows.
        if (_showAnotherWindow)
        {
            ImGui.Begin("Another Window", ref _showAnotherWindow);
            ImGui.Text("Hello from another window!");
            if (ImGui.Button("Close Me"))
                _showAnotherWindow = false;
            ImGui.End();
        }

        // 3. Show the ImGui demo window. Most of the sample code is in ImGui.ShowDemoWindow(). Read its code to learn more about Dear ImGui!
        if (_showDemoWindow)
        {
            // Normally user code doesn't need/want to call this because positions are saved in .ini file anyway.
            // Here we just want to make the demo initial state a bit more friendly!
            ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
            ImGui.ShowDemoWindow(ref _showDemoWindow);
        }
    }
}
