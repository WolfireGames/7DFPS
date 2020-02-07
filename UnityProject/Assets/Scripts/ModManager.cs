using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ModManager : MonoBehaviour {
    public static List<Mod> loadedGunMods = new List<Mod>();
    public static List<Mod> loadedLevelMods = new List<Mod>();
    public static List<Mod> loadedTapeMods = new List<Mod>();

    public static List<Mod> availableMods;

    public LevelCreatorScript levelCreatorScript;
    public GUISkinHolder guiSkinHolder;

    public static Dictionary<ModType, string> mainAssets = new Dictionary<ModType, string> {
        {ModType.Gun, "gun_holder.prefab"},
        {ModType.LevelTile, "tiles_holder.prefab"},
        {ModType.Tapes, "tape_holder.prefab"},
    };

    public void Awake() {
        //Make sure these folders are generated if they don't exist
        Directory.CreateDirectory(GetModsfolderPath());

        // Are mods enabled?
        if(PlayerPrefs.GetInt("mods_enabled", 1) != 1)
            return;

        if(availableMods == null) { //DEBUG load all mods
            UpdateMods();
            foreach (var mod in availableMods)
                LoadMod(mod);
        }
        
        InsertMods();
    }

    public static string GetModsfolderPath() {
        return Path.Combine(Application.persistentDataPath, "Mods").Replace('\\', '/');
    }

    public void InsertMods() {
        // Insert all gun mods
        ModLoadType gun_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_gun_loading", 0);
        if(gun_load_type != ModLoadType.DISABLED) {
            var guns = new List<GameObject>(guiSkinHolder.weapons);
            if(loadedGunMods.Count > 0 && gun_load_type == ModLoadType.EXCLUSIVE)
                guns.Clear();

            foreach (var mod in loadedGunMods)
                guns.Add(mod.mainAsset);
            guiSkinHolder.weapons = guns.ToArray();
        }

        // Insert all Level Tile mods
        ModLoadType tile_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tile_loading", 0);
        if(tile_load_type != ModLoadType.DISABLED) {
            var tiles = new List<GameObject>(levelCreatorScript.level_tiles);
            if(loadedLevelMods.Count > 0 && tile_load_type == ModLoadType.EXCLUSIVE)
                tiles.Clear();

            foreach (var mod in loadedLevelMods)
                foreach(GameObject tile in mod.mainAsset.GetComponent<ModTilesHolder>().tile_prefabs)
                    tiles.Add(tile);
            levelCreatorScript.level_tiles = tiles.ToArray();
        }

        // Insert all Tape mods
        ModLoadType tape_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tape_loading", 0);
        if(tape_load_type != ModLoadType.DISABLED) {
            if(loadedTapeMods.Count > 0 && tape_load_type == ModLoadType.EXCLUSIVE)
                guiSkinHolder.sound_tape_content.Clear();

            foreach (var mod in loadedTapeMods)
                foreach(AudioClip tape in mod.mainAsset.GetComponent<ModTapesHolder>().tapes)
                    guiSkinHolder.sound_tape_content.Add(tape);
        }
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

    public static void UpdateMods() {
        var folders = Directory.GetDirectories(GetModsfolderPath(), "modfile_*", SearchOption.AllDirectories);
        availableMods = new List<Mod>();
        
        Debug.Log($"Importing mods..");
        foreach(var path in folders) {
            try {
                UpdateMod(path);
            } catch (System.Exception e) {
                Debug.LogWarning($"Failed to import {path}: {e.Message}");
            }
        }
        Debug.Log($"Mod importing completed. Imported {availableMods.Count} mods!");
    }

    private static void UpdateMod(string path) {
        var manifestBundle = AssetBundle.LoadFromFile(Path.Combine(path, Path.GetFileName(path)));
        var manifest = manifestBundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
        foreach(var bundleName in manifest.GetAllAssetBundles()) {
            // Init
            var assetPath = Path.Combine(path, bundleName);
            var modBundle = AssetBundle.LoadFromFile(assetPath);

            // Generate Mod Object
            var mod = new Mod(assetPath);
            mod.name = bundleName;
            mod.modType = GetModTypeFromBundle(modBundle);

            // Register mod and clean up
            availableMods.Add(mod);
            modBundle.Unload(true);
            Debug.Log($" + {bundleName} ({mod.modType})");
        }
        manifestBundle.Unload(true);
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

public class Mod {
    public ModType modType;
    public string name = "None";

    public bool loaded = false;
    
    public string path;
    public AssetBundle assetBundle;

    public GameObject mainAsset;

    public Mod(string path) {
        this.path = path;
    }

    public void Load() {
        if(loaded)
            return;

        // Loading
        loaded = true;
        assetBundle = AssetBundle.LoadFromFile(path);
        mainAsset = assetBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(this.modType));

        if(modType == ModType.Gun)
            SetupGun();
    }

    private void SetupGun() {
        WeaponHolder weaponHolder = mainAsset.GetComponent<WeaponHolder>();

        // Set the display name to the bundle name if no custom name is provided
        if(weaponHolder.display_name == "My Gun")
            weaponHolder.display_name = name;
    }
    
    public void Unload() {
        if(!loaded)
            return;

        loaded = false;
        assetBundle.Unload(true);
    }
}
