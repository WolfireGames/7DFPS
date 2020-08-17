using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ModTilesHolder))]
public class ModTilesHolderEditor : Editor {
    string mainAssetName = Path.GetFileNameWithoutExtension(ModManager.GetMainAssetName(ModType.LevelTile));
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(((ModTilesHolder)target).tile_prefabs.Length <= 0)
            EditorGUILayout.HelpBox($"You don't have any tiles in this holder!\nDrag and drop your tile prefabs into the tile_prefabs array above!", MessageType.Error);

        if(target.name != mainAssetName) {
            EditorGUILayout.HelpBox($"If you plan on loading these tiles as a mod, then you need to name the prefab: \"{mainAssetName}\"!", MessageType.Warning);
            if(GUILayout.Button($"Set name to \"{mainAssetName}\"")) {
                AssetDatabase.RenameAsset(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target), mainAssetName);
            }
        }
    }
}
#endif

public class ModTilesHolder : MonoBehaviour {
    public GameObject[] tile_prefabs = new GameObject[0];
}
