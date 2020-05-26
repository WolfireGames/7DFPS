using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering;
using ImGuiNET;

public class ImGuiUnity : MonoBehaviour {
    public Material material;

    public bool drawDebugWindow = false;

    public string[] customFonts;
    public float customFontPixelSize;

    public string iconFont;
    public float iconFontPixelSize = 12.0f;
    public float iconFontMinAdvanceX;               // Use if you want to make the icon monospaced
    public string iconRangeMin;
    public string iconRangeMax;
    public bool useFreetype;                        // Use the Freetype rasterizer, only valid at startup
    ImRasterizerFlags freetype_flags = ImRasterizerFlags.NoHinting;
    int freetype_hinting = 0;
    bool freetype_bold = false;
    bool freetype_oblique = false;
    bool queue_font_rebuild = false;

    public float uiScale;                           // For scaling the entire UI eg. on Android

    public bool enableCustomCursors;
    public Texture2D customCursorArrow;
    public Texture2D customCursorTextInput;         // When hovering over InputText, etc.
    public Texture2D customCursorResizeNS;          // When hovering over an horizontal border
    public Texture2D customCursorResizeEW;          // When hovering over a vertical border or a column
    public Texture2D customCursorResizeNESW;        // When hovering over the bottom-left corner of a window
    public Texture2D customCursorResizeNWSE;
    public Vector2 cursorHotspot;

    public bool enableInput = true;
    public bool enableMouse = true;
    public bool enableKeyboard = true;
    public bool enableTextInput = true;
    public bool enableGameController = true;

    public string activateAxis = "Fire1";
    public string cancelAxis = "Fire2";
    public string inputAxis = "Fire3";

    // These indicate that ImGui control has handled mouse or keyboard and your code should ignore them
    public bool MouseGrabbed { get; private set; }
    public bool KeyboardGrabbed { get; private set; }

    Camera mainCamera;

    IntPtr context;

    static Dictionary<IntPtr, Texture> textures;
    MaterialPropertyBlock mpb;
    int main_tex_id;

    List<ImGuiMesh> meshes;
    CommandBuffer commandBuffer;
    
    Texture2D fontTexture;
    public ImFontPtr[] imFonts { get; private set; }

    KeyCode[] keyCodes;

    void DrawDebugWindow() {
        ImGui.Begin("ImGuiCommandBufferRenderer Debug");
        if(meshes != null){
            ImGui.Text(string.Format("meshes.Count: {0}", meshes.Count));
        }
        if(textures != null){
            ImGui.Text(string.Format("textures.Count: {0}", textures.Count));
        }
        if(ImGui.Checkbox("Use Freetype", ref useFreetype)){
            queue_font_rebuild = true;
        }
        if(useFreetype) {
            bool changed = false;
            changed |= ImGui.Combo("Hinting", ref freetype_hinting, new string[] {"Default", "No Hinting", "No Auto", "Force Auto", "Light", "Mono"});
            changed |= ImGui.Checkbox("Bold", ref freetype_bold);
            changed |= ImGui.Checkbox("Oblique", ref freetype_oblique);
            if(changed){
                freetype_flags = 0;
                switch(freetype_hinting) {
                    case 0: freetype_flags = ImRasterizerFlags.None; break;
                    case 1: freetype_flags = ImRasterizerFlags.NoHinting; break;
                    case 2: freetype_flags = ImRasterizerFlags.NoAutoHint; break;
                    case 3: freetype_flags = ImRasterizerFlags.ForceAutoHint; break;
                    case 4: freetype_flags = ImRasterizerFlags.LightHinting; break;
                    case 5: freetype_flags = ImRasterizerFlags.MonoHinting; break;
                }
                if(freetype_bold){
                    freetype_flags |= ImRasterizerFlags.Bold;
                }
                if (freetype_oblique) {
                    freetype_flags |= ImRasterizerFlags.Oblique;
                }
                queue_font_rebuild = true;
            }
        }
        ImGui.End();
    }

