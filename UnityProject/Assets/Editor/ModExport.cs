using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class ModExport : MonoBehaviour {
    [MenuItem("Wolfire/Export Mod")]
    public static void ExportMod () {
        string root_path = EditorUtility.OpenFolderPanel("Select Mod Folder", "Assets/Mods", "");
        if(root_path == "") // Happens when the user aborts path selection
            return;

        ExportBundle(root_path);
    }

    [MenuItem("Wolfire/Batch Export Mods")]
    public static void BatchExportMods () {
        string root_path = EditorUtility.OpenFolderPanel("Select Mods Folder", "Assets/Mods", "");
        if(root_path == "") // Happens when the user aborts path selection
            return;

        // Gather mods and names
        string[] paths = Directory.GetDirectories(root_path);
        List<string> names = new List<string>();
        foreach (string path in paths)
            names.Add(Path.GetFileName(path));

        if(names.Count <= 0)
            throw new System.Exception("There were no potential mods found in your folder selection. Make sure to select the parent folder of your mods when batch exporting!");

        // Present parameters
        if(!EditorUtility.DisplayDialog("Batch Export Mods", $"Are you sure you want to batch export following mods?\n - {string.Join("\n - ", names)}\nMods that raise errors will be skipped.", "Export", "Cancel"))
            return;

        // Export each path individually
        foreach (string path in paths) {
            try {
                ExportBundle(path);
            } catch (System.Exception e) {
                Debug.LogException(e);
            }
        }
    }

    [MenuItem("Wolfire/Open Mods Path")]
    public static void OpenModPath () {
        Application.OpenURL(ModManager.GetModsfolderPath());
    }

    public static void ExportBundle (string source) {
        string dest = Path.Combine(ModManager.GetModsfolderPath(), $"modfile_{Path.GetFileName(source)}");
        Debug.Log($"Exporting Mod: Source: \"{source}\", Target: \"{dest}\"");

        // Get Files and convert absolute paths to relative paths (required by the buildmap)
        string[] absolute_files = Directory.GetFiles(source).Where(name => !name.EndsWith(".meta") && !name.EndsWith(".cs")).ToArray();
        string[] files = new string[absolute_files.Length];

        if(files.Length > 0) {
            int index = absolute_files[0].IndexOf("Assets");
            for (int i = 0; i < files.Length; i++)
                files[i] = absolute_files[i].Substring(index);
        }

        if(!CheckHasHolder(files))
            throw new System.Exception($"Failed to export \"{Path.GetDirectoryName(source)}\". Make sure you have an appropriate holder included!");

        // Prepare Bundle
        AssetBundleBuild[] build_map = new AssetBundleBuild[1];
        build_map[0].assetNames = files;
        
        // Build Folder / Bundle
        Directory.CreateDirectory(dest);
        foreach (var target in new Dictionary<OperatingSystemFamily, BuildTarget> {{OperatingSystemFamily.Windows, BuildTarget.StandaloneWindows64}, {OperatingSystemFamily.Linux, BuildTarget.StandaloneLinux64}, {OperatingSystemFamily.MacOSX, BuildTarget.StandaloneOSX}}) {
            build_map[0].assetBundleName = $"{Path.GetFileName(source)}_{target.Key}";
            BuildPipeline.BuildAssetBundles(dest, build_map, BuildAssetBundleOptions.None, target.Value);
        }

        // Clean out manifest files and bundle holder
        try {
            if(Directory.GetFiles(dest).Count() > 10) { // FAILSAFE
                throw new System.ArgumentException($"failsafe triggered: too many files found in mod folder! Skipping deletion of unneeded files!");
            }

            File.Delete(Path.Combine(dest, Path.GetFileName(dest)));
            foreach (var manifest in Directory.GetFiles(dest, "*.manifest", SearchOption.TopDirectoryOnly)) {
                File.Delete(Path.Combine(dest, manifest));
            }
        } catch (System.UnauthorizedAccessException) {
            Debug.LogWarning("Unauthorized to delete obsolete files in the exported mod. These files are not needed and don't need to be removed manually.");
        } catch (System.ArgumentException e) {
            Debug.LogError(e);
        }

        // Make Thumbnail
        GameObject thumbnailMakerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThumbnailMaker.prefab");
        if(thumbnailMakerPrefab) {
            Mod mod = ModImporter.ImportModNow(dest, true);

            string lastScene = EditorSceneManager.GetActiveScene().path;
            int lastBuildIndex = EditorSceneManager.GetActiveScene().buildIndex;
            
            // Create scene
            Scene thumbnailScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            ThumbnailMaker thumbnailMaker = Instantiate(thumbnailMakerPrefab).GetComponent<ThumbnailMaker>();
            
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(mod.path), "thumbnail.jpg"), thumbnailMaker.CreateThumbnail(mod).EncodeToJPG(90));
            // Cleanup
            if(lastBuildIndex >= 0) {
                EditorSceneManager.OpenScene(lastScene, OpenSceneMode.Single);
            } else {
                EditorSceneManager.OpenScene(EditorSceneManager.GetSceneByBuildIndex(0).path);
            }
        }

        Debug.Log($"Export Completed. Name: \"{Path.GetFileName(source)}\" with {files.Length} files");
    }

    private static bool CheckHasHolder(string[] files) {
        foreach (string file in files)
            if(ModManager.mainAssets.Values.Contains(Path.GetFileName(file)))
                return true;
        return false;
    }
}