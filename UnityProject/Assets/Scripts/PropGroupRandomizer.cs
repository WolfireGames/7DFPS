using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PropGroupRandomizer : MonoBehaviour {
    public GameObject[] groups;

    void Start() {
        RandomizeGroup();
    }

    void RandomizeGroup() {
        if(groups.Length <= 0) {
            Debug.LogWarning($"Prop group {this.name} has no groups set up and will do nothing!");
            return;
        }

        // Disable all groups
        foreach (var group in groups) {
            if(group) {
                group.SetActive(false);
            }
        }

        // Enable one group
        var active = groups[Random.Range(0, groups.Length)];
        if(active)
            active.SetActive(true);
    }

    public void Reset() {
        // Make every child a group
        groups = new GameObject[transform.childCount + 1];
        for (int i = 0; i < transform.childCount; i++)
            groups[i] = transform.GetChild(i).gameObject;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PropGroupRandomizer))]
public class PropGroupRandomizerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if(GUILayout.Button("Select children as prop groups"))
            ((PropGroupRandomizer)target).Reset();

        EditorGUILayout.HelpBox("Disables every GameObject in the group, and enables one at random!", MessageType.Info);
    }
}
#endif