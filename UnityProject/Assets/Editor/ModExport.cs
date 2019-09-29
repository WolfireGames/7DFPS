using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ModExport : MonoBehaviour {
    [MenuItem("Wolfire/Modding/Export Gun Mod")]
    public static void ExportGun () {
        ExportBundle(ModType.Gun);
    }

    [MenuItem("Wolfire/Modding/Export Level Tile Mod")]
    public static void ExportLevelTile () {
        ExportBundle(ModType.LevelTile);
    }

    [MenuItem("Wolfire/Modding/Export Custom Mod")]
    public static void ExportCustom () {
        ExportBundle(ModType.Custom);
    }

    public static void ExportBundle (ModType modType) {
        var pathIn = EditorUtility.OpenFolderPanel("Select \"{modType.ToString()}\" Mod Folder", "Assets/", "");
        var pathOut = $"Assets/Mods/{modType.ToString()}/modfile_{Path.GetFileName(pathIn)}";
        
        if(pathIn == "") // Happens when the user aborts path selection
            return;
        
        Debug.Log($"Exporting \"{modType.ToString()}\" Mod: Source: \"{pathIn}\", Target: \"{pathOut}\"");

        // Get Files and convert absolute paths to relative paths (required by the buildmap)
        var absoluteFiles = Directory.GetFiles(pathIn).Where(name => !name.EndsWith(".meta") && !name.EndsWith(".cs")).ToArray();
        var files = new string[absoluteFiles.Length];

        if(files.Length > 0) {
            var index = absoluteFiles[0].IndexOf("Assets");
            for (int i = 0; i < files.Length; i++)
                files[i] = absoluteFiles[i].Substring(index);
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
}