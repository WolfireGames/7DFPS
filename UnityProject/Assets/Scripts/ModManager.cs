using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Steamworks;

public class ModManager : MonoBehaviour {
    public static List<Mod> loadedGunMods = new List<Mod>();
    public static List<Mod> loadedLevelMods = new List<Mod>();
    public static List<Mod> loadedTapeMods = new List<Mod>();

    public static List<Mod> availableMods = new List<Mod>();

    private static int numSteamMods = 0;

    public LevelCreatorScript levelCreatorScript;
    public GUISkinHolder guiSkinHolder;
    public InbuildMod[] inbuildMods;
    private SteamScript steamScript;

    public static Dictionary<ModType, string> mainAssets = new Dictionary<ModType, string> {
        {ModType.Gun, "gun_holder.prefab"},
        {ModType.LevelTile, "tiles_holder.prefab"},
        {ModType.Tapes, "tape_holder.prefab"},
    };

    private static ModManager _instance;
    public static ModManager instance {
        get {
            if(!_instance) // static "_instance" is cleared during hotswaps, reassign value
                _instance = UnityEngine.Object.FindObjectOfType<ModManager>();
            return _instance;
        }
    }

    public void Awake() {
        // Setup static reference
        ModManager._instance = this;
        steamScript = GameObject.FindObjectOfType<SteamScript>();

        //Make sure these folders are generated if they don't exist
        if(!Directory.Exists(GetModsfolderPath())) {
            Directory.CreateDirectory(GetModsfolderPath());

            // Generate inbuild mods
            foreach (var mod in inbuildMods)
                mod.Generate();
        }

        // Are mods enabled?
        if(PlayerPrefs.GetInt("mods_enabled", 0) != 1)
            return;

        LoadCache();

        if(GetAvailableLocalModCount() != GetModFolderCount()) { // Is our Cache up to date? (only consider local mods)
            ForceReimport();
        }

        // Load everything but guns
        foreach (var mod in availableMods.Where((mod) => mod.modType != ModType.Gun))
            mod.Load();

        InsertMods();
    }

    public static void ForceReimport() {
        Debug.LogWarning("Forced a mod Reimport");
        UnloadAll();
        ImportLocalMods();
        instance.steamScript.ImportSteamMods(); // Start importing steam mods, they will load in with a delay
        UpdateCache();
    }

    public void OnDestroy() {
        UnloadAll();
        availableMods.Clear();
    }

    public static string GetModsfolderPath() {
        return Path.Combine(Application.persistentDataPath, "Mods").Replace('\\', '/');
    }

    public void InsertMods() {
        // Insert all gun mods
        ModLoadType gun_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_gun_loading", 0);
        if(gun_load_type != ModLoadType.DISABLED) {
            var guns = new List<GameObject>(guiSkinHolder.weapons);
            var availableGuns = availableMods.Where((mod) => mod.modType == ModType.Gun);
            if(HasModsToInsert(ModType.Gun) && gun_load_type == ModLoadType.EXCLUSIVE)
                guns.Clear();

            foreach (var mod in availableGuns) {
                if(mod.ignore) {
                    continue;
                }
                WeaponHolder placeholder = new GameObject().AddComponent<WeaponHolder>();
                placeholder.gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                placeholder.mod = mod;
                placeholder.display_name = mod.steamworksItem.GetName();
                guns.Add(placeholder.gameObject);
            }
            guiSkinHolder.weapons = guns.ToArray();
        }

        // Insert all Level Tile mods
        if(levelCreatorScript) {
            ModLoadType tile_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tile_loading", 0);
            if(tile_load_type != ModLoadType.DISABLED) {
                var tiles = new List<GameObject>(levelCreatorScript.level_tiles);
                if(HasModsToInsert(ModType.LevelTile) && tile_load_type == ModLoadType.EXCLUSIVE)
                    tiles.Clear();

                foreach (var mod in loadedLevelMods) {
                    if(mod.ignore) {
                        continue;
                    }

                    foreach(GameObject tile in mod.mainAsset.GetComponent<ModTilesHolder>().tile_prefabs) {
                        tiles.Add(tile);
                    }
                }
                levelCreatorScript.level_tiles = tiles.ToArray();
            }
        }

        // Insert all Tape mods
        ModLoadType tape_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tape_loading", 0);
        if(tape_load_type != ModLoadType.DISABLED) {
            if(HasModsToInsert(ModType.Tapes) && tape_load_type == ModLoadType.EXCLUSIVE)
                guiSkinHolder.sound_tape_content.Clear();

            foreach (var mod in loadedTapeMods) {
                if(mod.ignore) {
                    continue;
                }

                foreach(AudioClip tape in mod.mainAsset.GetComponent<ModTapesHolder>().tapes) {
                    guiSkinHolder.sound_tape_content.Add(tape);
                }
            }
        }
    }

