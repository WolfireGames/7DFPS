using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering;
using System.IO;

public class EditorProgressBar
{
    const int kMaxDepth = 10;
    public string title;
    public string info;
    float[] old_progress = new float[kMaxDepth];
    float[] progress = new float[kMaxDepth];
    int depth = 0;

    float EvaluateProgress()
    {
        float interp = progress[depth];
        for (int temp_depth = depth - 1; temp_depth >= 0; --temp_depth)
        {
            interp = Mathf.Lerp(old_progress[temp_depth], progress[temp_depth], interp);
        }
        /*if (depth == 0) {
            return progress[0];
        }
        if (depth == 1) {
            return Mathf.Lerp(old_progress[0], progress[0], progress[1]);
        }
        if (depth == 2) {
            return Mathf.Lerp(old_progress[0], progress[0], Mathf.Lerp(old_progress[1], progress[1], progress[2]));
        }*/
        return interp;
    }

    public void SetProgress(float val)
    {
        old_progress[depth] = progress[depth];
        progress[depth] = val;
    }

    public void Push()
    {
        ++depth;
        if (depth >= kMaxDepth)
        {
            Debug.LogError("Pushing too many progress bar layers");
            depth = kMaxDepth - 1;
        }
    }

    public void Pop()
    {
        --depth;
        if (depth < 0)
        {
            Debug.LogError("Popping too many progress bar layers");
            depth = 0;
        }
    }

    public bool Display()
    {
        return EditorUtility.DisplayCancelableProgressBar(title, info, EvaluateProgress());
    }

    public bool Display(string new_info, float progress)
    {
        info = new_info;
        SetProgress(progress);
        return EditorUtility.DisplayCancelableProgressBar(title, info, EvaluateProgress());
    }

    public EditorProgressBar(string new_title)
    {
        title = new_title;
    }

    public EditorProgressBar()
    {
        title = "Progress";
    }
}

public class BuildLogger {
    StreamWriter file;
    public BuildLogger(bool build_system) {
        if(build_system) {
            file = new StreamWriter("unitybuild.txt",false);
            file.AutoFlush = true;
        } else {
            file = null;
        }
    }

    public void Log(string message) {
        if( file == null) { 
            Debug.Log(message);
        } else {
            file.WriteLine(message);
        }
    }
}

[Serializable]
public class BuildInfo {
    public int buildnumber;
    public string branch;
}

public class ReceiverBuildScript {
    public static int buildnumber = 0;
    public static void UpdateBuildInfo() {
        string buildinfo_path = Application.dataPath + "/../buildinfo.json";
        string git_ref_file = Application.dataPath + "/../ref.txt";

        try { 
            BuildInfo buildinfo = JsonUtility.FromJson<BuildInfo>(File.ReadAllText(buildinfo_path));

            buildnumber = buildinfo.buildnumber;
        } catch (Exception e) {
            Debug.LogWarning("Unable to open buildinfo.json: " + e.ToString());
        }

        try {
            string git_hash = File.ReadAllText(git_ref_file);
        } catch (Exception e) {
            Debug.LogWarning("Unable to open git ref file. " + e.ToString());
        }
    }

    static string[] base_scenes = {
        "Assets/splashscreen.unity",
        "Assets/scene.unity",
        "Assets/winscene.unity",
        "Assets/Gun Range/range.unity",
    };

    public class BuildConfiguration {
        public string target_path;
        public BuildTarget build_target;
        public BuildOptions build_options;
        public string[] define_symbols = new string[0];
    }

    static void BuildWithConfiguration(BuildConfiguration configuration) {
        UpdateBuildInfo();

        PlayerSettings.SplashScreen.showUnityLogo = false;

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = base_scenes;
        options.locationPathName = configuration.target_path;
        options.target = configuration.build_target;
        options.options = configuration.build_options;

        BuildTargetGroup btg = BuildPipeline.GetBuildTargetGroup(configuration.build_target);
        string original_defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);

