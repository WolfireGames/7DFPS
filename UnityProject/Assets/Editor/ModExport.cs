using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ModExport : MonoBehaviour {
    [MenuItem("Wolfire/Export Mod")]
    public static void ExportMod () {
        ExportBundle();
    }

    public static void ExportBundle () {
        var pathIn = EditorUtility.OpenFolderPanel("Select Mod Folder", "Assets/Mods", "");
        var pathOut = Path.Combine(ModManager.GetModsfolderPath(), $"modfile_{Path.GetFileName(pathIn)}");
        
        if(pathIn == "") // Happens when the user aborts path selection
            return;
        
        Debug.Log($"Exporting Mod: Source: \"{pathIn}\", Target: \"{pathOut}\"");

        // Get Files and convert absolute paths to relative paths (required by the buildmap)
        var absoluteFiles = Directory.GetFiles(pathIn).Where(name => !name.EndsWith(".meta") && !name.EndsWith(".cs")).ToArray();
        var files = new string[absoluteFiles.Length];

        if(files.Length > 0) {
            var index = absoluteFiles[0].IndexOf("Assets");
            for (int i = 0; i < files.Length; i++)
                files[i] = absoluteFiles[i].Substring(index);
        }

        if(!CheckHasHolder(files)) {
            Debug.LogError($"Failed to export \"{Path.GetDirectoryName(pathIn)}\". Make sure you have an appropriate holder included!");
            return;
        }

        // Prepare Bundle
        var buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = Path.GetFileName(pathIn);
        buildMap[0].assetNames = files;

        // Build Folder / Bundle
        Directory.CreateDirectory(pathOut);
        BuildPipeline.BuildAssetBundles(pathOut, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        Debug.Log($"Export Completed. Name: \"{Path.GetFileName(pathIn)}\" with {files.Length} files");
    }

    private static bool CheckHasHolder(string[] files) {
        foreach (string file in files)
            if(ModManager.mainAssets.Values.Contains(Path.GetFileName(file)))
                return true;
        return false;
    }
}