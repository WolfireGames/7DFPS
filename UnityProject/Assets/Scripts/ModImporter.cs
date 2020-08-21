using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using UnityEngine;

public class ModImporter : Singleton<ModImporter> {
    private static Queue<ImportData> modImportQueue = new Queue<ImportData>();
    private static Coroutine currentModRoutine = null;

    public override void Init() { }

    private void Update() {
        if(modImportQueue.Any() && currentModRoutine == null ) {
            ImportData importData = modImportQueue.Dequeue();
            currentModRoutine = StartCoroutine(ImportModCoroutine(importData.path, importData.local, importData.owner));
        }
    }


    // User Methods

    /// <summary> Loads every local mod at the default path </summary>
    public static void ImportLocalMods() {
        foreach(var path in ModManager.GetModPaths()) {
            ImportMod(path, true);
        }
    }

    /// <summary> Imports a mod without waiting for the queue, this will block and it will throw if the same mod is currently loaded through the queue </summary>
    public static Mod ImportModNow(string path, bool local, bool owner = true) {
        return (Mod) SyncronousCoroutine(ImportModCoroutine(path, local, owner));
    }

    /// <summary> Queue a mod for import </summary>
    public static void ImportMod(string path, bool local, bool owner = true) {
        modImportQueue.Enqueue(new ImportData(path, local, owner));
    }


    // Internal Methods

    private static object SyncronousCoroutine(IEnumerator coroutine) {
        object last = null;
        while(coroutine.MoveNext()) {
            if(coroutine.Current != null) {
                last = coroutine.Current;
                if(last is IEnumerator)
                    last = SyncronousCoroutine((IEnumerator) last);
            }
        }
        return last;
    }

    private static IEnumerator ImportModCoroutine(string path, bool local, bool owner) {
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
        AssetBundle modBundle = GetLoadedAssetBundle(assetPath);
        if(modBundle == null) { // Wasn't loaded before, load it!
            AssetBundleCreateRequest assetBundleCreateRequest = null;
            try {
                assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetPath);
            } catch (Exception e) {
                Debug.LogException(e);
                currentModRoutine = null;
                yield break;
            }

            yield return assetBundleCreateRequest;
            modBundle = assetBundleCreateRequest.assetBundle;
        }

        // Generate Mod Object
        var mod = new Mod(assetPath, local);
        mod.assetBundle = modBundle;
        mod.modType = GetModTypeFromBundle(modBundle);
        mod.steamworksItem = new SteamworksUGCItem(mod);
        mod.steamworksItem.SetOwner(owner);
        mod.mainAsset = modBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(mod.modType));

        // Default steam name to *something* more descriptive than nothing
        if(mod.steamworksItem.GetName() == "") {
            if(mod.modType == ModType.Gun)
                mod.steamworksItem.SetName(modBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(ModType.Gun)).GetComponent<WeaponHolder>().display_name);
            else
                mod.steamworksItem.SetName(modBundle.name);
        }

        // Register mod
        ModManager.importedMods.Add(mod);

        // Make sure we already have access to the gun mods in the current run, (required for the gun selection)
        if(Application.isPlaying && mod.modType == ModType.Gun)
            GameObject.FindObjectOfType<GUISkinHolder>().InsertGunMods();

        Debug.Log($" + {bundleName} ({mod.modType})");

        currentModRoutine = null;
        yield return mod;
    }

    private static AssetBundle GetLoadedAssetBundle(string path) {
        return AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault( (bundle) => bundle.name == Path.GetFileNameWithoutExtension(path));
    }

    public static ModType GetModTypeFromBundle(AssetBundle assetBundle) {
        foreach (ModType modType in ModManager.mainAssets.Keys)
            if(assetBundle.Contains(ModManager.mainAssets[modType]))
                return modType;

        throw new System.InvalidOperationException($"Unable to find Mod Type for \"{assetBundle.name}\"");
    }


    private struct ImportData {
        public string path;
        public bool local;
        public bool owner;

        public ImportData(string path, bool local, bool owner) {
            this.path = path;
            this.local = local;
            this.owner = owner;
        }
    }
}
