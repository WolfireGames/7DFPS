using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class ModManagerEditor : EditorWindow {
    private string newModName = "";
    private string newModCreator = "";
    private ModType newModType = ModType.Tapes;

    private class ModData {
        public string path;
        public string name;
    }

    private ModData[] modList = new ModData[0];
    private bool exportedThisDraw = false;
    private string modSourcePath = "";
    private Vector2 modListScroll = new Vector2();

    [MenuItem("Wolfire/Mod Manager Window")]
    private static void ShowWindow() {
        ModManagerEditor window = GetWindow<ModManagerEditor>();
        window.titleContent = new GUIContent("ModManager");

        window.Show(true);
    }

    private void Awake() {
        newModCreator = System.Environment.UserName;
        modSourcePath = Path.Combine(Application.dataPath, "mods");
    }

    void OnEnable() {
        RefreshModList();
    }

    private void RefreshModList() {
        List<ModData> newList = new List<ModData>();

        string[] paths = Directory.GetDirectories(modSourcePath, "*", SearchOption.TopDirectoryOnly);
        foreach (string path in paths) {
            newList.Add(new ModData() {
                path = path,
                name = Path.GetFileNameWithoutExtension(path),
            });
        }

        modList = newList.ToArray();
    }

    private void OnGUI() {
        exportedThisDraw = false;

        modListScroll = GUILayout.BeginScrollView(modListScroll, false, false);
        DrawModList(modList);
        if (exportedThisDraw) {
            // The layout is no longer the same since we exported
            // just return here instead of ending a scroll view that no longer exists
            return;
        }
        GUILayout.EndScrollView();

        // Misc controll buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh")) {
            RefreshModList();
        }

        // Batch export buttons
        GUILayout.BeginVertical();
        if (GUILayout.Button("Export ALL")) {
            ModExport.ExportBundles(modList.Select((x) => x.path).ToArray());
            exportedThisDraw = true;
            return;
        }
        if (GUILayout.Button("Export Folder")) {
            ModExport.ExportBundles();
            exportedThisDraw = true;
            return;
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        // Mod Creation Utility
        GUILayout.Space(10);
        GUILayout.Label("Create new mod:", EditorStyles.boldLabel);
        newModName = EditorGUILayout.TextField("Name", newModName).ToLower();
        newModCreator = EditorGUILayout.TextField("Creator", newModCreator).ToLower();
        newModType = (ModType) EditorGUILayout.EnumPopup("Type", newModType);

        bool valid = DrawParamValidation();

        EditorGUI.BeginDisabledGroup(!valid);
        if (GUILayout.Button("Create new")) {
            CreateEmptyMod(newModName, newModCreator, newModType);
        }
        EditorGUI.EndDisabledGroup();
    }

    // This method both draws warnings if a parameter is not valid and returns if one was not valid
    private bool DrawParamValidation() {
        if (string.IsNullOrEmpty(newModName)) {
            EditorGUILayout.HelpBox("Please select a name for your mod!", MessageType.Error);
            return false;
        }

        if (Directory.Exists(GetModPath(newModName, newModCreator))) {
            EditorGUILayout.HelpBox("There is already a mod with that name!", MessageType.Error);
            return false;
        }

        return true;
    }

    private string GetModPath(string modName, string modCreator) {
        return Path.Combine(modSourcePath, $"{modCreator}_{modName}");
    }

    private void CreateEmptyMod(string modName, string modCreator, ModType type) {
        string modPath = GetModPath(modName, modCreator);
        Directory.CreateDirectory(modPath);

        GameObject holder = PrepareModHolder(type);
        holder = PrefabUtility.SaveAsPrefabAsset(holder, Path.Combine(modPath, $"{holder.name}.prefab"));
        Selection.activeGameObject = holder;
    }

    private GameObject PrepareModHolder(ModType type) {
        GameObject modHolder = new GameObject {
            name = Path.GetFileNameWithoutExtension(ModManager.GetMainAssetName(type))
        };

        switch (type) {
            case ModType.Gun:
                modHolder.AddComponent<WeaponHolder>();
                break;
            case ModType.Tapes:
                modHolder.AddComponent<ModTapesHolder>();
                break;
            case ModType.LevelTile:
                modHolder.AddComponent<ModTilesHolder>();
                break;
            default:
                throw new System.Exception($"We couldn't generate a mod holder for mods of type: {type}");
        }
        return modHolder;
    }

    // Draw mods column by column instead of row by row for better layouting
    // This ensures every column has the same width
    private void DrawModList(ModData[] modList) {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        foreach (ModData mod in modList) {
            GUILayout.Label(mod.name);
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        foreach (ModData mod in modList) {
            if (GUILayout.Button("Export")) {
                ModExport.ExportBundle(mod.path);
                exportedThisDraw = true;
                return;
            }
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}