    void RebuildFonts() {
        ImGuiIOPtr io = ImGui.GetIO();
        // Default font
        ImFontAtlasPtr fonts = io.Fonts;
        fonts.Clear();
        using (ImFontConfig config = new ImFontConfig()) {
            config.SizePixels = 13.0f * uiScale;    // ImGui default font size is 13 pixels
            config.OversampleH = 1;
            fonts.AddFontDefault(config);
        }

#if !UNITY_ANDROID
        // Icon font
        if (iconFont.Length > 0) {
            using (ImFontConfig config = new ImFontConfig()) {
                config.MergeMode = true;
                if (iconFontMinAdvanceX > 0) {
                    config.GlyphMinAdvanceX = iconFontMinAdvanceX;
                    config.GlyphMaxAdvanceX = iconFontMinAdvanceX;
                }
                config.PixelSnapH = true;
                config.OversampleH = 1;
                ushort rangeMin = ushort.Parse(iconRangeMin, NumberStyles.HexNumber);
                ushort rangeMax = ushort.Parse(iconRangeMax, NumberStyles.HexNumber);
                if (rangeMin != 0 && rangeMax != 0) {
                    ushort[] icon_ranges = { rangeMin, rangeMax, 0 };
                    ImGui.AddFontFromFileTTF(fonts, Application.streamingAssetsPath + "/" + iconFont, iconFontPixelSize * uiScale, config, icon_ranges);
                }
            }
        }

        // Custom fonts
        imFonts = new ImFontPtr[customFonts.Length];
        for (int i = 0; i < customFonts.Length; i++) {
            using (ImFontConfig config = new ImFontConfig()) {
                config.OversampleH = 1;
                config.OversampleV = 1;
                imFonts[i] = fonts.AddFontFromFileTTF(Application.streamingAssetsPath + "/" + customFonts[i], customFontPixelSize * uiScale, config);
            }
        }
#endif

        if (useFreetype) {
            // Freetype rasterizer
            ImGui.BuildFontAtlas(fonts, freetype_flags);
        } else {
            fonts.Build();
        }
        RecreateFontTexture();
    }

    void Awake() {
        mainCamera = gameObject.GetComponent<Camera>();

        textures = new Dictionary<IntPtr, Texture>();
        mpb = new MaterialPropertyBlock();
        main_tex_id = Shader.PropertyToID("_MainTex");

        context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        ImGuiIOPtr io = ImGui.GetIO();
        io.SetIniFile(string.Format("{0}/dear_imgui.ini", Application.persistentDataPath));

        ImGui.GetStyle().ScaleAllSizes(uiScale);

        RebuildFonts();

        SetKeyMappings();

        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        io.BackendFlags |= ImGuiBackendFlags.HasGamepad;
    }

