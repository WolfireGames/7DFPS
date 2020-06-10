using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ImGuiSetup {
    static ImGuiSetup() {
        ImGuiSetupWindow.OpenWindowIfNeeded();
    }
}

public class ImGuiSetupWindow : EditorWindow {
// Add menu named "My Window" to the Window menu
    [MenuItem("Window/Dear ImGui Installer")]
    public static void OpenWindow() {
        // Get existing open window or if none, make a new one
        //var window = GetWindow<ImGuiSetupWindow>();
        var window = GetWindowWithRect<ImGuiSetupWindow>(new Rect(0, 0, 270, 150));
        window.Show();
    }

    private void OnEnable() {
        titleContent = new GUIContent("Installer");
        minSize = new Vector2(200,150);
        maxSize = new Vector2(400,400);
    }

    // Can also be used to check if any settings need to be changed
    public static bool UpdatePlayerSettings(bool actually_apply){
        bool something_changed = false;
        if (!PlayerSettings.allowUnsafeCode) {
            something_changed = true;
            if(actually_apply){
                PlayerSettings.allowUnsafeCode = true;
            }
        }
        if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest) {
            something_changed = true;
            if (actually_apply) {
                PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
                Debug.Log("Set Scripting Runtime Version to .NET 4.x, restart editor!");
            }
        }
        if (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone) != ApiCompatibilityLevel.NET_4_6) {
            something_changed = true;
            if (actually_apply) {
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
                Debug.Log("Set Api Compatibility Level to .NET 4.x, restart editor!");
            }
        }
        return something_changed;
    }

    public static bool IsImGuiUnityInstalled() {
        return System.Type.GetType("ImGuiUnityInstalled") != null;
    }

    public static void OpenWindowIfNeeded() {
        if(!IsImGuiUnityInstalled() || UpdatePlayerSettings(false)){
            OpenWindow();
        }
    }
    
    void OnGUI() {
        var style = new GUIStyle(GUI.skin.label);
        style.richText = true;
        style.wordWrap = true;
        EditorGUILayout.LabelField("<b>Dear ImGui Installer</b>", style);
        EditorGUILayout.LabelField("Update project settings for ImGui and import scripts. You will need to save the scene and restart editor after this for them to take effect.", style);
        GUILayout.FlexibleSpace();
        EditorGUILayout.Separator();
        using (new EditorGUI.DisabledScope(!UpdatePlayerSettings(false))) {
            if (GUILayout.Button("Update Project Settings")) {
                UpdatePlayerSettings(true);
            }
        }
        using (new EditorGUI.DisabledScope(IsImGuiUnityInstalled())) {
            if (GUILayout.Button("Install Scripts")) {
                string path = Application.dataPath + "/Plugins/ImGui/ImGui.unitypackage";
                if (System.IO.File.Exists(path)) {
                    try {
                        AssetDatabase.ImportPackage(path, true);
                    } catch (System.Exception e) {
                        Debug.LogError("Unity package import failed: " + e);
                    }
                }
            }
        }
        EditorGUILayout.Separator();
    }
}