        try {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, string.Join(";", configuration.define_symbols));
            BuildPlayer(options);
        } finally {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, original_defines);
        }
    }

    public static class Configurations {

        public static BuildConfiguration win64 = new BuildConfiguration() {
            target_path = "Build/win64/Receiver/Receiver.exe",
            build_target = BuildTarget.StandaloneWindows64,
            build_options = BuildOptions.None,
            define_symbols = new string[0],
        };

        public static BuildConfiguration win32 = new BuildConfiguration() {
            target_path = "Build/win32/Receiver/Receiver.exe",
            build_target = BuildTarget.StandaloneWindows,
            build_options = BuildOptions.None,
            define_symbols = new string[0],
        };

        public static BuildConfiguration linux64 = new BuildConfiguration() {
            target_path = "Build/linux64/Receiver/receiver",
            build_target = BuildTarget.StandaloneLinux64,
            build_options = BuildOptions.None,
            define_symbols = new string[0],
        };

        public static BuildConfiguration linux32 = new BuildConfiguration() {
            target_path = "Build/linux32/Receiver/receiver",
            build_target = BuildTarget.StandaloneLinux64,
            build_options = BuildOptions.None,
            define_symbols = new string[0],
        };

        public static BuildConfiguration macosx64 = new BuildConfiguration() {
            target_path = "Build/macosx64/Receiver",
            build_target = BuildTarget.StandaloneOSX,
            build_options = BuildOptions.None,
            define_symbols = new string[0],
        };

        public static BuildConfiguration macosx64_profiling = new BuildConfiguration() {
            target_path = "Build/macosx64_profiling/Receiver",
            build_target = BuildTarget.StandaloneOSX,
            build_options = BuildOptions.Development,
            define_symbols = new string[0],
        };

        public static BuildConfiguration linux64_profiling = new BuildConfiguration() {
            target_path = "Build/linux64_profiling/Receiver",
            build_target = BuildTarget.StandaloneLinux64,
            build_options = BuildOptions.Development,
            define_symbols = new string[0],
        };

        public static BuildConfiguration win64_profiling = new BuildConfiguration() {
            target_path = "Build/win64_profiling/Receiver/Receiver.exe",
            build_target = BuildTarget.StandaloneWindows64,
            build_options = BuildOptions.Development,
            define_symbols = new string[0],
        };
    }

    [MenuItem("Wolfire/Build/Build Windows 64")]
    public static void BuildWindows64 () {
        BuildWithConfiguration(Configurations.win64);
    }

    [MenuItem("Wolfire/Build/Build Windows 32")]
    public static void BuildWindows32 () {
        BuildWithConfiguration(Configurations.win32);
    }

    [MenuItem("Wolfire/Build/Build Linux 64")]
    public static void BuildLinux64() {
        BuildWithConfiguration(Configurations.linux64);
    }

    [MenuItem("Wolfire/Build/Build Linux 32")]
    public static void BuildLinux32() {
        BuildWithConfiguration(Configurations.linux32);
    }

    [MenuItem("Wolfire/Build/Build MacOSX 64")]
    public static void BuildMacosx64() {
        BuildWithConfiguration(Configurations.macosx64);
    }

    [MenuItem("Wolfire/Build/Build Windows 64 With Profiling Enabled")]
    public static void BuildWindows64ProfilingEnabled() {
        BuildWithConfiguration(Configurations.win64_profiling);
    }

    [MenuItem("Wolfire/Build/Build MacOSX 64 With Profiling Enabled")]
    public static void BuildMacosx64ProfilingEnabled() {
        BuildWithConfiguration(Configurations.macosx64_profiling);
    }

    [MenuItem("Wolfire/Build/Build Linux 64 With Profiling Enabled")]
    public static void BuildLinux64ProfilingEnabled() {
        BuildWithConfiguration(Configurations.linux64_profiling);
    }

    private static void BuildPlayer(BuildPlayerOptions options) {
        UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(options);
        switch (report.summary.result) {
            case UnityEditor.Build.Reporting.BuildResult.Succeeded:
                Debug.Log("Build finished successfully!");
                break;
            case UnityEditor.Build.Reporting.BuildResult.Failed:
                Debug.LogError("Build failed");
                break;
            case UnityEditor.Build.Reporting.BuildResult.Cancelled:
            case UnityEditor.Build.Reporting.BuildResult.Unknown:
                Debug.LogWarning("Build cancelled");
                break;
        }
    }

    [MenuItem("Wolfire/Build/Build All")]
    public static void BuildAll() {
        BuildWindows64();
        BuildWindows32();
        BuildLinux64();
        BuildLinux32();
        BuildMacosx64();
    }

    [MenuItem("Wolfire/Build/Build All (Profiling)")]
    public static void BuildAllProfiling() {
        BuildWindows64ProfilingEnabled();
        BuildLinux64ProfilingEnabled();
        BuildMacosx64ProfilingEnabled();
    }
}