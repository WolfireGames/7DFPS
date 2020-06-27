using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ModTapesHolder))]
public class ModTapesHolderEditor : Editor {
    string mainAssetName = Path.GetFileNameWithoutExtension(ModManager.GetMainAssetName(ModType.Tapes));
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(((ModTapesHolder)target).tapes.Length <= 0)
            EditorGUILayout.HelpBox($"You don't have any tapes in this holder!\nDrag and drop your audio files into the tapes array above!", MessageType.Error);

        if(target.name != mainAssetName) {
            EditorGUILayout.HelpBox($"If you plan on loading these tapes as a mod, then you need to name the prefab: \"{mainAssetName}\"!", MessageType.Warning);
            if(GUILayout.Button($"Set name to \"{mainAssetName}\"")) {
                AssetDatabase.RenameAsset(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target), mainAssetName);
            }
        }
    }
}
#endif

public class ModTapesHolder : MonoBehaviour {
    public AudioClip[] tapes = new AudioClip[0];
}
