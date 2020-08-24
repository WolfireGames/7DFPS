using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ModManager : Singleton<ModManager> {
    public static List<Mod> importedMods = new List<Mod>();

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
        }
    }

    public void Start() {
        // Are mods enabled?
        if(!IsModsEnabled())
            return;

        ModImporter.ImportLocalMods();
    }

    public static bool IsModsEnabled() {
        return PlayerPrefs.GetInt("mods_enabled", 0) == 1;
    }

    public static string GetModsfolderPath() {
        return Path.Combine(Application.persistentDataPath, "Mods").Replace('\\', '/');
    }

    public static IEnumerable<Mod> GetAvailableMods(ModType type) {
        return importedMods.Where((mod) => mod.modType == type && !mod.steamworksItem.ignore);
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

    public static string[] GetModPaths() {
        return Directory.GetDirectories(GetModsfolderPath(), "modfile_*", SearchOption.AllDirectories);
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