    public static void UnloadAll() {
        foreach (var mod in availableMods) {
            mod.Unload();
        }
    }

    public static bool ShouldInsertMod(Mod mod) {
        return !mod.ignore;
    }

    public bool HasModsToInsert(ModType modType) {
        return GetModList(modType).Count( (mod) => !mod.ignore) > 0;
    }

    private static List<Mod> GetModList(ModType modType) {
        switch (modType) {
            case ModType.Gun: return loadedGunMods;
            case ModType.LevelTile: return loadedLevelMods;
            case ModType.Tapes: return loadedTapeMods;

            default:
                throw new System.InvalidOperationException($"Unknown Mod Type \"{modType.ToString()}\"");
        }
    }

    public static String GetMainAssetName(ModType modType) {
        if(!mainAssets.ContainsKey(modType))
            throw new System.InvalidOperationException($"Unknown Mod Type \"{modType.ToString()}\"");

        return mainAssets[modType];
    }

    /// <summary> Add / Remove mod from the ModManager.loadedGunMods, ModManager.loadedLevelMods or ModManager.loadedLevelMods list. <summary>
    public static void UpdateModInLoadedModlist(Mod mod) {
        List<Mod> list = GetModList(mod.modType);

        if(list.Contains(mod) == mod.loaded) // Is the mod already registered as loaded/unloaded?
            return;

        if(mod.loaded) {
            list.Add(mod);
        } else {
            list.Remove(mod);
        }
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

    public static int GetModFolderCount() {
        return GetModPaths().Count();
    }

    public static int GetAvailableLocalModCount() {
        return availableMods.Count( (x) => x.IsLocalMod() );
    }

    public static int GetAvailableSteamModCount() {
        return availableMods.Count( (x) => !x.IsLocalMod() );
    }

    public static void ImportLocalMods() {
        Debug.Log($"Importing mods..");
        availableMods = new List<Mod>();
        foreach(var path in GetModPaths()) {
            try {
                ImportMod(path, true);
            } catch (System.Exception e) {
                Debug.LogWarning($"Failed to import {path}: {e.Message}");
            }
        }
        Debug.Log($"Local mod importing completed. Imported {availableMods.Count} mods!");
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
        var modBundle = AssetBundle.LoadFromFile(assetPath);

        // Generate Mod Object
        var mod = new Mod(assetPath, local);
        mod.modType = GetModTypeFromBundle(modBundle);
        mod.steamworksItem = new SteamworksUGCItem(mod);

        // Determine gun display name for the cache
        if(mod.modType == ModType.Gun)
            mod.steamworksItem.SetName(modBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(ModType.Gun)).GetComponent<WeaponHolder>().display_name);

        // Register mod and clean up
        availableMods.Add(mod);
        modBundle.Unload(true);
        //Debug.Log($" + {bundleName} ({mod.modType})");

        return mod;
    }

    public static Mod GetAvailableModWithDirectoryPath(string folder) {
        for (int i = 0; i < ModManager.availableMods.Count; i++)
            if(Path.Equals(Path.GetDirectoryName(ModManager.availableMods[i].path), folder))
                return ModManager.availableMods[i];
        return null;
    }

    private static int GetModCountInCache() {
        if(CacheExists())
            return LoadRawCacheFile().mods.Length;
        return 0;
    }

    private static void LoadCache() {
        if(CacheExists())
            availableMods = new List<Mod> (LoadRawCacheFile().mods);
        else
            availableMods = new List<Mod> ();

        // Reload steamworks item data
        foreach (Mod mod in availableMods)
            mod.steamworksItem = new SteamworksUGCItem(mod);
    }

    public static void UpdateCache() {
        try {
            string path = GetCachePath();

            if(File.Exists(path))
                File.Delete(path);

            File.Create(path).Close();
            File.WriteAllText(path, JsonUtility.ToJson(new Cache(availableMods.ToArray()), true));
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    private static bool CacheExists() {
        return File.Exists(GetCachePath());
    }

    private static Cache LoadRawCacheFile() {
        return JsonUtility.FromJson<Cache>(File.ReadAllText(GetCachePath()));
    }

    private static string GetCachePath() {
        return Path.Combine(ModManager.GetModsfolderPath(), "cache");
    }

    [System.Serializable]
    private struct Cache {
        public Mod[] mods;

        public Cache (Mod[] mods) {
            this.mods = mods;
        }
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

    [NonSerialized] public bool loaded = false;
    
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
        if(loaded)
            return;

        // Loading
        loaded = true;
        assetBundle = AssetBundle.LoadFromFile(path);
        mainAsset = assetBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(this.modType));
        ModManager.UpdateModInLoadedModlist(this);
    }

    public void Unload() {
        if(!loaded)
            return;

        loaded = false;
        assetBundle.Unload(true);
        ModManager.UpdateModInLoadedModlist(this);
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