    void OnDestroy()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        // DestroyContext will write the ini file
        ImGui.DestroyContext(context);
        // We must set it to null to clear the allocated buffer or Unity will complain
        io.SetIniFile(null);
    }

    void OnDisable()
    {
        if (commandBuffer != null)
        {
            if (mainCamera)
            {
                mainCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
            }
            commandBuffer = null;
        }
    }

    void Update()
    {
        if (textures == null)
        {
            textures = new Dictionary<IntPtr, Texture>();
        }

        mainCamera.gameObject.transform.position = new Vector3(0, 0, -10);

        // ImGuiRenderer should be first in Script Execution Order
        SetPerFrameImGuiData();
        if (enableInput)
        {
            UpdateInput();
        }

        ImGui.NewFrame();
    }

    void LateUpdate() {
        if (drawDebugWindow) {
            DrawDebugWindow();
        }
        ImGui.EndFrame();

        if (queue_font_rebuild) {
            RebuildFonts();
            queue_font_rebuild = false;
        }

        // custom cursors
        if (enableCustomCursors)
        {
            ImGuiMouseCursor cursor = ImGui.GetMouseCursor();
            Texture2D cursorTex = null;
            Vector2 hotspot = cursorHotspot;
            switch (cursor)
            {
                case ImGuiMouseCursor.Arrow:
                    cursorTex = customCursorArrow;
                    break;
                case ImGuiMouseCursor.TextInput:
                    cursorTex = customCursorTextInput;
                    break;
                case ImGuiMouseCursor.ResizeEW:
                    cursorTex = customCursorResizeEW;
                    break;
                case ImGuiMouseCursor.ResizeNESW:
                    cursorTex = customCursorResizeNESW;
                    hotspot.x += 5; hotspot.y += 5;
                    break;
                case ImGuiMouseCursor.ResizeNS:
                    cursorTex = customCursorResizeNS;
                    break;
                case ImGuiMouseCursor.ResizeNWSE:
                    cursorTex = customCursorResizeNWSE;
                    hotspot.x += 5; hotspot.y += 5;
                    break;
                default:
                    break;
            }
            if (cursorTex != null)
            {
                Cursor.SetCursor(cursorTex, hotspot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
            }
        }

        ImGui.Render();

        // render ImGui
        ImDrawDataPtr data = ImGui.GetDrawData();

        // resize meshes array
        int numDrawCommands = 0;
        for (int i = 0; i < data.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = data.getDrawListPtr(i);
            var cmdBuffer = cmdList.CmdBuffer;
            numDrawCommands += cmdBuffer.Size;
        }

        if (meshes == null)
        {
            meshes = new List<ImGuiMesh>();
        }

        if (meshes.Count != numDrawCommands)
        {
            // add new meshes to list if needed
            for (int i = meshes.Count; i < numDrawCommands; i++)
            {
                ImGuiMesh mesh = new ImGuiMesh();
                meshes.Add(mesh);
            }
            // delete extra meshes if needed
            for (int i = meshes.Count - 1; i >= numDrawCommands; i--)
            {
                Destroy(meshes[i].mesh);
                meshes.RemoveAt(i);
            }
        }

        if (commandBuffer == null)
        {
            commandBuffer = new CommandBuffer();
            commandBuffer.name = "ImGui Renderer";
            commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            mainCamera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
        }

        commandBuffer.Clear();
        commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
        // orthogonal projection of GUI mesh to camera space
        Matrix4x4 matrix = Matrix4x4.Ortho(0, Screen.width, 0, Screen.height, 0, 0.1f);
        // negate world to camera transform and projection which Unity applies itself
        matrix = mainCamera.cameraToWorldMatrix * mainCamera.projectionMatrix.inverse * matrix;

        // update Command Buffers
        int count = 0;
        for (int i = 0; i < data.CmdListsCount; i++) {
            ImDrawListPtr cmdList = data.getDrawListPtr(i);
            var cmdBuffer = cmdList.CmdBuffer;

            uint startElement = 0;
            for (int j = 0; j < cmdBuffer.Size; j++) {
                ImDrawCmd cmd = cmdBuffer[j];
                Rect rect = new Rect
                {
                    min = new Vector2(cmd.ClipRect.x, Screen.height - cmd.ClipRect.y),
                    max = new Vector2(cmd.ClipRect.z, Screen.height - cmd.ClipRect.w)
                };
                commandBuffer.EnableScissorRect(rect);

                meshes[count].UpdateMesh(cmd, cmdList.IdxBuffer, cmdList.VtxBuffer, (int)startElement);
                if (textures.ContainsKey(cmd.TextureId))
                {
                    mpb.SetTexture(main_tex_id, textures[cmd.TextureId]);
                    commandBuffer.DrawMesh(meshes[count].mesh, matrix, material, 0, 0, mpb);
                }
                else
                {
                    Debug.LogWarning("Image texture missing!");
                }

                startElement += cmd.ElemCount;
                count++;
            }
        }

        textures.Clear();
        textures.Add(fontTexture.GetNativeTexturePtr(), fontTexture);
    }

    unsafe public void RecreateFontTexture()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        // Build
        byte* pixels;
        int width, height, bytesPerPixel;
        io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);

        // Flip font texture Y since we must flip all texture UVs
        Color32[] tempTexture = new Color32[width * height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                tempTexture[j + (height - i - 1) * width] = ((Color32*)pixels)[j + i * width];
            }
        }
        fontTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);  //R8_G8_B8_A8_UNorm
        fontTexture.SetPixels32(tempTexture, 0);
        fontTexture.Apply();
        material.mainTexture = fontTexture;
        // Store our identifier
        io.Fonts.SetTexID(fontTexture.GetNativeTexturePtr());

        io.Fonts.ClearTexData();
    }

    void SetPerFrameImGuiData()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(Screen.width, Screen.height);
        io.DeltaTime = Time.deltaTime; // in seconds
    }

    void UpdateInput()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        MouseGrabbed = io.WantCaptureMouse;
        KeyboardGrabbed = io.WantCaptureKeyboard;

        if (enableMouse)
        {
            Vector2 mPos = Input.mousePosition;
            mPos.y = Screen.height - mPos.y;
            io.MousePos = mPos;
            io.MouseWheel = Input.mouseScrollDelta.y;
            RangeAccessor<Bool8> mouseDown = io.MouseDown;
            mouseDown[0] = Input.GetMouseButton(0);
            mouseDown[1] = Input.GetMouseButton(1);
            mouseDown[2] = Input.GetMouseButton(2);
        }

        if (enableTextInput)
        {
            foreach (char c in Input.inputString)
            {
                io.AddInputCharacter(c);
            }
        }

        if (enableKeyboard)
        {
            RangeAccessor<Bool8> keysDown = io.KeysDown;

            if (keyCodes == null)
            {
                keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
            }

            foreach (KeyCode keyCode in keyCodes)
            {
                keysDown[(int)keyCode] = Input.GetKey(keyCode);
            }

            io.KeyCtrl = Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl);
            io.KeyAlt = Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt);
            io.KeyShift = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            io.KeySuper = Input.GetKey(KeyCode.RightWindows) || Input.GetKey(KeyCode.LeftWindows);
        }

        // TODO: io.NavInputs seems to be invalid. Find out why.
        /*
        if (enableGameController)
        {
            RangeAccessor<float> buttonsDown = io.NavInputs;
            buttonsDown[(int)ImGuiNavInput.Activate] = Input.GetAxis(activateAxis);
            buttonsDown[(int)ImGuiNavInput.Cancel] = Input.GetAxis(cancelAxis);
            buttonsDown[(int)ImGuiNavInput.Input] = Input.GetAxis(inputAxis);

            float horizontal = Input.GetAxis("Horizontal");
            if (horizontal < 0.0f)
            {
                buttonsDown[(int)ImGuiNavInput.LStickLeft] = - horizontal;
                buttonsDown[(int)ImGuiNavInput.KeyLeft] = - horizontal;
            }
            else
            {
                buttonsDown[(int)ImGuiNavInput.LStickRight] = horizontal;
                buttonsDown[(int)ImGuiNavInput.KeyRight] = horizontal;
            }
            float vertical = Input.GetAxis("Vertical");
            if (vertical < 0.0f)
            {
                buttonsDown[(int)ImGuiNavInput.LStickDown] = - vertical;
                buttonsDown[(int)ImGuiNavInput.KeyDown] = - vertical;
            }
            else
            {
                buttonsDown[(int)ImGuiNavInput.LStickUp] = vertical;
                buttonsDown[(int)ImGuiNavInput.KeyUp] = vertical;
            }
        }
        */
    }

    void SetKeyMappings()
    {
        RangeAccessor<int> keyMap = ImGui.GetIO().KeyMap;
        keyMap[(int)ImGuiKey.Tab] = (int)KeyCode.Tab;
        keyMap[(int)ImGuiKey.LeftArrow] = (int)KeyCode.LeftArrow;
        keyMap[(int)ImGuiKey.RightArrow] = (int)KeyCode.RightArrow;
        keyMap[(int)ImGuiKey.UpArrow] = (int)KeyCode.UpArrow;
        keyMap[(int)ImGuiKey.DownArrow] = (int)KeyCode.DownArrow;
        keyMap[(int)ImGuiKey.PageUp] = (int)KeyCode.PageUp;
        keyMap[(int)ImGuiKey.PageDown] = (int)KeyCode.PageDown;
        keyMap[(int)ImGuiKey.Home] = (int)KeyCode.Home;
        keyMap[(int)ImGuiKey.Insert] = (int)KeyCode.Insert;
        keyMap[(int)ImGuiKey.End] = (int)KeyCode.End;
        keyMap[(int)ImGuiKey.Delete] = (int)KeyCode.Delete;
        keyMap[(int)ImGuiKey.Backspace] = (int)KeyCode.Backspace;
        keyMap[(int)ImGuiKey.Space] = (int)KeyCode.Space;
        keyMap[(int)ImGuiKey.Enter] = (int)KeyCode.Return;
        keyMap[(int)ImGuiKey.Escape] = (int)KeyCode.Escape;
        keyMap[(int)ImGuiKey.A] = (int)KeyCode.A;
        keyMap[(int)ImGuiKey.C] = (int)KeyCode.C;
        keyMap[(int)ImGuiKey.V] = (int)KeyCode.V;
        keyMap[(int)ImGuiKey.X] = (int)KeyCode.X;
        keyMap[(int)ImGuiKey.Y] = (int)KeyCode.Y;
        keyMap[(int)ImGuiKey.Z] = (int)KeyCode.Z;
    }

    public static void Image(Texture texture, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 tint_col, Vector4 border_col)
    {
        AddTexture(texture);
        ImGuiNative.igImage(texture.GetNativeTexturePtr(), size, uv0, uv1, tint_col, border_col);
    }

    public static bool ImageButton(Texture texture, Vector2 size, Vector2 uv0, Vector2 uv1, int frame_padding, Vector4 bg_col, Vector4 tint_col)
    {
        AddTexture(texture);
        byte ret = ImGuiNative.igImageButton(texture.GetNativeTexturePtr(), size, uv0, uv1, frame_padding, bg_col, tint_col);
        return ret != 0;
    }

    public static unsafe void AddDrawListImageQuad(ImDrawList* list, Texture texture, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 uv_a, Vector2 uv_b, Vector2 uv_c, Vector2 uv_d, uint col)
    {
        AddTexture(texture);
        ImGuiNative.ImDrawList_AddImageQuad(list, texture.GetNativeTexturePtr(), a, b, c, d, uv_a, uv_b, uv_c, uv_d, col);
    }

    public static unsafe void AddDrawListImage(ImDrawList* list, Texture texture, Vector2 a, Vector2 b, Vector2 uv_a, Vector2 uv_b, uint col)
    {
        AddTexture(texture);
        ImGuiNative.ImDrawList_AddImage(list, texture.GetNativeTexturePtr(), a, b, uv_a, uv_b, col);
    }

    public static unsafe void AddDrawListImageRounded(ImDrawList* list, Texture texture, Vector2 a, Vector2 b, Vector2 uv_a, Vector2 uv_b, uint col, float rounding, int rounding_corners)
    {
        AddTexture(texture);
        ImGuiNative.ImDrawList_AddImageRounded(list, texture.GetNativeTexturePtr(), a, b, uv_a, uv_b, col, rounding, rounding_corners);
    }

    static void AddTexture(Texture texture)
    {
        IntPtr ptr = texture.GetNativeTexturePtr();
        if (!textures.ContainsKey(ptr))
        {
            textures[ptr] = texture;
        }
    }
}
