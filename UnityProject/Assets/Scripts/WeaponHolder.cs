using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class WeaponHolder : MonoBehaviour {
    public string display_name = "My Gun";

    public GameObject gun_object;
    public GameObject mag_object;
    public GameObject bullet_object;
    public GameObject casing_object;

    [NonSerialized] public Mod mod = null;

    public void Load() {
        if(mod == null)
            return; // Don't need to load anything if it isn't a mod placeholder

        WeaponHolder holder = mod.mainAsset.GetComponent<WeaponHolder>();
        this.display_name = holder.display_name;

        this.gun_object = holder.gun_object;
        this.mag_object = holder.mag_object;
        this.bullet_object = holder.bullet_object;
        this.casing_object = holder.casing_object;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(WeaponHolder))]
public class WeaponHolderEditor : Editor {
    string mainAssetName = Path.GetFileNameWithoutExtension(ModManager.GetMainAssetName(ModType.Gun));
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(((WeaponHolder)target).display_name == "My Gun")
            EditorGUILayout.HelpBox($"Please consider giving your gun a new name!\nThis name will be visible in the gun selection menu!", MessageType.Warning);

        if(target.name != mainAssetName) {
            EditorGUILayout.HelpBox($"If you plan on loading this gun as a mod, then you need to name the prefab: \"{mainAssetName}\"!", MessageType.Warning);
            if(GUILayout.Button($"Set name to \"{mainAssetName}\"")) {
                AssetDatabase.RenameAsset(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target), mainAssetName);
            }
        }
    }
}
#endif