using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ModManager : Singleton<ModManager> {
    public static List<Mod> importedMods = new List<Mod>();

    public InbuildMod[] inbuildMods;
    private SteamScript steamScript;

    public static Dictionary<ModType, string> mainAssets = new Dictionary<ModType, string> {
        {ModType.Gun, "gun_holder.prefab"},
        {ModType.LevelTile, "tiles_holder.prefab"},
        {ModType.Tapes, "tape_holder.prefab"},
    };

    public override void Init() {
        // Setup static reference
        steamScript = GameObject.FindObjectOfType<SteamScript>();

        //Make sure these folders are generated if they don't exist
        if(!Directory.Exists(GetModsfolderPath())) {
            Directory.CreateDirectory(GetModsfolderPath());

            // Generate inbuild mods
            foreach (var mod in inbuildMods)
                mod.Generate();
        }
    }

    public void Start() {
        // Are mods enabled?
        if(!IsModsEnabled())
            return;

        ImportLocalMods();
    }

    public static bool IsModsEnabled() {
        return PlayerPrefs.GetInt("mods_enabled", 0) == 1;
    }

    public static string GetModsfolderPath() {
        return Path.Combine(Application.persistentDataPath, "Mods").Replace('\\', '/');
    }

    public static IEnumerable<Mod> GetAvailableMods(ModType type) {
        return importedMods.Where((mod) => mod.modType == type && !mod.ignore);
    }

    public static String GetMainAssetName(ModType modType) {
        if(!mainAssets.ContainsKey(modType))
            throw new System.InvalidOperationException($"Unknown Mod Type \"{modType.ToString()}\"");

        return mainAssets[modType];
    }

    public static string GetModsFolder(ModType modType) {
        var path = Path.Combine(GetModsfolderPath(), modType.ToString());
        
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public static ModType GetModTypeFromBundle(AssetBundle assetBundle) {
        foreach (ModType modType in mainAssets.Keys)
            if(assetBundle.Contains(mainAssets[modType]))
                return modType;

        throw new System.InvalidOperationException($"Unable to find Mod Type for \"{assetBundle.name}\"");
    }

    public static string[] GetModPaths() {
        return Directory.GetDirectories(GetModsfolderPath(), "modfile_*", SearchOption.AllDirectories);
    }

    public static void ImportLocalMods() {
        Debug.Log($"Importing mods..");
        importedMods = new List<Mod>();
        foreach(var path in GetModPaths()) {
            try {
                ImportMod(path, true);
            } catch (System.Exception e) {
                Debug.LogWarning($"Failed to import {path}: {e.Message}");
            }
        }
        Debug.Log($"Local mod importing completed. Imported {importedMods.Count} mods!");
    }

    public static Mod ImportMod(string path, bool local) {
        string[] bundles = Directory.GetFiles(path);
        string bundleName = bundles.FirstOrDefault((name) => name.EndsWith(SystemInfo.operatingSystemFamily.ToString(), true, null));

        // Fallback to unsigned mods (old naming version without os versions)
        if(bundleName == null && Path.GetFileName(path).StartsWith("modfile_")) {
            bundleName = bundles.FirstOrDefault((name) => name.EndsWith(Path.GetFileName(path).Substring(8), true, null) && !Path.GetFileName(name).StartsWith("modfile_"));
            if(bundleName == null) {
                throw new Exception($"No compatible mod version found for os family: '{SystemInfo.operatingSystemFamily}' for mod: '{path}'");
            }
        }

        // Init
        var assetPath = Path.Combine(path, bundleName);
        var modBundle = LoadAssetBundle(assetPath);

        // Generate Mod Object
        var mod = new Mod(assetPath, local);
        mod.modType = GetModTypeFromBundle(modBundle);
        mod.steamworksItem = new SteamworksUGCItem(mod);

        // Default steam name to *something* more descriptive than nothing
        if(mod.steamworksItem.GetName() == "") {
            if(mod.modType == ModType.Gun)
                mod.steamworksItem.SetName(modBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(ModType.Gun)).GetComponent<WeaponHolder>().display_name);
            else
                mod.steamworksItem.SetName(modBundle.name);
        }

        // Register mod and clean up
        importedMods.Add(mod);
        mod.Load();

        // Make sure we already have access to the gun mods in the current run, (required for the gun selection)
        if(mod.modType == ModType.Gun)
            GameObject.FindObjectOfType<GUISkinHolder>().InsertGunMods();

        //Debug.Log($" + {bundleName} ({mod.modType})");

        return mod;
    }

    public static AssetBundle LoadAssetBundle(string path) {
        AssetBundle loadedAssetBundle = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault( (bundle) => bundle.name == Path.GetFileNameWithoutExtension(path));
        return loadedAssetBundle ?? AssetBundle.LoadFromFile(path);
    }
}

public enum ModLoadType {
    ENABLED,
    EXCLUSIVE,
    DISABLED,
}

public enum ModType {
    Gun,
    Tapes,
    LevelTile
}

[System.Serializable]
public class Mod {
    public ModType modType;

    private UnityWebRequest thumbnailProcess;
    private Texture2D _thumbnail;
    public Texture2D thumbnail {
        get {
            if(_thumbnail)
                return _thumbnail;

            if(thumbnailProcess == null) {
                if(File.Exists(GetThumbnailPath())) {
                    thumbnailProcess = UnityWebRequestTexture.GetTexture($"file:///{GetThumbnailPath()}");
                    thumbnailProcess.SendWebRequest();
                } else {
                    _thumbnail = new Texture2D(450, 450);
                }
            } else if(thumbnailProcess.isDone && !thumbnailProcess.isNetworkError) {
                return _thumbnail = DownloadHandlerTexture.GetContent(thumbnailProcess);
            }

            return new Texture2D(450, 450);
        }
    }
    
    [SerializeField] private bool isLocal = false;

    public string path;
    public bool ignore = false;
    [NonSerialized] public AssetBundle assetBundle;

    [NonSerialized] public GameObject mainAsset;

    [NonSerialized] public SteamworksUGCItem steamworksItem;

    public Mod(string path, bool isLocal) {
        this.path = Path.GetFullPath(path);
        this.isLocal = isLocal;
    }

    public bool IsLocalMod() {
        return isLocal;
    }

    public void Load() {
        assetBundle = ModManager.LoadAssetBundle(path);
        mainAsset = assetBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(this.modType));
    }

    public string GetTypeString() {
        switch (modType) {
            case ModType.Gun:
                return "Gun";
            case ModType.LevelTile:
                return "Tile";
            case ModType.Tapes:
                return "Tapes";
        }
        return "";
    }

    public string GetThumbnailPath() {
        return Path.Combine(Path.GetDirectoryName(path), "thumbnail.jpg");
    }
}

[System.Serializable]
public class InbuildMod {
    public string name;
    public TextAsset[] files;

    public void Generate() {
        try {
            // Create directory
            string directory = Path.Combine(ModManager.GetModsfolderPath(), $"modfile_inbuild_{name}");
            Directory.CreateDirectory(directory);

            // Create files
            foreach(TextAsset file in files) {
                string path = Path.Combine(directory, Path.GetFileNameWithoutExtension(file.name));
                File.Create(path).Close();
                File.WriteAllBytes(path, file.bytes);
            }
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }
}
