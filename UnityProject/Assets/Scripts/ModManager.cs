using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ModManager : MonoBehaviour {
    public static List<Mod> loadedGunMods = new List<Mod>();
    public static List<Mod> loadedLevelMods = new List<Mod>();
    public static List<Mod> loadedCustomMods = new List<Mod>();

    public static List<Mod> availableMods;

    public LevelCreatorScript levelCreatorScript;
    public GUISkinHolder guiSkinHolder;

    public void Awake() {
        //Make sure these folders are generated if they don't exist
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Mods"));
        GetModsFolder(ModType.Gun);
        GetModsFolder(ModType.LevelTile);

        if(availableMods == null) { //DEBUG load all mods
            UpdateMods();
            foreach (var mod in availableMods)
                LoadMod(mod);
        }

        /*
        // Instantiate all custom mods
        foreach (var mod in loadedCustomMods) {
            var scriptHolder = Instantiate(mod.mainAsset);
            scriptHolder.transform.parent = transform;
            scriptHolder.name = mod.name;

            scriptHolder.AddComponent(mod.GetScript());
        } */

        // Insert all gun mods
        var guns = new List<GameObject>(guiSkinHolder.weapons);
        foreach (var mod in loadedGunMods)
            guns.Add(mod.mainAsset);
        guiSkinHolder.weapons = guns.ToArray();

        // Insert all Level Tile mods
        var tiles = new List<GameObject>(levelCreatorScript.level_tiles);
        foreach (var mod in loadedLevelMods)
            foreach(Transform child in mod.mainAsset.transform)
                tiles.Add(child.gameObject);
        levelCreatorScript.level_tiles = tiles.ToArray();
    }

    public void LoadMod(Mod mod) {
        SetModLoaded(mod, true);
    }

    public void UnloadMod(Mod mod) {
        SetModLoaded(mod, false);
    }

    private List<Mod> GetModList(ModType modType) {
        switch (modType) {
            case ModType.Gun: return loadedGunMods;
            case ModType.LevelTile: return loadedLevelMods;
            case ModType.Custom: return loadedCustomMods;

            default:
                throw new System.InvalidOperationException($"Unknown Mod Type \"{modType.ToString()}\"");
        }
    }

    private static String GetMainAssetName(ModType modType) {
        switch (modType) {
            case ModType.Gun: return "gun_holder.prefab";
            case ModType.LevelTile: return "tiles_holder.prefab";
            case ModType.Custom: return "script_holder.prefab";

            default:
                throw new System.InvalidOperationException($"Unknown Mod Type \"{modType.ToString()}\"");
        }
    }

    private void SetModLoaded(Mod mod, bool loadMod) {
        List<Mod> list = GetModList(mod.modType);

        if(loadMod) {
            mod.Load();
            list.Add(mod);   
        } else {
            mod.Unload();
            list.Remove(mod);
        }
    }

    public static string GetModsFolder(ModType modType) {
        var path = Path.Combine(Application.dataPath, "Mods", modType.ToString());
        
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public static void UpdateMods() {
        var rootFolders = Directory.GetDirectories(Path.Combine(Application.dataPath, "Mods"));
        availableMods = new List<Mod>();
        
        foreach (var folder in rootFolders) {
            var modType = (ModType) Enum.Parse(typeof(ModType), Path.GetFileName(folder));
            var modFolders = Directory.GetDirectories(folder);

            Debug.Log($"Importing \"{modType}\" mods..");

            foreach(var path in modFolders) {            
                var manifestBundle = AssetBundle.LoadFromFile(Path.Combine(path, Path.GetFileName(path)));
                var manifest = manifestBundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");

                foreach(var bundleName in manifest.GetAllAssetBundles()) {
                    var assetPath = Path.Combine(path, bundleName);
                    Debug.Log($" - {bundleName}");

                    /*
                    // Check if this mod has a custom script inside
                    var modBundle = AssetBundle.LoadFromFile(assetPath);
                    var hasScript = modBundle.Contains("scripts.bytes");
                    modBundle.Unload(true);
                    */

                    // Generate Mod Object
                    var mod = new Mod(GetMainAssetName(modType), assetPath, modType);
                    mod.name = bundleName;
                    //mod.hasCustomScript = hasScript;

                    availableMods.Add(mod);
                }
                manifestBundle.Unload(true);
            }
        }

        Debug.Log($"Mod importing completed. Imported {availableMods.Count} mods!");
    }
}

public enum ModType {
    Custom,
    Gun,
    LevelTile
}

public class Mod {
    public ModType modType;
    public string name = "None";
    public bool hasCustomScript = false;

    public bool loaded = false;
    
    public string path;
    public AssetBundle assetBundle;

    public GameObject mainAsset;
    public string mainAssetName;

    public Mod(string mainAssetName, string path, ModType modType = ModType.Custom) {
        this.mainAssetName = mainAssetName;
        this.path = path;
        this.modType = modType;
    }

    public System.Type GetScript() {
        var txt = assetBundle.LoadAsset<TextAsset>("scripts.bytes");

        var assembly = System.Reflection.Assembly.Load(txt.bytes);
        var type = assembly.GetType("CustomScript");

        return type;
    }

    public void Load() {
        if(loaded)
            return;

        // Loading
        loaded = true;
        assetBundle = AssetBundle.LoadFromFile(path);
        mainAsset = assetBundle.LoadAsset<GameObject>(mainAssetName);

        // Attach script for gun mods
        if(hasCustomScript && modType == ModType.Gun)
            mainAsset.GetComponent<WeaponHolder>().gun_object.AddComponent(GetScript());
    }
    
    public void Unload() {
        if(!loaded)
            return;

        loaded = false;
        assetBundle.Unload(true);
    }
}
