using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(mag_script))]
public class MagScriptEditor : Editor {
    public override void OnInspectorGUI() {
        // Draw MagScript
        base.OnInspectorGUI();

        // Buttons
        if(GUILayout.Button("Open Bullet Stacker Utility")) {
            EditorWindow.GetWindow(typeof(RoundStackerUtility), false, "Bullet Stacker");
            //EditorWindow.GetWindowWithRect(typeof(RoundStackerUtility), new Rect(0, 0, 300, 200), false, "Bullet Stacker"); // Window variant to force a specific size
        }
    }
}
