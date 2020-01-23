using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ModManager : MonoBehaviour {
    public static List<Mod> loadedGunMods = new List<Mod>();
    public static List<Mod> loadedLevelMods = new List<Mod>();
    public static List<Mod> loadedCustomMods = new List<Mod>();
    public static List<Mod> loadedTapeMods = new List<Mod>();

    public static List<Mod> availableMods;

    public LevelCreatorScript levelCreatorScript;
    public GUISkinHolder guiSkinHolder;

    public static Dictionary<ModType, string> mainAssets = new Dictionary<ModType, string> {
        {ModType.Gun, "gun_holder.prefab"},
        {ModType.LevelTile, "tiles_holder.prefab"},
        {ModType.Custom, "script_holder.prefab"},
        {ModType.Tapes, "tape_holder.prefab"},
    };

    public void Awake() {
        //Make sure these folders are generated if they don't exist
        Directory.CreateDirectory(GetModsfolderPath());

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
        if(loadedGunMods.Count > 0 && PlayerPrefs.GetInt("ignore_vanilla_guns", 0) == 1)
            guns.Clear();

        foreach (var mod in loadedGunMods)
            guns.Add(mod.mainAsset);
        guiSkinHolder.weapons = guns.ToArray();

        // Insert all Level Tile mods
        var tiles = new List<GameObject>(levelCreatorScript.level_tiles);
        if(loadedLevelMods.Count > 0 && PlayerPrefs.GetInt("ignore_vanilla_tiles", 0) == 1)
            tiles.Clear();

        foreach (var mod in loadedLevelMods)
            foreach(Transform child in mod.mainAsset.transform)
                tiles.Add(child.gameObject);
        levelCreatorScript.level_tiles = tiles.ToArray();

        // Insert all Tape mods
        if(loadedTapeMods.Count > 0 && PlayerPrefs.GetInt("ignore_vanilla_tapes", 0) == 1)
            guiSkinHolder.sound_tape_content.Clear();

        foreach (var mod in loadedTapeMods)
            foreach(AudioClip tape in mod.mainAsset.GetComponent<ModTapesHolder>().tapes)
                guiSkinHolder.sound_tape_content.Add(tape);
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
            //mod.hasCustomScript = modBundle.Contains("scripts.bytes");

            // Register mod and clean up
            availableMods.Add(mod);
            modBundle.Unload(true);
            Debug.Log($" + {bundleName} ({mod.modType})");
        }
        manifestBundle.Unload(true);
    }
}

public enum ModType {
    Custom,
    Gun,
    Tapes,
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

    public Mod(string path) {
        this.path = path;
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
        mainAsset = assetBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(this.modType));

        if(modType == ModType.Gun)
            SetupGun();
    }

    private void SetupGun() {
        WeaponHolder weaponHolder = mainAsset.GetComponent<WeaponHolder>();

        // Set the display name to the bundle name if no custom name is provided
        if(weaponHolder.display_name == "My Gun")
            weaponHolder.display_name = name;

        // Attach script for gun mods
        if(hasCustomScript)
            weaponHolder.gun_object.AddComponent(GetScript());
    }
    
    public void Unload() {
        if(!loaded)
            return;

        loaded = false;
        assetBundle.Unload(true);
    }
}
